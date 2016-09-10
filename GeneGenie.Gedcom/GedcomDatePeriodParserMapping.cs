// <copyright file="GedcomDatePeriodParserMapping.cs" company="GeneGenie.com">
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

    /// <summary>
    /// Maps a date / date range indicator from the GEDCOM file to it's enum.
    /// </summary>
    public class GedcomDatePeriodParserMapping
    {
        /// <summary>Gets or sets the date period that this element maps to.</summary>
        public GedcomDatePeriod MapsTo { get; set; }

        /// <summary>Gets or sets the text that is searched for in the GEDCOM date line.</summary>
        public string Text { get; set; }

        /// <summary>Gets or sets the position of the text that is searched for.</summary>
        public GedcomDatePeriodPosition TextPosition { get; set; }
    }
}
