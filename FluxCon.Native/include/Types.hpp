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

#include <cstdint>

namespace Flux
{
    enum class MessageType : uint32_t
    {
        Init = 1,
        Register = 2,
        UnRegister = 3,
        Log = 4,
        LogEx = 5,
    };

    struct MessageHeader
    {
        uint32_t type;
        uint32_t length;
    };

    enum class PipeState : uint8_t
    {
        NotStarted = 0,
        Connecting = 1,
        Connected = 2,
        Reconnecting = 3,
        ShuttingDown = 4,
    };
} // namespace Flux
