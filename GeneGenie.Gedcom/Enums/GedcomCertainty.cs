// <copyright file="GedcomCertainty.cs" company="GeneGenie.com">
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
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Enums
{
    /// <summary>
    /// Indicates the credibility of a piece of information, based upon its supporting evidence.
    /// </summary>
    public enum GedcomCertainty
    {
        /// <summary>
        /// Unreliable
        /// </summary>
        Unreliable = 0,

        /// <summary>
        /// Questionable
        /// </summary>
        Questionable = 1,

        /// <summary>
        /// Secondary
        /// </summary>
        Secondary = 2,

        /// <summary>
        /// Primary
        /// </summary>
        Primary = 3,

        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 4
    }
}
