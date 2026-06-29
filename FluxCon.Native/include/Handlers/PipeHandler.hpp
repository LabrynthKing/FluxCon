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

// ReSharper disable CppDFANotInitializedField
#pragma once

#include <atomic>
#include <condition_variable>
#include <cstdint>
#include <mutex>
#include <queue>
#include <thread>
#include <vector>

#include <windows.h>

#include "Types.hpp"

namespace Flux::Handlers
{
    struct OutgoingMessage
    {
        MessageType type;
        std::vector<uint8_t> payload;
    };

    class PipeHandler
    {
        // Maybe Lower This?
        static constexpr size_t MaxQueuedMessages = 2048;

        std::thread m_pipe_worker;
        std::atomic<bool> m_running{true};
        std::atomic<PipeState> m_state{PipeState::NotStarted};

        std::mutex m_queue_mutex;
        std::condition_variable m_queue_cv;
        std::queue<OutgoingMessage> m_outgoing;
        std::atomic<uint64_t> m_dropped_messages{0};

        HANDLE m_shutdown_event = nullptr;
        PROCESS_INFORMATION m_child_process{};
        bool m_has_child_process = false;

        void pipe_worker_start();
        bool ensure_child_process_running();
        bool connect_pipe(HANDLE& pipe_handle) const;
        bool write_message(HANDLE pipe_handle, const OutgoingMessage& msg) const;

    public:
        PipeHandler() = default;
        ~PipeHandler();

        PipeHandler(const PipeHandler&) = delete;
        PipeHandler& operator=(const PipeHandler&) = delete;

        static PipeHandler& Get()
        {
            static PipeHandler instance;
            return instance;
        }

        void Initialize();

        PipeState GetState() const { return m_state.load(); }
        uint64_t GetDroppedMessageCount() const { return m_dropped_messages.load(); }

        void Send(MessageType type, std::vector<uint8_t> payload);
    };
} // namespace Flux::Handlers
