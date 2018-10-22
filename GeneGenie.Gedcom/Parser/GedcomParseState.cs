// <copyright file="GedcomParseState.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>

namespace GeneGenie.Gedcom.Parser
{
    using System;
    using System.Collections.Generic;
    using GeneGenie.Gedcom.Helpers;

    /// <summary>
    /// GedcomParseState is used to maintain the current parser status
    /// for GedcomRecordReader
    /// </summary>
    public class GedcomParseState
    {
        private GedcomTagLevel[] pairPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomParseState"/> class.
        /// </summary>
        public GedcomParseState()
        {
            Records = new Stack<GedcomRecord>();
            Database = new GedcomDatabase();

            // max level of 99, pre alloc size of stack
            PreviousTags = new Stack<GedcomTagLevel>(100);

            pairPool = new GedcomTagLevel[100];
        }

        /// <summary>
        /// Gets the previous tag name.
        /// </summary>
        public string PreviousTag
        {
            get
            {
                string ret = string.Empty;

                if (PreviousTags.Count > 0)
                {
                    ret = PreviousTags.Peek().Name;
                }

                return ret;
            }
        }

        /// <summary>
        /// Gets the level of the previous tag.
        /// </summary>
        public int PreviousLevel
        {
            get
            {
                int ret = -1;

                if (PreviousTags.Count > 0)
                {
                    ret = PreviousTags.Peek().Level;
                }

                return ret;
            }
        }

        /// <summary>
        /// Gets or sets the stack of previous tag names / levels
        /// </summary>
        public Stack<GedcomTagLevel> PreviousTags { get; protected set; }

        /// <summary>
        /// Gets or sets the parse stack of current records, back to the last level 0 record.
        /// </summary>
        public Stack<GedcomRecord> Records { get; protected set; }

        /// <summary>
        /// Gets or sets the current database the GedcomRecordReader is working with.
        /// </summary>
        public GedcomDatabase Database { get; set; }

        /// <summary>
        /// Obtain the name of the parent GEDCOM tag
        /// </summary>
        /// <param name="level">
        /// A <see cref="int"/>.  The level of the current tag
        /// </param>
        /// <returns>
        /// A <see cref="string"/>.  The name of the parent GEDCOM tag.
        /// </returns>
        public string ParentTag(int level)
        {
            string ret = string.Empty;

            if (PreviousTags.Count > 0)
            {
                foreach (GedcomTagLevel previous in PreviousTags)
                {
                    if (previous.Level < level)
                    {
                        ret = previous.Name;
                        break;
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Adds the previous tag.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="level">The level.</param>
        /// <exception cref="Exception">Only 99 levels supported, as per GEDCOM spec</exception>
        public void AddPreviousTag(string name, int level)
        {
            if (level > 99)
            {
                throw new Exception("Only 99 levels supported, as per GEDCOM spec");
            }
            else
            {
                GedcomTagLevel pair = pairPool[level];
                if (pair == null)
                {
                    pair = new GedcomTagLevel();
                    pairPool[level] = pair;
                }

                pair.Name = name;
                pair.Level = level;

                PreviousTags.Push(pair);
            }
        }
    }
}
