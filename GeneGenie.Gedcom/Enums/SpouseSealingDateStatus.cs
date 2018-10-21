// <copyright file="SpouseSealingDateStatus.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2017 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Enums
{
    using System.ComponentModel;

    /// <summary>
    /// Status of a spouse sealing at a specific date.
    /// </summary>
    public enum SpouseSealingDateStatus
    {
        /// <summary>
        /// Status not set, record should not be output to GEDCOM file.
        /// </summary>
        NotSet = 0,

        /// <summary>
        /// Canceled and considered invalid.
        /// </summary>
        CANCELED = 1,

        /// <summary>
        /// Completed but the date is not known.
        /// </summary>
        COMPLETED = 2,

        /// <summary>
        /// Patron excluded this ordinance from being cleared in this submission.
        /// </summary>
        EXCLUDED = 3,

        /// <summary>
        /// This ordinance is not authorized.
        /// </summary>
        DNS = 4,

        /// <summary>
        /// This ordinance is not authorized, previous sealing cancelled.
        /// </summary>
        [Description("DNS/CAN")]
        DNS_CAN = 5,

        /// <summary>
        /// Ordinance is likely completed, another ordinance for this person was converted
        /// from temple records of work completed before 1970, therefore this ordinance is
        /// assumed to be complete until all records are converted.
        /// </summary>
        [Description("PRE-1970")]
        PRE_1970 = 6,

        /// <summary>
        /// Ordinance was previously submitted.
        /// </summary>
        SUBMITTED = 7,

        /// <summary>
        /// Data for clearing ordinance request was insufficient.
        /// </summary>
        UNCLEARED = 8,
    }
}
