// <copyright file="GedcomLineValueType.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>

namespace GeneGenie.Gedcom.Enums
{
    /// <summary>
    /// Line values in GEDCOM can either be a pointer to another record, or the data itself.
    /// </summary>
    public enum GedcomLineValueType
    {
        /// <summary>
        /// No line value
        /// </summary>
        NoType,

        /// <summary>
        /// Line value is a pointer to another record
        /// </summary>
        PointerType,

        /// <summary>
        /// Line value is the actual data
        /// </summary>
        DataType,
    }
}
