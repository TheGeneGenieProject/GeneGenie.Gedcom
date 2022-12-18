// <copyright file="GedcomRecord.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xml;
    using GeneGenie.Gedcom.Enums;

    /// <summary>
    /// GEDCOM Record.
    /// </summary>
    public abstract class GedcomRecord
    {
        private GedcomRestrictionNotice restrictionNotice;

        /// <summary>
        /// The level.
        /// </summary>
        private int level;

        /// <summary>
        /// The user reference number.
        /// </summary>
        private string userReferenceNumber;

        /// <summary>
        /// The user reference type.
        /// </summary>
        private string userReferenceType;

        /// <summary>
        /// The automated record identifier.
        /// </summary>
        private string automatedRecordId;

        /// <summary>
        /// The change date.
        /// </summary>
        private GedcomChangeDate changeDate;

        /// <summary>
        /// The notes.
        /// </summary>
        private GedcomRecordList<string> notes;

        /// <summary>
        /// The multimedia.
        /// </summary>
        private GedcomRecordList<string> multimedia;

        /// <summary>
        /// The sources.
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
        /// Gets the GEDCOM tag.
        /// </summary>
        /// <value>
        /// The GEDCOM tag.
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
                ParsingLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets the parsing level.
        /// When we are removing inline note records etc. the new
        /// record is set to level 0, this breaks the parsing mechanism,
        /// so we need to record the level the record used to occur on
        /// TODO: this is a bit of a hack as it adds parsing related code to non
        /// parsing data.
        /// </summary>
        public int ParsingLevel { get; set; }

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
        public string AutomatedRecordId
        {
            get
            {
                return automatedRecordId;
            }

            set
            {
                if (value != automatedRecordId)
                {
                    automatedRecordId = value;
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
        /// <exception cref="Exception">MISSING DATABASE: " + this.RecordType.ToString().</exception>
        public virtual GedcomChangeDate ChangeDate
        {
            get
            {
                GedcomChangeDate realChangeDate = changeDate;
                GedcomRecord record;
                GedcomChangeDate childChangeDate;
                if (Database == null)
                {
                    // TODO: Don't throw exceptions in properties, need to work around.
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
        /// Gets a list of cross references to notes for this record.
        /// </summary>
        public GedcomRecordList<string> Notes
        {
            // TODO: This lookup is not easy to use, can we simply this to a list of note records?
            get
            {
                if (notes == null)
                {
                    notes = new GedcomRecordList<string>();
                    notes.CollectionChanged += ListChanged;
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
                    multimedia = new GedcomRecordList<string>();
                    multimedia.CollectionChanged += ListChanged;
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
                    sources = new GedcomRecordList<GedcomSourceCitation>();
                    sources.CollectionChanged += ListChanged;
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
        /// </summary>
        /// <remarks>
        /// Not standard GEDCOM, but no reason not to put a restriction notice at this level.
        /// </remarks>
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
        /// Gets the list of informational, warning and error messages generated when parsing this record.
        /// </summary>
        public List<GedcomParserMessage> ParserMessages { get; } = new List<GedcomParserMessage>();

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        /// <exception cref="Exception">Ref Count already 0.</exception>
        public virtual void Delete()
        {
            /* if (RefCount == 0) // This was causing individual deletes not to happen properly.
            {
                // TODO: Not good, need to feed back to user instead of blowing up.
                throw new Exception("Ref Count already 0");
            } */

            RefCount--;
            if (RefCount <= 0)
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
        /// <param name="root">The root node.</param>
        public virtual void GenerateXML(XmlNode root)
        {
        }

        /// <summary>
        /// Generates the note XML.
        /// </summary>
        /// <param name="root">The root node.</param>
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
        /// Outputs this instance as a GEDCOM record.
        /// </summary>
        /// <param name="sw">The writer to output to.</param>
        public virtual void Output(TextWriter sw)
        {
            sw.Write(Environment.NewLine);
            sw.Write(Level.ToString());
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
        /// Must be overridden in derived classes to compare the user entered data for that instance.
        /// Called from the <see cref="Equals(GedcomRecord)" /> method before it checks common
        /// data elements (notes, sources etc.).
        /// We use the word equivalent so that we avoid using the word equals. This is because we are
        /// checking user entered data only and as far as the end user cares, two records can be equivalent
        /// (matching) but they might be two different individuals / families etc.
        /// </summary>
        /// <param name="obj">The object to compare this instance against.</param>
        /// <returns>True if instance matches user data, otherwise False.</returns>
        public abstract bool IsEquivalentTo(object obj);

        /// <summary>
        /// Compares the inheriting instance user entered data against the passed GedcomRecord.
        /// If that matches, will then compare the common elements of the passed GedcomRecord
        /// against this instance (Source etc. which are common to all inheritors).
        /// </summary>
        /// <param name="obj">The GedcomRecord to compare against.</param>
        /// <returns>True if the core base properties match, otherwise False.</returns>
        public bool Equals(GedcomRecord obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            // Ask the inheriting object if its user entered data is the same.
            if (!IsEquivalentTo(obj))
            {
                return false;
            }

            var record = obj as GedcomRecord;

            if (!Equals(Level, record.Level))
            {
                return false;
            }

            if (!Equals(RestrictionNotice, record.RestrictionNotice))
            {
                return false;
            }

            if (!GedcomGenericListComparer.CompareGedcomRecordLists(Sources, record.Sources))
            {
                return false;
            }

            if (!GedcomGenericListComparer.CompareLists(Multimedia, record.Multimedia))
            {
                return false;
            }

            /* TODO: Notes are hard work, we need to do lookups by xref instead of just having a list of GedcomNote records attached. Need to fix this as a pain to test and use as well.
            //if (!GedcomGenericListComparer.CompareLists(Notes, record.Notes))
            //{
            //    return false;
            //}
            */

            return true;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as GedcomRecord);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            // Overflow is fine, just wrap.
            unchecked
            {
                int hash = 17;

                // TODO: Add in more here and match up with Equals above.
                hash *= 23 + Sources.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Splits the text.
        /// </summary>
        /// <param name="sw">The streamwriter.</param>
        /// <param name="line">The line.</param>
        /// <param name="level">The level.</param>
        protected static void SplitText(StreamWriter sw, string line, int level)
        {
            Gedcom.Util.SplitText(sw, line, level, 248, int.MaxValue, false);
        }

        /// <summary>
        /// Adds a warning, information or error message for the user to review after parsing.
        /// </summary>
        /// <param name="warningId">The warning identifier.</param>
        /// <param name="additional">An array of additional data for context on the error.</param>
        protected void AddParserMessage(ParserMessageIds warningId, params object[] additional)
        {
            // TODO: Can we figure out what field this issue occurred on? For date, death, burial etc.
            ParserMessages.Add(new GedcomParserMessage(warningId, additional));
        }

        /// <summary>
        /// Update the GedcomChangeDate for this record.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ListChanged(object sender, EventArgs e)
        {
            Changed();
        }

        /// <summary>
        /// Update the GedcomChangeDate for this record.
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

                DateTime now = SystemTime.Now;

                changeDate.Date1 = now.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
                changeDate.Time = now.ToString("HH:mm:ss");
                changeDate.DatePeriod = GedcomDatePeriod.Exact;
            }
        }

        /// <summary>
        /// Splits the text.
        /// </summary>
        /// <param name="sw">The stream writer.</param>
        /// <param name="line">The line.</param>
        protected void SplitText(StreamWriter sw, string line)
        {
            Gedcom.Util.SplitText(sw, line, Level + 1, 248, int.MaxValue, false);
        }

        /// <summary>
        /// Outputs the standard.
        /// </summary>
        /// <param name="sw">The writer.</param>
        protected void OutputStandard(TextWriter sw)
        {
            string levelPlusOne = null;

            if (ChangeDate != null)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (Level + 1).ToString();
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
                    levelPlusOne = (Level + 1).ToString();
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
                    levelPlusOne = (Level + 1).ToString();
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
                    levelPlusOne = (Level + 1).ToString();
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" REFN ");
                string line = UserReferenceNumber.Replace("@", "@@");
                sw.Write(line);

                if (!string.IsNullOrEmpty(UserReferenceType))
                {
                    sw.Write(Environment.NewLine);
                    sw.Write((Level + 2).ToString());
                    sw.Write(" REFN ");
                    line = UserReferenceType.Replace("@", "@@");
                    sw.Write(line);
                }
            }

            if (!string.IsNullOrEmpty(AutomatedRecordId))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (Level + 1).ToString();
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" RIN ");
                string line = AutomatedRecordId.Replace("@", "@@");
                sw.Write(line);
            }
        }
    }
}
