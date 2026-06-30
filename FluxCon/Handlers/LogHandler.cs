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

namespace FluxCon.Handlers;

internal static class LogHandler
{
    public static void HandleLog(ModHandler handler, uint modId, LogLevel level, string message)
    {
        var logger = handler.GetLogger(modId);

        if (logger is null) return;

        switch (level)
        {
            case LogLevel.Info:
                logger.Info(message);
                break;
            case LogLevel.Debug:
                logger.Debug(message);
                break;
            case LogLevel.Verbose:
                logger.Verbose(message);
                break;
            case LogLevel.Warning:
                logger.Warn(message);
                break;
            case LogLevel.Error:
                logger.Error(message);
                break;
            case LogLevel.Fatal:
                logger.Fatal(message);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }

    public static void HandleLogEx(ModHandler handler, uint modId, LogLevel level, string message, FluxEx ex)
    {
        var logger = handler.GetLogger(modId);

        if (logger is null) return;

        var exception = CastExceptionSpell(ex);

        switch (level)
        {
            case LogLevel.Info:
                logger.Info(exception, message);
                break;
            case LogLevel.Debug:
                logger.Debug(exception, message);
                break;
            case LogLevel.Verbose:
                logger.Verbose(exception, message);
                break;
            case LogLevel.Warning:
                logger.Warn(exception, message);
                break;
            case LogLevel.Error:
                logger.Error(exception, message);
                break;
            case LogLevel.Fatal:
                logger.Fatal(exception, message);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }

    private static Exception? CastExceptionSpell(FluxEx ex)
    {
        return ex switch
        {
            FluxEx.None => null,
            FluxEx.UnknownRuntimeError => new Exception(),
            FluxEx.Timeout => new TimeoutException(),
            FluxEx.InvalidCast => new InvalidCastException(),
            FluxEx.KeyNotFound => new KeyNotFoundException(),
            FluxEx.PathNotFound => new DirectoryNotFoundException(),
            FluxEx.NullReference => new NullReferenceException(),
            FluxEx.InvalidArgument => new ArgumentException(),
            FluxEx.IndexOutOfBounds => new IndexOutOfRangeException(),
            FluxEx.InvalidOperation => new InvalidOperationException(),
            FluxEx.ApiVersionMismatch => new Exception("API Version Mismatch"),
            _ => throw new ArgumentOutOfRangeException(nameof(ex), ex, null)
        };
    }
}