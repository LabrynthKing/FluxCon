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

using System.Collections.Concurrent;
using System.Collections.Immutable;
using FluxCon.Types;

namespace FluxCon.Handlers;

internal static class ModsChecker
{
    private static readonly string AppDir = AppDomain.CurrentDomain.BaseDirectory;

    // FluxCon -> Mods
    private static readonly string ModsDir = Path.GetFullPath(Path.Combine(AppDir, ".."));

    public static ImmutableList<ModInfoSimple> AllMods { get; private set; } = [];

    public static void GetAllMods()
    {
        // Check All Folders In Current Dir, Then Just Assign Mod Stuff
        foreach (var folder in Directory.GetDirectories(ModsDir))
        {
            var modsTxtPath = Path.Combine(ModsDir, "mods.txt");
            var modsTxtRegistry = ParseModsTxt(modsTxtPath);

            var folders = Directory.GetDirectories(ModsDir);

            var baggy = new ConcurrentBag<ModInfoSimple>();

            Parallel.ForEach(folders, folderPath =>
            {
                var modName = Path.GetFileName(folderPath);

                // Ignore shared Directory As It Doesn't Have Use (I Think)
                if (modName.Equals("shared", StringComparison.OrdinalIgnoreCase)) return;

                var type = ModType.None;

                // Check Lua
                if (File.Exists(Path.Combine(folderPath, "Scripts", "main.lua")))
                    type |= ModType.Lua;

                // Check C++
                if (File.Exists(Path.Combine(folderPath, "dlls", "main.dll")))
                    type |= ModType.Cpp;

                // Check BP
                // Mods -> ue4ss -> Win64 -> Binaries
                var bpPakPath = Path.Combine(ModsDir, "..", "..", "..", "Content", "Paks", "LogicMods", modName,
                    $"{modName}.pak");
                if (File.Exists(Path.GetFullPath(bpPakPath)))
                    type |= ModType.Blueprint;

                var enabledInfo = EnabledInfo.Disabled;

                uint? loadOrder = null;

                if (modsTxtRegistry.TryGetValue(modName, out var registryInfo))
                {
                    if (registryInfo.IsEnabled)
                    {
                        enabledInfo = EnabledInfo.ModsTxt;
                        loadOrder = registryInfo.Order + 1;
                    }
                }
                else if (File.Exists(Path.Combine(folderPath, "enabled.txt")))
                {
                    enabledInfo = EnabledInfo.EnabledTxt;
                }
                else if (type.HasFlag(ModType.Blueprint))
                {
                    enabledInfo = EnabledInfo.Auto;
                }

                baggy.Add(new ModInfoSimple(modName, type, enabledInfo, loadOrder));
            });

            AllMods = baggy
                .OrderBy(x => x.LoadOrder.HasValue)
                .ThenBy(x => x.LoadOrder)
                .ToImmutableList();
        }
    }

    private static Dictionary<string, (bool IsEnabled, uint Order)> ParseModsTxt(string path)
    {
        var registry = new Dictionary<string, (bool, uint)>(StringComparer.OrdinalIgnoreCase);
        if (!File.Exists(path)) return registry;

        uint currentOrderIndex = 0;

        foreach (var rawLine in File.ReadLines(path))
        {
            var line = rawLine.Trim();

            // Ignore Comment Stuff
            if (string.IsNullOrEmpty(line) || line.StartsWith(';') || line.StartsWith('#')) continue;

            var separatorIndex = line.IndexOf(':');
            if (separatorIndex == -1) continue;

            var modName = line[..separatorIndex].Trim();
            var stateValue = line[(separatorIndex + 1)..].Trim();

            // 1 -> Enabled; 0 -> Disabled
            if (stateValue.StartsWith('1'))
                registry[modName] = (true, currentOrderIndex++);
            else if (stateValue.StartsWith('0')) registry[modName] = (false, 0);
        }

        return registry;
    }
}