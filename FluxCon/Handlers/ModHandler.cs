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
using FluxCon.Types;
using FluxCon.Utils;
using static FluxCon.Program;

namespace FluxCon.Handlers;

/// <summary>
///     Handles All Registered Mods
/// </summary>
internal class ModHandler
{
    /// <summary>
    ///     Concurrent Dictionary Of All Mod Loggers Using ModId For Fast LookUp
    /// </summary>
    private readonly ConcurrentDictionary<uint, VLog> _loggers = new();

    /// <summary>
    ///     Concurrent Dictionary Of All Registered Mods
    /// </summary>
    public ConcurrentDictionary<uint, ModInfo> Mods = new();

    /// <summary>
    ///     Registers The Mod And Creates A Logger
    /// </summary>
    /// <param name="mod">ModInfo Of The Mod</param>
    public void RegisterMod(ModInfo mod)
    {
        Mods[mod.ModId] = mod;

        Logger.Info("Registered Mod: {0} {1} ({2}) -> ID: {3}", mod.DisplayName, mod.Version,
            LogUtils.ModTypeToString(mod.Type), mod.ModId);

        // Register Logger And Save (Weird Ahh Syntax)
        _loggers.GetOrAdd(mod.ModId, static (_, m) => new VLog(m), mod);
    }

    /// <summary>
    ///     UnRegisters The Mod And Disposes The Logger
    /// </summary>
    /// <param name="modId">The Mod's Hashed ID</param>
    public void UnRegisterMod(uint modId)
    {
        if (Mods.Remove(modId, out var mod))
            Logger.Info("UnRegistered Mod: {0} {1} ({2}) -> ID: {3}", mod.DisplayName, mod.Version,
                LogUtils.ModTypeToString(mod.Type), modId);

        if (_loggers.TryRemove(modId, out var logger)) logger.Dispose();
    }

    /// <summary>
    ///     Gets The Mod's Logger Instance
    /// </summary>
    /// <param name="modId">The Mod's Hashed ID</param>
    /// <returns></returns>
    public VLog? GetLogger(uint modId)
    {
        // ReSharper disable once CanSimplifyDictionaryTryGetValueWithGetValueOrDefault
        return _loggers.TryGetValue(modId, out var logger) ? logger : null;
    }
}