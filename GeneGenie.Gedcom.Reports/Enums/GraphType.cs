// <copyright file="GraphType.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Reports.Enums
{
    using System;

    /// <summary>
    /// TODO: Doc
    /// </summary>
    [Flags]
    public enum GraphType
    {
        /// <summary>
        /// Ancestors.
        /// </summary>
        Ancestors = 0,

        /// <summary>
        /// Decendants.
        /// </summary>
        Decendants = 1,

        /// <summary>
        /// Siblings.
        /// </summary>
        Siblings = 2
    }
}
