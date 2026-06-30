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
#include <map>
#include <string>

#include <LuaMadeSimple/LuaMadeSimple.hpp>

#include "API/FluxCon.hpp"

namespace Flux::Handlers
{
    const auto LoggerStateTable = std::map<std::string, uint32_t>{
        {"NotStarted", 0}, {"Connecting", 1}, {"Connected", 2}, {"Reconnecting", 3}, {"ShuttingDown", 4},
    };

    const auto ModTypeTable = std::map<std::string, uint32_t>{
        {"None", 0},   {"Lua", 1},          {"Cpp", 2},          {"Blueprint", 4},      {"Pak", 8},
        {"LuaCpp", 3}, {"LuaBlueprint", 5}, {"CppBlueprint", 6}, {"LuaCppBlueprint", 7}};

    const auto LogLevelTable = std::map<std::string, uint32_t>{{"Info", 0},    {"Debug", 1}, {"Verbose", 2},
                                                               {"Warning", 3}, {"Error", 4}, {"Fatal", 5}};

    const auto ExceptionTable = std::map<std::string, uint32_t>{{"None", 0},
                                                                {"UnknownRuntimeError", 1},
                                                                {"Timeout", 2},
                                                                {"InvalidCast", 3},
                                                                {"KeyNotFound", 4},
                                                                {"PathNotFound", 5},
                                                                {"NullReference", 6},
                                                                {"InvalidArgument", 7},
                                                                {"IndexOutOfBounds", 8},
                                                                {"InvalidOperation", 9},
                                                                {"ApiVersionMismatch", 10}};

    // Reference:- https://github.com/LimoDerEchte/SDF/blob/master/src/api/lua/LuaStatics.hpp
    // Reference 2:- https://github.com/LimoDerEchte/SDF/blob/master/src/api/lua/LuaStatics.cpp
    // Original Work:- Copyright (C) 2026  Julian "LimoDerEchte" Vogel || GNU GPLv3
    class LuaTypeFactory
    {
        const RC::LuaMadeSimple::Lua& lua;

    public:
        explicit LuaTypeFactory(const RC::LuaMadeSimple::Lua& lua);

        void add_enum(const std::string& key, const std::map<std::string, uint32_t>& entries) const;
        void add_function(const std::string& key, const RC::LuaMadeSimple::Lua::LuaFunction& function) const;

        void make_global(const std::string& key) const;
    };

    class LuaHandler
    {
        static int GetLoggerStateFunc(const RC::LuaMadeSimple::Lua& lua);
        static int RegisterModFunc(const RC::LuaMadeSimple::Lua& lua);
        static int UnRegisterModFunc(const RC::LuaMadeSimple::Lua& lua);

        static int LogFunc(lua_State* lua_state, LogLevel level);

    public:
        static void RegisterLua(const RC::LuaMadeSimple::Lua& lua);
    };
} // namespace Flux::Handlers
