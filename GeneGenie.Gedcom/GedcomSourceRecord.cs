/*
 *  $Id: GedcomSourceRecord.cs 200 2008-11-30 14:34:07Z davek $
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

    public class GedcomSourceRecord : GedcomRecord, IComparable
    {
        private GedcomRecordList<GedcomRecordedEvent> _EventsRecorded;

        private string _Agency;
        private GedcomRecordList<string> _DataNotes;
        private string _Originator;
        private string _Title;
        private string _FiledBy;
        private string _PublicationFacts;
        private string _Text;

        private GedcomRecordList<GedcomRepositoryCitation> _RepositoryCitations;

        private GedcomRecordList<GedcomSourceCitation> _Citations;

        // hacks
        public StringBuilder OriginatorText;
        public StringBuilder TitleText;
        public StringBuilder PublicationText;
        public StringBuilder TextText;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomSourceRecord"/> class.
        /// </summary>
        public GedcomSourceRecord()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomSourceRecord"/> class.
        /// </summary>
        /// <param name="database"></param>
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

        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Source; }
        }

        public override string GedcomTag
        {
            get { return "SOUR"; }
        }

        public GedcomRecordList<GedcomRecordedEvent> EventsRecorded
        {
            get
            {
                if (_EventsRecorded == null)
                {
                    _EventsRecorded = new GedcomRecordList<GedcomRecordedEvent>();
                    _EventsRecorded.Changed += ListChanged;
                }

                return _EventsRecorded;
            }
        }

        public string Agency
        {
            get
            {
                return _Agency;
            }

            set
            {
                if (value != _Agency)
                {
                    _Agency = value;
                    Changed();
                }
            }
        }

        public GedcomRecordList<string> DataNotes
        {
            get
            {
                if (_DataNotes == null)
                {
                    _DataNotes = new GedcomRecordList<string>();
                    _DataNotes.Changed += ListChanged;
                }

                return _DataNotes;
            }
        }

        public string Originator
        {
            get
            {
                return _Originator;
            }

            set
            {
                if (value != _Originator)
                {
                    _Originator = value;
                    Changed();
                }
            }
        }

        public string Title
        {
            get
            {
                return _Title;
            }

            set
            {
                if (value != _Title)
                {
                    _Title = value;
                    Changed();
                }
            }
        }

        public string FiledBy
        {
            get
            {
                return _FiledBy;
            }

            set
            {
                if (value != _FiledBy)
                {
                    _FiledBy = value;
                    Changed();
                }
            }
        }

        public string PublicationFacts
        {
            get
            {
                return _PublicationFacts;
            }

            set
            {
                if (value != _PublicationFacts)
                {
                    _PublicationFacts = value;
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

        public GedcomRecordList<GedcomRepositoryCitation> RepositoryCitations
        {
            get
            {
                if (_RepositoryCitations == null)
                {
                    _RepositoryCitations = new GedcomRecordList<GedcomRepositoryCitation>();
                    _RepositoryCitations.Changed += ListChanged;
                }

                return _RepositoryCitations;
            }
        }

        public GedcomRecordList<GedcomSourceCitation> Citations
        {
            get
            {
                if (_Citations == null)
                {
                    _Citations = new GedcomRecordList<GedcomSourceCitation>();
                    _Citations.Changed += ListChanged;
                }

                return _Citations;
            }
        }

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

                if (_EventsRecorded != null)
                {
                    foreach (GedcomRecordedEvent recEvent in _EventsRecorded)
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

        public int CompareTo(object sourceB)
        {
            return GedcomSourceRecord.CompareByTitle(this, (GedcomSourceRecord)sourceB);
        }

        public override void Delete()
        {
            // deleting a source should be done by first deleting any citations
            // which reference it
            base.Delete();

            if (_RepositoryCitations != null && _refCount == 0)
            {
                foreach (GedcomRepositoryCitation citation in _RepositoryCitations)
                {
                    citation.Delete();
                }
            }
        }

        public static int CompareByTitle(GedcomSourceRecord sourceA, GedcomSourceRecord sourceB)
        {
            return string.Compare(sourceA.Title, sourceB.Title);
        }

        public override void GenerateXML(XmlNode root)
        {
            XmlDocument doc = root.OwnerDocument;

            XmlNode node = doc.CreateElement("SourceRec");
            XmlAttribute attr;

            attr = doc.CreateAttribute("Id");
            attr.Value = XRefID;

            // FIXME: Type attribute, GEDCOM 6 mapping problem, Type
            // appears to be a media type, but that is held in the repo citations
            // even worse is that there can be multiple media types per citation
            node.Attributes.Append(attr);

            if (_RepositoryCitations != null)
            {
                foreach (GedcomRepositoryCitation citation in _RepositoryCitations)
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
            if (!string.IsNullOrEmpty(_Title))
            {
                titleNode.AppendChild(doc.CreateTextNode(_Title));
            }

            node.AppendChild(titleNode);

            if (!string.IsNullOrEmpty(_Text))
            {
                XmlNode articleNode = doc.CreateElement("Article");
                articleNode.AppendChild(doc.CreateTextNode(_Text));
                node.AppendChild(articleNode);
            }

            if (!string.IsNullOrEmpty(_Originator))
            {
                XmlNode authorNode = doc.CreateElement("Author");
                authorNode.AppendChild(doc.CreateTextNode(_Originator));
                node.AppendChild(authorNode);
            }

            if (!string.IsNullOrEmpty(_PublicationFacts))
            {
                XmlNode publishingNode = doc.CreateElement("Publishing");
                publishingNode.AppendChild(doc.CreateTextNode(_PublicationFacts));
                node.AppendChild(publishingNode);
            }

            GenerateNoteXML(node);
            GenerateChangeDateXML(node);

            root.AppendChild(node);
        }

        public override void Output(TextWriter sw)
        {
            base.Output(sw);

            string levelPlusOne = null;
            string levelPlusTwo = null;

            if (!string.IsNullOrEmpty(Agency) ||
                (_DataNotes != null && DataNotes.Count > 0) ||
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

                if (_DataNotes != null)
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

                if (_EventsRecorded != null && EventsRecorded.Count > 0)
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

            if (!string.IsNullOrEmpty(_Originator))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" AUTH ");
                Util.SplitLineText(sw, _Originator, Level + 1, 248);
            }

            if (!string.IsNullOrEmpty(_Title))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" TITL ");
                Util.SplitLineText(sw, _Title, Level + 1, 248);
            }

            if (!string.IsNullOrEmpty(_FiledBy))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" ABBR ");
                Util.SplitLineText(sw, _FiledBy, Level + 1, 60, 1, true);
            }

            if (!string.IsNullOrEmpty(_PublicationFacts))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" PUBL ");
                Util.SplitLineText(sw, _PublicationFacts, Level + 1, 248);
            }

            if (!string.IsNullOrEmpty(_Text))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" TEXT ");
                Util.SplitLineText(sw, _Text, Level + 1, 248);
            }

            if (_RepositoryCitations != null)
            {
                foreach (GedcomRepositoryCitation citation in RepositoryCitations)
                {
                    citation.Output(sw);
                }
            }
        }
    }
}
