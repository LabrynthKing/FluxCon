// FluxCon => A Diagnostic Logger For Subnautica 2
// Copyright (C)  2026  LabrynthKing
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

namespace FluxCon.Types;

internal enum MessageType : uint
{
    Init = 1
}

[Flags]
internal enum ModType : uint
{
    None = 0,
    Lua = 1 << 0,
    Cpp = 1 << 1,
    Blueprint = 1 << 2,
    Pak = 1 << 3,

    // Hybrids
    LuaCpp = Lua | Cpp,
    LuaBlueprint = Lua | Blueprint,
    CppBlueprint = Cpp | Blueprint,
    LuaCppBlueprint = Lua | Cpp | Blueprint
}

internal enum EnabledInfo : uint
{
    Disabled,
    EnabledTxt,
    ModsTxt,
    Auto
}

internal readonly struct ModInfoSimple(string name, ModType type, EnabledInfo enabledInfo, uint? loadOrder)
{
    public readonly string Name = name;
    public readonly ModType Type = type;
    public readonly EnabledInfo EnabledInfo = enabledInfo;
    public readonly uint? LoadOrder = loadOrder;
}