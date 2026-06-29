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
using FluxCon.Utils;

namespace FluxCon.Handlers;

internal sealed class PipeHandler : IAsyncDisposable
{
    private const string PipeName = "FluxConLogger";

    private readonly CancellationTokenSource _cts = new();
    private Task? _listenTask;
    private NamedPipeServerStream? _pipeServer;

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

    public event Action<Init>? OnInit;

    public event Action<ModRegistration>? OnRegister;

    public event Action<ModUnRegistration>? OnUnRegister;

    public event Action<Exception>? OnError;

    public void Start()
    {
        _listenTask = Task.Run(() => ListenLoopAsync(_cts.Token));
    }

    private async Task ListenLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                _pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);

                VLog.Verbose("Waiting For Pipe Connection...", true);
                await _pipeServer.WaitForConnectionAsync(ct);
                VLog.Debug("Pipe Client Connected", true);
                await ReadLoopAsync(_pipeServer, ct);
            }
            catch (OperationCanceledException)
            {
                VLog.Debug("Pipe Listener Cancelled; Shutting Down", true);
                break;
            }
            catch (IOException ex)
            {
                VLog.Warn($"Pipe Disconnected: {ex.Message}", true);
                OnError?.Invoke(ex);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
            }
            finally
            {
                _pipeServer?.Dispose();
                _pipeServer = null;
            }

            // Small Delay Before Re-Running
            if (!ct.IsCancellationRequested) await Task.Delay(250, ct).ContinueWith(_ => { }, ct);
        }
    }

    private async Task ReadLoopAsync(Stream pipe, CancellationToken ct)
    {
        var headerBuf = new byte[8];

        while (!ct.IsCancellationRequested)
        {
            if (!await ReadExactAsync(pipe, headerBuf, 8, ct))
            {
                VLog.Debug("Pipe Closed By Client");
                return; // Pipe Closed
            }

            var type = BitConverter.ToUInt32(headerBuf, 0);
            var length = BitConverter.ToUInt32(headerBuf, 4);

            var payload = length > 0 ? new byte[length] : Array.Empty<byte>();

            if (length > 0 && !await ReadExactAsync(pipe, payload, (int)length, ct))
            {
                VLog.Debug("Pipe Closed By Client Mid-Message");
                return;
            }

            Dispatch((MessageType)type, payload);
        }
    }

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

    private void Dispatch(MessageType type, byte[] payload)
    {
        try
        {
            switch (type)
            {
                case MessageType.Init:
                    VLog.Verbose("Received Init Message", true);
                    OnInit?.Invoke(new Init());
                    break;

                case MessageType.Register:
                    VLog.Verbose("Received Mod Registration Message");
                    OnRegister?.Invoke(DeserializeModRegistration(payload));
                    break;

                case MessageType.UnRegister:
                    VLog.Verbose("Received Mod UnRegistration Message");
                    OnUnRegister?.Invoke(DeserializeModUnRegistration(payload));
                    break;

                default:
                    OnError?.Invoke(new InvalidDataException($"Unknown Message Type: {type}"));
                    break;
            }
        }
        catch (Exception ex)
        {
            VLog.Error($"Dispatch Failed For {type}");
            OnError?.Invoke(ex);
        }
    }

    private static string ReadString(byte[] buf, ref int offset)
    {
        var len = BitConverter.ToUInt32(buf, offset);
        offset += 4;
        var s = Encoding.UTF8.GetString(buf, offset, (int)len);
        offset += (int)len;
        return s;
    }

    private static string? ReadOptionalString(byte[] buf, ref int offset)
    {
        var hasValue = buf[offset] != 0;
        offset += 1;
        return hasValue ? ReadString(buf, ref offset) : null;
    }

    private static List<string> ReadStringList(byte[] buf, ref int offset)
    {
        var count = BitConverter.ToUInt32(buf, offset);
        offset += 4;

        var list = new List<string>((int)count);
        for (var i = 0; i < count; i++)
            list.Add(ReadString(buf, ref offset));

        return list;
    }

    private static uint ReadUInt32(byte[] buf, ref int offset)
    {
        var v = BitConverter.ToUInt32(buf, offset);
        offset += 4;
        return v;
    }

    private static ModRegistration DeserializeModRegistration(byte[] payload)
    {
        var offset = 0;

        var name = ReadString(payload, ref offset);
        var modType = (ModType)ReadUInt32(payload, ref offset);
        var author = ReadString(payload, ref offset);
        var version = ReadString(payload, ref offset);
        var nexusLink = ReadOptionalString(payload, ref offset);
        var gitHubLink = ReadOptionalString(payload, ref offset);
        var dependencies = ReadStringList(payload, ref offset);

        var info = new ModInfo(name, modType, author, version)
        {
            NexusLink = nexusLink,
            GitHubLink = gitHubLink,
            Dependencies = dependencies
        };

        return new ModRegistration(info);
    }

    private static ModUnRegistration DeserializeModUnRegistration(byte[] payload)
    {
        var offset = 0;

        var modId = ReadUInt32(payload, ref offset);

        return new ModUnRegistration(modId);
    }
}