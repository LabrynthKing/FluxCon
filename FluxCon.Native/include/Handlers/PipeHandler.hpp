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

#pragma once

#include <atomic>
#include <thread>

namespace Flux::Handlers
{
    class PipeHandler
    {
        std::thread m_pipe_worker;

        // I. AM. ATOMIC.
        std::atomic<bool> m_running{true};

        inline static std::atomic<bool> m_init{false};

        void pipe_worker_start() const;

    public:
        PipeHandler() = default;
        ~PipeHandler();

        void Initialize();

        static bool HasInitialized();
    };
} // namespace Flux::Handlers
