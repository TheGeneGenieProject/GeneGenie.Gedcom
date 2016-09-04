// <copyright file="GedcomRecord.cs" company="GeneGenie.com">
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
    using System.Xml;
    using Enums;

    /// <summary>
    /// TODO: Doc
    /// </summary>
    public class GedcomRecord
    {
        private GedcomRestrictionNotice restrictionNotice;

        /// <summary>
        /// The level
        /// </summary>
        private int level;

        /// <summary>
        /// When we are removing inline note records etc. the new
        /// record is set to level 0, this breaks the parsing mechanism,
        /// so we need to record the level the record used to occur on
        /// TODO: this is a bit of a hack as it adds parsing related code to non
        /// parsing data
        /// </summary>
        private int parsingLevel;

        /// <summary>
        /// The user reference number
        /// </summary>
        private string userReferenceNumber;

        /// <summary>
        /// The user reference type
        /// </summary>
        private string userReferenceType;

        /// <summary>
        /// The automated record identifier
        /// </summary>
        private string automatedRecordID;

        /// <summary>
        /// The change date
        /// </summary>
        private GedcomChangeDate changeDate;

        /// <summary>
        /// The notes
        /// </summary>
        private GedcomRecordList<string> notes;

        /// <summary>
        /// The multimedia
        /// </summary>
        private GedcomRecordList<string> multimedia;

        /// <summary>
        /// The sources
        /// </summary>
        private GedcomRecordList<GedcomSourceCitation> sources;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomRecord"/> class.
        /// </summary>
        public GedcomRecord()
        {
        }

        /// <summary>
        /// Gets or sets a backpointer to know which database this record is in.
        /// </summary>
        public virtual GedcomDatabase Database { get; set; }

        /// <summary>
        /// Gets or sets the xref identifier.
        /// </summary>
        public string XrefId { get; set; }

        /// <summary>
        /// Gets the type of the record.
        /// </summary>
        /// <value>
        /// The type of the record.
        /// </value>
        public virtual GedcomRecordType RecordType
        {
            get { return GedcomRecordType.GenericRecord; }
        }

        /// <summary>
        /// Gets the gedcom tag.
        /// </summary>
        /// <value>
        /// The gedcom tag.
        /// </value>
        public virtual string GedcomTag
        {
            get { return "_UNKN"; }
        }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public int Level
        {
            get
            {
                return level;
            }

            set
            {
                level = value;
                parsingLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets the parsing level.
        /// </summary>
        /// <value>
        /// The parsing level.
        /// </value>
        public int ParsingLevel
        {
            get { return parsingLevel; }
            set { parsingLevel = value; }
        }

        /// <summary>
        /// Gets or sets the x reference identifier.
        /// </summary>
        /// <value>
        /// The x reference identifier.
        /// </value>
        public string XRefID
        {
            get { return XrefId; }
            set { XrefId = value; }
        }

        /// <summary>
        /// Gets or sets the user reference number.
        /// </summary>
        /// <value>
        /// The user reference number.
        /// </value>
        public string UserReferenceNumber
        {
            get
            {
                return userReferenceNumber;
            }

            set
            {
                if (value != userReferenceNumber)
                {
                    userReferenceNumber = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the user reference.
        /// </summary>
        /// <value>
        /// The type of the user reference.
        /// </value>
        public string UserReferenceType
        {
            get
            {
                return userReferenceType;
            }

            set
            {
                if (value != userReferenceType)
                {
                    userReferenceType = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the automated record identifier.
        /// </summary>
        /// <value>
        /// The automated record identifier.
        /// </value>
        public string AutomatedRecordID
        {
            get
            {
                return automatedRecordID;
            }

            set
            {
                if (value != automatedRecordID)
                {
                    automatedRecordID = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the change date.
        /// </summary>
        /// <value>
        /// The change date.
        /// </value>
        /// <exception cref="Exception">MISSING DATABASE: " + this.RecordType.ToString()</exception>
        public virtual GedcomChangeDate ChangeDate
        {
            get
            {
                GedcomChangeDate realChangeDate = changeDate;
                GedcomRecord record;
                GedcomChangeDate childChangeDate;
                if (Database == null)
                {
                    throw new Exception("MISSING DATABASE: " + this.RecordType.ToString());
                }

                if (notes != null)
                {
                    foreach (string noteID in Notes)
                    {
                        record = Database[noteID];
                        if (record != null)
                        {
                            childChangeDate = record.ChangeDate;
                            if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                            {
                                realChangeDate = childChangeDate;
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Missing Note: " + noteID);
                        }
                    }
                }

                if (multimedia != null)
                {
                    foreach (string mediaID in Multimedia)
                    {
                        record = Database[mediaID];
                        if (record != null)
                        {
                            childChangeDate = record.ChangeDate;
                            if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                            {
                                realChangeDate = childChangeDate;
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Missing Media: " + mediaID);
                        }
                    }
                }

                if (sources != null)
                {
                    foreach (GedcomSourceCitation citation in Sources)
                    {
                        childChangeDate = citation.ChangeDate;
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
                changeDate = value;
            }
        }

        /// <summary>
        /// Gets the notes.
        /// </summary>
        /// <value>
        /// The notes.
        /// </value>
        public GedcomRecordList<string> Notes
        {
            get
            {
                if (notes == null)
                {
                    notes = new GedcomRecordList<string>(1);
                    notes.Changed += ListChanged;
                }

                return notes;
            }
        }

        /// <summary>
        /// Gets the multimedia.
        /// </summary>
        /// <value>
        /// The multimedia.
        /// </value>
        public GedcomRecordList<string> Multimedia
        {
            get
            {
                if (multimedia == null)
                {
                    multimedia = new GedcomRecordList<string>(1);
                    multimedia.Changed += ListChanged;
                }

                return multimedia;
            }
        }

        /// <summary>
        /// Gets the sources.
        /// </summary>
        /// <value>
        /// The sources.
        /// </value>
        public GedcomRecordList<GedcomSourceCitation> Sources
        {
            get
            {
                if (sources == null)
                {
                    sources = new GedcomRecordList<GedcomSourceCitation>(1);
                    sources.Changed += ListChanged;
                }

                return sources;
            }
        }

        /// <summary>
        /// Gets or sets the reference count.
        /// </summary>
        /// <value>
        /// The reference count.
        /// </value>
        public int RefCount { get; set; }

        /// <summary>
        /// Gets or sets the restriction notice.
        /// The restriction notice.
        /// not standard GEDCOM, but no reason not to put a restriction notice at this level
        /// </summary>
        /// <value>
        /// The restriction notice.
        /// </value>
        public GedcomRestrictionNotice RestrictionNotice
        {
            get
            {
                return restrictionNotice;
            }

            set
            {
                if (value != restrictionNotice)
                {
                    restrictionNotice = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        /// <exception cref="Exception">Ref Count already 0</exception>
        public virtual void Delete()
        {
            if (RefCount == 0)
            {
                throw new Exception("Ref Count already 0");
            }

            RefCount--;
            if (RefCount == 0)
            {
                if (multimedia != null)
                {
                    foreach (string objeID in multimedia)
                    {
                        GedcomMultimediaRecord obje = (GedcomMultimediaRecord)Database[objeID];
                        obje.Delete();
                    }
                }

                if (sources != null)
                {
                    foreach (GedcomSourceCitation citation in sources)
                    {
                        citation.Delete();
                    }
                }

                if (notes != null)
                {
                    foreach (string noteID in notes)
                    {
                        GedcomNoteRecord note = (GedcomNoteRecord)Database[noteID];
                        note.Delete();
                    }
                }

                if (!string.IsNullOrEmpty(XrefId))
                {
                    Database.Remove(XrefId, this);
                }
            }
        }

        /// <summary>
        /// Generates the XML.
        /// </summary>
        /// <param name="root">The root.</param>
        public virtual void GenerateXML(XmlNode root)
        {
        }

        /// <summary>
        /// Generates the note XML.
        /// </summary>
        /// <param name="root">The root.</param>
        public void GenerateNoteXML(XmlNode root)
        {
            foreach (string noteID in Notes)
            {
                GedcomNoteRecord note = Database[noteID] as GedcomNoteRecord;
                if (note != null)
                {
                    note.GenerateXML(root);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Pointer to non existant note");
                }
            }
        }

        /// <summary>
        /// Generates the citations XML.
        /// </summary>
        /// <param name="recNode">The record node.</param>
        public void GenerateCitationsXML(XmlNode recNode)
        {
            XmlDocument doc = recNode.OwnerDocument;

            if (Sources.Count > 0)
            {
                XmlNode evidenceNode = doc.CreateElement("Evidence");

                foreach (GedcomSourceCitation citation in Sources)
                {
                    citation.GenerateXML(evidenceNode);
                }

                recNode.AppendChild(evidenceNode);
            }
        }

        /// <summary>
        /// Generates the multimedia XML.
        /// </summary>
        /// <param name="recNode">The record node.</param>
        public void GenerateMultimediaXML(XmlNode recNode)
        {
            // TODO: append media
        }

        /// <summary>
        /// Generates the change date XML.
        /// </summary>
        /// <param name="recNode">The record node.</param>
        public void GenerateChangeDateXML(XmlNode recNode)
        {
            XmlDocument doc = recNode.OwnerDocument;

            if (ChangeDate != null)
            {
                XmlNode changeNode = doc.CreateElement("Changed");

                changeNode.Attributes.Append(doc.CreateAttribute("Date"));
                changeNode.Attributes.Append(doc.CreateAttribute("Time"));

                // Should always have a GedcomDate that can be a DateTime,
                // if not pretend the change date is right now so the
                // xml stays valid
                DateTime date = ChangeDate.DateTime1 ?? DateTime.Now;

                changeNode.Attributes["Date"].Value = date.ToString("dd MMM yyyy");
                changeNode.Attributes["Time"].Value = date.ToString("HH:mm:ss");

                ChangeDate.GenerateNoteXML(changeNode);
            }
        }

        /// <summary>
        /// Outputs the specified sw.
        /// </summary>
        /// <param name="sw">The sw.</param>
        public virtual void Output(TextWriter sw)
        {
            sw.Write(Environment.NewLine);
            sw.Write(Util.IntToString(Level));
            sw.Write(" ");

            if (!string.IsNullOrEmpty(XrefId))
            {
                sw.Write("@");
                sw.Write(XrefId);
                sw.Write("@ ");
            }

            sw.Write(GedcomTag);

            OutputStandard(sw);
        }

        /// <summary>
        /// Splits the text.
        /// </summary>
        /// <param name="sw">The sw.</param>
        /// <param name="line">The line.</param>
        /// <param name="level">The level.</param>
        protected static void SplitText(StreamWriter sw, string line, int level)
        {
            Gedcom.Util.SplitText(sw, line, level, 248, int.MaxValue, false);
        }

        /// <summary>
        /// Lists the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ListChanged(object sender, EventArgs e)
        {
            Changed();
        }

        /// <summary>
        /// Changeds this instance.
        /// </summary>
        protected virtual void Changed()
        {
            if (Database == null)
            {
                // System.Console.WriteLine("Changed() called on record with no database set");
                //
                //              System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
                //              foreach (System.Diagnostics.StackFrame f in trace.GetFrames())
                //              {
                //                  System.Console.WriteLine(f);
                //              }
            }
            else if (!Database.Loading)
            {
                if (changeDate == null)
                {
                    changeDate = new GedcomChangeDate(Database);
                    changeDate.Level = Level + 1;
                }

                DateTime now = DateTime.Now;

                changeDate.Date1 = now.ToString("dd MMM yyyy");
                changeDate.Time = now.ToString("hh:mm:ss");
            }
        }

        /// <summary>
        /// Splits the text.
        /// </summary>
        /// <param name="sw">The sw.</param>
        /// <param name="line">The line.</param>
        protected void SplitText(StreamWriter sw, string line)
        {
            Gedcom.Util.SplitText(sw, line, Level + 1, 248, int.MaxValue, false);
        }

        /// <summary>
        /// Outputs the standard.
        /// </summary>
        /// <param name="sw">The sw.</param>
        protected void OutputStandard(TextWriter sw)
        {
            string levelPlusOne = null;

            if (ChangeDate != null)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" CHAN ");

                ChangeDate.Output(sw);
            }

            if (notes != null)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                foreach (string noteID in Notes)
                {
                    sw.Write(Environment.NewLine);
                    sw.Write("{0} NOTE @{1}@", levelPlusOne, noteID);
                }
            }

            if (sources != null)
            {
                foreach (GedcomSourceCitation citation in Sources)
                {
                    citation.Output(sw);
                }
            }

            if (multimedia != null)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                foreach (string multimediaID in Multimedia)
                {
                    sw.Write(Environment.NewLine);
                    sw.Write("{0} OBJE @{1}@", levelPlusOne, multimediaID);
                }
            }

            if (!string.IsNullOrEmpty(UserReferenceNumber))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" REFN ");
                string line = UserReferenceNumber.Replace("@", "@@");
                sw.Write(line);

                if (!string.IsNullOrEmpty(UserReferenceType))
                {
                    sw.Write(Environment.NewLine);
                    sw.Write(Util.IntToString(Level + 2));
                    sw.Write(" REFN ");
                    line = UserReferenceType.Replace("@", "@@");
                    sw.Write(line);
                }
            }

            if (!string.IsNullOrEmpty(AutomatedRecordID))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" RIN ");
                string line = AutomatedRecordID.Replace("@", "@@");
                sw.Write(line);
            }
        }
    }
}
