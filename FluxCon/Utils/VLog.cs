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

using Serilog;

#pragma warning disable CA2254

namespace FluxCon.Utils;

internal static class VLog
{
    static VLog()
    {
        if (File.Exists("FluxCon.log")) File.Delete("FluxCon.log");

        var config = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Verbose()
#else
            .MinimumLevel.Information()
#endif
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level:u3}] [{Thing}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("FluxCon.log",
                outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level:u3}] [{Thing}] {Message:lj}{NewLine}{Exception}");

        Log.Logger = config.CreateLogger();
    }


    public static void Info(string message, bool init = false, Exception? ex = null)
    {
        Log.ForContext("Thing", init ? "Init" : "FluxCon").Information(ex, message);
    }

    public static void Debug(string message, bool init = false, Exception? ex = null)
    {
        Log.ForContext("Thing", init ? "Init" : "FluxCon").Debug(ex, message);
    }

    public static void Verbose(string message, bool init = false, Exception? ex = null)
    {
        Log.ForContext("Thing", init ? "Init" : "FluxCon").Verbose(ex, message);
    }

    public static void Warn(string message, bool init = false, Exception? ex = null)
    {
        Log.ForContext("Thing", init ? "Init" : "FluxCon").Warning(ex, message);
    }

    public static void Error(string message, bool init = false, Exception? ex = null)
    {
        Log.ForContext("Thing", init ? "Init" : "FluxCon").Error(ex, message);
    }

    public static void Fatal(string message, bool init = false, Exception? ex = null)
    {
        Log.ForContext("Thing", init ? "Init" : "FluxCon").Fatal(ex, message);
    }
}