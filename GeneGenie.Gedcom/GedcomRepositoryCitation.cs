/*
 *  $Id: GedcomRepositoryCitation.cs 200 2008-11-30 14:34:07Z davek $
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
    using System;
    using System.IO;
    using System.Xml;

    public class GedcomRepositoryCitation : GedcomRecord
    {
        private string _Repository;

        private GedcomRecordList<string> _CallNumbers;
        private GedcomRecordList<SourceMediaType> _MediaTypes;

        // This is a hack for broken GEDCOM files that misuse MEDI
        private GedcomRecordList<string> _OtherMediaTypes;

        public GedcomRepositoryCitation()
        {
            _CallNumbers = new GedcomRecordList<string>();
            _CallNumbers.Changed += ListChanged;

            _MediaTypes = new GedcomRecordList<SourceMediaType>();
            _MediaTypes.Changed += ListChanged;
        }

        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.RepositoryCitation; }
        }

        public override string GedcomTag
        {
            get { return "REPO"; }
        }

        public string Repository
        {
            get
            {
                return _Repository;
            }

            set
            {
                if (value != _Repository)
                {
                    _Repository = value;
                    Changed();
                }
            }
        }

        public GedcomRecordList<string> CallNumbers
        {
            get { return _CallNumbers; }
        }

        public GedcomRecordList<SourceMediaType> MediaTypes
        {
            get { return _MediaTypes; }
        }

        public GedcomRecordList<string> OtherMediaTypes
        {
            get
            {
                if (_OtherMediaTypes == null)
                {
                    _OtherMediaTypes = new GedcomRecordList<string>();
                    _OtherMediaTypes.Changed += ListChanged;
                }

                return _OtherMediaTypes;
            }
            set
            {
                if (value != _OtherMediaTypes)
                {
                    _OtherMediaTypes = value;
                    Changed();
                }
            }
        }

        public override void Delete()
        {
            base.Delete();

            GedcomRepositoryRecord repo = (GedcomRepositoryRecord)_database[_Repository];

            repo.Citations.Remove(this);

            repo.Delete();
        }

        public void GenerateXML(XmlNode root, int num)
        {
            XmlDocument doc = root.OwnerDocument;

            XmlNode node = doc.CreateElement("Repository");

            if (!string.IsNullOrEmpty(_Repository))
            {
                XmlNode repoLink = doc.CreateElement("Link");

                XmlAttribute attr = doc.CreateAttribute("Target");
                attr.Value = "RepositoryRec";
                repoLink.Attributes.Append(attr);

                attr = doc.CreateAttribute("Ref");
                attr.Value = _Repository;
                repoLink.Attributes.Append(attr);

                node.AppendChild(repoLink);
            }

            // GEDCOM 6 doesn't map to GEDCOM 5.5 very well,
            // have to do some work in GedcomSourceRec to output one SourceRec
            // per call number in each citation
            string callNumber = _CallNumbers[num];

            XmlNode callNo = doc.CreateElement("CallNbr");
            callNo.AppendChild(doc.CreateTextNode(callNumber));

            node.AppendChild(callNo);

            root.AppendChild(node);
        }

        public override void Output(TextWriter sw)
        {
            sw.Write(Environment.NewLine);
            sw.Write(Util.IntToString(Level));
            sw.Write(" ");
            sw.Write(GedcomTag);

            if (!string.IsNullOrEmpty(Repository))
            {
                sw.Write(" @");
                sw.Write(Repository);
                sw.Write("@ ");
            }

            OutputStandard(sw);

            if (CallNumbers.Count > 0)
            {
                string levelPlusOne = Util.IntToString(Level + 1);
                string levelPlusTwo = null;

                int i = 0;
                foreach (string callNumber in CallNumbers)
                {
                    sw.Write(Environment.NewLine);
                    sw.Write(levelPlusOne);
                    sw.Write(" CALN ");
                    string line = callNumber.Replace("@", "@@");
                    sw.Write(line);

                    SourceMediaType mediaType = MediaTypes[i];
                    int otherIndex = 0;
                    if (mediaType != SourceMediaType.None)
                    {
                        if (levelPlusTwo == null)
                        {
                            levelPlusTwo = Util.IntToString(Level + 2);
                        }

                        sw.Write(Environment.NewLine);
                        sw.Write(levelPlusTwo);
                        sw.Write(" MEDI ");
                        if (mediaType != SourceMediaType.Other)
                        {
                            string type = mediaType.ToString().Replace('_', ' ');
                            sw.Write(type);
                        }
                        else
                        {
                            sw.Write(_OtherMediaTypes[otherIndex++]);
                        }
                    }

                    i++;
                }
            }
        }
    }
}
