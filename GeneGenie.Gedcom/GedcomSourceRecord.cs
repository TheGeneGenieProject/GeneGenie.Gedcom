// <copyright file="GedcomSourceRecord.cs" company="GeneGenie.com">
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
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// TODO: Doc
    /// </summary>
    /// <seealso cref="GedcomRecord" />
    /// <seealso cref="System.IComparable" />
    public class GedcomSourceRecord : GedcomRecord, IComparable
    {
        /// <summary>
        /// The events recorded
        /// </summary>
        private GedcomRecordList<GedcomRecordedEvent> eventsRecorded;

        /// <summary>
        /// The agency
        /// </summary>
        private string agency;

        /// <summary>
        /// The data notes
        /// </summary>
        private GedcomRecordList<string> dataNotes;

        /// <summary>
        /// The originator
        /// </summary>
        private string originator;

        /// <summary>
        /// The title
        /// </summary>
        private string title;

        /// <summary>
        /// The filed by
        /// </summary>
        private string filedBy;

        /// <summary>
        /// The publication facts
        /// </summary>
        private string publicationFacts;

        /// <summary>
        /// The text
        /// </summary>
        private string text;

        /// <summary>
        /// The repository citations
        /// </summary>
        private GedcomRecordList<GedcomRepositoryCitation> repositoryCitations;

        /// <summary>
        /// The citations
        /// </summary>
        private GedcomRecordList<GedcomSourceCitation> citations;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomSourceRecord"/> class.
        /// </summary>
        public GedcomSourceRecord()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomSourceRecord"/> class.
        /// </summary>
        /// <param name="database">The database to associate with this record.</param>
        public GedcomSourceRecord(GedcomDatabase database)
            : this()
        {
            Database = database;
            Level = 0;

            Title = "New Source";

            // default to filer being current user
#if __MonoCS__
			// bug in mono code, doesn't correctly get the real name, need to strip off , chars
			FiledBy = UnixUserInfo.GetRealUser().RealName.Trim(new char[] { ',' });
#endif

            if (string.IsNullOrEmpty(FiledBy))
            {
                FiledBy = Environment.UserName;
            }

            XRefID = database.GenerateXref("SOURCE");
            database.Add(XRefID, this);
        }

        /// <summary>
        /// Gets or sets the originator text. HACK
        /// </summary>
        public StringBuilder OriginatorText { get; set; }

        /// <summary>
        /// Gets or sets the title text.
        /// </summary>
        public StringBuilder TitleText { get; set; }

        /// <summary>
        /// Gets or sets the publication text.
        /// </summary>
        public StringBuilder PublicationText { get; set; }

        /// <summary>
        /// Gets or sets the text text. TODO: What?
        /// </summary>
        public StringBuilder TextText { get; set; }

        /// <summary>
        /// Gets the type of the record.
        /// </summary>
        /// <value>
        /// The type of the record.
        /// </value>
        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Source; }
        }

        /// <summary>
        /// Gets the gedcom tag.
        /// </summary>
        /// <value>
        /// The gedcom tag.
        /// </value>
        public override string GedcomTag
        {
            get { return "SOUR"; }
        }

        /// <summary>
        /// Gets the events recorded.
        /// </summary>
        /// <value>
        /// The events recorded.
        /// </value>
        public GedcomRecordList<GedcomRecordedEvent> EventsRecorded
        {
            get
            {
                if (eventsRecorded == null)
                {
                    eventsRecorded = new GedcomRecordList<GedcomRecordedEvent>();
                    eventsRecorded.Changed += ListChanged;
                }

                return eventsRecorded;
            }
        }

        /// <summary>
        /// Gets or sets the agency.
        /// </summary>
        /// <value>
        /// The agency.
        /// </value>
        public string Agency
        {
            get
            {
                return agency;
            }

            set
            {
                if (value != agency)
                {
                    agency = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets the data notes.
        /// </summary>
        /// <value>
        /// The data notes.
        /// </value>
        public GedcomRecordList<string> DataNotes
        {
            get
            {
                if (dataNotes == null)
                {
                    dataNotes = new GedcomRecordList<string>();
                    dataNotes.Changed += ListChanged;
                }

                return dataNotes;
            }
        }

        /// <summary>
        /// Gets or sets the originator.
        /// </summary>
        /// <value>
        /// The originator.
        /// </value>
        public string Originator
        {
            get
            {
                return originator;
            }

            set
            {
                if (value != originator)
                {
                    originator = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                if (value != title)
                {
                    title = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the filed by.
        /// </summary>
        /// <value>
        /// The filed by.
        /// </value>
        public string FiledBy
        {
            get
            {
                return filedBy;
            }

            set
            {
                if (value != filedBy)
                {
                    filedBy = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the publication facts.
        /// </summary>
        /// <value>
        /// The publication facts.
        /// </value>
        public string PublicationFacts
        {
            get
            {
                return publicationFacts;
            }

            set
            {
                if (value != publicationFacts)
                {
                    publicationFacts = value;
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
        /// Gets the repository citations.
        /// </summary>
        /// <value>
        /// The repository citations.
        /// </value>
        public GedcomRecordList<GedcomRepositoryCitation> RepositoryCitations
        {
            get
            {
                if (repositoryCitations == null)
                {
                    repositoryCitations = new GedcomRecordList<GedcomRepositoryCitation>();
                    repositoryCitations.Changed += ListChanged;
                }

                return repositoryCitations;
            }
        }

        /// <summary>
        /// Gets the citations.
        /// </summary>
        /// <value>
        /// The citations.
        /// </value>
        public GedcomRecordList<GedcomSourceCitation> Citations
        {
            get
            {
                if (citations == null)
                {
                    citations = new GedcomRecordList<GedcomSourceCitation>();
                    citations.Changed += ListChanged;
                }

                return citations;
            }
        }

        /// <summary>
        /// Gets or sets the change date.
        /// </summary>
        /// <value>
        /// The change date.
        /// </value>
        public override GedcomChangeDate ChangeDate
        {
            get
            {
                GedcomChangeDate realChangeDate = base.ChangeDate;
                GedcomChangeDate childChangeDate;
                foreach (GedcomRepositoryCitation citation in RepositoryCitations)
                {
                    childChangeDate = citation.ChangeDate;
                    if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                    {
                        realChangeDate = childChangeDate;
                    }
                }

                foreach (GedcomSourceCitation citation in Citations)
                {
                    childChangeDate = citation.ChangeDate;
                    if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                    {
                        realChangeDate = childChangeDate;
                    }
                }

                if (EventsRecorded != null)
                {
                    foreach (GedcomRecordedEvent recEvent in EventsRecorded)
                    {
                        childChangeDate = recEvent.ChangeDate;
                        if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                        {
                            realChangeDate = childChangeDate;
                        }
                    }
                }

                if (realChangeDate != null)
                {
                    realChangeDate.Level = Level + 2;
                }

                return realChangeDate;
            }

            set
            {
                base.ChangeDate = value;
            }
        }

        /// <summary>
        /// Compares two source records.
        /// </summary>
        /// <param name="sourceA">The source a.</param>
        /// <param name="sourceB">The source b.</param>
        /// <returns>TODO: Doc</returns>
        public static int CompareByTitle(GedcomSourceRecord sourceA, GedcomSourceRecord sourceB)
        {
            return string.Compare(sourceA.Title, sourceB.Title);
        }

        /// <summary>
        /// Compares this instance of a source record against the passed one.
        /// </summary>
        /// <param name="sourceB">The source b.</param>
        /// <returns>TODO: Doc</returns>
        public int CompareTo(object sourceB)
        {
            return GedcomSourceRecord.CompareByTitle(this, (GedcomSourceRecord)sourceB);
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public override void Delete()
        {
            // deleting a source should be done by first deleting any citations
            // which reference it
            base.Delete();

            if (RepositoryCitations != null && RefCount == 0)
            {
                foreach (GedcomRepositoryCitation citation in RepositoryCitations)
                {
                    citation.Delete();
                }
            }
        }

        /// <summary>
        /// Generates the XML.
        /// </summary>
        /// <param name="root">The root.</param>
        public override void GenerateXML(XmlNode root)
        {
            XmlDocument doc = root.OwnerDocument;

            XmlNode node = doc.CreateElement("SourceRec");
            XmlAttribute attr;

            attr = doc.CreateAttribute("Id");
            attr.Value = XRefID;

            // TODO: Type attribute, GEDCOM 6 mapping problem, Type
            // appears to be a media type, but that is held in the repo citations
            // even worse is that there can be multiple media types per citation
            node.Attributes.Append(attr);

            if (RepositoryCitations != null)
            {
                foreach (GedcomRepositoryCitation citation in RepositoryCitations)
                {
                    // GEDCOM 6 doesn't map well, <Repository> only allows
                    // a single CallNo, but GEDCOM 5.5 can have multiple per citation,
                    // so lets output separate <Repository> tags per call number
                    for (int i = 0; i < citation.CallNumbers.Count; i++)
                    {
                        citation.GenerateXML(node, i);
                    }
                }
            }

            XmlNode titleNode = doc.CreateElement("Title");
            if (!string.IsNullOrEmpty(Title))
            {
                titleNode.AppendChild(doc.CreateTextNode(Title));
            }

            node.AppendChild(titleNode);

            if (!string.IsNullOrEmpty(Text))
            {
                XmlNode articleNode = doc.CreateElement("Article");
                articleNode.AppendChild(doc.CreateTextNode(Text));
                node.AppendChild(articleNode);
            }

            if (!string.IsNullOrEmpty(Originator))
            {
                XmlNode authorNode = doc.CreateElement("Author");
                authorNode.AppendChild(doc.CreateTextNode(Originator));
                node.AppendChild(authorNode);
            }

            if (!string.IsNullOrEmpty(PublicationFacts))
            {
                XmlNode publishingNode = doc.CreateElement("Publishing");
                publishingNode.AppendChild(doc.CreateTextNode(PublicationFacts));
                node.AppendChild(publishingNode);
            }

            GenerateNoteXML(node);
            GenerateChangeDateXML(node);

            root.AppendChild(node);
        }

        /// <summary>
        /// Outputs the specified sw.
        /// </summary>
        /// <param name="sw">The sw.</param>
        public override void Output(TextWriter sw)
        {
            base.Output(sw);

            string levelPlusOne = null;
            string levelPlusTwo = null;

            if (!string.IsNullOrEmpty(Agency) ||
                (DataNotes != null && DataNotes.Count > 0) ||
                EventsRecorded.Count > 0)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" DATA ");

                if (!string.IsNullOrEmpty(Agency))
                {
                    if (levelPlusTwo == null)
                    {
                        levelPlusTwo = Util.IntToString(Level + 2);
                    }

                    sw.Write(Environment.NewLine);
                    sw.Write(levelPlusTwo);
                    sw.Write(" AGNC ");
                    string line = Agency.Replace("@", "@@");
                    sw.Write(line);
                }

                if (DataNotes != null)
                {
                    if (levelPlusTwo == null)
                    {
                        levelPlusTwo = Util.IntToString(Level + 2);
                    }

                    foreach (string noteID in DataNotes)
                    {
                        sw.Write(Environment.NewLine);
                        sw.Write(levelPlusTwo);
                        sw.Write(" NOTE ");
                        sw.Write("@");
                        sw.Write(noteID);
                        sw.Write("@");
                    }
                }

                if (EventsRecorded != null && EventsRecorded.Count > 0)
                {
                    if (levelPlusTwo == null)
                    {
                        levelPlusTwo = Util.IntToString(Level + 2);
                    }

                    foreach (GedcomRecordedEvent recordedEvent in EventsRecorded)
                    {
                        sw.Write(Environment.NewLine);
                        sw.Write(levelPlusTwo);
                        sw.Write(" EVEN ");
                        bool first = true;

                        foreach (GedcomEventType eventType in recordedEvent.Types)
                        {
                            if (!first)
                            {
                                sw.Write(",");
                            }

                            sw.Write(GedcomEvent.TypeToTag(eventType));
                            first = false;
                        }

                        if (recordedEvent.Date != null)
                        {
                            recordedEvent.Date.Output(sw);
                        }

                        if (recordedEvent.Place != null)
                        {
                            recordedEvent.Place.Output(sw);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(Originator))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" AUTH ");
                Util.SplitLineText(sw, Originator, Level + 1, 248);
            }

            if (!string.IsNullOrEmpty(Title))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" TITL ");
                Util.SplitLineText(sw, Title, Level + 1, 248);
            }

            if (!string.IsNullOrEmpty(FiledBy))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" ABBR ");
                Util.SplitLineText(sw, FiledBy, Level + 1, 60, 1, true);
            }

            if (!string.IsNullOrEmpty(PublicationFacts))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" PUBL ");
                Util.SplitLineText(sw, PublicationFacts, Level + 1, 248);
            }

            if (!string.IsNullOrEmpty(Text))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" TEXT ");
                Util.SplitLineText(sw, Text, Level + 1, 248);
            }

            if (RepositoryCitations != null)
            {
                foreach (GedcomRepositoryCitation citation in RepositoryCitations)
                {
                    citation.Output(sw);
                }
            }
        }
    }
}
