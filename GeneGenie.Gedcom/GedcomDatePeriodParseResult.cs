// <copyright file="GedcomDatePeriodParseResult.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom;

/// <summary>The result of parsing and extracting a date period from a string.</summary>

public class GedcomDatePeriodParseResult
{
    /// <summary>Gets or sets the string that shows the parsed data with the date period extracted.</summary>
    public string DataAfterExtraction { get; set; }

    /// <summary>Gets or sets the date period that has been parsed from the raw text.</summary>
    public GedcomDatePeriod DatePeriod { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the date period was worked out from the date rather than
    /// written in it, such as a year on its own read as a range covering that year.
    /// </summary>
    public bool PeriodInferred { get; set; }
}
