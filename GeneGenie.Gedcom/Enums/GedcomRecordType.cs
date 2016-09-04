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
    /// TODO: Doc
    /// </summary>
    public enum GedcomRecordType
    {
        /// <summary>
        /// The generic record
        /// </summary>
        GenericRecord = 0,

        /// <summary>
        /// The family
        /// </summary>
        Family,

        /// <summary>
        /// The individual
        /// </summary>
        Individual,

        /// <summary>
        /// The multimedia
        /// </summary>
        Multimedia,

        /// <summary>
        /// The note
        /// </summary>
        Note,

        /// <summary>
        /// The repository
        /// </summary>
        Repository,

        /// <summary>
        /// The source
        /// </summary>
        Source,

        /// <summary>
        /// The submitter
        /// </summary>
        Submitter,

        // non top level records

        /// <summary>
        /// The submission
        /// </summary>
        Submission,

        /// <summary>
        /// The event
        /// </summary>
        Event,

        /// <summary>
        /// The family event
        /// </summary>
        FamilyEvent,

        /// <summary>
        /// The place
        /// </summary>
        Place,

        /// <summary>
        /// The source citation
        /// </summary>
        SourceCitation,

        /// <summary>
        /// The family link
        /// </summary>
        FamilyLink,

        /// <summary>
        /// The association
        /// </summary>
        Association,

        /// <summary>
        /// The name
        /// </summary>
        Name,

        /// <summary>
        /// The individual event
        /// </summary>
        IndividualEvent,

        /// <summary>
        /// The date
        /// </summary>
        Date,

        /// <summary>
        /// The repository citation
        /// </summary>
        RepositoryCitation,

        // GEDCOM allows custom records, beginging with _

        /// <summary>
        /// The custom record
        /// </summary>
        CustomRecord,

        /// <summary>
        /// The header
        /// </summary>
        Header
    }
}
