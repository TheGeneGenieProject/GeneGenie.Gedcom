// <copyright file="GedcomDatePeriodParseResult.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom
{
    using GeneGenie.Gedcom.Enums;

    /// <summary>The result of parsing and extracting a date period from a string.</summary>
    public class GedcomDatePeriodParseResult
    {
        /// <summary>Gets or sets the string that shows the parsed data with the date period extracted.</summary>
        public string DataAfterExtraction { get; set; }

        /// <summary>Gets or sets the date period that has been parsed from the raw text.</summary>
        public GedcomDatePeriod DatePeriod { get; set; }
    }
}
