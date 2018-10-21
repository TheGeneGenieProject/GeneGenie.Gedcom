// <copyright file="GedcomDatePeriodParserMapping.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
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
