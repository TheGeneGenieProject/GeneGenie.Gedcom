// <copyright file="GedcomSourceCitation.cs" company="GeneGenie.com">
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

#define XML_NODE_UNDEFINED

namespace GeneGenie.Gedcom
{
    using System;
    using System.IO;
    using System.Text;
#if (!XML_NODE_UNDEFINED)
    using System.Xml;
#endif
    using Enums;

    /// <summary>
    /// TODO: Doc
    /// </summary>
    /// <seealso cref="GedcomRecord" />
    public class GedcomSourceCitation : GedcomRecord
    {
        private string source;

        // source citation fields
        private string page;
        private string eventType;
        private string role;
        private GedcomCertainty certainty = GedcomCertainty.Unknown;

        private GedcomDate date;
        private string text;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomSourceCitation"/> class.
        /// </summary>
        public GedcomSourceCitation()
        {
        }

        /// <summary>
        /// Gets or sets the parsed text. HACK.
        /// </summary>
        public StringBuilder ParsedText { get; set; }

        /// <summary>
        /// Gets the type of the record.
        /// </summary>
        /// <value>
        /// The type of the record.
        /// </value>
        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.SourceCitation; }
        }

        /// <summary>
        /// Gets the GEDCOM tag for a source citation.
        /// </summary>
        /// <value>
        /// The GEDCOM tag.
        /// </value>
        public override string GedcomTag
        {
            get { return "SOUR"; }
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public string Source
        {
            get
            {
                return source;
            }

            set
            {
                if (value != source)
                {
                    source = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>
        /// The page.
        /// </value>
        public string Page
        {
            get
            {
                return page;
            }

            set
            {
                if (value != page)
                {
                    page = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the event.
        /// </summary>
        /// <value>
        /// The type of the event.
        /// </value>
        public string EventType
        {
            get
            {
                return eventType;
            }

            set
            {
                if (value != eventType)
                {
                    eventType = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        public string Role
        {
            get
            {
                return role;
            }

            set
            {
                if (value != role)
                {
                    role = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the certainty.
        /// </summary>
        /// <value>
        /// The certainty.
        /// </value>
        public GedcomCertainty Certainty
        {
            get
            {
                return certainty;
            }

            set
            {
                if (value != certainty)
                {
                    certainty = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public GedcomDate Date
        {
            get
            {
                return date;
            }

            set
            {
                if (value != date)
                {
                    date = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                if (value != text)
                {
                    text = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public override void Delete()
        {
            base.Delete();

            GedcomSourceRecord source = (GedcomSourceRecord)Database[Source];

            source.Citations.Remove(this);

            source.Delete();
        }

#if (!XML_NODE_UNDEFINED)
        /// <summary>
        /// Generates the XML.
        /// </summary>
        /// <param name="root">The root node.</param>
        public override void GenerateXML(XmlNode root)
        {
            XmlDocument doc = root.OwnerDocument;

            XmlNode node = doc.CreateElement("Citation");
            XmlAttribute attr;

            if (!string.IsNullOrEmpty(Source))
            {
                GedcomSourceRecord source = Database[Source] as GedcomSourceRecord;
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

                // TODO: output source citation fields
                //   Caption,     element
                //   Extract,    element
                GenerateNoteXML(node);
            }

            root.AppendChild(node);
        }
#endif

        /// <summary>
        /// Outputs this source citation as a GEDCOM record.
        /// </summary>
        /// <param name="sw">The writer to output to.</param>
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

        /// <summary>
        /// Compare the user entered data against the passed instance for similarity.
        /// </summary>
        /// <param name="obj">The object to compare this instance against.</param>
        /// <returns>
        /// True if instance matches user data, otherwise false.
        /// </returns>
        public override bool IsEquivalentTo(object obj)
        {
            var citation = obj as GedcomSourceCitation;

            if (citation == null)
            {
                return false;
            }

            if (!Equals(Certainty, citation.Certainty))
            {
                return false;
            }

            if (!Equals(Date, citation.Date))
            {
                return false;
            }

            if (!Equals(EventType, citation.EventType))
            {
                return false;
            }

            if (!Equals(Page, citation.Page))
            {
                return false;
            }

            if (!Equals(Role, citation.Role))
            {
                return false;
            }

            if (!Equals(Text, citation.Text))
            {
                return false;
            }

            return true;
        }
    }
}
