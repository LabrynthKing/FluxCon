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

#include <windows.h>

namespace Flux::Handlers
{
    void PipeHandler::Initialize() { m_pipe_worker = std::thread(&PipeHandler::pipe_worker_start, this); }

    PipeHandler::~PipeHandler()
    {
        m_running = false;
        if (m_pipe_worker.joinable())
        {
            m_pipe_worker.join();
        }
    }

    bool PipeHandler::HasInitialized() { return m_init.load(); }

    void PipeHandler::pipe_worker_start() const
    {
        const auto pipe_name = R"(\\.\pipe\FluxConLogger)";

        if (!WaitNamedPipeA(pipe_name, 100))
        {
            STARTUPINFOA si{};
            PROCESS_INFORMATION pi{};
            si.cb = sizeof(si);

            char exe_path[] = R"(.\ue4ss\Mods\FluxCon\FluxCon.exe)";
            const auto work_dir = R"(.\ue4ss\Mods\FluxCon\)";

            if (CreateProcessA(exe_path, nullptr, nullptr, nullptr, FALSE, CREATE_NEW_CONSOLE, nullptr, work_dir, &si,
                               &pi))
            {
                CloseHandle(pi.hProcess);
                CloseHandle(pi.hThread);
            }
        }

        auto pipe_handle = INVALID_HANDLE_VALUE;
        while (m_running && pipe_handle == INVALID_HANDLE_VALUE)
        {
            pipe_handle = CreateFileA(pipe_name, GENERIC_WRITE, 0, nullptr, OPEN_EXISTING, 0, nullptr);
            if (pipe_handle == INVALID_HANDLE_VALUE)
            {
                std::this_thread::sleep_for(std::chrono::milliseconds(250));
            }
        }

        if (!m_running || pipe_handle == INVALID_HANDLE_VALUE)
        {
            if (pipe_handle != INVALID_HANDLE_VALUE)
                CloseHandle(pipe_handle);
            return;
        }

        const auto initialization_msg = "FluxCon Init\n";
        DWORD bytes_written;
        WriteFile(pipe_handle, initialization_msg, static_cast<DWORD>(strlen(initialization_msg)), &bytes_written,
                  nullptr);

        m_init = true;

        while (m_running)
        {
            std::this_thread::sleep_for(std::chrono::milliseconds(10));
        }

        m_init = false;
        CloseHandle(pipe_handle);
    }
} // namespace Flux::Handlers
