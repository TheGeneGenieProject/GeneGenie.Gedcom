/*
 *  $Id: GedcomRecord.cs 200 2008-11-30 14:34:07Z davek $
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
    public enum GedcomRecordType
    {
        GenericRecord = 0,
        Family,
        Individual,
        Multimedia,
        Note,
        Repository,
        Source,
        Submitter,

        // non top level records
        Submission,

        Event,
        FamilyEvent,
        Place,
        SourceCitation,
        FamilyLink,
        Association,
        Name,
        IndividualEvent,
        Date,
        RepositoryCitation,

        // GEDCOM allows custom records, beginging with _
        CustomRecord,
        Header
    }

    public enum GedcomSex
    {
        Undetermined,
        Male,
        Female,
        Both,
        Neuter
    }

    public enum PedegreeLinkageType
    {
        Unknown = 0,
        Adopted,
        Birth,
        Foster,
        Sealing,

        // not part of standard GEDCOM
        // Family Tree Maker (at least in some versions) has custom _FREL and _MREL tags
        // on CHIL in the FAM record
        FatherAdopted,
        MotherAdopted
    }

    public enum ChildLinkageStatus
    {
        Unknown,
        Challenged,
        Disproven,
        Proven
    }

    public enum GedcomRestrictionNotice
    {
        None = 0,
        Confidential,
        Locked,
        Privacy
    }

    public enum GedcomCertainty
    {
        Unreliable = 0,
        Questionable = 1,
        Secondary = 2,
        Primary = 3,
        Unknown = 4
    }

    public enum GedcomAdoptionType
    {
        None = 0,
        Husband,
        Wife,
        HusbandAndWife
    }

    public enum SourceMediaType
    {
        None,
        Audio,
        Book,
        Card,
        Electronic,
        Fiche,
        Film,
        Magazine,
        Manuscript,
        Map,
        Newspaper,
        Photo,
        Tombstone,
        Video,
        // non standard gedcom media types
        Civil_Registry,
        Family_Archive_CD,
        Microfilm,
        Census,
        Letter,
        Official_Document,
        Microfiche, // FIXME: we should correct this one to be Fiche
        Other
    }

    public enum MarriageStartStatus
    {
        Single = 0,
        Private = 1,
        Partners = 2,
        Other = 3,
        Unknown = 4
    }

    public class GedcomRecord
    {
        protected int _Level;

        // When we are removing inline note records etc. the new
        // record is set to level 0, this breaks the parsing mechanism,
        // so we need to record the level the record used to occur on
        // FIXME: this is a bit of a hack as it adds parsing related code to non
        // parsing data
        protected int _ParsingLevel;

        protected string _XrefID;

        protected string _UserReferenceNumber;
        protected string _UserReferenceType;
        protected string _AutomatedRecordID;

        protected GedcomChangeDate _ChangeDate;
        protected GedcomRecordList<string> _Notes;
        protected GedcomRecordList<string> _Multimedia;
        protected GedcomRecordList<GedcomSourceCitation> _Sources;

        // not standard GEDCOM, but no reason not to put a restriction notice at this level
        protected GedcomRestrictionNotice _RestrictionNotice;

        // backpointer to know which database this record is in
        protected GedcomDatabase _database;

        // how many other records are referencing this one?
        protected int _refCount = 1;

        public GedcomRecord()
        {
        }

        public virtual GedcomRecordType RecordType
        {
            get { return GedcomRecordType.GenericRecord; }
        }

        public virtual string GedcomTag
        {
            get { return "_UNKN"; }
        }

        public int Level
        {
            get { return _Level; }
            set { _Level = value; _ParsingLevel = value; }
        }

        public int ParsingLevel
        {
            get { return _ParsingLevel; }
            set { _ParsingLevel = value; }
        }

        public string XRefID
        {
            get { return _XrefID; }
            set { _XrefID = value; }
        }

        public string UserReferenceNumber
        {
            get
            {
                return _UserReferenceNumber;
            }

            set
            {
                if (value != _UserReferenceNumber)
                {
                    _UserReferenceNumber = value;
                    Changed();
                }
            }
        }

        public string UserReferenceType
        {
            get
            {
                return _UserReferenceType;
            }

            set
            {
                if (value != _UserReferenceType)
                {
                    _UserReferenceType = value;
                    Changed();
                }
            }
        }

        public string AutomatedRecordID
        {
            get
            {
                return _AutomatedRecordID;
            }

            set
            {
                if (value != _AutomatedRecordID)
                {
                    _AutomatedRecordID = value;
                    Changed();
                }
            }
        }

        public virtual GedcomChangeDate ChangeDate
        {
            get
            {
                GedcomChangeDate realChangeDate = _ChangeDate;
                GedcomRecord record;
                GedcomChangeDate childChangeDate;
                if (_database == null)
                {
                    throw new Exception("MISSING DATABASE: " + this.RecordType.ToString());
                }

                if (_Notes != null)
                {
                    foreach (string noteID in Notes)
                    {
                        record = _database[noteID];
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

                if (_Multimedia != null)
                {
                    foreach (string mediaID in Multimedia)
                    {
                        record = _database[mediaID];
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

                if (_Sources != null)
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
                _ChangeDate = value;
            }
        }

        public GedcomRecordList<string> Notes
        {
            get
            {
                if (_Notes == null)
                {
                    _Notes = new GedcomRecordList<string>(1);
                    _Notes.Changed += ListChanged;
                }

                return _Notes;
            }
        }

        public GedcomRecordList<string> Multimedia
        {
            get
            {
                if (_Multimedia == null)
                {
                    _Multimedia = new GedcomRecordList<string>(1);
                    _Multimedia.Changed += ListChanged;
                }

                return _Multimedia;
            }
        }

        public GedcomRecordList<GedcomSourceCitation> Sources
        {
            get
            {
                if (_Sources == null)
                {
                    _Sources = new GedcomRecordList<GedcomSourceCitation>(1);
                    _Sources.Changed += ListChanged;
                }

                return _Sources;
            }
        }

        public virtual GedcomDatabase Database
        {
            get { return _database; }
            set { _database = value; }
        }

        public int RefCount
        {
            get { return _refCount; }
            set { _refCount = value; }
        }

        public GedcomRestrictionNotice RestrictionNotice
        {
            get
            {
                return _RestrictionNotice;
            }

            set
            {
                if (value != _RestrictionNotice)
                {
                    _RestrictionNotice = value;
                    Changed();
                }
            }
        }

        protected void ListChanged(object sender, EventArgs e)
        {
            Changed();
        }

        protected virtual void Changed()
        {
            if (_database == null)
            {
                //              System.Console.WriteLine("Changed() called on record with no database set");
                //
                //              System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
                //              foreach (System.Diagnostics.StackFrame f in trace.GetFrames())
                //              {
                //                  System.Console.WriteLine(f);
                //              }
            }
            else if (!_database.Loading)
            {
                if (_ChangeDate == null)
                {
                    _ChangeDate = new GedcomChangeDate(_database);
                    _ChangeDate.Level = Level + 1;
                }

                DateTime now = DateTime.Now;

                _ChangeDate.Date1 = now.ToString("dd MMM yyyy");
                _ChangeDate.Time = now.ToString("hh:mm:ss");
            }
        }

        public virtual void Delete()
        {
            if (_refCount == 0)
            {
                throw new Exception("Ref Count already 0");
            }

            _refCount--;
            if (_refCount == 0)
            {
                if (_Multimedia != null)
                {
                    foreach (string objeID in _Multimedia)
                    {
                        GedcomMultimediaRecord obje = (GedcomMultimediaRecord)_database[objeID];
                        obje.Delete();
                    }
                }

                if (_Sources != null)
                {
                    foreach (GedcomSourceCitation citation in _Sources)
                    {
                        citation.Delete();
                    }
                }

                if (_Notes != null)
                {
                    foreach (string noteID in _Notes)
                    {
                        GedcomNoteRecord note = (GedcomNoteRecord)_database[noteID];
                        note.Delete();
                    }
                }

                if (!string.IsNullOrEmpty(_XrefID))
                {
                    _database.Remove(_XrefID, this);
                }
            }
        }

        protected void SplitText(StreamWriter sw, string line)
        {
            Gedcom.Util.SplitText(sw, line, Level + 1, 248, int.MaxValue, false);
        }

        protected static void SplitText(StreamWriter sw, string line, int level)
        {
            Gedcom.Util.SplitText(sw, line, level, 248, int.MaxValue, false);
        }

        public virtual void GenerateXML(XmlNode root)
        {
        }

        public void GenerateNoteXML(XmlNode root)
        {
            foreach (string noteID in Notes)
            {
                GedcomNoteRecord note = _database[noteID] as GedcomNoteRecord;
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

        public void GenerateMultimediaXML(XmlNode recNode)
        {
            // FIXME: append media
        }

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

            if (_Notes != null)
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

            if (_Sources != null)
            {
                foreach (GedcomSourceCitation citation in Sources)
                {
                    citation.Output(sw);
                }
            }

            if (_Multimedia != null)
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

        public virtual void Output(TextWriter sw)
        {
            sw.Write(Environment.NewLine);
            sw.Write(Util.IntToString(Level));
            sw.Write(" ");

            if (!string.IsNullOrEmpty(_XrefID))
            {
                sw.Write("@");
                sw.Write(_XrefID);
                sw.Write("@ ");
            }

            sw.Write(GedcomTag);

            OutputStandard(sw);
        }
    }
}
