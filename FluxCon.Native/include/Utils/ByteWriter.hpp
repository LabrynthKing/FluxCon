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

#include <cstdint>
#include <optional>
#include <string>
#include <vector>

namespace Flux
{
    // Basic Ahh Binary Writer
    class ByteWriter
    {
    public:
        std::vector<uint8_t> buffer;

        void WriteU32(const uint32_t v) { append(&v, sizeof(v)); }
        void WriteU8(const uint8_t v) { append(&v, sizeof(v)); }
        void WriteBool(const bool v) { WriteU8(v ? 1 : 0); }

        void WriteString(const std::string& s)
        {
            WriteU32(static_cast<uint32_t>(s.size()));
            append(s.data(), s.size());
        }

        void WriteOptionalString(const std::optional<std::string>& s)
        {
            WriteBool(s.has_value());
            if (s.has_value())
                WriteString(*s);
        }

        void WriteStringList(const std::vector<std::string>& list)
        {
            WriteU32(static_cast<uint32_t>(list.size()));
            for (const auto& s : list)
                WriteString(s);
        }

    private:
        void append(const void* data, const size_t len)
        {
            const auto* p = static_cast<const uint8_t*>(data);
            buffer.insert(buffer.end(), p, p + len);
        }
    };
} // namespace Flux
