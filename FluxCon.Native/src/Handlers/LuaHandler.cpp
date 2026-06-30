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

#include <cstdint>

#include <LuaMadeSimple/LuaMadeSimple.hpp>

#include "API/FluxCon.hpp"
#include "Handlers/LogHandler.hpp"
#include "Handlers/ModHandler.hpp"
#include "Handlers/PipeHandler.hpp"


namespace Flux::Handlers
{
    LuaTypeFactory::LuaTypeFactory(const RC::LuaMadeSimple::Lua& lua) : lua(lua)
    {
        lua_createtable(lua.get_lua_state(), 0, 0);
    }

    void LuaTypeFactory::add_enum(const std::string& key, const std::map<std::string, uint32_t>& entries) const
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

        const uint32_t modId = FluxConAPI::GetFNV1aHash(info.name);

        lua_createtable(lua_state, 0, 7);

        auto push_logger_method = [&](const char* name, const lua_CFunction func)
        {
            lua_pushinteger(lua_state, static_cast<lua_Integer>(modId));
            lua_pushcclosure(lua_state, func, 1);
            lua_setfield(lua_state, -2, name);
        };

        push_logger_method("Info", [](lua_State* L) { return LogFunc(L, LogLevel::Info); });
        push_logger_method("Debug", [](lua_State* L) { return LogFunc(L, LogLevel::Debug); });
        push_logger_method("Verbose", [](lua_State* L) { return LogFunc(L, LogLevel::Verbose); });
        push_logger_method("Warn", [](lua_State* L) { return LogFunc(L, LogLevel::Warning); });
        push_logger_method("Error", [](lua_State* L) { return LogFunc(L, LogLevel::Error); });
        push_logger_method("Fatal", [](lua_State* L) { return LogFunc(L, LogLevel::Fatal); });

        return 1;
    }

    int LuaHandler::UnRegisterModFunc(const RC::LuaMadeSimple::Lua& lua)
    {
        const auto lua_state = lua.get_lua_state();

        if (lua_gettop(lua_state) < 1)
        {
            lua.throw_error("Flux.UnRegisterMod Expects Either The Logger Object Or The Mod Name!");
            return 0;
        }

        uint32_t modId = 0;

        if (lua_istable(lua_state, 1))
        {
            lua_getfield(lua_state, 1, "Info");
            if (lua_iscfunction(lua_state, -1))
            {
                if (const char* upvalue_name = lua_getupvalue(lua_state, -1, 1); upvalue_name != nullptr)
                {
                    modId = static_cast<uint32_t>(lua_tointeger(lua_state, -1));
                    lua_pop(lua_state, 1);
                }
                lua_pop(lua_state, 1);
            }
            else
            {
                lua_pop(lua_state, 1);
                lua.throw_error("Flux.UnRegisterMod: Invalid Logger Instance Object Passed!");
                return 0;
            }
        }
        else if (lua_isstring(lua_state, 1))
        {
            const std::string mod_name = lua_tostring(lua_state, 1);
            modId = FluxConAPI::GetFNV1aHash(mod_name);
        }
        else
        {
            lua.throw_error("Flux.UnRegisterMod Expects Either The Logger Object Or The Mod Name String!");
            return 0;
        }

        ModHandler::UnRegisterMod(modId);

        return 0;
    }

    int LuaHandler::LogFunc(lua_State* lua_state, const LogLevel level)
    {
        const int arg_count = lua_gettop(lua_state);

        const auto modId = static_cast<uint32_t>(lua_tointeger(lua_state, lua_upvalueindex(1)));

        int current_arg = 1;

        if (current_arg <= arg_count && lua_istable(lua_state, current_arg))
        {
            current_arg++;
        }

        if (current_arg > arg_count)
        {
            luaL_error(lua_state, "No Args Provided To Log Function!");
            return 0;
        }

        auto ex = Exception::None;
        if (lua_isinteger(lua_state, current_arg))
        {
            ex = static_cast<Exception>(lua_tointeger(lua_state, current_arg));
            current_arg++;
        }

        if (current_arg > arg_count || !lua_isstring(lua_state, current_arg))
        {
            luaL_error(lua_state, "No Matching Overload For Log Func Found");
            return 0;
        }

        std::string target_msg = lua_tostring(lua_state, current_arg);
        current_arg++;

        if (current_arg <= arg_count)
        {
            lua_getglobal(lua_state, "string");
            lua_getfield(lua_state, -1, "format");

            lua_pushstring(lua_state, target_msg.c_str());

            for (int i = current_arg; i <= arg_count; ++i)
            {
                lua_pushvalue(lua_state, i);
            }

            if (const int total_format_args = (arg_count - current_arg) + 2;
                lua_pcall(lua_state, total_format_args, 1, 0) == 0)
            {
                target_msg = lua_tostring(lua_state, -1);
                lua_pop(lua_state, 1);
                lua_pop(lua_state, 1);
            }
            else
            {
                lua_pop(lua_state, 1);
                lua_pop(lua_state, 1);
                luaL_error(lua_state, "Failed To Evaluate Variable Formatting Layout!");
                return 0;
            }
        }

        if (ex == Exception::None)
        {
            LogHandler::Log(modId, level, target_msg);
        }
        else
        {
            LogHandler::Log(modId, level, target_msg, ex);
        }

        return 0;
    }

    void LuaHandler::RegisterLua(const RC::LuaMadeSimple::Lua& lua)
    {
        const LuaTypeFactory type(lua);

        type.add_enum("LoggerState", LoggerStateTable);
        type.add_enum("ModType", ModTypeTable);
        type.add_enum("LogLevel", LogLevelTable);
        type.add_enum("Exception", ExceptionTable);

        type.add_function("GetLoggerState", GetLoggerStateFunc);
        type.add_function("RegisterMod", RegisterModFunc);
        type.add_function("UnRegisterMod", UnRegisterModFunc);

        type.make_global("Flux");
    }
} // namespace Flux::Handlers
