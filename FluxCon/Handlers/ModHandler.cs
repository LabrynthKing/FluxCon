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
using FluxCon.Utils;

namespace FluxCon.Handlers;

internal class ModHandler
{
    public Dictionary<uint, ModInfo> Mods = new();

    public void RegisterMod(ModInfo mod)
    {
        Mods[mod.ModId] = mod;

        VLog.Info($"Registered Mod: {mod.Name} {mod.Version} ({LogUtils.ModTypeToString(mod.Type)})");
    }

    public void UnRegisterMod(uint modId)
    {
        if (Mods.Remove(modId, out var mod))
            VLog.Info($"UnRegistered Mod: {mod.Name} {mod.Version} ({LogUtils.ModTypeToString(mod.Type)})");
    }
}