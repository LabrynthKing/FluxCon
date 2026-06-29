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

#include <map>

#include <LuaMadeSimple/LuaMadeSimple.hpp>

namespace Flux::Handlers
{
    const auto LoggerStateTable = std::map<std::string, int>{
        {"NotStarted", 0}, {"Connecting", 1}, {"Connected", 2}, {"Reconnecting", 3}, {"ShuttingDown", 4},
    };

    const auto ModTypeTable = std::map<std::string, int>{
        {"None", 0},   {"Lua", 1},          {"Cpp", 2},          {"Blueprint", 4},      {"Pak", 8},
        {"LuaCpp", 3}, {"LuaBlueprint", 5}, {"CppBlueprint", 6}, {"LuaCppBlueprint", 7}};

    // Reference:- https://github.com/LimoDerEchte/SDF/blob/master/src/api/lua/LuaStatics.hpp
    // Reference 2:- https://github.com/LimoDerEchte/SDF/blob/master/src/api/lua/LuaStatics.cpp
    // Original Work:- Copyright (C) 2026  Julian "LimoDerEchte" Vogel || GNU GPLv3
    class LuaTypeFactory
    {
        const RC::LuaMadeSimple::Lua& lua;

    public:
        explicit LuaTypeFactory(const RC::LuaMadeSimple::Lua& lua);

        void add_enum(const std::string& key, const std::map<std::string, int>& entries) const;
        void add_function(const std::string& key, const RC::LuaMadeSimple::Lua::LuaFunction& function) const;

        void make_global(const std::string& key) const;
    };

    class LuaHandler
    {
        static int GetLoggerStateFunc(const RC::LuaMadeSimple::Lua& lua);
        static int RegisterModFunc(const RC::LuaMadeSimple::Lua& lua);
        static int UnRegisterModFunc(const RC::LuaMadeSimple::Lua& lua);

    public:
        static void RegisterLua(const RC::LuaMadeSimple::Lua& lua);
    };
} // namespace Flux::Handlers
