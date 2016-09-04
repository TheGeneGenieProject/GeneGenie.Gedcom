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
    /// TODO: Doc
    /// </summary>
    public enum SourceMediaType
    {
        /// <summary>
        /// The none
        /// </summary>
        None,

        /// <summary>
        /// The audio
        /// </summary>
        Audio,

        /// <summary>
        /// The book
        /// </summary>
        Book,

        /// <summary>
        /// The card
        /// </summary>
        Card,

        /// <summary>
        /// The electronic
        /// </summary>
        Electronic,

        /// <summary>
        /// The fiche
        /// </summary>
        Fiche,

        /// <summary>
        /// The film
        /// </summary>
        Film,

        /// <summary>
        /// The magazine
        /// </summary>
        Magazine,

        /// <summary>
        /// The manuscript
        /// </summary>
        Manuscript,

        /// <summary>
        /// The map
        /// </summary>
        Map,

        /// <summary>
        /// The newspaper
        /// </summary>
        Newspaper,

        /// <summary>
        /// The photo
        /// </summary>
        Photo,

        /// <summary>
        /// The tombstone
        /// </summary>
        Tombstone,

        /// <summary>
        /// The video
        /// </summary>
        Video,

        // non standard gedcom media types

        /// <summary>
        /// The civil registry
        /// </summary>
        Civil_Registry,

        /// <summary>
        /// The family archive cd
        /// </summary>
        Family_Archive_CD,

        /// <summary>
        /// The microfilm
        /// </summary>
        Microfilm,

        /// <summary>
        /// The census
        /// </summary>
        Census,

        /// <summary>
        /// The letter
        /// </summary>
        Letter,

        /// <summary>
        /// The official document
        /// </summary>
        Official_Document,

        /// <summary>
        /// The microfiche
        /// </summary>
        Microfiche, // TODO: we should correct this one to be Fiche

        /// <summary>
        /// The other
        /// </summary>
        Other
    }
}
