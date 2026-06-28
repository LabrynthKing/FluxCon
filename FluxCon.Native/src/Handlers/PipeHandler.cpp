// FluxCon => A Diagnostic Logger For Subnautica 2
// Copyright (C) 2026 LabrynthKing
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

#include "Handlers/PipeHandler.hpp"

#include <chrono>

namespace Flux::Handlers
{
    constexpr auto PIPE_NAME = R"(\\.\pipe\FluxConLogger)";

    void PipeHandler::Initialize()
    {
        m_shutdown_event = CreateEventA(nullptr, TRUE, FALSE, nullptr);
        m_pipe_worker = std::thread(&PipeHandler::pipe_worker_start, this);
    }

    PipeHandler::~PipeHandler()
    {
        m_running = false;

        SetEvent(m_shutdown_event);
        m_queue_cv.notify_all();

        if (m_pipe_worker.joinable())
            m_pipe_worker.join();

        CloseHandle(m_shutdown_event);
    }

    bool PipeHandler::HasInitialized() const { return m_init.load(); }

    void PipeHandler::Send(const MessageType type, std::vector<uint8_t> payload)
    {
        {
            std::lock_guard lock(m_queue_mutex);
            m_outgoing.push(OutgoingMessage{type, std::move(payload)});
        }

        m_queue_cv.notify_one();
    }

    bool PipeHandler::connect_or_launch(HANDLE& pipe_handle) const
    {
        if (!WaitNamedPipeA(PIPE_NAME, 100))
        {
            STARTUPINFOA si{};
            PROCESS_INFORMATION pi{};
            si.cb = sizeof(si);
            constexpr char exe_path[] = R"(.\ue4ss\Mods\FluxCon\FluxCon.exe)";
            if (const auto work_dir = R"(.\ue4ss\Mods\FluxCon\)"; CreateProcessA(
                    exe_path, nullptr, nullptr, nullptr, FALSE, CREATE_NEW_CONSOLE, nullptr, work_dir, &si, &pi))
            {
                CloseHandle(pi.hProcess);
                CloseHandle(pi.hThread);
            }
        }

        pipe_handle = INVALID_HANDLE_VALUE;
        while (m_running && pipe_handle == INVALID_HANDLE_VALUE)
        {
            pipe_handle =
                CreateFileA(PIPE_NAME, GENERIC_WRITE, 0, nullptr, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, nullptr);
            if (pipe_handle == INVALID_HANDLE_VALUE)
                std::this_thread::sleep_for(std::chrono::milliseconds(250));
        }
        return pipe_handle != INVALID_HANDLE_VALUE;
    }

    bool PipeHandler::write_message(const HANDLE pipe_handle, const OutgoingMessage& msg) const
    {
        const MessageHeader header{static_cast<uint32_t>(msg.type), static_cast<uint32_t>(msg.payload.size())};

        std::vector<uint8_t> frame(sizeof(header) + msg.payload.size());

        memcpy(frame.data(), &header, sizeof(header));
        if (!msg.payload.empty())
            memcpy(frame.data() + sizeof(header), msg.payload.data(), msg.payload.size());

        OVERLAPPED ov{};

        ov.hEvent = CreateEventA(nullptr, TRUE, FALSE, nullptr);

        if (const BOOL ok = WriteFile(pipe_handle, frame.data(), static_cast<DWORD>(frame.size()), nullptr, &ov);
            !ok && GetLastError() != ERROR_IO_PENDING)
        {
            CloseHandle(ov.hEvent);
            return false;
        }

        const HANDLE waitHandles[2] = {ov.hEvent, m_shutdown_event};
        const DWORD wait = WaitForMultipleObjects(2, waitHandles, FALSE, INFINITE);

        bool success = false;
        if (wait == WAIT_OBJECT_0)
        {
            DWORD bytes_written = 0;
            success = GetOverlappedResult(pipe_handle, &ov, &bytes_written, FALSE) && bytes_written == frame.size();
        }
        else
        {
            CancelIoEx(pipe_handle, &ov);
        }

        CloseHandle(ov.hEvent);
        return success;
    }

    void PipeHandler::pipe_worker_start()
    {
        while (m_running)
        {
            auto pipe_handle = INVALID_HANDLE_VALUE;
            if (!connect_or_launch(pipe_handle))
                break;

            m_init = true;

            bool pipe_alive = true;
            while (m_running && pipe_alive)
            {
                OutgoingMessage msg;
                {
                    std::unique_lock lock(m_queue_mutex);
                    m_queue_cv.wait(lock, [this] { return !m_running || !m_outgoing.empty(); });
                    if (!m_running)
                        break;
                    msg = std::move(m_outgoing.front());
                    m_outgoing.pop();
                }

                if (!write_message(pipe_handle, msg))
                    pipe_alive = false;
            }

            m_init = false;
            CloseHandle(pipe_handle);
        }
    }
} // namespace Flux::Handlers
