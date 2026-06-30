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

namespace FluxCon.Types;

/// <summary>
///     Init Model, Nothing Special
/// </summary>
internal record Init;

/// <summary>
///     Mod Registration Model
/// </summary>
/// <param name="ModInfo">The Mod Info From C++</param>
internal record ModRegistration(ModInfo ModInfo);

/// <summary>
///     Mod UnRegistration Model
/// </summary>
/// <param name="ModId">The ModId From C++</param>
internal record ModUnRegistration(uint ModId);

/// <summary>
///     Log Model
/// </summary>
/// <param name="ModId">The ModId From C++</param>
/// <param name="Level">The Log's Level From C++</param>
/// <param name="Message">The Log Message</param>
internal record Log(uint ModId, LogLevel Level, string Message);

/// <summary>
///     Log Model With Custom Exception
/// </summary>
/// <param name="ModId">The ModId From C++</param>
/// <param name="Level">The Log's Level From C++</param>
/// <param name="Message">The Log Message</param>
/// <param name="Ex">Custom Exception Value From C++</param>
internal record LogEx(uint ModId, LogLevel Level, string Message, FluxEx Ex);