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

using FluxCon.Utils;

namespace FluxCon.Types;

internal enum MessageType : uint
{
    Init = 1,
    Register = 2,
    UnRegister = 3,
    Log = 4,
    LogEx = 5
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

internal enum LogLevel : uint
{
    Info,
    Debug,
    Verbose,
    Warning,
    Error,
    Fatal
}

internal enum FluxEx : uint
{
    None,
    UnknownRuntimeError,
    Timeout,
    InvalidCast,
    KeyNotFound,
    PathNotFound,
    NullReference,
    InvalidArgument,
    IndexOutOfBounds,
    InvalidOperation,
    ApiVersionMismatch
}

internal readonly record struct ModInfoSimple(string Name, ModType Type, EnabledInfo EnabledInfo, uint? LoadOrder);

internal struct ModInfo(string name, string displayName, ModType type, string author, string version)
{
    // Use Hash For ModID, Why? IDK I Just Wanna
    public readonly uint ModId = HashUtils.GetFNV1aHash(name);

    // Basic Stuff
    public string Name = name;
    public string DisplayName = displayName;
    public ModType Type = type;
    public string Author = author;
    public string Version = version;

    // Links
    public string? NexusLink = null;
    public string? GitHubLink = null;

    // Other Stuff
    public List<string> Dependencies = [];
}