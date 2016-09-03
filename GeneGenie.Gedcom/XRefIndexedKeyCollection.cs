/*
 *  $Id: XRefIndexedKeyCollection.cs 183 2008-06-08 15:31:15Z davek $
 *
 *  Copyright (C) 2007 David A Knight <david@ritter.demon.co.uk>
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 *
 */

namespace GeneGenie.Gedcom
{
    using System.Collections.Generic;
    using Utility;

    public class XRefIndexedKeyCollection : IndexedKeyCollection
    {
        private GedcomDatabase _database;

        private List<string> _replacementXRefs;

        private bool _replaceXrefs;

        public XRefIndexedKeyCollection()
        {
            _replacementXRefs = new List<string>();
        }

        public GedcomDatabase Database
        {
            get { return _database; }
            set { _database = value; }
        }

        public bool ReplaceXRefs
        {
            get { return _replaceXrefs; }
            set { _replaceXrefs = value; }
        }

        public override string this[string str, int startIndex, int length]
        {
            get
            {
                string ret = null;

                int pos;
                bool found = Find(str, startIndex, length, out pos);

                if (!found)
                {
                    _strings.Insert(pos, str.Substring(startIndex, length).Trim());
                    if (_replaceXrefs)
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

                        _replacementXRefs.Insert(pos, _database.GenerateXref(prefix));
                    }
                }

                if (_replaceXrefs)
                {
                    ret = _replacementXRefs[pos];
                }
                else
                {
                    ret = _strings[pos];
                }

                return ret;
            }
        }
    }
}
