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

using FluxCon.Handlers;
using FluxCon.Types;
using FluxCon.Utils;

namespace FluxCon;

public static class Program
{
    private static ModHandler _modHandler = new();

    private static async Task Main(string[] args)
    {
        VLog.Info("FluxCon Version 0.0.1.2 Initializing...", true);

        VLog.Verbose("Starting Pipe Listener...", true);
        var pipeHandler = new PipeHandler();

        pipeHandler.Start();

        VLog.Debug("Pipe Listener Initialized", true);

        pipeHandler.OnInit += _ =>
        {
            ModsChecker.GetAllMods();

            var totalCount = ModsChecker.AllMods.Count;
            var enabledCount = ModsChecker.AllMods.Count(m => m.EnabledInfo != EnabledInfo.Disabled);

            VLog.Info($"Found {ModsChecker.AllMods.Count} Mods");
            VLog.Info($"Enabled: {enabledCount} | Disabled: {totalCount - enabledCount}");

            // OMG FORMATTING PRO
            VLog.Info("======================== BEGIN MOD LIST ========================");
            var count = 1;
            foreach (var mod in ModsChecker.AllMods)
            {
                VLog.Info(
                    mod.LoadOrder is not null
                        ? $"Mod {count} => {mod.Name} ({LogUtils.ModTypeToString(mod.Type)}) | {LogUtils.EnabledInfoToString(mod.EnabledInfo)} | Load Order: {mod.LoadOrder}"
                        : $"Mod {count} => {mod.Name} ({LogUtils.ModTypeToString(mod.Type)}) | {LogUtils.EnabledInfoToString(mod.EnabledInfo)}");
                count++;
            }

            VLog.Info("========================= END MOD LIST ==========================");

            // Create New ModHandler Instance
            _modHandler = new ModHandler();
        };

        pipeHandler.OnRegister += registration => { _modHandler.RegisterMod(registration.ModInfo); };

        pipeHandler.OnUnRegister += unregister => { _modHandler.UnRegisterMod(unregister.ModId); };

        pipeHandler.OnError += exception => { VLog.Error("Pipe Listener Error", true, exception); };

        VLog.Info("FluxCon Version 0.0.1.2 Initialized");

        var shutdownTcs = new TaskCompletionSource();

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            shutdownTcs.TrySetResult();
        };

        AppDomain.CurrentDomain.ProcessExit += (_, _) => shutdownTcs.TrySetResult();

        await shutdownTcs.Task;

        VLog.Info("Shutting Down FluxCon...");
        await pipeHandler.DisposeAsync();
    }
}