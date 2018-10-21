// <copyright file="GedcomRestrictionNotice.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Enums
{
    /// <summary>
    /// GEDCOM Restriction Types
    /// </summary>
    /// <remarks>
    /// Signifies that access to information has been denied or otherwise restricted.
    /// </remarks>
    public enum GedcomRestrictionNotice
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// Confidential
        /// </summary>
        Confidential,

        /// <summary>
        /// Locked
        /// </summary>
        Locked,

        /// <summary>
        /// Privacy
        /// </summary>
        Privacy,
    }
}
