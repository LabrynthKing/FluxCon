// FluxCon => A Diagnostic Logger For Subnautica 2
// Copyright (C) 2026 LabrynthKing
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

#pragma once

#include "API/FluxCon.hpp"

namespace Flux
{
    class FluxConAPI_Impl final : public FluxConAPI
    {
    protected:
        LoggerState GetLoggerStateInternal() override;
        void RegisterModInternal(const ModInfo& info) override;
        void UnRegisterModInternal(uint32_t modId) override;
        void LogInternal(uint32_t modId, LogLevel level, const std::string& message) override;
        void LogInternal(uint32_t modId, LogLevel level, const std::string& message, Exception ex) override;

    public:
        static FluxConAPI_Impl* InternalInstance();
    };
} // namespace Flux
