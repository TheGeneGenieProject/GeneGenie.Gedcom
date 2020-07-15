// <copyright file="IndexedKeyCollection.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Used to track the cross references that are parsed from the GEDCOM file.
    /// This seems like quite a complex class and I'd like to see if we can benchmark
    /// and unit test it.
    /// </summary>
    public class IndexedKeyCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexedKeyCollection"/> class.
        /// </summary>
        public IndexedKeyCollection()
        {
            Strings = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexedKeyCollection"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        public IndexedKeyCollection(int size)
        {
            Strings = new List<string>(size);
        }

        /// <summary>
        /// Gets or sets the strings which form the collection.
        /// </summary>
        protected List<string> Strings { get; set; }

        /// <summary>
        /// Gets the <see cref="string"/> with the specified key from the xref collection.
        /// </summary>
        /// <param name="str">The key for looking up the xref.</param>
        /// <returns>The xref as a string.</returns>
        public virtual string this[string str]
        {
            get { return this[str, 0, str.Length]; }
        }

        /// <summary>
        /// Gets the <see cref="string" /> with the specified key from the xref collection.
        /// </summary>
        /// <param name="str">The key for looking up the xref.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>
        /// The xref as a string.
        /// </returns>
        /// <exception cref="Exception">ERROR FINDING EXISTING KEY:" + insert.</exception>
        public virtual string this[string str, int startIndex, int length]
        {
            get
            {
                string ret = null;

                if (length == 0)
                {
                    ret = string.Empty;
                }
                else
                {
                    bool found = Find(str, startIndex, length, out int pos);

                    if (!found)
                    {
                        string insert = str.Substring(startIndex, length).Trim();
                        if (Strings.Contains(insert))
                        {
                            throw new Exception("ERROR FINDING EXISTING KEY:" + insert);
                        }

                        Strings.Insert(pos, insert);
                    }

                    ret = Strings[pos];
                }

                return ret;
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("{");
            foreach (string s in Strings)
            {
                if (sb.Length > 1)
                {
                    sb.Append(",");
                }

                sb.Append(s);
            }

            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// Finds the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <param name="pos">The position the string was found at.</param>
        /// <returns>True if string found, false otherwise.</returns>
        public virtual bool Find(string str, int startIndex, int length, out int pos)
        {
            // TODO: Needs unit testing and benchmarking.
            bool found = false;

            int i = 0;
            int j = Strings.Count - 1;

            // trim leading white space
            while (length > 0 && char.IsWhiteSpace(str[startIndex]))
            {
                startIndex++;
                length--;
            }

            pos = 0;
            if (Strings.Count > 0)
            {
                while (i <= j)
                {
                    pos = (i + j) / 2;

                    string s = Strings[pos];

                    bool match = true;

                    for (int k = 0; k < length; k++)
                    {
                        if (s.Length <= k)
                        {
                            // could still be a match if the rest of the
                            // input string is white space, trim here
                            // to avoid missing any places where we are called
                            bool whiteSpace = true;
                            while (k < length)
                            {
                                if (!char.IsWhiteSpace(str[startIndex + k]))
                                {
                                    whiteSpace = false;
                                    break;
                                }

                                k++;
                            }

                            if (!whiteSpace)
                            {
                                i = pos + 1;
                                match = false;
                            }
                            else
                            {
                                // need to correct length for white space removal
                                length = s.Length;
                            }

                            break;
                        }
                        else
                        {
                            char c = s[k];

                            char c2 = str[startIndex + k];

                            if (c > c2)
                            {
                                j = pos - 1;
                                match = false;
                                break;
                            }
                            else if (c < c2)
                            {
                                i = pos + 1;
                                match = false;
                                break;
                            }
                        }
                    }

                    if (match)
                    {
                        if (s.Length == length)
                        {
                            found = true;

                            break;
                        }
                        else if (s.Length > length)
                        {
                            j = pos - 1;
                        }
                    }
                }

                // correct insertion position if needed
                if (!found)
                {
                    pos = i;
                }
            }

            return found;
        }
    }
}
