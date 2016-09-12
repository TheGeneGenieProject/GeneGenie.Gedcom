// <copyright file="ParserMessageIds.cs" company="GeneGenie.com">
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
    /// <summary>
    /// Informational, warnings and error codes for the import.
    /// Codes;
    ///  001 - 100 - Informational
    ///  101 - 200 - Warnings
    ///  201 - 300 - Errors
    /// </summary>
    public enum ParserMessageIds
    {
        /// <summary>So that we don't accidentally default to the first id if not initialised, we use unknown.</summary>
        Unknown = 0,

        /// <summary>We only received a year as input which is ambiguous. Interpreted as a range, the original date was kept.</summary>
        InterpretedAsYearRange = 1,

        /// <summary>We only received a year and date as input which is ambiguous. Interpreted as a range, the original date was kept.</summary>
        InterpretedAsMonthRange = 2,

        /// <summary>Parsing a date led to an invalid day of the month which was adjusted.</summary>
        DayOfDateAdjusted = 101,

        /// <summary>The date could not be turned into a valid date time.</summary>
        DateIsNotValid = 201,
    }
}
