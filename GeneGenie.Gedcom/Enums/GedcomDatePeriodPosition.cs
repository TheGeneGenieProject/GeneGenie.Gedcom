// <copyright file="GedcomDatePeriodPosition.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Enums
{
    /// <summary>When parsing date formats dates can be prefixed but are sometimes suffixed.
    /// This defines where to look for specific date period indicators.</summary>
    public enum GedcomDatePeriodPosition
    {
        /// <summary>Error state for uninitialised instances.</summary>
        NotSet = 0,

        /// <summary>The text denoting the date period is before the dates.</summary>
        Prefix = 1,

        /// <summary>The text denoting the date period is after the dates.</summary>
        Suffix = 2,
    }
}
