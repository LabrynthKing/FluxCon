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

#include "Handlers/ModHandler.hpp"

#include "API/FluxCon.hpp"
#include "Handlers/PipeHandler.hpp"
#include "Utils/ByteWriter.hpp"

namespace Flux::Handlers
{
    void ModHandler::RegisterMod(const ModInfo& info)
    {
        ByteWriter w;

        w.WriteString(info.name);
        w.WriteU32(static_cast<uint32_t>(info.type));
        w.WriteString(info.author);
        w.WriteString(info.version);
        w.WriteOptionalString(info.nexusLink);
        w.WriteOptionalString(info.gitHubLink);
        w.WriteStringList(info.dependencies);

        PipeHandler::Get().Send(MessageType::Register, std::move(w.buffer));
    }

    void ModHandler::UnRegisterMod(const uint32_t modId)
    {
        ByteWriter w;

        w.WriteU32(modId);

        PipeHandler::Get().Send(MessageType::UnRegister, std::move(w.buffer));
    }
} // namespace Flux::Handlers
