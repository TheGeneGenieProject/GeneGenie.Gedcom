// <copyright file="GedcomIndividualRecord.cs" company="GeneGenie.com">
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
    using System.Globalization;
    using System.IO;
    using System.Xml;
    using Enums;

    /// <summary>
    /// Details about a given individual
    /// </summary>
    public class GedcomIndividualRecord : GedcomRecord, IComparable
    {
        /// <summary>
        /// The unknown name
        /// </summary>
        public const string UnknownName = "unknown /unknown/";

        /// <summary>
        /// The unknown name part
        /// </summary>
        public const string UnknownNamePart = "unknown";

        /// <summary>
        /// The unknown soundex
        /// </summary>
        public const string UnknownSoundex = "u525";

        private GedcomRecordList<GedcomName> names;
        private GedcomSex sex;

        private GedcomRecordList<GedcomIndividualEvent> events;
        private GedcomRecordList<GedcomIndividualEvent> attributes;

        // TODO
        private object lDSIndividualOrdinances;

        private GedcomRecordList<GedcomFamilyLink> childIn;
        private GedcomRecordList<GedcomFamilyLink> spouseIn;

        private GedcomRecordList<string> submitterRecords;

        private GedcomRecordList<GedcomAssociation> associations;

        private GedcomRecordList<string> alia;
        private GedcomRecordList<string> anci;
        private GedcomRecordList<string> desi;

        private string permanentRecordFileNumber;

        /// <summary>
        /// The ancestral file number
        /// </summary>
        private string ancestralFileNumber;

        // This is a hack,  not according to the spec, but Family Tree Maker sticks
        // an address under an individual.
        // GedcomRecordReader will build the address up here, then create a RESI record from it
        private GedcomAddress address;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomIndividualRecord"/> class.
        /// </summary>
        public GedcomIndividualRecord()
        {
            // avoid resize, indi will nearly always have a name
            names = new GedcomRecordList<GedcomName>(1);
            names.Changed += ListChanged;

            // do not set capacity on events, uses more memory, at least for Database1.ged
            events = new GedcomRecordList<GedcomIndividualEvent>();
            events.Changed += ListChanged;

            childIn = new GedcomRecordList<GedcomFamilyLink>();
            childIn.Changed += ListChanged;

            spouseIn = new GedcomRecordList<GedcomFamilyLink>();
            spouseIn.Changed += ListChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomIndividualRecord"/> class.
        /// </summary>
        /// <param name="database">The database to associate with this record.</param>
        public GedcomIndividualRecord(GedcomDatabase database)
            : this(database, "unknown")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomIndividualRecord" /> class.
        /// </summary>
        /// <param name="database">The database to associate with this record.</param>
        /// <param name="surname">The surname.</param>
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

        /// <summary>
        /// Gets the type of the record.
        /// </summary>
        /// <value>
        /// The type of the record.
        /// </value>
        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Individual; }
        }

        /// <summary>
        /// Gets the gedcom tag.
        /// </summary>
        /// <value>
        /// The gedcom tag.
        /// </value>
        public override string GedcomTag
        {
            get { return "INDI"; }
        }

        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        public GedcomRecordList<GedcomName> Names
        {
            get { return names; }
        }

        /// <summary>
        /// Gets the sex character.
        /// </summary>
        /// <value>
        /// The sex character.
        /// </value>
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

        /// <summary>
        /// Gets or sets the sex.
        /// </summary>
        /// <value>
        /// The sex.
        /// </value>
        public GedcomSex Sex
        {
            get
            {
                return sex;
            }

            set
            {
                if (value != sex)
                {
                    // walk through all families for the individual, where they are
                    // the only spouse switch them to be husband or wife, whichever
                    // is correct
                    foreach (GedcomFamilyLink link in SpouseIn)
                    {
                        GedcomFamilyRecord fam = Database[link.Family] as GedcomFamilyRecord;
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

                    sex = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        public GedcomRecordList<GedcomIndividualEvent> Events
        {
            get { return events; }
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public GedcomRecordList<GedcomIndividualEvent> Attributes
        {
            get
            {
                if (attributes == null)
                {
                    attributes = new GedcomRecordList<GedcomIndividualEvent>();
                    attributes.Changed += ListChanged;
                }

                return attributes;
            }
        }

        /// <summary>
        /// Gets the child in.
        /// </summary>
        /// <value>
        /// The child in.
        /// </value>
        public GedcomRecordList<GedcomFamilyLink> ChildIn
        {
            get { return childIn; }
        }

        /// <summary>
        /// Gets the spouse in.
        /// </summary>
        /// <value>
        /// The spouse in.
        /// </value>
        public GedcomRecordList<GedcomFamilyLink> SpouseIn
        {
            get { return spouseIn; }
        }

        /// <summary>
        /// Gets the submitter records.
        /// </summary>
        /// <value>
        /// The submitter records.
        /// </value>
        public GedcomRecordList<string> SubmitterRecords
        {
            get
            {
                if (submitterRecords == null)
                {
                    submitterRecords = new GedcomRecordList<string>();
                    submitterRecords.Changed += ListChanged;
                }

                return submitterRecords;
            }
        }

        /// <summary>
        /// Gets the associations.
        /// </summary>
        /// <value>
        /// The associations.
        /// </value>
        public GedcomRecordList<GedcomAssociation> Associations
        {
            get
            {
                if (associations == null)
                {
                    associations = new GedcomRecordList<GedcomAssociation>();
                    associations.Changed += ListChanged;
                }

                return associations;
            }
        }

        /// <summary>
        /// Gets the alia.
        /// </summary>
        /// <value>
        /// The alia.
        /// </value>
        public GedcomRecordList<string> Alia
        {
            get
            {
                if (alia == null)
                {
                    alia = new GedcomRecordList<string>();
                    alia.Changed += ListChanged;
                }

                return alia;
            }
        }

        /// <summary>
        /// Gets the anci.
        /// </summary>
        /// <value>
        /// The anci.
        /// </value>
        public GedcomRecordList<string> Anci
        {
            get
            {
                if (anci == null)
                {
                    anci = new GedcomRecordList<string>();
                    anci.Changed += ListChanged;
                }

                return anci;
            }
        }

        /// <summary>
        /// Gets the desi.
        /// </summary>
        /// <value>
        /// The desi.
        /// </value>
        public GedcomRecordList<string> Desi
        {
            get
            {
                if (desi == null)
                {
                    desi = new GedcomRecordList<string>();
                    desi.Changed += ListChanged;
                }

                return desi;
            }
        }

        /// <summary>
        /// Gets or sets the permanent record file number.
        /// </summary>
        /// <value>
        /// The permanent record file number.
        /// </value>
        public string PermanentRecordFileNumber
        {
            get
            {
                return permanentRecordFileNumber;
            }

            set
            {
                if (value != permanentRecordFileNumber)
                {
                    permanentRecordFileNumber = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the ancestral file number.
        /// </summary>
        /// <value>
        /// The ancestral file number.
        /// </value>
        public string AncestralFileNumber
        {
            get
            {
                return ancestralFileNumber;
            }

            set
            {
                if (value != ancestralFileNumber)
                {
                    ancestralFileNumber = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the address.
        /// This is a hack,  not according to the spec, but Family Tree Maker sticks
        /// an address under an individual.
        /// GedcomRecordReader will build the address up here, then create a RESI record from it
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public GedcomAddress Address
        {
            get
            {
                return address;
            }

            set
            {
                if (value != address)
                {
                    address = value;
                    Changed();
                }
            }
        }

        // Util properties to get specific events

        /// <summary>
        /// Gets the birth.
        /// </summary>
        /// <value>
        /// The birth.
        /// </value>
        public GedcomIndividualEvent Birth
        {
            get { return FindEvent(GedcomEventType.BIRT); }
        }

        /// <summary>
        /// Gets the death.
        /// </summary>
        /// <value>
        /// The death.
        /// </value>
        public GedcomIndividualEvent Death
        {
            get { return FindEvent(GedcomEventType.DEAT); }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public GedcomIndividualEvent Height
        {
            get { return Attributes.Find(a => a.EventType == GedcomEventType.GenericFact && string.Compare(a.EventName, "Height") == 0); }
        }

        /// <summary>
        /// Gets the weight.
        /// </summary>
        /// <value>
        /// The weight.
        /// </value>
        public GedcomIndividualEvent Weight
        {
            get { return Attributes.Find(a => a.EventType == GedcomEventType.GenericFact && string.Compare(a.EventName, "Weight") == 0); }
        }

        /// <summary>
        /// Gets the medical.
        /// </summary>
        /// <value>
        /// The medical.
        /// </value>
        public GedcomIndividualEvent Medical
        {
            get { return Attributes.Find(a => a.EventType == GedcomEventType.GenericFact && string.Compare(a.EventName, "Medical") == 0); }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="GedcomIndividualRecord"/> is dead.
        /// </summary>
        /// <value>
        ///   <c>true</c> if dead; otherwise, <c>false</c>.
        /// </value>
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

                            // TODO: Arbitrary age limit, yes someone may somehow reach 120
                            // but not likely, should output "probably dead" but that
                            // would be invalid
                            if (diff > 120)
                            {
                                ret = true;
                            }
                        }
                    }

                    // TODO: we can do better here, children can't be born > 9.x months
                    // after death of a parent, marriage can't be after death,
                    // can look at parents
                    // birth date and see from that if this indi has to be dead or not
                }

                return ret;
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
                    record = Database[submitterID];
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

        /// <summary>
        /// Compares the name of the by.
        /// </summary>
        /// <param name="indiA">The indi a.</param>
        /// <param name="indiB">The indi b.</param>
        /// <returns>TODO: Doc</returns>
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

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="indiB">The indi b.</param>
        /// <returns>TODO: Doc</returns>
        public int CompareTo(object indiB)
        {
            return GedcomIndividualRecord.CompareByName(this, (GedcomIndividualRecord)indiB);
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public override void Delete()
        {
            base.Delete();

            // remove from families
            if (RefCount == 0)
            {
                if (spouseIn != null)
                {
                    foreach (GedcomFamilyLink spouseLink in SpouseIn)
                    {
                        GedcomFamilyRecord famRec = (GedcomFamilyRecord)Database[spouseLink.Family];

                        // don't use famRec.RemoveHusband/famRecRemoveWife, they
                        // will attempt to remove the link
                        if (famRec.Husband == XrefId)
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

                if (childIn != null)
                {
                    foreach (GedcomFamilyLink childLink in ChildIn)
                    {
                        GedcomFamilyRecord famRec = (GedcomFamilyRecord)Database[childLink.Family];

                        // don't use famRec.RemoveChild as it will attempt to remove the link
                        famRec.Children.Remove(XrefId);

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
                if (names != null)
                {
                    foreach (GedcomName name in names)
                    {
                        name.Delete();
                    }
                }

                if (events != null)
                {
                    foreach (GedcomEvent ev in events)
                    {
                        ev.Delete();
                    }
                }

                if (attributes != null)
                {
                    foreach (GedcomEvent ev in attributes)
                    {
                        ev.Delete();
                    }
                }

                if (associations != null)
                {
                    foreach (GedcomAssociation asso in associations)
                    {
                        asso.Delete();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>TODO: Doc</returns>
        public GedcomName GetName()
        {
            GedcomName ret = Names.Find(n => n.PreferedName == true);
            if (ret == null && Names.Count > 0)
            {
                ret = Names[0];
            }

            return ret;
        }

        /// <summary>
        /// Sets the name of the prefered.
        /// </summary>
        /// <param name="name">The name.</param>
        public void SetPreferedName(GedcomName name)
        {
            foreach (GedcomName n in Names)
            {
                n.PreferedName = n == name;
            }
        }

        /// <summary>
        /// Spouses the in family.
        /// </summary>
        /// <param name="family">The family.</param>
        /// <returns>TODO: Doc</returns>
        public bool SpouseInFamily(string family)
        {
            GedcomFamilyLink tmp;

            return SpouseInFamily(family, out tmp);
        }

        /// <summary>
        /// Spouses the in family.
        /// </summary>
        /// <param name="family">The family.</param>
        /// <param name="famLink">The fam link.</param>
        /// <returns>TODO: Doc</returns>
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

        /// <summary>
        /// Childs the in family.
        /// </summary>
        /// <param name="family">The family.</param>
        /// <returns>TODO: Doc</returns>
        public bool ChildInFamily(string family)
        {
            GedcomFamilyLink tmp;

            return ChildInFamily(family, out tmp);
        }

        /// <summary>
        /// Determines if there is a child in the family.
        /// </summary>
        /// <param name="family">The family.</param>
        /// <param name="famLink">The fam link.</param>
        /// <returns>TODO: Doc</returns>
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

        /// <summary>
        /// Gets the family.
        /// </summary>
        /// <returns>TODO: Doc</returns>
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
                fam = Database[link.Family] as GedcomFamilyRecord;
            }

            return fam;
        }

        /// <summary>
        /// Sets the prefered spouse.
        /// </summary>
        /// <param name="xrefID">The xref identifier.</param>
        public void SetPreferedSpouse(string xrefID)
        {
            foreach (GedcomFamilyLink link in SpouseIn)
            {
                string famID = link.Family;
                GedcomFamilyRecord famRec = Database[famID] as GedcomFamilyRecord;
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

        /// <summary>
        /// Finds the event.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>TODO: Doc</returns>
        public GedcomIndividualEvent FindEvent(GedcomEventType eventType)
        {
            return events.Find(e => e.EventType == eventType);
        }

        /// <summary>
        /// Matches the surname.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="soundex">if set to <c>true</c> [soundex].</param>
        /// <returns>TODO: Doc</returns>
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

        /// <summary>
        /// Matches the firstname.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="soundex">if set to <c>true</c> [soundex].</param>
        /// <returns>TODO: Doc</returns>
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

        /// <summary>
        /// Determines whether the specified indi is match.
        /// </summary>
        /// <param name="indi">The indi.</param>
        /// <returns>TODO: Doc</returns>
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

                // System.Console.WriteLine("name: " + nameMatch + "\tdob: " + dobMatch + "\tdod: " + dodMatch);
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
            foreach (GedcomFamilyRecord famRec in Database.Families)
            {
                if (XrefId == famRec.Husband || XrefId == famRec.Wife)
                {
                    foreach (string childID in famRec.Children)
                    {
                        fam.Children.Add(childID);
                    }
                }
            }

            return fam;
        }

        /// <summary>
        /// Generates the XML.
        /// </summary>
        /// <param name="root">The root.</param>
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
                // TODO: should output all parts
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

                // node.AppendChild(doc.CreateTextNode(name.Name));
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

            if (events != null)
            {
                foreach (GedcomIndividualEvent indiEvent in events)
                {
                    indiEvent.GeneratePersInfoXML(indiNode);
                }
            }

            if (attributes != null)
            {
                foreach (GedcomIndividualEvent indiAttribute in attributes)
                {
                    indiAttribute.GeneratePersInfoXML(indiNode);
                }
            }

            foreach (GedcomAssociation asso in Associations)
            {
                GedcomIndividualRecord assoIndi = Database[asso.Individual] as GedcomIndividualRecord;
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

        /// <summary>
        /// Outputs the specified sw.
        /// </summary>
        /// <param name="sw">The sw.</param>
        public override void Output(TextWriter sw)
        {
            base.Output(sw);

            GedcomName preferedName = Names.Find(n => n.PreferedName == true);
            if (preferedName != null)
            {
                preferedName.Output(sw);
            }

            foreach (GedcomName name in names)
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

            foreach (GedcomIndividualEvent individualEvent in events)
            {
                individualEvent.Output(sw);
            }

            if (attributes != null)
            {
                foreach (GedcomIndividualEvent individualEvent in attributes)
                {
                    individualEvent.Output(sw);
                }
            }

            foreach (GedcomFamilyLink link in childIn)
            {
                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" FAMC ");
                sw.Write("@");
                sw.Write(link.Family);
                sw.Write("@");

                switch (link.Pedigree)
                {
                    case PedigreeLinkageType.Adopted:
                        sw.Write(Environment.NewLine);
                        sw.Write(Util.IntToString(Level + 2));
                        sw.Write(" PEDI adopted");
                        break;
                    case PedigreeLinkageType.Birth:
                        sw.Write(Environment.NewLine);
                        sw.Write(Util.IntToString(Level + 2));
                        sw.Write(" PEDI birth");
                        break;
                    case PedigreeLinkageType.Foster:
                        sw.Write(Environment.NewLine);
                        sw.Write(Util.IntToString(Level + 2));
                        sw.Write(" PEDI foster");
                        break;
                    case PedigreeLinkageType.Sealing:
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

            foreach (GedcomFamilyLink link in spouseIn)
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

            if (submitterRecords != null)
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

            if (alia != null)
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

            if (anci != null)
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

            if (desi != null)
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
