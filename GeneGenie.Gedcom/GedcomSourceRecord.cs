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
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using System.Xml;
    using GeneGenie.Gedcom.Enums;

    /// <summary>
    /// TODO: Doc
    /// </summary>
    /// <seealso cref="GedcomRecord" />
    /// <seealso cref="System.IComparable" />
    public class GedcomSourceRecord : GedcomRecord, IComparable, IComparable<GedcomSourceRecord>, IEquatable<GedcomSourceRecord>
    {
        /// <summary>
        /// The events recorded
        /// </summary>
        private ObservableCollection<GedcomRecordedEvent> eventsRecorded;

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
        /// Gets the GEDCOM tag for a source record.
        /// </summary>
        /// <value>
        /// The GEDCOM tag.
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
        public ObservableCollection<GedcomRecordedEvent> EventsRecorded
        {
            get
            {
                if (eventsRecorded == null)
                {
                    eventsRecorded = new ObservableCollection<GedcomRecordedEvent>();
                    eventsRecorded.CollectionChanged += ListChanged;
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
                    dataNotes.CollectionChanged += ListChanged;
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
                    repositoryCitations.CollectionChanged += ListChanged;
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
                    citations.CollectionChanged += ListChanged;
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
        /// Compares two source records by title.
        /// </summary>
        /// <param name="sourceA">The first source record.</param>
        /// <param name="sourceB">The second source record.</param>
        /// <returns>
        /// &lt;0 if the first record's title precedes the second in the sort order;
        /// &gt;0 if the second record's title precedes the first;
        /// 0 if the titles are equal
        /// </returns>
        public static int CompareByTitle(GedcomSourceRecord sourceA, GedcomSourceRecord sourceB)
        {
            return string.Compare(sourceA.Title, sourceB.Title);
        }

        /// <summary>
        /// Compares this source record to another record.
        /// </summary>
        /// <param name="sourceB">A source record.</param>
        /// <returns>
        /// &lt;0 if the first record precedes the second in the sort order;
        /// &gt;0 if the second record precedes the first;
        /// 0 if the records are equal
        /// </returns>
        public int CompareTo(object sourceB)
        {
            return CompareTo(sourceB as GedcomSourceRecord);
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
        /// <param name="root">The root node.</param>
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
        /// Outputs this source record as a GEDCOM record.
        /// </summary>
        /// <param name="tw">The writer to output to.</param>
        public override void Output(TextWriter tw)
        {
            base.Output(tw);

            string levelPlusOne = null;
            string levelPlusTwo = null;

            if (!string.IsNullOrEmpty(Agency) ||
                (DataNotes != null && DataNotes.Count > 0) ||
                EventsRecorded.Count > 0)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (Level + 1).ToString();
                }

                tw.Write(Environment.NewLine);
                tw.Write(levelPlusOne);
                tw.Write(" DATA ");

                if (!string.IsNullOrEmpty(Agency))
                {
                    if (levelPlusTwo == null)
                    {
                        levelPlusTwo = (Level + 2).ToString();
                    }

                    tw.Write(Environment.NewLine);
                    tw.Write(levelPlusTwo);
                    tw.Write(" AGNC ");
                    string line = Agency.Replace("@", "@@");
                    tw.Write(line);
                }

                if (DataNotes != null)
                {
                    if (levelPlusTwo == null)
                    {
                        levelPlusTwo = (Level + 2).ToString();
                    }

                    foreach (string noteID in DataNotes)
                    {
                        tw.Write(Environment.NewLine);
                        tw.Write(levelPlusTwo);
                        tw.Write(" NOTE ");
                        tw.Write("@");
                        tw.Write(noteID);
                        tw.Write("@");
                    }
                }

                if (EventsRecorded != null && EventsRecorded.Count > 0)
                {
                    if (levelPlusTwo == null)
                    {
                        levelPlusTwo = (Level + 2).ToString();
                    }

                    foreach (GedcomRecordedEvent recordedEvent in EventsRecorded)
                    {
                        tw.Write(Environment.NewLine);
                        tw.Write(levelPlusTwo);
                        tw.Write(" EVEN ");
                        bool first = true;

                        foreach (GedcomEventType eventType in recordedEvent.Types)
                        {
                            if (!first)
                            {
                                tw.Write(",");
                            }

                            tw.Write(GedcomEvent.TypeToTag(eventType));
                            first = false;
                        }

                        if (recordedEvent.Date != null)
                        {
                            recordedEvent.Date.Output(tw);
                        }

                        if (recordedEvent.Place != null)
                        {
                            recordedEvent.Place.Output(tw);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(Originator))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (Level + 1).ToString();
                }

                tw.Write(Environment.NewLine);
                tw.Write(levelPlusOne);
                tw.Write(" AUTH ");
                Util.SplitLineText(tw, Originator, Level + 1, 248);
            }

            if (!string.IsNullOrEmpty(Title))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (Level + 1).ToString();
                }

                tw.Write(Environment.NewLine);
                tw.Write(levelPlusOne);
                tw.Write(" TITL ");
                Util.SplitLineText(tw, Title, Level + 1, 248);
            }

            if (!string.IsNullOrEmpty(FiledBy))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (Level + 1).ToString();
                }

                tw.Write(Environment.NewLine);
                tw.Write(levelPlusOne);
                tw.Write(" ABBR ");
                Util.SplitLineText(tw, FiledBy, Level + 1, 60, 1, true);
            }

            if (!string.IsNullOrEmpty(PublicationFacts))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (Level + 1).ToString();
                }

                tw.Write(Environment.NewLine);
                tw.Write(levelPlusOne);
                tw.Write(" PUBL ");
                Util.SplitLineText(tw, PublicationFacts, Level + 1, 248);
            }

            if (!string.IsNullOrEmpty(Text))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (Level + 1).ToString();
                }

                tw.Write(Environment.NewLine);
                tw.Write(levelPlusOne);
                tw.Write(" TEXT ");
                Util.SplitLineText(tw, Text, Level + 1, 248);
            }

            if (RepositoryCitations != null)
            {
                foreach (GedcomRepositoryCitation citation in RepositoryCitations)
                {
                    citation.Output(tw);
                }
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
            var source = obj as GedcomSourceRecord;

            if (source == null)
            {
                return false;
            }

            if (!Equals(Agency, source.Agency))
            {
                return false;
            }

            if (!GedcomGenericListComparer.CompareGedcomRecordLists(Citations, source.Citations))
            {
                return false;
            }

            if (!GedcomGenericListComparer.CompareLists(DataNotes, source.DataNotes))
            {
                return false;
            }

            if (!GedcomGenericListComparer.CompareLists(EventsRecorded, source.EventsRecorded))
            {
                return false;
            }

            if (!Equals(FiledBy, source.FiledBy))
            {
                return false;
            }

            if (!Equals(Originator, source.Originator))
            {
                return false;
            }

            if (!Equals(Convert.ToString(OriginatorText), Convert.ToString(source.OriginatorText)))
            {
                return false;
            }

            if (!Equals(PublicationFacts, source.PublicationFacts))
            {
                return false;
            }

            if (!Equals(Convert.ToString(PublicationText), Convert.ToString(source.PublicationText)))
            {
                return false;
            }

            if (!GedcomGenericListComparer.CompareGedcomRecordLists(RepositoryCitations, source.RepositoryCitations))
            {
                return false;
            }

            if (!Equals(Text, source.Text))
            {
                return false;
            }

            if (!Equals(Convert.ToString(TextText), Convert.ToString(source.TextText)))
            {
                return false;
            }

            if (!Equals(Title, source.Title))
            {
                return false;
            }

            if (!Equals(Convert.ToString(TitleText), Convert.ToString(source.TitleText)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Compares this source record to another record.
        /// </summary>
        /// <param name="other">A source record.</param>
        /// <returns>
        /// &lt;0 if the first record precedes the second in the sort order;
        /// &gt;0 if the second record precedes the first;
        /// 0 if the records are equal
        /// </returns>
        public int CompareTo(GedcomSourceRecord other)
        {
            return CompareByTitle(this, other);
        }

        /// <summary>
        /// Compare the user entered data against the passed instance for similarity.
        /// </summary>
        /// <param name="other">The GedcomSourceRecord to compare this instance against.</param>
        /// <returns>
        /// True if instance matches user data, otherwise false.
        /// </returns>
        public bool Equals(GedcomSourceRecord other)
        {
            return IsEquivalentTo(other);
        }

        /// <summary>
        /// Compare the user entered data against the passed instance for similarity.
        /// </summary>
        /// <param name="obj">The GedcomSourceRecord to compare this instance against.</param>
        /// <returns>
        /// True if instance matches user data, otherwise false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return IsEquivalentTo(obj as GedcomSourceRecord);
        }

        public override int GetHashCode()
        {
            return new
            {
                Agency,
                Citations,
                DataNotes,
                EventsRecorded,
                FiledBy,
                Originator,
                OriginatorText,
                PublicationFacts,
                PublicationText,
                RepositoryCitations,
                Text,
                TextText,
                Title,
                TitleText,
            }.GetHashCode();
        }
    }
}
