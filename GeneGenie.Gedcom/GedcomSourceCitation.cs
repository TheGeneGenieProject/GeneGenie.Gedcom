/*
 *  $Id: GedcomSourceCitation.cs 200 2008-11-30 14:34:07Z davek $
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
    using System.Text;
    using System.Xml;

    public class GedcomSourceCitation : GedcomRecord
    {
        private string _Source;

        // source citation fields
        private string _Page;
        private string _EventType;
        private string _Role;
        private GedcomCertainty _Certainty = GedcomCertainty.Unknown;

        private GedcomDate _Date;
        private string _Text;

        // hack
        public StringBuilder ParsedText;

        public GedcomSourceCitation()
        {
        }

        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.SourceCitation; }
        }

        public override string GedcomTag
        {
            get { return "SOUR"; }
        }

        public string Source
        {
            get
            {
                return _Source;
            }

            set
            {
                if (value != _Source)
                {
                    _Source = value;
                    Changed();
                }
            }
        }

        public string Page
        {
            get
            {
                return _Page;
            }

            set
            {
                if (value != _Page)
                {
                    _Page = value;
                    Changed();
                }
            }
        }

        public string EventType
        {
            get
            {
                return _EventType;
            }

            set
            {
                if (value != _EventType)
                {
                    _EventType = value;
                    Changed();
                }
            }
        }

        public string Role
        {
            get
            {
                return _Role;
            }

            set
            {
                if (value != _Role)
                {
                    _Role = value;
                    Changed();
                }
            }
        }

        public GedcomCertainty Certainty
        {
            get
            {
                return _Certainty;
            }

            set
            {
                if (value != _Certainty)
                {
                    _Certainty = value;
                    Changed();
                }
            }
        }

        public GedcomDate Date
        {
            get
            {
                return _Date;
            }

            set
            {
                if (value != _Date)
                {
                    _Date = value;
                    Changed();
                }
            }
        }

        public string Text
        {
            get
            {
                return _Text;
            }

            set
            {
                if (value != _Text)
                {
                    _Text = value;
                    Changed();
                }
            }
        }

        public override void Delete()
        {
            base.Delete();

            GedcomSourceRecord source = (GedcomSourceRecord)_database[_Source];

            source.Citations.Remove(this);

            source.Delete();
        }

        public override void GenerateXML(XmlNode root)
        {
            XmlDocument doc = root.OwnerDocument;

            XmlNode node = doc.CreateElement("Citation");
            XmlAttribute attr;

            if (!string.IsNullOrEmpty(Source))
            {
                GedcomSourceRecord source = _database[Source] as GedcomSourceRecord;
                if (source != null)
                {
                    XmlNode sourceNode = doc.CreateElement("Source");

                    XmlNode linkNode = doc.CreateElement("Link");

                    attr = doc.CreateAttribute("Target");
                    attr.Value = "SourceRec";
                    linkNode.Attributes.Append(attr);

                    attr = doc.CreateAttribute("Ref");
                    attr.Value = Source;
                    linkNode.Attributes.Append(attr);

                    sourceNode.AppendChild(linkNode);

                    node.AppendChild(sourceNode);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Pointer to non existant source");
                }

                if (!string.IsNullOrEmpty(Page))
                {
                    XmlNode whereNode = doc.CreateElement("WhereInSource");
                    whereNode.AppendChild(doc.CreateTextNode(Page));

                    node.AppendChild(whereNode);
                }

                if (Date != null)
                {
                    XmlNode whenNode = doc.CreateElement("WhenRecorded");
                    whenNode.AppendChild(doc.CreateTextNode(Date.DateString));
                }

                // FIXME: output source citation fields
                //   Caption,     element
                //   Extract,    element
                GenerateNoteXML(node);
            }

            root.AppendChild(node);
        }

        public override void Output(TextWriter sw)
        {
            sw.Write(Environment.NewLine);
            sw.Write(Util.IntToString(Level));
            sw.Write(" SOUR ");

            // should always have a Source, but check anyway
            if (!string.IsNullOrEmpty(Source))
            {
                sw.Write("@");
                sw.Write(Source);
                sw.Write("@");
            }

            OutputStandard(sw);

            string levelPlusOne = null;
            string levelPlusTwo = null;

            if (!string.IsNullOrEmpty(Page))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" PAGE ");
                string line = Page.Replace("@", "@@");
                sw.Write(line);
            }

            if (!string.IsNullOrEmpty(EventType))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" EVEN ");
                string line = EventType.Replace("@", "@@");
                sw.Write(line);

                if (!string.IsNullOrEmpty(Role))
                {
                    if (levelPlusTwo == null)
                    {
                        levelPlusTwo = Util.IntToString(Level + 2);
                    }

                    sw.Write(Environment.NewLine);
                    sw.Write(levelPlusTwo);
                    sw.Write(" ROLE ");
                    line = Role.Replace("@", "@@");
                    sw.Write(line);
                }
            }

            if (Date != null || !string.IsNullOrEmpty(Text))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" DATA ");

                if (Date != null)
                {
                    Date.Output(sw);
                }

                if (!string.IsNullOrEmpty(Text))
                {
                    if (levelPlusTwo == null)
                    {
                        levelPlusTwo = Util.IntToString(Level + 2);
                    }

                    sw.Write(Environment.NewLine);
                    sw.Write(levelPlusTwo);
                    sw.Write(" TEXT ");

                    Util.SplitLineText(sw, Text, Level + 2, 248);
                }
            }

            if (Certainty != GedcomCertainty.Unknown)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" QUAY ");
                sw.Write(Util.IntToString((int)Certainty));
            }
        }
    }
}
