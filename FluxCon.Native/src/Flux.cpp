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

#include <Mod/CppUserModBase.hpp>

#include <thread>
#include <chrono>
#include <atomic>
#include <iostream>

#include <windows.h>

#include "FluxCon.h"

namespace Flux
{
    using namespace RC::Unreal;

    class FluxCon : public CppUserModBase
    {
        // Use External Thread For Pipe
        std::thread m_pipe_worker;

        // This Is So Reloading Mods Doesn't Crash It Cuz Of Weird Thready Thingy
        std::atomic<bool> m_running{true};

        static void pipe_worker_start(const FluxCon* instance)
        {
            const auto pipe_name = R"(\\.\pipe\FluxConLogger)";

            // Check If It's Already Running
            if (!WaitNamedPipeA(pipe_name, 100))
            {
                STARTUPINFOA si{};
                PROCESS_INFORMATION pi{};

                si.cb = sizeof(si);

                char exe_path[] = R"(.\ue4ss\Mods\FluxCon\FluxCon.exe)";
                const auto work_dir = R"(.\ue4ss\Mods\FluxCon\)";

                if (CreateProcessA(
                    exe_path,
                    nullptr,
                    nullptr,
                    nullptr,
                    FALSE,
                    CREATE_NEW_CONSOLE,
                    nullptr,
                    work_dir,
                    &si,
                    &pi
                ))
                {
                    CloseHandle(pi.hProcess);
                    CloseHandle(pi.hThread);
                }
            }

            auto pipe_handle = INVALID_HANDLE_VALUE;

            while (instance->m_running && pipe_handle == INVALID_HANDLE_VALUE)
            {
                pipe_handle = CreateFileA(
                    pipe_name,
                    GENERIC_WRITE,
                    0,
                    nullptr,
                    OPEN_EXISTING,
                    0,
                    nullptr
                );

                if (pipe_handle == INVALID_HANDLE_VALUE)
                {
                    std::this_thread::sleep_for(std::chrono::milliseconds(250));
                }
            }

            if (!instance->m_running || pipe_handle == INVALID_HANDLE_VALUE)
            {
                if (pipe_handle != INVALID_HANDLE_VALUE) CloseHandle(pipe_handle);
                return;
            }

            const auto initialization_msg = "FluxCon Init\n";

            DWORD bytes_written;
            WriteFile(pipe_handle, initialization_msg, static_cast<DWORD>(strlen(initialization_msg)), &bytes_written,
                      nullptr);

            while (instance->m_running)
            {
                // TODO: Pull Stuff Here
                std::this_thread::sleep_for(std::chrono::milliseconds(10));
            }

            CloseHandle(pipe_handle);
        }

    public:
        FluxCon()
        {
            ModName = STR("FluxCon");
            ModDescription = STR("A Diagnostic Logger For Subnautica 2");
            ModVersion = W(FluxVersion);
            ModAuthors = STR("LabrynthKing");
        }

        ~FluxCon() override
        {
            m_running = false;

            if (m_pipe_worker.joinable())
            {
                m_pipe_worker.join();
            }
        };

        auto on_program_start() -> void override
        {
            m_pipe_worker = std::thread(&FluxCon::pipe_worker_start, this);
        }
    };
}

#define FLUX_API __declspec(dllexport)

extern "C" {
FLUX_API CppUserModBase* start_mod()
{
    return new Flux::FluxCon();
}

FLUX_API void uninstall_mod(const CppUserModBase* mod)
{
    delete mod;
}
}
