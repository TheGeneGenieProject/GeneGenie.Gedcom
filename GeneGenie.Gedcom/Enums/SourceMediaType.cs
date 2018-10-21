// <copyright file="SourceMediaType.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
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
        Other,
    }
}
