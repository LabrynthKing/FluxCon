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
using Serilog;
using Serilog.Events;

namespace FluxCon.Utils;

public class VLog : IDisposable, IAsyncDisposable
{
    private readonly uint _modId;
    private readonly string _modName;

    public readonly ModInfo ModInfo;

    private bool _disposed; // I WILL DISPOSE YOU MUAHAHAH JK

    static VLog()
    {
        try
        {
            var logFile = Path.Combine(ModsChecker.Win64Dir, "FluxCon.log");

            // Delete Older File
            // TODO: Add A Config To Not Do This
            if (File.Exists(logFile)) File.Delete(logFile);
        }
        catch
        {
            // Shouldn't Happen Unless OS Blocks It I Think
            Console.WriteLine("Error Deleting FluxCon.log!!");
        }

        // Setup Config
        // TODO: Add Configuration Options For This Too
        var config = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Verbose()
#else
            .MinimumLevel.Information()
#endif
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level:u3}] [{ModLabel}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("FluxCon.log",
                outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level:u3}] [{ModLabel}] {Message:lj}{NewLine}{Exception}");

        Log.Logger = config.CreateLogger();
    }

    public VLog(ModInfo modInfo)
    {
        ModInfo = modInfo;
        _modId = ModInfo.ModId;
        _modName = ModInfo.DisplayName;
    }

    public ValueTask DisposeAsync()
    {
        _disposed = true;
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private void WriteLog(LogEventLevel level, Exception? ex, string message, params object[] args)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(VLog),
                $"Cannot Log Via '{_modName}' Because The Mod Has Been UnRegistered!.");

        var context = Log.ForContext("ModLabel", _modName);
        if (ex is null)
            context.Write(level, message, args);
        else
            context.Write(level, ex, message, args);
    }

    // Oh God Time To Type All This Shi
    public void Info(string message)
    {
        WriteLog(LogEventLevel.Information, null, message);
    }

    public void Info(string message, params object[] args)
    {
        WriteLog(LogEventLevel.Information, null, message, args);
    }

    public void Info(Exception? ex, string message)
    {
        WriteLog(LogEventLevel.Information, ex, message);
    }

    public void Info(Exception? ex, string message, params object[] args)
    {
        WriteLog(LogEventLevel.Information, ex, message, args);
    }

    public void Debug(string message)
    {
        WriteLog(LogEventLevel.Debug, null, message);
    }

    public void Debug(string message, params object[] args)
    {
        WriteLog(LogEventLevel.Debug, null, message, args);
    }

    public void Debug(Exception? ex, string message)
    {
        WriteLog(LogEventLevel.Debug, ex, message);
    }

    public void Debug(Exception? ex, string message, params object[] args)
    {
        WriteLog(LogEventLevel.Debug, ex, message, args);
    }

    public void Verbose(string message)
    {
        WriteLog(LogEventLevel.Verbose, null, message);
    }

    public void Verbose(string message, params object[] args)
    {
        WriteLog(LogEventLevel.Verbose, null, message, args);
    }

    public void Verbose(Exception? ex, string message)
    {
        WriteLog(LogEventLevel.Verbose, ex, message);
    }

    public void Verbose(Exception? ex, string message, params object[] args)
    {
        WriteLog(LogEventLevel.Verbose, ex, message, args);
    }

    public void Warn(string message)
    {
        WriteLog(LogEventLevel.Warning, null, message);
    }

    public void Warn(string message, params object[] args)
    {
        WriteLog(LogEventLevel.Warning, null, message, args);
    }

    public void Warn(Exception? ex, string message)
    {
        WriteLog(LogEventLevel.Warning, ex, message);
    }

    public void Warn(Exception? ex, string message, params object[] args)
    {
        WriteLog(LogEventLevel.Warning, ex, message, args);
    }

    public void Error(string message)
    {
        WriteLog(LogEventLevel.Error, null, message);
    }

    public void Error(string message, params object[] args)
    {
        WriteLog(LogEventLevel.Error, null, message, args);
    }

    public void Error(Exception? ex, string message)
    {
        WriteLog(LogEventLevel.Error, ex, message);
    }

    public void Error(Exception? ex, string message, params object[] args)
    {
        WriteLog(LogEventLevel.Error, ex, message, args);
    }

    public void Fatal(string message)
    {
        WriteLog(LogEventLevel.Fatal, null, message);
    }

    public void Fatal(string message, params object[] args)
    {
        WriteLog(LogEventLevel.Fatal, null, message, args);
    }

    public void Fatal(Exception? ex, string message)
    {
        WriteLog(LogEventLevel.Fatal, ex, message);
    }

    public void Fatal(Exception? ex, string message, params object[] args)
    {
        WriteLog(LogEventLevel.Fatal, ex, message, args);
    }
}