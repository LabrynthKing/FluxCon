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

#include "Handlers/LuaHandler.hpp"

#include <LuaMadeSimple/LuaMadeSimple.hpp>

#include "Handlers/PipeHandler.hpp"

// TODO: THIS IS SO WRONG FIX THIS
namespace Flux::Handlers
{
    using namespace RC::LuaMadeSimple;

    void LuaHandler::RegisterAll(const std::string& name, const auto& callback, Lua& main_lua, Lua& async_lua,
                                 Lua* hook_lua)
    {
        main_lua.register_function(name, callback);
        async_lua.register_function(name, callback);

        if (hook_lua != nullptr)
        {
            hook_lua->register_function(name, callback);
        }
    }

    bool LuaHandler::HasInitialized() { return m_init.load(); }

    // ReSharper disable once CppParameterMayBeConstPtrOrRef
    void LuaHandler::InitLuaAPIs(Lua& main_lua, Lua& async_lua, Lua* hook_lua)
    {
        // Register Init Func
        RegisterAll(
            "IsLoggerInit",
            [](const RC::LuaMadeSimple::Lua& lua_ctx) -> int
            {
                if (lua_ctx.get_stack_size() >= 1)
                {
                    lua_ctx.throw_error("IsLoggerInit Has No Parameters But 1 Or More Than 1 Were Provided!");
                    return 0;
                }

                lua_State* L = lua_ctx.get_lua_state();
                lua_pushboolean(L, PipeHandler::Get().HasInitialized());
                return 1;
            },
            main_lua, async_lua, hook_lua);

        // All APIs Registered
        m_init = true;
    }
} // namespace Flux::Handlers
