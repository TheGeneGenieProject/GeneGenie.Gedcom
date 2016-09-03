/*
 *  $Id: GedcomIndividualRecord.cs 200 2008-11-30 14:34:07Z davek $
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
    using System.Globalization;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Details about a given individual
    /// </summary>
    public class GedcomIndividualRecord : GedcomRecord, IComparable
    {
        private GedcomRecordList<GedcomName> _Names;
        private GedcomSex _Sex;

        private GedcomRecordList<GedcomIndividualEvent> _Events;
        private GedcomRecordList<GedcomIndividualEvent> _Attributes;

        // FIXME
        private object _LDSIndividualOrdinances;

        private GedcomRecordList<GedcomFamilyLink> _ChildIn;
        private GedcomRecordList<GedcomFamilyLink> _SpouseIn;

        private GedcomRecordList<string> _SubmitterRecords;

        private GedcomRecordList<GedcomAssociation> _Associations;

        private GedcomRecordList<string> _Alia;
        private GedcomRecordList<string> _Anci;
        private GedcomRecordList<string> _Desi;

        private string _PermanentRecordFileNumber;
        private string _AncestralFileNumber;

        // This is a hack,  not according to the spec, but Family Tree Maker sticks
        // an address under an individual.
        // GedcomRecordReader will build the address up here, then create a RESI record from it
        private GedcomAddress _Address;

        public const string UnknownName = "unknown /unknown/";

        public const string UnknownNamePart = "unknown";
        public const string UnknownSoundex = "u525";

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomIndividualRecord"/> class.
        /// </summary>
        public GedcomIndividualRecord()
        {
            // avoid resize, indi will nearly always have a name
            _Names = new GedcomRecordList<GedcomName>(1);
            _Names.Changed += ListChanged;

            // do not set capacity on events, uses more memory, at least for Database1.ged
            _Events = new GedcomRecordList<GedcomIndividualEvent>();
            _Events.Changed += ListChanged;

            _ChildIn = new GedcomRecordList<GedcomFamilyLink>();
            _ChildIn.Changed += ListChanged;

            _SpouseIn = new GedcomRecordList<GedcomFamilyLink>();
            _SpouseIn.Changed += ListChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomIndividualRecord"/> class.
        /// </summary>
        /// <param name="database"></param>
        public GedcomIndividualRecord(GedcomDatabase database)
            : this(database, "unknown")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomIndividualRecord"/> class.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="surname"></param>
        public GedcomIndividualRecord(GedcomDatabase database, string surname)
            : this()
        {
            Database = database;
            Level = 0;
            Sex = GedcomSex.Undetermined;
            XRefID = database.GenerateXref("I");

            GedcomName name = new GedcomName();
            name.Level = 1;
            name.Database = database;
            name.Name = "unknown /" + surname + "/";
            name.PreferedName = true;

            Names.Add(name);

            database.Add(XRefID, this);
        }

        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Individual; }
        }

        public override string GedcomTag
        {
            get { return "INDI"; }
        }

        public GedcomRecordList<GedcomName> Names
        {
            get { return _Names; }
        }

        public string SexChar
        {
            get
            {
                string str = "U";
                switch (Sex)
                {
                    case GedcomSex.Undetermined:
                        break;
                    case GedcomSex.Male:
                        str = "M";
                        break;
                    case GedcomSex.Female:
                        str = "F";
                        break;
                    case GedcomSex.Both:
                        str = "B";
                        break;
                    case GedcomSex.Neuter:
                        str = "N";
                        break;
                }

                return str;
            }
        }

        public GedcomSex Sex
        {
            get
            {
                return _Sex;
            }

            set
            {
                if (value != _Sex)
                {
                    // walk through all families for the individual, where they are
                    // the only spouse switch them to be husband or wife, whichever
                    // is correct
                    foreach (GedcomFamilyLink link in SpouseIn)
                    {
                        GedcomFamilyRecord fam = _database[link.Family] as GedcomFamilyRecord;
                        if (fam != null)
                        {
                            if ((fam.Husband == XRefID && string.IsNullOrEmpty(fam.Wife)) ||
                                (fam.Wife == XRefID && string.IsNullOrEmpty(fam.Husband)))
                            {
                                if (value == GedcomSex.Male)
                                {
                                    fam.Husband = XRefID;
                                    fam.Wife = null;
                                }
                                else
                                {
                                    fam.Wife = XRefID;
                                    fam.Husband = null;
                                }
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Family link points to non family record");
                        }
                    }

                    _Sex = value;
                    Changed();
                }
            }
        }

        public GedcomRecordList<GedcomIndividualEvent> Events
        {
            get { return _Events; }
        }

        public GedcomRecordList<GedcomIndividualEvent> Attributes
        {
            get
            {
                if (_Attributes == null)
                {
                    _Attributes = new GedcomRecordList<GedcomIndividualEvent>();
                    _Attributes.Changed += ListChanged;
                }

                return _Attributes;
            }
        }

        public GedcomRecordList<GedcomFamilyLink> ChildIn
        {
            get { return _ChildIn; }
        }

        public GedcomRecordList<GedcomFamilyLink> SpouseIn
        {
            get { return _SpouseIn; }
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

        public GedcomRecordList<GedcomAssociation> Associations
        {
            get
            {
                if (_Associations == null)
                {
                    _Associations = new GedcomRecordList<GedcomAssociation>();
                    _Associations.Changed += ListChanged;
                }

                return _Associations;
            }
        }

        public GedcomRecordList<string> Alia
        {
            get
            {
                if (_Alia == null)
                {
                    _Alia = new GedcomRecordList<string>();
                    _Alia.Changed += ListChanged;
                }

                return _Alia;
            }
        }

        public GedcomRecordList<string> Anci
        {
            get
            {
                if (_Anci == null)
                {
                    _Anci = new GedcomRecordList<string>();
                    _Anci.Changed += ListChanged;
                }

                return _Anci;
            }
        }

        public GedcomRecordList<string> Desi
        {
            get
            {
                if (_Desi == null)
                {
                    _Desi = new GedcomRecordList<string>();
                    _Desi.Changed += ListChanged;
                }

                return _Desi;
            }
        }

        public string PermanentRecordFileNumber
        {
            get
            {
                return _PermanentRecordFileNumber;
            }

            set
            {
                if (value != _PermanentRecordFileNumber)
                {
                    _PermanentRecordFileNumber = value;
                    Changed();
                }
            }
        }

        public string AncestralFileNumber
        {
            get
            {
                return _AncestralFileNumber;
            }

            set
            {
                if (value != _AncestralFileNumber)
                {
                    _AncestralFileNumber = value;
                    Changed();
                }
            }
        }

        // This is a hack,  not according to the spec, but Family Tree Maker sticks
        // an address under an individual.
        // GedcomRecordReader will build the address up here, then create a RESI record from it
        public GedcomAddress Address
        {
            get
            {
                return _Address;
            }

            set
            {
                if (value != _Address)
                {
                    _Address = value;
                    Changed();
                }
            }
        }

        // Util properties to get specific events
        public GedcomIndividualEvent Birth
        {
            get { return FindEvent(GedcomEventType.BIRT); }
        }

        public GedcomIndividualEvent Death
        {
            get { return FindEvent(GedcomEventType.DEAT); }
        }

        public GedcomIndividualEvent Height
        {
            get { return Attributes.Find(a => a.EventType == GedcomEventType.GenericFact && string.Compare(a.EventName, "Height") == 0); }
        }

        public GedcomIndividualEvent Weight
        {
            get { return Attributes.Find(a => a.EventType == GedcomEventType.GenericFact && string.Compare(a.EventName, "Weight") == 0); }
        }

        public GedcomIndividualEvent Medical
        {
            get { return Attributes.Find(a => a.EventType == GedcomEventType.GenericFact && string.Compare(a.EventName, "Medical") == 0); }
        }

        public bool Dead
        {
            get
            {
                bool ret = false;
                if (Death != null)
                {
                    // don't care if death date is in the future
                    // that is the fault of whoever entered the data
                    ret = true;
                }
                else
                {
                    // do we have a birth? if so check to see if they are most likely dead
                    if (Birth != null && Birth.Date != null)
                    {
                        DateTime? date = Birth.Date.DateTime1;

                        if (date != null && date.HasValue)
                        {
                            int diff = DateTime.Today.Year - date.Value.Year;

                            // FIXME: Arbitrary age limit, yes someone may somehow reach 120
                            // but not likely, should output "probably dead" but that
                            // would be invalid
                            if (diff > 120)
                            {
                                ret = true;
                            }
                        }
                    }

                    // FIXME: we can do better here, children can't be born > 9.x months
                    // after death of a parent, marriage can't be after death,
                    // can look at parents
                    // birth date and see from that if this indi has to be dead or not
                }

                return ret;
            }
        }

        public override GedcomChangeDate ChangeDate
        {
            get
            {
                GedcomChangeDate realChangeDate = base.ChangeDate;
                GedcomRecord record;
                GedcomChangeDate childChangeDate;
                foreach (GedcomName name in Names)
                {
                    childChangeDate = name.ChangeDate;
                    if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                    {
                        realChangeDate = childChangeDate;
                    }
                }

                foreach (GedcomIndividualEvent indiEvent in Events)
                {
                    childChangeDate = indiEvent.ChangeDate;
                    if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                    {
                        realChangeDate = childChangeDate;
                    }
                }

                foreach (GedcomIndividualEvent indiEvent in Attributes)
                {
                    childChangeDate = indiEvent.ChangeDate;
                    if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                    {
                        realChangeDate = childChangeDate;
                    }
                }

                foreach (GedcomFamilyLink childIn in ChildIn)
                {
                    childChangeDate = childIn.ChangeDate;
                    if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                    {
                        realChangeDate = childChangeDate;
                    }
                }

                foreach (GedcomFamilyLink spouseIn in SpouseIn)
                {
                    childChangeDate = spouseIn.ChangeDate;
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

                foreach (GedcomAssociation asso in Associations)
                {
                    childChangeDate = asso.ChangeDate;
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

        public int CompareTo(object indiB)
        {
            return GedcomIndividualRecord.CompareByName(this, (GedcomIndividualRecord)indiB);
        }

        public override void Delete()
        {
            base.Delete();

            // remove from families
            if (_refCount == 0)
            {
                if (_SpouseIn != null)
                {
                    foreach (GedcomFamilyLink spouseLink in SpouseIn)
                    {
                        GedcomFamilyRecord famRec = (GedcomFamilyRecord)_database[spouseLink.Family];

                        // don't use famRec.RemoveHusband/famRecRemoveWife, they
                        // will attempt to remove the link
                        if (famRec.Husband == _XrefID)
                        {
                            famRec.Husband = null;
                        }
                        else
                        {
                            famRec.Wife = null;
                        }

                        // does the family have anyone left?
                        if (string.IsNullOrEmpty(famRec.Husband) &&
                            string.IsNullOrEmpty(famRec.Wife) &&
                            famRec.Children.Count == 0)
                        {
                            // no, remove it
                            famRec.Delete();
                        }
                    }
                }

                if (_ChildIn != null)
                {
                    foreach (GedcomFamilyLink childLink in ChildIn)
                    {
                        GedcomFamilyRecord famRec = (GedcomFamilyRecord)_database[childLink.Family];

                        // don't use famRec.RemoveChild as it will attempt to remove the link
                        famRec.Children.Remove(_XrefID);

                        // does the family have anyone left?
                        if (string.IsNullOrEmpty(famRec.Husband) &&
                            string.IsNullOrEmpty(famRec.Wife) &&
                            famRec.Children.Count == 0)
                        {
                            // no, remove it
                            famRec.Delete();
                        }
                    }
                }

                // remove related records, sources, notes, media etc.
                if (_Names != null)
                {
                    foreach (GedcomName name in _Names)
                    {
                        name.Delete();
                    }
                }

                if (_Events != null)
                {
                    foreach (GedcomEvent ev in _Events)
                    {
                        ev.Delete();
                    }
                }

                if (_Attributes != null)
                {
                    foreach (GedcomEvent ev in _Attributes)
                    {
                        ev.Delete();
                    }
                }

                if (_Associations != null)
                {
                    foreach (GedcomAssociation asso in _Associations)
                    {
                        asso.Delete();
                    }
                }
            }
        }

        public static int CompareByName(GedcomIndividualRecord indiA, GedcomIndividualRecord indiB)
        {
            int ret = -1;

            if (indiA != null && indiB != null)
            {
                string nameA;
                string nameB;

                GedcomName aName = indiA.GetName();
                GedcomName bName = indiB.GetName();

                if (aName != null)
                {
                    nameA = aName.Name;
                }
                else
                {
                    nameA = UnknownName;
                }

                if (bName != null)
                {
                    nameB = bName.Name;
                }
                else
                {
                    nameB = UnknownName;
                }

                ret = string.Compare(nameA, nameB);
            }
            else if (indiA != null)
            {
                ret = 1;
            }

            return ret;
        }

        public GedcomName GetName()
        {
            GedcomName ret = Names.Find(n => n.PreferedName == true);
            if (ret == null && Names.Count > 0)
            {
                ret = Names[0];
            }

            return ret;
        }

        public void SetPreferedName(GedcomName name)
        {
            foreach (GedcomName n in Names)
            {
                n.PreferedName = n == name;
            }
        }

        public bool SpouseInFamily(string family)
        {
            GedcomFamilyLink tmp;

            return SpouseInFamily(family, out tmp);
        }

        public bool SpouseInFamily(string family, out GedcomFamilyLink famLink)
        {
            bool ret = false;
            famLink = null;

            foreach (GedcomFamilyLink link in SpouseIn)
            {
                if (link.Family == family)
                {
                    ret = true;
                    famLink = link;
                    break;
                }
            }

            return ret;
        }

        public bool ChildInFamily(string family)
        {
            GedcomFamilyLink tmp;

            return ChildInFamily(family, out tmp);
        }

        public bool ChildInFamily(string family, out GedcomFamilyLink famLink)
        {
            bool ret = false;
            famLink = null;

            foreach (GedcomFamilyLink link in ChildIn)
            {
                if (link.Family == family)
                {
                    ret = true;
                    famLink = link;
                    break;
                }
            }

            return ret;
        }

        public GedcomFamilyRecord GetFamily()
        {
            GedcomFamilyRecord fam = null;

            GedcomFamilyLink link = SpouseIn.Find(f => (f.PreferedSpouse == true));

            // shouldn't need this as we automatically set the prefered on loading
            // do the check anyway though just incase.
            if (link == null && SpouseIn.Count > 0)
            {
                link = SpouseIn[0];
            }

            if (link != null)
            {
                fam = _database[link.Family] as GedcomFamilyRecord;
            }

            return fam;
        }

        public void SetPreferedSpouse(string xrefID)
        {
            foreach (GedcomFamilyLink link in SpouseIn)
            {
                string famID = link.Family;
                GedcomFamilyRecord famRec = _database[famID] as GedcomFamilyRecord;
                if (famRec != null)
                {
                    link.PreferedSpouse = famRec.Husband == xrefID || famRec.Wife == xrefID;
                }
                else
                {
                    link.PreferedSpouse = false;
                    System.Diagnostics.Debug.WriteLine("Unable to find family for link with spouse: " + xrefID);
                }
            }
        }

        public GedcomIndividualEvent FindEvent(GedcomEventType eventType)
        {
            return _Events.Find(e => e.EventType == eventType);
        }

        public bool MatchSurname(string prefix, bool soundex)
        {
            bool ret = false;
            string surname;

            GedcomName name = GetName();

            if (name != null)
            {
                if (!soundex)
                {
                    surname = name.Surname;
                }
                else
                {
                    surname = name.SurnameSoundex;
                }
            }
            else
            {
                if (!soundex)
                {
                    surname = UnknownNamePart;
                }
                else
                {
                    surname = UnknownSoundex;
                }
            }

            if (!soundex)
            {
                ret = surname.StartsWith(prefix, true, CultureInfo.CurrentCulture);
            }
            else
            {
                ret = surname.StartsWith(prefix);
            }

            return ret;
        }

        public bool MatchFirstname(string prefix, bool soundex)
        {
            bool ret = false;
            string checkName;

            GedcomName name = GetName();

            if (name != null)
            {
                if (!soundex)
                {
                    checkName = name.Given;
                }
                else
                {
                    checkName = name.FirstnameSoundex;
                }
            }
            else
            {
                if (!soundex)
                {
                    checkName = UnknownNamePart;
                }
                else
                {
                    checkName = UnknownSoundex;
                }
            }

            if (!soundex)
            {
                ret = checkName.StartsWith(prefix, true, CultureInfo.CurrentCulture);
            }
            else
            {
                ret = checkName.StartsWith(prefix);
            }

            return ret;
        }

        // % match
        public float IsMatch(GedcomIndividualRecord indi)
        {
            float match = 0F;

            // check name
            float nameMatch = 0F;
            foreach (GedcomName indiName in indi.Names)
            {
                foreach (GedcomName name in Names)
                {
                    float currentNameMatch = name.IsMatch(indiName);

                    nameMatch = Math.Max(nameMatch, currentNameMatch);
                }
            }

            // 0% name match would be pointless checking other details
            if (nameMatch != 0)
            {
                // check gender
                float genderMatch = 0;
                if (Sex == indi.Sex)
                {
                    genderMatch = 100.0F;
                }

                // check dob
                float dobMatch = 0F;
                GedcomEvent birth = Birth;
                GedcomEvent indiBirth = indi.Birth;
                if (birth != null && indiBirth != null)
                {
                    dobMatch = birth.IsMatch(indiBirth);
                }
                else if (birth == null && indiBirth == null)
                {
                    dobMatch = 100.0F;
                }

                // check dod
                float dodMatch = 0F;
                GedcomEvent death = Death;
                GedcomEvent indiDeath = indi.Death;
                if (death != null && indiDeath != null)
                {
                    dodMatch = death.IsMatch(indiDeath);
                }
                else if (death == null && indiDeath == null)
                {
                    dodMatch = 100.0F;
                }

                // check parents ?

                //  System.Console.WriteLine("name: " + nameMatch + "\tdob: " + dobMatch + "\tdod: " + dodMatch);
                match = (nameMatch + genderMatch + dobMatch + dodMatch) / 4.0F;
            }

            return match;
        }

        /// <summary>
        /// Gets a "fake" family record that contains all of this individuals children.
        /// The record will only contain children
        /// </summary>
        /// <returns>
        /// A <see cref="GedcomFamilyRecord"/>
        /// </returns>
        public GedcomFamilyRecord GetAllChildren()
        {
            GedcomFamilyRecord fam = new GedcomFamilyRecord();
            foreach (GedcomFamilyRecord famRec in _database.Families)
            {
                if (_XrefID == famRec.Husband || _XrefID == famRec.Wife)
                {
                    foreach (string childID in famRec.Children)
                    {
                        fam.Children.Add(childID);
                    }
                }
            }

            return fam;
        }

        public override void GenerateXML(XmlNode root)
        {
            XmlDocument doc = root.OwnerDocument;

            XmlNode node;
            XmlAttribute attr;

            XmlNode indiNode = doc.CreateElement("IndividualRec");
            attr = doc.CreateAttribute("Id");
            attr.Value = XRefID;
            indiNode.Attributes.Append(attr);

            foreach (GedcomName name in Names)
            {
                // FIXME: should output all parts
                node = doc.CreateElement("IndivName");

                XmlNode givenNode = doc.CreateElement("NamePart");
                attr = doc.CreateAttribute("Type");
                attr.Value = "given name";
                givenNode.Attributes.Append(attr);
                attr = doc.CreateAttribute("Level");
                attr.Value = "3";
                givenNode.Attributes.Append(attr);
                givenNode.AppendChild(doc.CreateTextNode(name.Given));
                node.AppendChild(givenNode);

                XmlNode surnameNode = doc.CreateElement("NamePart");
                attr = doc.CreateAttribute("Type");
                attr.Value = "surname";
                surnameNode.Attributes.Append(attr);
                attr = doc.CreateAttribute("Level");
                attr.Value = "1";
                surnameNode.Attributes.Append(attr);
                surnameNode.AppendChild(doc.CreateTextNode(name.Surname));
                node.AppendChild(surnameNode);

                //node.AppendChild(doc.CreateTextNode(name.Name));
                indiNode.AppendChild(node);
            }

            node = doc.CreateElement("Gender");
            node.AppendChild(doc.CreateTextNode(Sex.ToString()));
            indiNode.AppendChild(node);

            if (Death != null)
            {
                node = doc.CreateElement("DeathStatus");

                string status = "dead";
                if (Death.Age != null)
                {
                    if (Death.Age.StillBorn)
                    {
                        status = "stillborn";
                    }
                    else if (Death.Age.Infant)
                    {
                        status = "infant";
                    }
                    else if (Death.Age.Child)
                    {
                        status = "child";
                    }
                }

                node.AppendChild(doc.CreateTextNode(status));
                indiNode.AppendChild(node);
            }
            else if (Dead)
            {
                node = doc.CreateElement("DeathStatus");
                node.AppendChild(doc.CreateTextNode("dead"));
                indiNode.AppendChild(node);
            }

            if (_Events != null)
            {
                foreach (GedcomIndividualEvent indiEvent in _Events)
                {
                    indiEvent.GeneratePersInfoXML(indiNode);
                }
            }

            if (_Attributes != null)
            {
                foreach (GedcomIndividualEvent indiAttribute in _Attributes)
                {
                    indiAttribute.GeneratePersInfoXML(indiNode);
                }
            }

            foreach (GedcomAssociation asso in Associations)
            {
                GedcomIndividualRecord assoIndi = _database[asso.Individual] as GedcomIndividualRecord;
                if (assoIndi != null)
                {
                    node = doc.CreateElement("AssocIndiv");

                    XmlNode linkNode = doc.CreateElement("Link");

                    attr = doc.CreateAttribute("Target");
                    attr.Value = "IndividualRec";
                    linkNode.Attributes.Append(attr);

                    attr = doc.CreateAttribute("Ref");
                    attr.Value = asso.Individual;
                    linkNode.Attributes.Append(attr);

                    node.AppendChild(linkNode);

                    XmlNode assoNode = doc.CreateElement("Association");
                    assoNode.AppendChild(doc.CreateTextNode(asso.Description));

                    node.AppendChild(assoNode);

                    asso.GenerateNoteXML(node);
                    asso.GenerateCitationsXML(node);

                    indiNode.AppendChild(node);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Pointer to non existant associated individual");
                }
            }

            GenerateNoteXML(indiNode);
            GenerateCitationsXML(indiNode);
            GenerateMultimediaXML(indiNode);

            GenerateChangeDateXML(indiNode);

            root.AppendChild(indiNode);
        }

        public override void Output(TextWriter sw)
        {
            base.Output(sw);

            GedcomName preferedName = Names.Find(n => n.PreferedName == true);
            if (preferedName != null)
            {
                preferedName.Output(sw);
            }

            foreach (GedcomName name in _Names)
            {
                if (name != preferedName)
                {
                    name.Output(sw);
                }
            }

            string levelPlusOne = Util.IntToString(Level + 1);

            sw.Write(Environment.NewLine);
            sw.Write(levelPlusOne);
            sw.Write(" SEX ");
            switch (Sex)
            {
                case GedcomSex.Male:
                    sw.Write("M");
                    break;
                case GedcomSex.Female:
                    sw.Write("F");
                    break;
                case GedcomSex.Neuter:
                    sw.Write("N");
                    break;
                case GedcomSex.Both:
                    sw.Write("B");
                    break;
                case GedcomSex.Undetermined:
                    sw.Write("U");
                    break;
                default:
                    sw.Write("U");
                    break;
            }

            if (RestrictionNotice != GedcomRestrictionNotice.None)
            {
                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" RESN ");
                sw.Write(RestrictionNotice.ToString().ToLower());
            }

            foreach (GedcomIndividualEvent individualEvent in _Events)
            {
                individualEvent.Output(sw);
            }

            if (_Attributes != null)
            {
                foreach (GedcomIndividualEvent individualEvent in _Attributes)
                {
                    individualEvent.Output(sw);
                }
            }

            foreach (GedcomFamilyLink link in _ChildIn)
            {
                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" FAMC ");
                sw.Write("@");
                sw.Write(link.Family);
                sw.Write("@");

                switch (link.Pedigree)
                {
                    case PedegreeLinkageType.Adopted:
                        sw.Write(Environment.NewLine);
                        sw.Write(Util.IntToString(Level + 2));
                        sw.Write(" PEDI adopted");
                        break;
                    case PedegreeLinkageType.Birth:
                        sw.Write(Environment.NewLine);
                        sw.Write(Util.IntToString(Level + 2));
                        sw.Write(" PEDI birth");
                        break;
                    case PedegreeLinkageType.Foster:
                        sw.Write(Environment.NewLine);
                        sw.Write(Util.IntToString(Level + 2));
                        sw.Write(" PEDI foster");
                        break;
                    case PedegreeLinkageType.Sealing:
                        sw.Write(Environment.NewLine);
                        sw.Write(Util.IntToString(Level + 2));
                        sw.Write(" PEDI sealing");
                        break;
                    default:
                        // FatherAdopted / MotherAdopted are
                        // non standard, we will put them out in
                        // the family record as _FREL/_MREL
                        // like Family Tree Maker
                        break;
                }
            }

            GedcomFamilyLink prefSpouse = SpouseIn.Find(s => s.PreferedSpouse == true);
            if (prefSpouse != null)
            {
                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" FAMS ");
                sw.Write("@");
                sw.Write(prefSpouse.Family);
                sw.Write("@");
            }

            foreach (GedcomFamilyLink link in _SpouseIn)
            {
                if (link != prefSpouse)
                {
                    sw.Write(Environment.NewLine);
                    sw.Write(levelPlusOne);
                    sw.Write(" FAMS ");
                    sw.Write("@");
                    sw.Write(link.Family);
                    sw.Write("@");
                }
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

            foreach (GedcomAssociation association in Associations)
            {
                association.Output(sw);
            }

            if (_Alia != null)
            {
                foreach (string alia in Alia)
                {
                    sw.Write(Environment.NewLine);
                    sw.Write(levelPlusOne);
                    sw.Write(" ALIA ");
                    sw.Write("@");
                    sw.Write(alia);
                    sw.Write("@");
                }
            }

            if (_Anci != null)
            {
                foreach (string anci in Anci)
                {
                    sw.Write(Environment.NewLine);
                    sw.Write(levelPlusOne);
                    sw.Write(" ANCI ");
                    sw.Write("@");
                    sw.Write(anci);
                    sw.Write("@");
                }
            }

            if (_Desi != null)
            {
                foreach (string anci in Desi)
                {
                    sw.Write(Environment.NewLine);
                    sw.Write(levelPlusOne);
                    sw.Write(" DESI ");
                    sw.Write("@");
                    sw.Write(anci);
                    sw.Write("@");
                }
            }

            if (!string.IsNullOrEmpty(PermanentRecordFileNumber))
            {
                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" RFN ");
                string line = PermanentRecordFileNumber.Replace("@", "@@");
                sw.Write(line);
            }

            if (!string.IsNullOrEmpty(AncestralFileNumber))
            {
                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" AFN ");
                string line = AncestralFileNumber.Replace("@", "@@");
                sw.Write(line);
            }

            // NOTE: this should always be NULL, it is here as
            // a hack to load data from family tree maker.
            // the address will have been converted to a RESI event upon loading
            if (Address != null)
            {
                Address.Output(sw, Level + 1);
            }
        }
    }
}
