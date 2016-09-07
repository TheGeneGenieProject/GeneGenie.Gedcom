// <copyright file="GedcomParseState.cs" company="GeneGenie.com">
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
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>

namespace GeneGenie.Gedcom.Parser
{
    using GeneGenui.Gedcom.Utility;
    using System;
    using System.Collections.Generic;
    using Utility;

    /// <summary>
    /// GedcomParseState is used to maintain the current parser status
    /// for GedcomRecordReader
    /// </summary>
    public class GedcomParseState
    {
        private Pair<string, int>[] pairPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomParseState"/> class.
        /// </summary>
        public GedcomParseState()
        {
            Records = new Stack<GedcomRecord>();
            Database = new GedcomDatabase();

            // max level of 99, pre alloc size of stack
            PreviousTags = new Stack<Pair<string, int>>(100);

            pairPool = new Pair<string, int>[100];
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
                    ret = PreviousTags.Peek().First;
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
                    ret = PreviousTags.Peek().Second;
                }

                return ret;
            }
        }

        /// <summary>
        /// Gets or sets the stack of previous tag names / levels
        /// </summary>
        public Stack<Pair<string, int>> PreviousTags { get; protected set; }

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
                foreach (Pair<string, int> previous in PreviousTags)
                {
                    if (previous.Second < level)
                    {
                        ret = previous.First;
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
                Pair<string, int> pair = pairPool[level];
                if (pair == null)
                {
                    pair = new Pair<string, int>();
                    pairPool[level] = pair;
                }

                pair.First = name;
                pair.Second = level;

                PreviousTags.Push(pair);
            }
        }
    }
}
