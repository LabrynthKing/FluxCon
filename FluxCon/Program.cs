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
    private const string Version = "0.0.1.4";

    internal static VLog Logger = null!;

    private static ModHandler _modHandler = new();

    private static readonly ModInfo FluxConModInfo = new()
    {
        Name = "FluxCon",
        DisplayName = "FluxCon",
        Type = ModType.Cpp, // Technically C++ + C# But Eh
        Version = Version,
        Author = "LabrynthKing",
        GitHubLink = "https://github.com/LabrynthKing/FluxCon",
        NexusLink = null,
        Dependencies = []
    };

    private static async Task Main(string[] args)
    {
        // Init Logger First
        Logger = new VLog(FluxConModInfo);

        Logger.Info($"FluxCon Version {Version} Initializing...");

        Logger.Verbose("Starting Pipe Listener...");
        var pipeHandler = new PipeHandler();

        pipeHandler.Start();
        RegisterPipeHandlerActions(pipeHandler);

        Logger.Debug("Pipe Listener Initialized");

        Logger.Info($"FluxCon Version {Version} Initialized");

        // The Below Is Weird
        var shutdownTcs = new TaskCompletionSource();

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            shutdownTcs.TrySetResult();
        };

        AppDomain.CurrentDomain.ProcessExit += (_, _) => shutdownTcs.TrySetResult();

        await shutdownTcs.Task;

        Logger.Info("Shutting Down FluxCon...");
        await pipeHandler.DisposeAsync();
    }

    private static void RegisterPipeHandlerActions(PipeHandler pipeHandler)
    {
        pipeHandler.OnInit += _ =>
        {
            ModsChecker.GetAllMods();

            var totalCount = ModsChecker.AllMods.Count;
            var enabledCount = ModsChecker.AllMods.Count(m => m.EnabledInfo != EnabledInfo.Disabled);

            Logger.Info($"Found {ModsChecker.AllMods.Count} Mods");
            Logger.Info($"Enabled: {enabledCount} | Disabled: {totalCount - enabledCount}");

            // OMG FORMATTING PRO
            Logger.Info("======================== BEGIN MOD LIST ========================");
            var count = 1;
            foreach (var mod in ModsChecker.AllMods)
            {
                Logger.Info(
                    mod.LoadOrder is not null
                        ? $"Mod {count} => {mod.Name} ({LogUtils.ModTypeToString(mod.Type)}) | {LogUtils.EnabledInfoToString(mod.EnabledInfo)} | Load Order: {mod.LoadOrder}"
                        : $"Mod {count} => {mod.Name} ({LogUtils.ModTypeToString(mod.Type)}) | {LogUtils.EnabledInfoToString(mod.EnabledInfo)}");
                count++;
            }

            Logger.Info("========================= END MOD LIST ==========================");

            // Create New ModHandler Instance
            _modHandler = new ModHandler();
        };

        pipeHandler.OnRegister += registration => { _modHandler.RegisterMod(registration.ModInfo); };

        pipeHandler.OnUnRegister += unregister => { _modHandler.UnRegisterMod(unregister.ModId); };

        pipeHandler.OnLog += log => { LogHandler.HandleLog(_modHandler, log.ModId, log.Level, log.Message); };

        pipeHandler.OnLogEx += log =>
        {
            LogHandler.HandleLogEx(_modHandler, log.ModId, log.Level, log.Message, log.Ex);
        };

        pipeHandler.OnError += exception => { Logger.Error(exception, "Pipe Listener Error"); };
    }
}