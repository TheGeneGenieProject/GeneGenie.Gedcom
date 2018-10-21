// <copyright file="GedcomDateType.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Enums
{
    /// <summary>
    /// Calendars recognised in GEDCOM format.
    /// </summary>
    public enum GedcomDateType
    {
        /// <summary>Gregorian calendar.</summary>
        Gregorian,

        /// <summary>Julian calendar.</summary>
        Julian,

        /// <summary>Hebrew calendar.</summary>
        Hebrew,

        /// <summary>French calendar.</summary>
        French,

        /// <summary>Roman calendar.</summary>
        Roman,

        /// <summary>Unknown calendar.</summary>
        Unknown, // TODO: Shouldn't this be first, as in 0 for the default int value?
    }
}
