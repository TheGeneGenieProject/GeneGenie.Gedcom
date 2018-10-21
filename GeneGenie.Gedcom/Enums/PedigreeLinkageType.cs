// <copyright file="PedigreeLinkageType.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Enums
{
    /// <summary>
    /// Indicates the child-to-family relationship for pedigree navigation purposes.
    /// </summary>
    public enum PedigreeLinkageType
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Adopted
        /// </summary>
        Adopted,

        /// <summary>
        /// Biological
        /// </summary>
        Birth,

        /// <summary>
        /// Foster
        /// </summary>
        Foster,

        /// <summary>
        /// Sealing
        /// </summary>
        Sealing,

        // not part of standard GEDCOM
        // Family Tree Maker (at least in some versions) has custom _FREL and _MREL tags
        // on CHIL in the FAM record

        /// <summary>
        /// Father Adopted
        /// </summary>
        FatherAdopted,

        /// <summary>
        /// Mother Adopted
        /// </summary>
        MotherAdopted,
    }
}
