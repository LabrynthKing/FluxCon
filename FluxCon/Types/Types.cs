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

/// <summary>
///     Internal MessageType Header Enum For Pipe Communication
/// </summary>
internal enum MessageType : uint
{
    Init = 1,
    Register = 2,
    UnRegister = 3,
    Log = 4,
    LogEx = 5
}

/// <summary>
///     ModType Enum For Well...ModType Duh
/// </summary>
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

/// <summary>
///     EnabledInfo Enum For Well...How The Mod Is Enabled Duh
/// </summary>
internal enum EnabledInfo : uint
{
    Disabled,
    EnabledTxt,
    ModsTxt,
    Auto
}

/// <summary>
///     Enum Of All Log Levels Supported By FluxCon
/// </summary>
internal enum LogLevel : uint
{
    Info,
    Debug,
    Verbose,
    Warning,
    Error,
    Fatal
}

/// <summary>
///     Enum Containing Allowed Exceptions (May Increase In The Future)
/// </summary>
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

/// <summary>
///     Simple Mod Info For UnRegistered Mods And Detected Mods
/// </summary>
/// <param name="Name">The Name Of The Mod</param>
/// <param name="Type">The Type Of The Mod</param>
/// <param name="EnabledInfo">The Way The Mod Is Enabled</param>
/// <param name="LoadOrder">The Load Order (If Applicable) Of The Mod</param>
internal readonly record struct ModInfoSimple(string Name, ModType Type, EnabledInfo EnabledInfo, uint? LoadOrder);

/// <summary>
///     ModInfo For Registered Mods
/// </summary>
/// <param name="name">The Unique Name Of The Mod</param>
/// <param name="displayName">The Display Name Of The Mod</param>
/// <param name="type">The Type Of The Mod</param>
/// <param name="author">The Author Of The Mod</param>
/// <param name="version">The Version Of The Mod</param>
internal struct ModInfo(string name, string displayName, ModType type, string author, string version)
{
    /// <summary>
    ///     ModId As Hashed Mod Unique Name
    /// </summary>
    public readonly uint ModId = HashUtils.GetFNV1aHash(name);

    /// <summary>
    ///     Name Of The Mod
    /// </summary>
    public string Name = name;

    /// <summary>
    ///     Display Name Of The Mod
    /// </summary>
    public string DisplayName = displayName;

    /// <summary>
    ///     The Type Of The Mod
    /// </summary>
    public ModType Type = type;

    /// <summary>
    ///     Author Of The Mod
    /// </summary>
    public string Author = author;

    /// <summary>
    ///     The Version Of The Mod
    /// </summary>
    public string Version = version;

    /// <summary>
    ///     NexusMods Link Of The Mod
    /// </summary>
    public string? NexusLink = null;

    /// <summary>
    ///     GitHub Link Of The Mod
    /// </summary>
    public string? GitHubLink = null;

    /// <summary>
    ///     Dependencies Of The Mod; Stored As Unique Mod Name
    /// </summary>
    public List<string> Dependencies = [];
}