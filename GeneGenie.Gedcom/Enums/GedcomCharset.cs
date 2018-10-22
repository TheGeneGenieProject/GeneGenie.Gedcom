// <copyright file="GedcomCharset.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>

namespace GeneGenie.Gedcom.Enums
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
        Unsupported,
    }
}
