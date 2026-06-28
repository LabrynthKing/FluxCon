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

#include <LuaMadeSimple/LuaMadeSimple.hpp>

namespace Flux::Handlers
{
    class LuaHandler
    {
        inline static std::atomic<bool> m_init{false};

        static void RegisterAll(const std::string& name, const auto& callback, RC::LuaMadeSimple::Lua& main_lua,
                                RC::LuaMadeSimple::Lua& async_lua, RC::LuaMadeSimple::Lua* hook_lua);

    public:
        static bool HasInitialized();
        static void InitLuaAPIs(RC::LuaMadeSimple::Lua& main_lua, RC::LuaMadeSimple::Lua& async_lua,
                                RC::LuaMadeSimple::Lua* hook_lua);
    };
} // namespace Flux::Handlers
