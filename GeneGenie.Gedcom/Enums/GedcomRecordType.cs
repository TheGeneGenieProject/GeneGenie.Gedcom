// <copyright file="GedcomRecordType.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Enums
{
    /// <summary>
    /// GEDCOM Record Types
    /// </summary>
    public enum GedcomRecordType
    {
        /// <summary>
        /// Generic Record
        /// </summary>
        GenericRecord = 0,

        /// <summary>
        /// Family
        /// </summary>
        Family,

        /// <summary>
        /// Individual
        /// </summary>
        Individual,

        /// <summary>
        /// Multimedia
        /// </summary>
        Multimedia,

        /// <summary>
        /// Note
        /// </summary>
        Note,

        /// <summary>
        /// Repository
        /// </summary>
        Repository,

        /// <summary>
        /// Source
        /// </summary>
        Source,

        /// <summary>
        /// Submitter
        /// </summary>
        Submitter,

        // non top level records

        /// <summary>
        /// Submission
        /// </summary>
        Submission,

        /// <summary>
        /// Event
        /// </summary>
        Event,

        /// <summary>
        /// Family Event
        /// </summary>
        FamilyEvent,

        /// <summary>
        /// Place
        /// </summary>
        Place,

        /// <summary>
        /// Source Citation
        /// </summary>
        SourceCitation,

        /// <summary>
        /// Latter Day Saints Spouse Sealing record for a married couple.
        /// </summary>
        SpouseSealing,

        /// <summary>
        /// Family Link
        /// </summary>
        FamilyLink,

        /// <summary>
        /// Association
        /// </summary>
        Association,

        /// <summary>
        /// Name
        /// </summary>
        Name,

        /// <summary>
        /// Individual Event
        /// </summary>
        IndividualEvent,

        /// <summary>
        /// Date
        /// </summary>
        Date,

        /// <summary>
        /// Repository Citation
        /// </summary>
        RepositoryCitation,

        // GEDCOM allows custom records, beginging with _

        /// <summary>
        /// Custom Record
        /// </summary>
        CustomRecord,

        /// <summary>
        /// Header
        /// </summary>
        Header,
    }
}
