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

using FluxCon.Types;

namespace FluxCon.Utils;

/// <summary>
///     Simple Logging Utilities
/// </summary>
internal static class LogUtils
{
    /// <summary>
    ///     Converts ModType To Presentable String
    /// </summary>
    /// <param name="modType">The Mod's Type</param>
    /// <returns>GigaChad String Of The Mod's Type</returns>
    public static string ModTypeToString(ModType modType)
    {
        if (modType == ModType.None) return "Unknown";

        var parts = new List<string>();

        if (modType.HasFlag(ModType.Lua)) parts.Add("Lua");
        if (modType.HasFlag(ModType.Cpp)) parts.Add("C++");
        if (modType.HasFlag(ModType.Blueprint)) parts.Add("Blueprint");
        if (modType.HasFlag(ModType.Pak)) parts.Add("Pak");

        return string.Join("/", parts);
    }

    /// <summary>
    ///     Converted EnabledInfo To Presentable String
    /// </summary>
    /// <param name="enabledInfo">The Mod's EnabledInfo</param>
    /// <returns>GigaChad String Of The Mod's EnabledInfo</returns>
    /// <exception cref="ArgumentOutOfRangeException">Should Never Happen, But It's Here Because My IDE Was Crying</exception>
    public static string EnabledInfoToString(EnabledInfo enabledInfo)
    {
        return enabledInfo switch
        {
            EnabledInfo.Disabled => "Disabled",
            EnabledInfo.EnabledTxt => "Enabled By enabled.txt",
            EnabledInfo.ModsTxt => "Enabled By mods.txt",
            EnabledInfo.Auto => "Enabled Automatically",
            _ => throw new ArgumentOutOfRangeException(nameof(enabledInfo), enabledInfo, null)
        };
    }
}