// <copyright file="XRefIndexedKeyCollection.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom
{
    using System.Collections.Generic;
    using GeneGenie.Gedcom.Helpers;

    /// <summary>
    /// TODO: Doc
    /// </summary>
    /// <seealso cref="IndexedKeyCollection" />
    public class XRefIndexedKeyCollection : IndexedKeyCollection
    {
        private GedcomDatabase database;

        private List<string> replacementXRefs;

        private bool replaceXrefs;

        /// <summary>
        /// Initializes a new instance of the <see cref="XRefIndexedKeyCollection"/> class.
        /// </summary>
        public XRefIndexedKeyCollection()
        {
            replacementXRefs = new List<string>();
        }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public GedcomDatabase Database
        {
            get { return database; }
            set { database = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [replace x refs].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [replace x refs]; otherwise, <c>false</c>.
        /// </value>
        public bool ReplaceXRefs
        {
            get { return replaceXrefs; }
            set { replaceXrefs = value; }
        }

        /// <summary>
        /// Gets the TODO: Doc
        /// </summary>
        /// <value>
        /// The <see cref="string"/>.
        /// </value>
        /// <param name="str">The string.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>TODO: Doc</returns>
        public override string this[string str, int startIndex, int length]
        {
            get
            {
                string ret = null;

                int pos;
                bool found = Find(str, startIndex, length, out pos);

                if (!found)
                {
                    Strings.Insert(pos, str.Substring(startIndex, length).Trim());
                    if (replaceXrefs)
                    {
                        int prefixLen = 0;
                        while (char.IsLetter(str[prefixLen]))
                        {
                            prefixLen++;
                        }

                        string prefix;
                        if (prefixLen > 0)
                        {
                            prefix = str.Substring(0, prefixLen);
                        }
                        else
                        {
                            prefix = "XREF";
                        }

                        replacementXRefs.Insert(pos, database.GenerateXref(prefix));
                    }
                }

                if (replaceXrefs)
                {
                    ret = replacementXRefs[pos];
                }
                else
                {
                    ret = Strings[pos];
                }

                return ret;
            }
        }
    }
}
