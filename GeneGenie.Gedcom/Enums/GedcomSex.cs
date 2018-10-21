// <copyright file="GedcomSex.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Enums
{
    /// <summary>
    /// The gender / sex of an individual.
    /// </summary>
    public enum GedcomSex
    {
        /// <summary>The sex of the individual has not been set yet and is equivalent to null.</summary>
        NotSet = 0,

        /// <summary>Undetermined from available records and not quite sure what the sex is./// </summary>
        Undetermined,

        /// <summary>The individual is male.</summary>
        Male,

        /// <summary>The individual is female.</summary>
        Female,

        /// <summary>The individual is both male and female.</summary>
        Both,

        /// <summary>The individual is neuter.</summary>
        Neuter,
    }
}
