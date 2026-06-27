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

namespace FluxCon;

public static class Program
{
    private static async Task Main(string[] args)
    {
        Console.Title = "FluxCon Logger";
        Console.WriteLine("Init: Starting Test...");

        const string pipeName = "FluxConLogger";

        while (true)
        {
            Console.WriteLine("[System] Waiting for Subnautica 2 to connect...");

            try
            {
                using var pipeServer = new NamedPipeServerStream(
                    pipeName,
                    PipeDirection.In,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous
                );

                await pipeServer.WaitForConnectionAsync();
                Console.WriteLine("[System] Subnautica 2 Engine Connected Safely!");

                using var reader = new StreamReader(pipeServer);

                string? logLine;
                while ((logLine = await reader.ReadLineAsync()) != null) Console.WriteLine(logLine);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[Error] Pipe connection interrupted: {ex.Message}");
            }

            Console.WriteLine("[System] Game disconnected.");
        }
    }
}