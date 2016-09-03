/*
 *  $Id: GedcomFamilyRecord.cs 200 2008-11-30 14:34:07Z davek $
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
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Defines a family, consisting of husband/wife and children, and
    /// family events.
    /// </summary>
    public class GedcomFamilyRecord : GedcomRecord
    {
        private GedcomRecordList<GedcomFamilyEvent> _Events;

        private string _Husband;
        private string _Wife;

        private GedcomRecordList<string> _Children;
        // not just _Children.Count, may be unknown children
        private int _NumberOfChildren;

        private GedcomRecordList<string> _SubmitterRecords;

        // FIXME
        private object _LDSSpouseSealings;

        private MarriageStartStatus _startStatus;

        // only used during parsing
        private Dictionary<string, Gedcom.PedegreeLinkageType> _linkageTypes;
        private Dictionary<string, Gedcom.PedegreeLinkageType> _husbLinkageTypes;
        private Dictionary<string, Gedcom.PedegreeLinkageType> _wifeLinkageTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomFamilyRecord"/> class.
        /// </summary>
        public GedcomFamilyRecord()
        {
            _Events = new GedcomRecordList<GedcomFamilyEvent>();
            _Events.Changed += ListChanged;
            _Children = new GedcomRecordList<string>();
            _Children.Changed += ListChanged;

            _startStatus = MarriageStartStatus.Unknown;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomFamilyRecord"/> class.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="indi1"></param>
        /// <param name="indi2"></param>
        public GedcomFamilyRecord(GedcomDatabase database, GedcomIndividualRecord indi1, GedcomIndividualRecord indi2)
            : this()
        {
            Level = 0;
            Database = database;
            XRefID = database.GenerateXref("FAM");

            if (indi1 != null)
            {
                GedcomFamilyLink link = new GedcomFamilyLink();
                link.Database = database;
                link.Family = XRefID;
                link.Indi = indi1.XRefID;
                indi1.SpouseIn.Add(link);

                if (indi2 != null)
                {
                    link = new GedcomFamilyLink();
                    link.Database = database;
                    link.Family = XRefID;
                    link.Indi = indi2.XRefID;
                    indi2.SpouseIn.Add(link);
                }

                switch (indi1.Sex)
                {
                    case GedcomSex.Female:
                        Wife = indi1.XRefID;
                        if (indi2 != null)
                        {
                            Husband = indi2.XRefID;
                        }

                        break;
                    default:
                        // got to put some where if not male or female,
                        // go with same as male
                        Husband = indi1.XRefID;
                        if (indi2 != null)
                        {
                            Wife = indi2.XRefID;
                        }

                        break;
                }
            }

            database.Add(XRefID, this);
        }

        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Family; }
        }

        public override string GedcomTag
        {
            get { return "FAM"; }
        }

        public GedcomRecordList<GedcomFamilyEvent> Events
        {
            get { return _Events; }
        }

        public string Husband
        {
            get
            {
                return _Husband;
            }

            set
            {
                if (value != _Husband)
                {
                    _Husband = value;
                    Changed();
                }
            }
        }

        public string Wife
        {
            get
            {
                return _Wife;
            }

            set
            {
                if (value != _Wife)
                {
                    _Wife = value;
                    Changed();
                }
            }
        }

        public GedcomRecordList<string> Children
        {
            get { return _Children; }
        }

        public int NumberOfChildren
        {
            get
            {
                return _NumberOfChildren;
            }

            set
            {
                if (value != _NumberOfChildren)
                {
                    _NumberOfChildren = value;
                    Changed();
                }
            }
        }

        public GedcomRecordList<string> SubmitterRecords
        {
            get
            {
                if (_SubmitterRecords == null)
                {
                    _SubmitterRecords = new GedcomRecordList<string>();
                    _SubmitterRecords.Changed += ListChanged;
                }

                return _SubmitterRecords;
            }
        }

        // Util properties to get marriage event
        public GedcomFamilyEvent Marriage
        {
            get
            {
                GedcomFamilyEvent marriage = null;

                foreach (GedcomFamilyEvent e in _Events)
                {
                    if (e.EventType == GedcomEventType.MARR)
                    {
                        marriage = e;
                        break;
                    }
                }

                return marriage;
            }
        }

        public MarriageStartStatus StartStatus
        {
            get
            {
                return _startStatus;
            }

            set
            {
                if (value != _startStatus)
                {
                    _startStatus = value;
                    Changed();
                }
            }
        }

        public override GedcomChangeDate ChangeDate
        {
            get
            {
                GedcomChangeDate realChangeDate = base.ChangeDate;
                GedcomRecord record;
                GedcomChangeDate childChangeDate;
                foreach (GedcomFamilyEvent famEvent in Events)
                {
                    childChangeDate = famEvent.ChangeDate;
                    if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                    {
                        realChangeDate = childChangeDate;
                    }
                }

                foreach (string submitterID in SubmitterRecords)
                {
                    record = _database[submitterID];
                    childChangeDate = record.ChangeDate;
                    if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                    {
                        realChangeDate = childChangeDate;
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

        public override void Delete()
        {
            base.Delete();

            if (_Events != null && _refCount == 0)
            {
                foreach (GedcomEvent ev in _Events)
                {
                    ev.Delete();
                }
            }
        }

        public GedcomFamilyEvent AddNewEvent(GedcomEventType type)
        {
            GedcomFamilyEvent familyEvent = new GedcomFamilyEvent();
            familyEvent.EventType = type;
            familyEvent.Level = Level + 1;
            familyEvent.FamRecord = this;

            Events.Add(familyEvent);

            return familyEvent;
        }

        public bool AddChild(GedcomIndividualRecord indi)
        {
            bool added = false;

            if (indi != null && ! Children.Contains(indi.XRefID))
            {
                if (string.IsNullOrEmpty(XRefID))
                {
                    XRefID = _database.GenerateXref("FAM");
                    _database.Add(XRefID,this);
                }

                if (!indi.ChildInFamily(XRefID))
                {
                    GedcomFamilyLink link = new GedcomFamilyLink();
                    link.Database = _database;
                    link.Family = XRefID;
                    link.Indi = indi.XRefID;
                    link.Level = 1;
                    indi.ChildIn.Add(link);
                }

                Children.Add(indi.XRefID);

                added = true;
            }

            return added;
        }

        public GedcomIndividualRecord AddNewChild()
        {
            GedcomIndividualRecord husband = null;
            GedcomIndividualRecord wife = null;

            if (!string.IsNullOrEmpty(_Husband))
            {
                husband = _database[_Husband] as GedcomIndividualRecord;
            }

            if (!string.IsNullOrEmpty(_Wife))
            {
                wife = _database[_Wife] as GedcomIndividualRecord;
            }

            string surname = "unknown";

            if (husband != null)
            {
                GedcomName husbandName = husband.GetName();
                if (husbandName != null)
                {
                    surname = husbandName.Surname;
                }
            }
            else if (wife != null)
            {
                GedcomName wifeName = wife.GetName();
                if (wifeName != null)
                {
                    surname = wifeName.Surname;
                }
            }

            GedcomIndividualRecord indi = new GedcomIndividualRecord(_database, surname);

            // don't care about failure here, won't happen as indi isn't null
            // and they aren't already in the family
            AddChild(indi);

            return indi;
        }

        public void RemoveChild(GedcomIndividualRecord child)
        {
            Children.Remove(child.XRefID);

            GedcomFamilyLink link;
            if (child.ChildInFamily(XRefID, out link))
            {
                child.ChildIn.Remove(link);
            }
        }

        public void ChangeHusband(GedcomIndividualRecord indi)
        {
            GedcomIndividualRecord husband = null;
            GedcomIndividualRecord wife = null;

            if (!string.IsNullOrEmpty(_Husband))
            {
                husband = _database[_Husband] as GedcomIndividualRecord;
            }

            if (!string.IsNullOrEmpty(_Wife))
            {
                wife = _database[_Wife] as GedcomIndividualRecord;
            }

            if (string.IsNullOrEmpty(XRefID))
            {
                XRefID = _database.GenerateXref("FAM");
                _database.Add(XRefID,this);
            }

            if (husband != null)
            {
                GedcomFamilyLink link;
                if (husband.SpouseInFamily(XRefID,out link))
                {
                    husband.SpouseIn.Remove(link);
                }
            }

            husband = indi;
            _Husband = string.Empty;

            if (husband != null)
            {
                _Husband = husband.XRefID;

                if (!husband.SpouseInFamily(XRefID))
                {
                    GedcomFamilyLink link = new GedcomFamilyLink();
                    link.Database = _database;
                    link.Family = XRefID;
                    link.Indi = _Husband;
                    husband.SpouseIn.Add(link);
                }
            }

            if (wife != null)
            {
                _Wife = wife.XRefID;

                if (!wife.SpouseInFamily(XRefID))
                {
                    GedcomFamilyLink link = new GedcomFamilyLink();
                    link.Database = _database;
                    link.Family = XRefID;
                    link.Indi = _Wife;
                    wife.SpouseIn.Add(link);
                }
            }
        }

        public void ChangeWife(GedcomIndividualRecord indi)
        {
            GedcomIndividualRecord husband = null;
            GedcomIndividualRecord wife = null;

            if (!string.IsNullOrEmpty(_Husband))
            {
                husband = _database[_Husband] as GedcomIndividualRecord;
            }

            if (!string.IsNullOrEmpty(_Wife))
            {
                wife = _database[_Wife] as GedcomIndividualRecord;
            }

            if (string.IsNullOrEmpty(XRefID))
            {
                XRefID = _database.GenerateXref("FAM");
                _database.Add(XRefID,this);
            }

            if (wife != null)
            {
                GedcomFamilyLink link;
                if (wife.SpouseInFamily(XRefID,out link))
                {
                    wife.SpouseIn.Remove(link);
                }
            }

            wife = indi;
            _Wife = string.Empty;

            if (husband != null)
            {
                _Husband = husband.XRefID;

                if (!husband.SpouseInFamily(XRefID))
                {
                    GedcomFamilyLink link = new GedcomFamilyLink();
                    link.Database = _database;
                    link.Family = XRefID;
                    link.Indi = _Husband;
                    husband.SpouseIn.Add(link);
                }
            }

            if (wife != null)
            {
                _Wife = wife.XRefID;

                if (!wife.SpouseInFamily(XRefID))
                {
                    GedcomFamilyLink link = new GedcomFamilyLink();
                    link.Database = _database;
                    link.Family = XRefID;
                    link.Indi = _Wife;
                    wife.SpouseIn.Add(link);
                }
            }
        }

        public void RemoveHusband(GedcomIndividualRecord indi)
        {
            GedcomFamilyLink link;

            if (_Husband == indi.XRefID)
            {
                _Husband = string.Empty;
            }

            if (indi.SpouseInFamily(XRefID, out link))
            {
                indi.SpouseIn.Remove(link);
            }
        }

        public void RemoveWife(GedcomIndividualRecord indi)
        {
            GedcomFamilyLink link;

            if (_Wife == indi.XRefID)
            {
                _Wife = string.Empty;
            }

            if (indi.SpouseInFamily(XRefID, out link))
            {
                indi.SpouseIn.Remove(link);
            }
        }

        public void ClearLinkageTypes()
        {
            if (_linkageTypes != null)
            {
                _linkageTypes.Clear();
                _linkageTypes = null;
            }

            if (_husbLinkageTypes != null)
            {
                _husbLinkageTypes.Clear();
                _husbLinkageTypes = null;
            }

            if (_wifeLinkageTypes != null)
            {
                _wifeLinkageTypes.Clear();
                _wifeLinkageTypes = null;
            }
        }

        public void SetLinkageType(string childXrefID, Gedcom.PedegreeLinkageType type)
        {
            SetLinkageType(childXrefID, type, Gedcom.GedcomAdoptionType.HusbandAndWife);
        }

        public void SetLinkageType(string childXrefID, Gedcom.PedegreeLinkageType type, Gedcom.GedcomAdoptionType to)
        {
            Dictionary<string, PedegreeLinkageType> dict;

            switch (to)
            {
                case Gedcom.GedcomAdoptionType.Husband:
                    if (_husbLinkageTypes == null)
                    {
                        _husbLinkageTypes = new Dictionary<string,PedegreeLinkageType>();
                    }

                    dict = _husbLinkageTypes;
                    break;
                case Gedcom.GedcomAdoptionType.Wife:
                    if (_wifeLinkageTypes == null)
                    {
                        _wifeLinkageTypes = new Dictionary<string,PedegreeLinkageType>();
                    }

                    dict = _wifeLinkageTypes;
                    break;
                case Gedcom.GedcomAdoptionType.HusbandAndWife:
                default:
                    if (_linkageTypes == null)
                    {
                        _linkageTypes = new Dictionary<string,PedegreeLinkageType>();
                    }

                    dict = _linkageTypes;
                    break;
            }

            if (dict.ContainsKey(childXrefID))
            {
                dict[childXrefID] = type;
            }
            else
            {
                dict.Add(childXrefID, type);
            }
        }

        public Gedcom.PedegreeLinkageType GetHusbandLinkageType(string childXrefID)
        {
            Gedcom.PedegreeLinkageType ret = Gedcom.PedegreeLinkageType.Unknown;

            if (_husbLinkageTypes != null && _husbLinkageTypes.ContainsKey(childXrefID))
            {
                ret = _husbLinkageTypes[childXrefID];
            }
            else
            {
                GedcomIndividualRecord child = (GedcomIndividualRecord)_database[childXrefID];
                if (child != null)
                {
                    GedcomFamilyLink link = null;
                    if (child.ChildInFamily(_XrefID, out link))
                    {
                        ret = link.FatherPedigree;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Child " + childXrefID + " is not in family " +
                                                           _XrefID + " in GetLinkageType");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Attempt to GetLinkageType of unknown child " +
                                                       childXrefID + " in " + _XrefID);
                }
            }

            return ret;
        }

        public Gedcom.PedegreeLinkageType GetWifeLinkageType(string childXrefID)
        {
            Gedcom.PedegreeLinkageType ret = Gedcom.PedegreeLinkageType.Unknown;

            if (_wifeLinkageTypes != null && _wifeLinkageTypes.ContainsKey(childXrefID))
            {
                ret = _wifeLinkageTypes[childXrefID];
            }
            else
            {
                GedcomIndividualRecord child = (GedcomIndividualRecord)_database[childXrefID];
                if (child != null)
                {
                    GedcomFamilyLink link = null;
                    if (child.ChildInFamily(_XrefID, out link))
                    {
                        ret = link.MotherPedigree;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Child " + childXrefID + " is not in family " +
                                                           _XrefID + " in GetLinkageType");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Attempt to GetLinkageType of unknown child " +
                                                       childXrefID + " in " + _XrefID);
                }
            }

            return ret;
        }

        public Gedcom.PedegreeLinkageType GetLinkageType(string childXrefID)
        {
            Gedcom.PedegreeLinkageType ret = Gedcom.PedegreeLinkageType.Unknown;

            if (_linkageTypes != null && _linkageTypes.ContainsKey(childXrefID))
            {
                ret = _linkageTypes[childXrefID];
            }
            else
            {
                GedcomIndividualRecord child = (GedcomIndividualRecord)_database[childXrefID];
                if (child != null)
                {
                    GedcomFamilyLink link = null;
                    if (child.ChildInFamily(_XrefID, out link))
                    {
                        ret = link.Pedigree;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Child " + childXrefID + " is not in family " +
                                                           _XrefID + " in GetLinkageType");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Attempt to GetLinkageType of unknown child " +
                                                       childXrefID + " in " + _XrefID);
                }
            }

            return ret;
        }

        public override void GenerateXML(XmlNode root)
        {
            XmlDocument doc = root.OwnerDocument;

            XmlNode node;
            XmlAttribute attr;

            XmlNode famNode = doc.CreateElement("FamilyRec");
            attr = doc.CreateAttribute("Id");
            attr.Value = XRefID;
            famNode.Attributes.Append(attr);

            if (!string.IsNullOrEmpty(Husband))
            {
                GedcomIndividualRecord husb = _database[Husband] as GedcomIndividualRecord;
                if (husb != null)
                {
                    node = doc.CreateElement("HusbFath");

                    XmlNode linkNode = doc.CreateElement("Link");

                    attr = doc.CreateAttribute("Target");
                    attr.Value = "IndividualRec";
                    linkNode.Attributes.Append(attr);

                    attr = doc.CreateAttribute("Ref");
                    attr.Value = Husband;
                    linkNode.Attributes.Append(attr);

                    node.AppendChild(linkNode);

                    famNode.AppendChild(node);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Pointer to non existant husband");
                }
            }

            if (!string.IsNullOrEmpty(Wife))
            {
                GedcomIndividualRecord wife = _database[Wife] as GedcomIndividualRecord;
                if (wife != null)
                {
                    node = doc.CreateElement("WifeMoth");

                    XmlNode linkNode = doc.CreateElement("Link");

                    attr = doc.CreateAttribute("Target");
                    attr.Value = "IndividualRec";
                    linkNode.Attributes.Append(attr);

                    attr = doc.CreateAttribute("Ref");
                    attr.Value = Wife;
                    linkNode.Attributes.Append(attr);

                    node.AppendChild(linkNode);

                    famNode.AppendChild(node);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Pointer to non existant wife");
                }
            }

            foreach (string child in Children)
            {
                GedcomIndividualRecord indi = _database[child] as GedcomIndividualRecord;
                if (indi != null)
                {
                    node = doc.CreateElement("Child");

                    XmlNode linkNode = doc.CreateElement("Link");

                    attr = doc.CreateAttribute("Target");
                    attr.Value = "IndividualRec";
                    linkNode.Attributes.Append(attr);

                    attr = doc.CreateAttribute("Ref");
                    attr.Value = child;
                    linkNode.Attributes.Append(attr);

                    node.AppendChild(linkNode);

                    // FIXME: add in <ChildNbr>
                    GedcomFamilyLink link = null;

                    if (indi.ChildInFamily(XRefID, out link))
                    {
                        XmlNode relNode = doc.CreateElement("RelToFath");
                        string relType = string.Empty;
                        switch (link.FatherPedigree)
                        {
                            case Gedcom.PedegreeLinkageType.Adopted:
                                relType = "adopted";
                                break;
                            case Gedcom.PedegreeLinkageType.Birth:
                                relType = "birth";
                                break;
                            case Gedcom.PedegreeLinkageType.Foster:
                                relType = "foster";
                                break;
                            case Gedcom.PedegreeLinkageType.Sealing:
                                relType = "sealing";
                                break;
                            default:
                                relType = "unknown";
                                break;
                        }

                        relNode.AppendChild(doc.CreateTextNode(relType));

                        relNode = doc.CreateElement("RelToMoth");
                        relType = string.Empty;
                        switch (link.MotherPedigree)
                        {
                            case Gedcom.PedegreeLinkageType.Adopted:
                                relType = "adopted";
                                break;
                            case Gedcom.PedegreeLinkageType.Birth:
                                relType = "birth";
                                break;
                            case Gedcom.PedegreeLinkageType.Foster:
                                relType = "foster";
                                break;
                            case Gedcom.PedegreeLinkageType.Sealing:
                                relType = "sealing";
                                break;
                            default:
                                relType = "unknown";
                                break;
                        }

                        relNode.AppendChild(doc.CreateTextNode(relType));
                    }

                    famNode.AppendChild(node);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Pointer to non existant child");
                }
            }

            XmlNode basedOnNode = doc.CreateElement("BasedOn");

            foreach (GedcomFamilyEvent famEvent in Events)
            {
                node = doc.CreateElement("Event");

                XmlNode linkNode = doc.CreateElement("Link");

                attr = doc.CreateAttribute("Target");
                attr.Value = "EventRec";
                linkNode.Attributes.Append(attr);

                attr = doc.CreateAttribute("Ref");
                attr.Value = famEvent.EventXRefID;
                linkNode.Attributes.Append(attr);

                node.AppendChild(linkNode);

                basedOnNode.AppendChild(node);
            }

            famNode.AppendChild(basedOnNode);

            GenerateNoteXML(famNode);
            GenerateCitationsXML(famNode);
            GenerateMultimediaXML(famNode);

            GenerateChangeDateXML(famNode);

            root.AppendChild(famNode);
        }

        public override void Output(TextWriter sw)
        {
            base.Output(sw);

            string levelPlusOne = Util.IntToString(Level + 1);

            if (RestrictionNotice != GedcomRestrictionNotice.None)
            {
                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" RESN ");
                sw.Write(RestrictionNotice.ToString().ToLower());
            }

            foreach (GedcomFamilyEvent familyEvent in _Events)
            {
                familyEvent.Output(sw);
            }

            if (!string.IsNullOrEmpty(_Husband))
            {
                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" HUSB ");
                sw.Write("@");
                sw.Write(_Husband);
                sw.Write("@");
            }

            if (!string.IsNullOrEmpty(_Wife))
            {
                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" WIFE ");
                sw.Write("@");
                sw.Write(_Wife);
                sw.Write("@");
            }

            string levelPlusTwo = Util.IntToString(Level + 2);
            foreach (string childID in _Children)
            {
                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" CHIL ");
                sw.Write("@");
                sw.Write(childID);
                sw.Write("@");

                GedcomIndividualRecord child = (GedcomIndividualRecord)_database[childID];
                if (child != null)
                {
                    // only output _FREL / _MREL value here,
                    // real PEDI goes on the FAMC on the INDI tag
                    GedcomFamilyLink link = null;
                    if (child.ChildInFamily(_XrefID, out link))
                    {
                        switch (link.Pedigree)
                        {
                            case Gedcom.PedegreeLinkageType.FatherAdopted:
                                sw.Write(Environment.NewLine);
                                sw.Write(levelPlusTwo);
                                sw.Write("_FREL Adopted");
                                break;
                            case Gedcom.PedegreeLinkageType.MotherAdopted:
                                sw.Write(Environment.NewLine);
                                sw.Write(levelPlusTwo);
                                sw.Write("_MREL Adopted");
                                break;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Missing child linkage for " + childID + " to family " + _XrefID);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Missing child " + childID + " when outputting family " + _XrefID);
                }
            }

            if (_NumberOfChildren != 0)
            {
                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" NCHI ");
                sw.Write("@");
                sw.Write(Util.IntToString(_NumberOfChildren));
                sw.Write("@");
            }

            if (_SubmitterRecords != null)
            {
                foreach (string submitter in SubmitterRecords)
                {
                    sw.Write(Environment.NewLine);
                    sw.Write(levelPlusOne);
                    sw.Write(" SUBM ");
                    sw.Write("@");
                    sw.Write(submitter);
                    sw.Write("@");
                }
            }

            if (StartStatus != MarriageStartStatus.Unknown)
            {
                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" _MSTAT ");
                sw.Write(StartStatus.ToString());
            }
        }
    }
}
