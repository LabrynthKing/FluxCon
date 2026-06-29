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

// ReSharper disable CppDFAConstantFunctionResult
#include "Handlers/LuaHandler.hpp"

#include <LuaMadeSimple/LuaMadeSimple.hpp>

#include "API/FluxCon.hpp"
#include "Handlers/ModHandler.hpp"
#include "Handlers/PipeHandler.hpp"


namespace Flux::Handlers
{
    LuaTypeFactory::LuaTypeFactory(const RC::LuaMadeSimple::Lua& lua) : lua(lua)
    {
        lua_createtable(lua.get_lua_state(), 0, 0);
    }

    void LuaTypeFactory::add_enum(const std::string& key, const std::map<std::string, int>& entries) const
    {
        const auto ta = lua.prepare_new_table();

        for (const auto& [key, value] : entries)
            ta.add_pair(key.c_str(), value);

        lua_setfield(lua.get_lua_state(), -2, key.c_str());
    }

    void LuaTypeFactory::add_function(const std::string& key, const RC::LuaMadeSimple::Lua::LuaFunction& function) const
    {
        lua.register_function(key, function);

        lua_getglobal(lua.get_lua_state(), key.c_str());
        lua_setfield(lua.get_lua_state(), -2, key.c_str());

        lua.set_nil();
        lua_setglobal(lua.get_lua_state(), key.c_str());
    }

    void LuaTypeFactory::make_global(const std::string& key) const { lua_setglobal(lua.get_lua_state(), key.c_str()); }

    int LuaHandler::GetLoggerStateFunc(const RC::LuaMadeSimple::Lua& lua)
    {
        const auto current_state = static_cast<int>(PipeHandler::Get().GetState());

        lua_pushinteger(lua.get_lua_state(), current_state);

        return 1;
    }

    int LuaHandler::RegisterModFunc(const RC::LuaMadeSimple::Lua& lua)
    {
        const auto lua_state = lua.get_lua_state();

        if (!lua_istable(lua_state, 1))
        {
            lua.throw_error("Flux.RegisterMod Expects A Table Containing ModInfo!");
            return 0;
        }

        ModInfo info{};

        auto get_string_field = [&](const char* key) -> std::string
        {
            lua_getfield(lua_state, 1, key);
            std::string result = lua_isstring(lua_state, -1) ? lua_tostring(lua_state, -1) : "";
            lua_pop(lua_state, 1);
            return result;
        };

        info.name = get_string_field("name");
        info.displayName = get_string_field("displayName");
        info.author = get_string_field("author");
        info.version = get_string_field("version");

        if (info.name.empty())
        {
            lua.throw_error("Flux.RegisterMod Failed: 'name' Cannot Be Empty Or Missing!");
            return 0;
        }
        if (info.version.empty())
        {
            lua.throw_error("Flux.RegisterMod Failed: 'version' Cannot Be Empty Or Missing!");
            return 0;
        }

        lua_getfield(lua_state, 1, "type");
        info.type = lua_isinteger(lua_state, -1) ? static_cast<ModType>(lua_tointeger(lua_state, -1)) : ModType::None;
        lua_pop(lua_state, 1);

        lua_getfield(lua_state, 1, "nexusLink");
        if (lua_isstring(lua_state, -1))
            info.nexusLink = lua_tostring(lua_state, -1);
        lua_pop(lua_state, 1);

        lua_getfield(lua_state, 1, "gitHubLink");
        if (lua_isstring(lua_state, -1))
            info.gitHubLink = lua_tostring(lua_state, -1);
        lua_pop(lua_state, 1);

        lua_getfield(lua_state, 1, "dependencies");
        if (lua_istable(lua_state, -1))
        {
            const size_t len = lua_rawlen(lua_state, -1);
            for (size_t i = 1; i <= len; ++i)
            {
                lua_rawgeti(lua_state, -1, i);
                if (lua_isstring(lua_state, -1))
                {
                    info.dependencies.emplace_back(lua_tostring(lua_state, -1));
                }
                lua_pop(lua_state, 1);
            }
        }
        lua_pop(lua_state, 1);

        ModHandler::RegisterMod(info);

        return 0;
    }

    int LuaHandler::UnRegisterModFunc(const RC::LuaMadeSimple::Lua& lua)
    {
        if (!lua.is_string())
        {
            lua.throw_error("Flux.UnRegisterMod Expects The Mod Folder Name!");
        }

        const auto hash = FluxConAPI::GetFNV1aHash(lua.get_string());

        ModHandler::UnRegisterMod(hash);

        return 0;
    }

    void LuaHandler::RegisterLua(const RC::LuaMadeSimple::Lua& lua)
    {
        const LuaTypeFactory type(lua);

        type.add_enum("LoggerState", LoggerStateTable);
        type.add_enum("ModType", ModTypeTable);

        type.add_function("GetLoggerState", GetLoggerStateFunc);
        type.add_function("RegisterMod", RegisterModFunc);
        type.add_function("UnRegisterMod", UnRegisterModFunc);

        type.make_global("Flux");
    }
} // namespace Flux::Handlers
