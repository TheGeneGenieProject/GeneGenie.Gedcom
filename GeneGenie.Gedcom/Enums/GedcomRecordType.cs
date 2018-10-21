// <copyright file="GedcomRecordType.cs" company="GeneGenie.com">
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see http:www.gnu.org/licenses/ .
//
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
