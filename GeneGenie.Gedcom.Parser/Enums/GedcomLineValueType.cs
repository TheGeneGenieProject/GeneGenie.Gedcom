// <copyright file="GedcomLineValueType.cs" company="GeneGenie.com">
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
    /// Line values in GEDCOM can either be a pointer to another record, or the data itself.
    /// </summary>
    public enum GedcomLineValueType
    {
        /// <summary>
        /// No line value
        /// </summary>
        NoType,

        /// <summary>
        /// Line value is a pointer to another record
        /// </summary>
        PointerType,

        /// <summary>
        /// Line value is the actual data
        /// </summary>
        DataType,
    }
}
