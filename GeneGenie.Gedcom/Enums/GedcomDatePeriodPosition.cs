// <copyright file="GedcomDatePeriodPosition.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
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
