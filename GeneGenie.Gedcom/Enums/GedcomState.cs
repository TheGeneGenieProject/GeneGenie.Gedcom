// <copyright file="GedcomState.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>

namespace GeneGenie.Gedcom.Enums
{
    /// <summary>
    /// Defines the parse states for GEDCOM file
    /// </summary>
    public enum GedcomState
    {
        /// <summary>
        /// Reading the current level
        /// </summary>
        Level,

        /// <summary>
        /// Reading the current ID
        /// </summary>
        XrefID,

        /// <summary>
        /// Reading the current tag name
        /// </summary>
        Tag,

        /// <summary>
        /// Reading the value for the current tag
        /// </summary>
        LineValue,
    }
}
