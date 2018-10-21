// <copyright file="ParserMessageIds.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
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
