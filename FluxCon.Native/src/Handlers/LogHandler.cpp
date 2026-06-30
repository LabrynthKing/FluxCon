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

#include "Handlers/LogHandler.hpp"

#include "Handlers/PipeHandler.hpp"
#include "Types.hpp"
#include "Utils/ByteWriter.hpp"

namespace Flux::Handlers
{
    void LogHandler::Log(const uint32_t modId, LogLevel level, const std::string& message)
    {
        ByteWriter w;

        w.WriteU32(modId);
        w.WriteU32(static_cast<uint32_t>(level));
        w.WriteString(message);

        PipeHandler::Get().Send(MessageType::Log, std::move(w.buffer));
    }

    void LogHandler::Log(const uint32_t modId, LogLevel level, const std::string& message, Exception ex)
    {
        ByteWriter w;

        w.WriteU32(modId);
        w.WriteU32(static_cast<uint32_t>(level));
        w.WriteString(message);
        w.WriteU32(static_cast<uint32_t>(ex));

        PipeHandler::Get().Send(MessageType::LogEx, std::move(w.buffer));
    }
} // namespace Flux::Handlers
