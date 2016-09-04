// <copyright file="GedcomCharset.cs" company="GeneGenie.com">
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see http:www.gnu.org/licenses/ .
//
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>

namespace GeneGenie.Gedcom.Parser.Enums
{
    /// <summary>
    /// The encoding used for the GEDCOM file.
    /// </summary>
    public enum GedcomCharset
    {
        /// <summary>Reserved default / error value.</summary>
        Unknown,

        /// <summary>ANSEL encoding.</summary>
        Ansel,

        /// <summary>ANSI encoding.</summary>
        Ansi,

        /// <summary>ASCII encoding.</summary>
        Ascii,

        /// <summary>UTF8 encoding.</summary>
        UTF8,

        /// <summary>UTF16, Big Endian.</summary>
        UTF16BE,

        /// <summary>UTF16, Little Endian.</summary>
        UTF16LE,

        /// <summary>UTF32, Big Endian.</summary>
        UTF32BE,

        /// <summary>UTF32, Little Endian.</summary>
        UTF32LE,

        /// <summary>Encoding was not found. We should default to unknown at the top instead and report error.</summary>
        UnSupported
    }
}
