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

#include <Mod/CppUserModBase.hpp>

#include <iostream>

#include <windows.h>

#include <LuaMadeSimple/LuaMadeSimple.hpp>

#include "FluxCon.h"
#include "Handlers/LuaHandler.hpp"
#include "Handlers/PipeHandler.hpp"

namespace Flux
{
    using namespace RC::Unreal;

    class FluxCon : public CppUserModBase
    {
        Handlers::PipeHandler m_pipe_handler;

    public:
        FluxCon()
        {
            ModName = STR("FluxCon");
            ModDescription = STR("A Diagnostic Logger For Subnautica 2");
            ModVersion = W(FluxVersion);
            ModAuthors = STR("LabrynthKing");
        }

        ~FluxCon() override = default;

        auto on_program_start() -> void override { m_pipe_handler.Initialize(); }

        auto on_lua_start(StringViewType mod_name, LuaMadeSimple::Lua& lua, LuaMadeSimple::Lua& main_lua,
                          LuaMadeSimple::Lua& async_lua, LuaMadeSimple::Lua* hook_lua) -> void override
        {
        }
    };
} // namespace Flux

#define FLUX_API __declspec(dllexport)

extern "C"
{
    FLUX_API CppUserModBase* start_mod() { return new Flux::FluxCon(); }
    FLUX_API void uninstall_mod(const CppUserModBase* mod) { delete mod; }
}
