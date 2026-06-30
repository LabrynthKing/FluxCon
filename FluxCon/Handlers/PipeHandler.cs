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

using System.IO.Pipes;
using System.Text;
using FluxCon.Types;
using static FluxCon.Program;

namespace FluxCon.Handlers;

/// <summary>
///     Pipe Handler To Manage The Pipe Connection Between C# And C++
/// </summary>
internal sealed class PipeHandler : IAsyncDisposable
{
    /// <summary>
    ///     The Pipe Name
    /// </summary>
    private const string PipeName = "FluxConLogger";

    /// <summary>
    ///     Cancellation Token Source
    /// </summary>
    private readonly CancellationTokenSource _cts = new();

    /// <summary>
    ///     Listening Task Instance
    /// </summary>
    private Task? _listenTask;

    /// <summary>
    ///     Named Pipe Server Stream
    /// </summary>
    private NamedPipeServerStream? _pipeServer;

    /// <summary>
    ///     Disposes Resources Asynchronously
    /// </summary>
    /// <returns>A ValueTask Duh</returns>
    public async ValueTask DisposeAsync()
    {
        await _cts.CancelAsync();

        if (_listenTask is not null)
            try
            {
                await _listenTask;
            }
            catch
            {
                // Ignore
            }

        _pipeServer?.Dispose();
        _cts.Dispose();
    }

    /// <summary>
    ///     Event Fired On Initialization Message
    /// </summary>
    public event Action<Init>? OnInit;

    /// <summary>
    ///     Event Fired On Mod Registration Message
    /// </summary>
    public event Action<ModRegistration>? OnRegister;

    /// <summary>
    ///     Event Fired On Mod Unregistration Message
    /// </summary>
    public event Action<ModUnRegistration>? OnUnRegister;

    /// <summary>
    ///     Event Fired On Standard Log Message
    /// </summary>
    public event Action<Log>? OnLog;

    /// <summary>
    ///     Event Fired On Extended Log Message
    /// </summary>
    public event Action<LogEx>? OnLogEx;

    /// <summary>
    ///     Event Fired On Pipe Exception
    /// </summary>
    public event Action<Exception>? OnError;

    /// <summary>
    ///     Starts The Pipe Listener
    /// </summary>
    public void Start()
    {
        _listenTask = Task.Run(() => ListenLoopAsync(_cts.Token));
    }

    /// <summary>
    ///     Asynchronous Server Connection Listening Loop
    /// </summary>
    /// <param name="ct">The Cancellation Token</param>
    /// <returns>A Task OMG PRO</returns>
    private async Task ListenLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await using (_pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.In, 1,
                             PipeTransmissionMode.Byte,
                             PipeOptions.Asynchronous))
            {
                try
                {
                    Logger.Verbose("Waiting For Pipe Connection...");
                    await _pipeServer.WaitForConnectionAsync(ct);
                    Logger.Debug("Pipe Client Connected");
                    await ReadLoopAsync(_pipeServer, ct);
                }
                catch (OperationCanceledException)
                {
                    Logger.Debug("Pipe Listener Cancelled; Shutting Down");
                    break;
                }
                catch (IOException ex)
                {
                    Logger.Warn(ex, "Pipe Disconnected");
                    OnError?.Invoke(ex);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Pipe Error");
                    OnError?.Invoke(ex);
                }
            }

            _pipeServer = null;

            // Small Delay Before Re-Running
            if (!ct.IsCancellationRequested) await Task.Delay(250, ct).ContinueWith(_ => { }, ct);
        }
    }

    /// <summary>
    ///     Asynchronous Packet Reading Loop
    /// </summary>
    /// <param name="pipe">The Connected Stream Target</param>
    /// <param name="ct">The Cancellation Token</param>
    /// <returns>A Task Strikes Again</returns>
    private async Task ReadLoopAsync(Stream pipe, CancellationToken ct)
    {
        var headerBuf = new byte[8];

        while (!ct.IsCancellationRequested)
        {
            if (!await ReadExactAsync(pipe, headerBuf, 8, ct))
            {
                Logger.Debug("Pipe Closed By Client");
                return; // Pipe Closed
            }

            var type = BitConverter.ToUInt32(headerBuf, 0);
            var length = BitConverter.ToUInt32(headerBuf, 4);

            var payload = length > 0 ? new byte[length] : Array.Empty<byte>();

            if (length > 0 && !await ReadExactAsync(pipe, payload, (int)length, ct))
            {
                Logger.Debug("Pipe Closed By Client Mid-Message");
                return;
            }

            Dispatch((MessageType)type, payload);
        }
    }

    /// <summary>
    ///     Reads Exact Number Of Bytes From Stream
    /// </summary>
    /// <param name="s">The Source Stream</param>
    /// <param name="buf">The Target Data Buffer</param>
    /// <param name="count">The Number Of Bytes To Read</param>
    /// <param name="ct">The Cancellation Token</param>
    /// <returns>True If Successfully Read, False Otherwise</returns>
    private static async Task<bool> ReadExactAsync(Stream s, byte[] buf, int count, CancellationToken ct)
    {
        var read = 0;
        while (read < count)
        {
            var n = await s.ReadAsync(buf.AsMemory(read, count - read), ct);
            if (n == 0)
                return false; // End Closed
            read += n;
        }

        return true;
    }

    /// <summary>
    ///     Dispatches Messages To Corresponding Events
    /// </summary>
    /// <param name="type">The Target Message Type</param>
    /// <param name="payload">The Incoming Message Byte Payload</param>
    private void Dispatch(MessageType type, byte[] payload)
    {
        try
        {
            switch (type)
            {
                case MessageType.Init:
                    Logger.Verbose("Received Init Message");
                    OnInit?.Invoke(new Init());
                    break;

                case MessageType.Register:
                    Logger.Verbose("Received Mod Registration Message");
                    OnRegister?.Invoke(DeserializeModRegistration(payload));
                    break;

                case MessageType.UnRegister:
                    Logger.Verbose("Received Mod UnRegistration Message");
                    OnUnRegister?.Invoke(DeserializeModUnRegistration(payload));
                    break;

                case MessageType.Log:
                    OnLog?.Invoke(DeserializeLog(payload));
                    break;

                case MessageType.LogEx:
                    OnLogEx?.Invoke(DeserializeLogEx(payload));
                    break;

                default:
                    OnError?.Invoke(new InvalidDataException($"Unknown Message Type: {type}"));
                    break;
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Dispatch Failed For {type}");
            OnError?.Invoke(ex);
        }
    }

    /// <summary>
    ///     Reads String Payload Explicitly
    /// </summary>
    /// <param name="buf">The Target Data Buffer</param>
    /// <param name="offset">The Tracked Data Offset Index</param>
    /// <returns>The Processed Target String</returns>
    private static string ReadString(byte[] buf, ref int offset)
    {
        var len = BitConverter.ToUInt32(buf, offset);
        offset += 4;
        var s = Encoding.UTF8.GetString(buf, offset, (int)len);
        offset += (int)len;
        return s;
    }

    /// <summary>
    ///     Reads Optional String Payload Privately
    /// </summary>
    /// <param name="buf">The Target Data Buffer</param>
    /// <param name="offset"> he Tracked Data Offset Index</param>
    /// <returns> The Processed Target String, Or Null If Not present </returns>
    private static string? ReadOptionalString(byte[] buf, ref int offset)
    {
        var hasValue = buf[offset] != 0;
        offset += 1;
        return hasValue ? ReadString(buf, ref offset) : null;
    }

    /// <summary>
    ///     Reads String List Payload Sequentially
    /// </summary>
    /// <param name="buf">The Target Data Buffer</param>
    /// <param name="offset">The Tracked Data Offset Index</param>
    /// <returns>The Parsed Sequential Collection Of Strings</returns>
    private static List<string> ReadStringList(byte[] buf, ref int offset)
    {
        var count = BitConverter.ToUInt32(buf, offset);
        offset += 4;

        var list = new List<string>((int)count);
        for (var i = 0; i < count; i++)
            list.Add(ReadString(buf, ref offset));

        return list;
    }

    /// <summary>
    ///     Reads Unsigned Integer Metadata Inline
    /// </summary>
    /// <param name="buf">The Target Data Buffer</param>
    /// <param name="offset">The Tracked Data Offset Index</param>
    /// <returns>The Read Unsigned Long Value</returns>
    private static uint ReadUInt32(byte[] buf, ref int offset)
    {
        var v = BitConverter.ToUInt32(buf, offset);
        offset += 4;
        return v;
    }

    /// <summary>
    ///     Deserializes Mod Registration Context Data
    /// </summary>
    /// <param name="payload">The Incoming Message Byte Payload</param>
    /// <returns>The Deserialized Mod Registration Target Object</returns>
    private static ModRegistration DeserializeModRegistration(byte[] payload)
    {
        var offset = 0;

        var name = ReadString(payload, ref offset);
        var displayName = ReadString(payload, ref offset);
        var modType = (ModType)ReadUInt32(payload, ref offset);
        var author = ReadString(payload, ref offset);
        var version = ReadString(payload, ref offset);
        var nexusLink = ReadOptionalString(payload, ref offset);
        var gitHubLink = ReadOptionalString(payload, ref offset);
        var dependencies = ReadStringList(payload, ref offset);

        var info = new ModInfo(name, displayName, modType, author, version)
        {
            NexusLink = nexusLink,
            GitHubLink = gitHubLink,
            Dependencies = dependencies
        };

        return new ModRegistration(info);
    }

    /// <summary>
    ///     Deserializes Mod Unregistration Context Data
    /// </summary>
    /// <param name="payload">The Incoming Message Byte Payload</param>
    /// <returns>The Deserialized Mod Unregistration Target Object</returns>
    private static ModUnRegistration DeserializeModUnRegistration(byte[] payload)
    {
        var offset = 0;

        var modId = ReadUInt32(payload, ref offset);

        return new ModUnRegistration(modId);
    }

    /// <summary>
    ///     Deserializes Basic Log Message Payload
    /// </summary>
    /// <param name="payload">The Incoming Message Byte Payload</param>
    /// <returns>The Deserialized Standard Log Target Object</returns>
    private static Log DeserializeLog(byte[] payload)
    {
        var offset = 0;

        var modId = ReadUInt32(payload, ref offset);
        var level = (LogLevel)ReadUInt32(payload, ref offset);
        var message = ReadString(payload, ref offset);

        return new Log(modId, level, message);
    }

    /// <summary>
    ///     Deserializes Extended Log Message Payload
    /// </summary>
    /// <param name="payload">The Incoming Message Byte Payload</param>
    /// <returns>The Deserialized Extended Log Target Object</returns>
    private static LogEx DeserializeLogEx(byte[] payload)
    {
        var offset = 0;

        var modId = ReadUInt32(payload, ref offset);
        var level = (LogLevel)ReadUInt32(payload, ref offset);
        var message = ReadString(payload, ref offset);
        var ex = (FluxEx)ReadUInt32(payload, ref offset);

        return new LogEx(modId, level, message, ex);
    }
}