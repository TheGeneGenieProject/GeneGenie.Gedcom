// <copyright file="GedcomDatePeriod.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Enums
{
    /// <summary>
    /// How accurate is the date and what range does it span?
    /// </summary>
    public enum GedcomDatePeriod
    {
        /// <summary>A default for dates so that none slip in without being explicitly checked.</summary>
        Unknown = 0,

        /// <summary>A single point in time.</summary>
        Exact,

        /// <summary>Any point in time after.</summary>
        After,

        /// <summary>Any point in time before.</summary>
        Before,

        /// <summary>Any point in time between.</summary>
        Between,

        /// <summary>Roughly near the date.</summary>
        About,

        /// <summary>Calculated / reverse engineered from another piece of data.</summary>
        Calculated,

        /// <summary>An estimated date, likely to be slightly wrong.</summary>
        Estimate,

        /// <summary>What someone thinks the date looks like based on reading old documents.</summary>
        Interpretation,

        /// <summary>A date range.</summary>
        Range,
    }
}
