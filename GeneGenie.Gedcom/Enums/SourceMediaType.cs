// <copyright file="SourceMediaType.cs" company="GeneGenie.com">
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
    /// Source Media Type
    /// </summary>
    public enum SourceMediaType
    {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// Audio
        /// </summary>
        Audio,

        /// <summary>
        /// Book
        /// </summary>
        Book,

        /// <summary>
        /// Card
        /// </summary>
        Card,

        /// <summary>
        /// Electronic
        /// </summary>
        Electronic,

        /// <summary>
        /// Fiche
        /// </summary>
        Fiche,

        /// <summary>
        /// Film
        /// </summary>
        Film,

        /// <summary>
        /// Magazine
        /// </summary>
        Magazine,

        /// <summary>
        /// Manuscript
        /// </summary>
        Manuscript,

        /// <summary>
        /// Map
        /// </summary>
        Map,

        /// <summary>
        /// Newspaper
        /// </summary>
        Newspaper,

        /// <summary>
        /// Photo
        /// </summary>
        Photo,

        /// <summary>
        /// Tombstone
        /// </summary>
        Tombstone,

        /// <summary>
        /// Video
        /// </summary>
        Video,

        // non standard gedcom media types

        /// <summary>
        /// Civil Registry
        /// </summary>
        Civil_Registry,

        /// <summary>
        /// Family Archive CD
        /// </summary>
        Family_Archive_CD,

        /// <summary>
        /// Microfilm
        /// </summary>
        Microfilm,

        /// <summary>
        /// Census
        /// </summary>
        Census,

        /// <summary>
        /// Letter
        /// </summary>
        Letter,

        /// <summary>
        /// Official Document
        /// </summary>
        Official_Document,

        /// <summary>
        /// Microfiche
        /// </summary>
        Microfiche, // TODO: we should correct this one to be Fiche

        /// <summary>
        /// Other
        /// </summary>
        Other
    }
}
