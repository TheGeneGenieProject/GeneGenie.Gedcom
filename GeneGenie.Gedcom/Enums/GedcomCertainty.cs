// <copyright file="GedcomCertainty.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Enums
{
    /// <summary>
    /// Indicates the credibility of a piece of information, based upon its supporting evidence.
    /// </summary>
    public enum GedcomCertainty
    {
        /// <summary>
        /// Unreliable
        /// </summary>
        Unreliable = 0,

        /// <summary>
        /// Questionable
        /// </summary>
        Questionable = 1,

        /// <summary>
        /// Secondary
        /// </summary>
        Secondary = 2,

        /// <summary>
        /// Primary
        /// </summary>
        Primary = 3,

        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 4,
    }
}
