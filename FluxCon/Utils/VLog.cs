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
using Log = Serilog.Log;

namespace FluxCon.Utils;

/// <summary>
///     The Main FluxCon Logger Class
/// </summary>
internal class VLog : IDisposable
{
    /// <summary>
    ///     Current ModInfo
    /// </summary>
    private readonly ModInfo _modInfo;

    /// <summary>
    ///     Current Mod's Display Name
    /// </summary>
    private readonly string _modName;

    /// <summary>
    ///     Simple Boolean To Check Disposal State
    /// </summary>
    private bool _disposed; // I WILL DISPOSE YOU MUAHAHAH JK

    /// <summary>
    ///     Static Constructor To Initialize Serilog's Logger
    /// </summary>
    static VLog()
    {
        var logFile = Path.Combine(ModsChecker.Win64Dir, "FluxCon.log");

        try
        {
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
            .WriteTo.File(logFile,
                outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level:u3}] [{ModLabel}] {Message:lj}{NewLine}{Exception}");

        Log.Logger = config.CreateLogger();
    }

    /// <summary>
    ///     Instance Constructor For Each Mod's Personal Logger
    /// </summary>
    /// <param name="modInfo">The ModInfo Of The Mod</param>
    public VLog(ModInfo modInfo)
    {
        _modInfo = modInfo;
        _modName = _modInfo.DisplayName;
    }

    /// <summary>
    ///     Disposes The Current Logger, If Disposed, The Logs Will Not Be Written
    /// </summary>
    public void Dispose()
    {
        _disposed = true;
        GC.SuppressFinalize(this); // TODO: Check .NET Docs About This A Lil Cuz I Forgot
    }

    /// <summary>
    ///     Helper Function To Write All Logs
    /// </summary>
    /// <param name="level">The Log Level</param>
    /// <param name="ex">Exception If Required</param>
    /// <param name="message">The Log Message Itself</param>
    /// <param name="args">Any Args For Formatted Logging</param>
    /// <exception cref="ObjectDisposedException">Exception Thrown When Current Logger Has Already Been Disposed</exception>
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
    /// <summary>
    ///     Information Log
    /// </summary>
    /// <param name="message">Message To Log</param>
    public void Info(string message)
    {
        WriteLog(LogEventLevel.Information, null, message);
    }

    /// <summary>
    ///     Information Log
    /// </summary>
    /// <param name="message">Message To Log</param>
    /// <param name="args">Args For Formatted Logging</param>
    public void Info(string message, params object[] args)
    {
        WriteLog(LogEventLevel.Information, null, message, args);
    }

    /// <summary>
    ///     Information Log
    /// </summary>
    /// <param name="ex">Exception If Required</param>
    /// <param name="message">Message To Log</param>
    public void Info(Exception? ex, string message)
    {
        WriteLog(LogEventLevel.Information, ex, message);
    }

    /// <summary>
    ///     Information Log
    /// </summary>
    /// <param name="ex">Exception If Required</param>
    /// <param name="message">Message To Log</param>
    /// <param name="args">Args For Formatted Logging</param>
    public void Info(Exception? ex, string message, params object[] args)
    {
        WriteLog(LogEventLevel.Information, ex, message, args);
    }

    /// <summary>
    ///     Debug Log
    /// </summary>
    /// <param name="message">Message To Log</param>
    public void Debug(string message)
    {
        WriteLog(LogEventLevel.Debug, null, message);
    }

    /// <summary>
    ///     Debug Log
    /// </summary>
    /// <param name="message">Message To Log</param>
    /// <param name="args">Args For Formatted Logging</param>
    public void Debug(string message, params object[] args)
    {
        WriteLog(LogEventLevel.Debug, null, message, args);
    }

    /// <summary>
    ///     Debug Log
    /// </summary>
    /// <param name="ex">Exception If Required</param>
    /// <param name="message">Message To Log</param>
    public void Debug(Exception? ex, string message)
    {
        WriteLog(LogEventLevel.Debug, ex, message);
    }

    /// <summary>
    ///     Debug Log
    /// </summary>
    /// <param name="ex">Exception If Required</param>
    /// <param name="message">Message To Log</param>
    /// <param name="args">Args For Formatted Logging</param>
    public void Debug(Exception? ex, string message, params object[] args)
    {
        WriteLog(LogEventLevel.Debug, ex, message, args);
    }

    /// <summary>
    ///     Verbose Log
    /// </summary>
    /// <param name="message">Message To Log</param>
    public void Verbose(string message)
    {
        WriteLog(LogEventLevel.Verbose, null, message);
    }

    /// <summary>
    ///     Verbose Log
    /// </summary>
    /// <param name="message">Message To Log</param>
    /// <param name="args">Args For Formatted Logging</param>
    public void Verbose(string message, params object[] args)
    {
        WriteLog(LogEventLevel.Verbose, null, message, args);
    }

    /// <summary>
    ///     Verbose Log
    /// </summary>
    /// <param name="ex">Exception If Required</param>
    /// <param name="message">Message To Log</param>
    public void Verbose(Exception? ex, string message)
    {
        WriteLog(LogEventLevel.Verbose, ex, message);
    }

    /// <summary>
    ///     Verbose Log
    /// </summary>
    /// <param name="ex">Exception If Required</param>
    /// <param name="message">Message To Log</param>
    /// <param name="args">Args For Formatted Logging</param>
    public void Verbose(Exception? ex, string message, params object[] args)
    {
        WriteLog(LogEventLevel.Verbose, ex, message, args);
    }

    /// <summary>
    ///     Warning Log
    /// </summary>
    /// <param name="message">Message To Log</param>
    public void Warn(string message)
    {
        WriteLog(LogEventLevel.Warning, null, message);
    }

    /// <summary>
    ///     Warning Log
    /// </summary>
    /// <param name="message">Message To Log</param>
    /// <param name="args">Args For Formatted Logging</param>
    public void Warn(string message, params object[] args)
    {
        WriteLog(LogEventLevel.Warning, null, message, args);
    }

    /// <summary>
    ///     Warning Log
    /// </summary>
    /// <param name="ex">Exception If Required</param>
    /// <param name="message">Message To Log</param>
    public void Warn(Exception? ex, string message)
    {
        WriteLog(LogEventLevel.Warning, ex, message);
    }

    /// <summary>
    ///     Warning Log
    /// </summary>
    /// <param name="ex">Exception If Required</param>
    /// <param name="message">Message To Log</param>
    /// <param name="args">Args For Formatted Logging</param>
    public void Warn(Exception? ex, string message, params object[] args)
    {
        WriteLog(LogEventLevel.Warning, ex, message, args);
    }

    /// <summary>
    ///     Error Log
    /// </summary>
    /// <param name="message">Message To Log</param>
    public void Error(string message)
    {
        WriteLog(LogEventLevel.Error, null, message);
    }

    /// <summary>
    ///     Error Log
    /// </summary>
    /// <param name="message">Message To Log</param>
    /// <param name="args">Args For Formatted Logging</param>
    public void Error(string message, params object[] args)
    {
        WriteLog(LogEventLevel.Error, null, message, args);
    }

    /// <summary>
    ///     Error Log
    /// </summary>
    /// <param name="ex">Exception If Required</param>
    /// <param name="message">Message To Log</param>
    public void Error(Exception? ex, string message)
    {
        WriteLog(LogEventLevel.Error, ex, message);
    }

    /// <summary>
    ///     Error Log
    /// </summary>
    /// <param name="ex">Exception If Required</param>
    /// <param name="message">Message To Log</param>
    /// <param name="args">Args For Formatted Logging</param>
    public void Error(Exception? ex, string message, params object[] args)
    {
        WriteLog(LogEventLevel.Error, ex, message, args);
    }

    /// <summary>
    ///     Fatal Log
    /// </summary>
    /// <param name="message">Message To Log</param>
    public void Fatal(string message)
    {
        WriteLog(LogEventLevel.Fatal, null, message);
    }

    /// <summary>
    ///     Fatal Log
    /// </summary>
    /// <param name="message">Message To Log</param>
    /// <param name="args">Args For Formatted Logging</param>
    public void Fatal(string message, params object[] args)
    {
        WriteLog(LogEventLevel.Fatal, null, message, args);
    }

    /// <summary>
    ///     Fatal Log
    /// </summary>
    /// <param name="ex">Exception If Required</param>
    /// <param name="message">Message To Log</param>
    public void Fatal(Exception? ex, string message)
    {
        WriteLog(LogEventLevel.Fatal, ex, message);
    }

    /// <summary>
    ///     Fatal Log
    /// </summary>
    /// <param name="ex">Exception If Required</param>
    /// <param name="message">Message To Log</param>
    /// <param name="args">Args For Formatted Logging</param>
    public void Fatal(Exception? ex, string message, params object[] args)
    {
        WriteLog(LogEventLevel.Fatal, ex, message, args);
    }
}