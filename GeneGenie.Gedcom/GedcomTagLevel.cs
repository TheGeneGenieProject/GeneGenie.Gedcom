// <copyright file="GedcomTagLevel.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Helpers
{
    /// <summary>
    /// Used by a stack to store a tag and level for tracking the parsing process.
    /// </summary>
    public class GedcomTagLevel
    {
        /// <summary>Gets or sets the current tag name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the current tag level.</summary>
        public int Level { get; set; }
    }
}
