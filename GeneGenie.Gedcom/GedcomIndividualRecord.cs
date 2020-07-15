// <copyright file="GedcomIndividualRecord.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

#define XML_NODE_UNDEFINED

namespace GeneGenie.Gedcom
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using GeneGenie.Gedcom.Enums;

#if !XML_NODE_UNDEFINED
    using System.Xml;
#endif

    /// <summary>
    /// Details about a given individual.
    /// </summary>
    public class GedcomIndividualRecord : GedcomRecord, IComparable, IComparable<GedcomIndividualRecord>, IEquatable<GedcomIndividualRecord>
    {
        private GedcomRecordList<GedcomName> names;
        private GedcomSex sex;

        /// <summary>Gets or sets the list of <see cref="GedcomCustomRecord"/> entries found when parsing an individual.</summary>
        public GedcomRecordList<GedcomCustomRecord> Custom { get; set; } = new GedcomRecordList<GedcomCustomRecord>();

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
        /// The ancestral file number..
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
            names = new GedcomRecordList<GedcomName>();
            names.CollectionChanged += ListChanged;

            // do not set capacity on events, uses more memory, at least for Database1.ged
            events = new GedcomRecordList<GedcomIndividualEvent>();
            events.CollectionChanged += ListChanged;

            childIn = new GedcomRecordList<GedcomFamilyLink>();
            childIn.CollectionChanged += ListChanged;

            spouseIn = new GedcomRecordList<GedcomFamilyLink>();
            spouseIn.CollectionChanged += ListChanged;
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
            Sex = GedcomSex.NotSet;
            XRefID = database.GenerateXref("I");

            GedcomName name = new GedcomName();
            name.Level = 1;
            name.Database = database;
            name.Name = "unknown /" + surname + "/";
            name.PreferredName = true;

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
        /// Gets the GEDCOM tag for an individual.
        /// </summary>
        /// <value>
        /// The GEDCOM tag.
        /// </value>
        public override string GedcomTag
        {
            get { return "INDI"; }
        }

        /// <summary>
        /// Gets the individual's names.
        /// </summary>
        /// <value>
        /// The names of the individual.
        /// </value>
        public GedcomRecordList<GedcomName> Names
        {
            get { return names; }
        }

        /// <summary>
        /// Gets a single letter representing the individual's gender.
        /// </summary>
        /// <value>
        /// The gender character.
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
        /// Gets the list of events.
        /// </summary>
        /// <value>
        /// The list of events.
        /// </value>
        public GedcomRecordList<GedcomIndividualEvent> Attributes
        {
            get
            {
                if (attributes == null)
                {
                    attributes = new GedcomRecordList<GedcomIndividualEvent>();
                    attributes.CollectionChanged += ListChanged;
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
                    submitterRecords.CollectionChanged += ListChanged;
                }

                return submitterRecords;
            }
        }

        /// <summary>
        /// Gets this individual's associations to others.
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
                    associations.CollectionChanged += ListChanged;
                }

                return associations;
            }
        }

        /// <summary>
        /// Gets the list of aliases.
        /// </summary>
        /// <value>
        /// The list of aliases.
        /// </value>
        public GedcomRecordList<string> Alia
        {
            get
            {
                if (alia == null)
                {
                    alia = new GedcomRecordList<string>();
                    alia.CollectionChanged += ListChanged;
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
                    anci.CollectionChanged += ListChanged;
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
                    desi.CollectionChanged += ListChanged;
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
        /// GedcomRecordReader will build the address up here, then create a RESI record from it.
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
            get { return FindEvent(GedcomEventType.Birth); }
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
            get { return Attributes.FirstOrDefault(a => a.EventType == GedcomEventType.GenericFact && string.Compare(a.EventName, "Height") == 0); }
        }

        /// <summary>
        /// Gets the weight.
        /// </summary>
        /// <value>
        /// The weight.
        /// </value>
        public GedcomIndividualEvent Weight
        {
            get { return Attributes.FirstOrDefault(a => a.EventType == GedcomEventType.GenericFact && string.Compare(a.EventName, "Weight") == 0); }
        }

        /// <summary>
        /// Gets the medical event.
        /// </summary>
        /// <value>
        /// The medical event.
        /// </value>
        public GedcomIndividualEvent Medical
        {
            get { return Attributes.FirstOrDefault(a => a.EventType == GedcomEventType.GenericFact && string.Compare(a.EventName, "Medical") == 0); }
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

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            // Overflow is fine, just wrap.
            unchecked
            {
                int hash = 17;

                var name = GetName();
                if (name != null)
                {
                    hash *= 23 + name.Name.GetHashCode();
                }

                return hash;
            }
        }

        /// <summary>
        /// Compares the current and passed individual to see if they are the same.
        /// Compares using user submitted data, not the internal ids which may change.
        /// </summary>
        /// <param name="obj">The object to compare the current individual instance against.</param>
        /// <returns>True if they match, false otherwise.</returns>
        public override bool IsEquivalentTo(object obj)
        {
            return CompareTo(obj as GedcomIndividualRecord) == 0;
        }

        /// <summary>
        /// Compares the current and passed individual to see if they are the same.
        /// Compares using user submitted data, not the internal ids which may change.
        /// </summary>
        /// <param name="individual">The individual to compare the current instance against.</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the value parameter.</returns>
        public int CompareTo(GedcomIndividualRecord individual)
        {
            /* SubmitterRecords was omitted from comparison.
             * It stores XRefId values to individuals who contribute genealogical
             * data to a file, which doesn't contribute to equality of an individual.
             *
             * ANCI and DESI were also omitted from comparison.
             * These fields indicate interest in additional research for
             * ancestors and descendants, and don't contribute to equality.
             *
             * Assocations are links to relatives, godparents, etc.
             * They're part of the comparison currently, but not sure if they should be.
             */

            if (individual == null)
            {
                return 1;
            }

            var compare = GedcomGenericListComparer.CompareListSortOrders(Names, individual.Names);
            if (compare != 0)
            {
                return compare;
            }

            compare = Sex.CompareTo(individual.Sex);
            if (compare != 0)
            {
                return compare;
            }

            compare = CompareEvents(Events, individual.Events);
            if (compare != 0)
            {
                return compare;
            }

            compare = CompareEvents(Attributes, individual.Attributes);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(UserReferenceNumber, individual.UserReferenceNumber);
            if (compare != 0)
            {
                return compare;
            }

            compare = GedcomGenericListComparer.CompareListSortOrders(ChildIn, individual.ChildIn);
            if (compare != 0)
            {
                return compare;
            }

            compare = GedcomGenericListComparer.CompareListSortOrders(SpouseIn, individual.SpouseIn);
            if (compare != 0)
            {
                return compare;
            }

            compare = GedcomGenericListComparer.CompareListSortOrders(Associations, individual.Associations);
            if (compare != 0)
            {
                return compare;
            }

            compare = GedcomGenericListComparer.CompareListOrder(Alia, individual.Alia);
            if (compare != 0)
            {
                return compare;
            }

            compare = GedcomGenericComparer.SafeCompareOrder(Address, individual.Address);
            if (compare != 0)
            {
                return compare;
            }

            return CompareNotes(individual);
        }

        /// <summary>
        /// Compares the current individual against the passed individual to see if they are
        /// essentially the same. Compares the content, not the structure.
        /// For example, names are compared but internal xref ids are not.
        /// </summary>
        /// <param name="individual">The second person to compare against.</param>
        /// <returns>TODO: Doc.</returns>
        public int CompareTo(object individual)
        {
            return CompareTo(individual as GedcomIndividualRecord);
        }

        /// <summary>
        /// Compares the current and passed individual to see if they are the same.
        /// Compares using user submitted data, not the internal ids which may change.
        /// </summary>
        /// <param name="other">The other individual to compare the current individual against.</param>
        /// <returns>True if they match, false otherwise.</returns>
        public bool Equals(GedcomIndividualRecord other)
        {
            return CompareTo(other) == 0;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public override void Delete()
        {
            base.Delete();

            // remove from families
            if (RefCount <= 0)
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
        /// Gets the preferred name (if set) or the first name if no preferred name is set.
        /// </summary>
        /// <returns>A GedcomName or null if no names found.</returns>
        public GedcomName GetName()
        {
            GedcomName ret = Names.FirstOrDefault(n => n.PreferredName == true);
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
                n.PreferredName = n == name;
            }
        }

        /// <summary>
        /// Determine whether a spouse is in the family.
        /// </summary>
        /// <param name="family">The family.</param>
        /// <returns>
        /// True if spouse is in the family; otherwise False.
        /// </returns>
        public bool SpouseInFamily(string family)
        {
            GedcomFamilyLink tmp;

            return SpouseInFamily(family, out tmp);
        }

        /// <summary>
        /// Determine whether a spouse is in the family.
        /// </summary>
        /// <param name="family">The family.</param>
        /// <param name="famLink">The family link.</param>
        /// <returns>
        /// True if spouse is in the family; otherwise False.
        /// </returns>
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
        /// Determine whether a child is in the family.
        /// </summary>
        /// <param name="family">The family.</param>
        /// <returns>
        /// True if child is in the family; otherwise False.
        /// </returns>
        public bool ChildInFamily(string family)
        {
            GedcomFamilyLink tmp;

            return ChildInFamily(family, out tmp);
        }

        /// <summary>
        /// Determines whether a child is in the family.
        /// </summary>
        /// <param name="family">The family.</param>
        /// <param name="famLink">The fam link.</param>
        /// <returns>
        /// True if child is in the family; otherwise False.
        /// </returns>
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
        /// <returns>
        /// Family.
        /// </returns>
        public GedcomFamilyRecord GetFamily()
        {
            GedcomFamilyRecord fam = null;

            GedcomFamilyLink link = SpouseIn.FirstOrDefault(f => (f.PreferedSpouse == true));

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
        /// Sets the preferred spouse.
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
        /// <returns>
        /// The event if found; otherwise null.
        /// </returns>
        public GedcomIndividualEvent FindEvent(GedcomEventType eventType)
        {
            return events.FirstOrDefault(e => e.EventType == eventType);
        }

        /// <summary>
        /// Matches the surname.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="soundex">if set to <c>true</c> [soundex].</param>
        /// <returns>
        /// True if the surname starts with prefix; otherwise False.
        /// </returns>
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
                    surname = Constants.UnknownNamePart;
                }
                else
                {
                    surname = Constants.UnknownSoundex;
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
        /// <returns>
        /// True if the firstname starts with prefix; otherwise False.
        /// </returns>
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
                    checkName = Constants.UnknownNamePart;
                }
                else
                {
                    checkName = Constants.UnknownSoundex;
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

        /// <summary>
        /// Determines whether the passed individual is a match for the current instance
        /// based on key user entered data.
        /// </summary>
        /// <param name="individual">The individual to compare this instance against.</param>
        /// <returns>A score from 0 to 100 representing the percentage match.</returns>
        public decimal CalculateSimilarityScore(GedcomIndividualRecord individual)
        {
            var match = decimal.Zero;

            // check name
            var nameMatch = decimal.Zero;
            foreach (GedcomName indiName in individual.Names)
            {
                foreach (GedcomName name in Names)
                {
                    var currentNameMatch = name.CalculateSimilarityScore(indiName);

                    nameMatch = Math.Max(nameMatch, currentNameMatch);
                }
            }

            // 0% name match would be pointless checking other details
            if (nameMatch != decimal.Zero)
            {
                // check gender
                var genderMatch = decimal.Zero;
                if (Sex == individual.Sex)
                {
                    genderMatch = 100m;
                }

                // check dob
                var dobMatch = decimal.Zero;
                GedcomEvent birth = Birth;
                GedcomEvent indiBirth = individual.Birth;
                if (birth != null && indiBirth != null)
                {
                    dobMatch = birth.CalculateSimilarityScore(indiBirth);
                }
                else if (birth == null && indiBirth == null)
                {
                    dobMatch = 100m;
                }

                // check dod
                var dodMatch = decimal.Zero;
                GedcomEvent death = Death;
                GedcomEvent indiDeath = individual.Death;
                if (death != null && indiDeath != null)
                {
                    dodMatch = death.CalculateSimilarityScore(indiDeath);
                }
                else if (death == null && indiDeath == null)
                {
                    dodMatch = 100m;
                }

                match = (nameMatch + genderMatch + dobMatch + dodMatch) / 4m;
            }

            return match;
        }

        /// <summary>
        /// Gets a "fake" family record that contains all of this individual's children.
        /// The record will only contain children.
        /// </summary>
        /// <returns>
        /// A <see cref="GedcomFamilyRecord"/>.
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

#if !XML_NODE_UNDEFINED
        /// <summary>
        /// Generates the XML.
        /// </summary>
        /// <param name="root">The root node.</param>
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
#endif

        /// <summary>
        /// Outputs this instance of an individual as a GEDCOM record.
        /// </summary>
        /// <param name="tw">The textwriter to output to.</param>
        public override void Output(TextWriter tw)
        {
            base.Output(tw);

            GedcomName preferedName = Names.FirstOrDefault(n => n.PreferredName == true);
            if (preferedName != null)
            {
                preferedName.Output(tw);
            }

            foreach (GedcomName name in names)
            {
                if (name != preferedName)
                {
                    name.Output(tw);
                }
            }

            string levelPlusOne = (Level + 1).ToString();

            if (Sex != GedcomSex.NotSet)
            {
                tw.Write(Environment.NewLine);
                tw.Write(levelPlusOne);
                tw.Write(" SEX ");
                switch (Sex)
                {
                    case GedcomSex.Male:
                        tw.Write("M");
                        break;
                    case GedcomSex.Female:
                        tw.Write("F");
                        break;
                    case GedcomSex.Neuter:
                        tw.Write("N");
                        break;
                    case GedcomSex.Both:
                        tw.Write("B");
                        break;
                    case GedcomSex.Undetermined:
                        tw.Write("U");
                        break;
                }
            }

            if (RestrictionNotice != GedcomRestrictionNotice.None)
            {
                tw.Write(Environment.NewLine);
                tw.Write(levelPlusOne);
                tw.Write(" RESN ");
                tw.Write(RestrictionNotice.ToString().ToLower());
            }

            foreach (GedcomIndividualEvent individualEvent in events)
            {
                individualEvent.Output(tw);
            }

            if (attributes != null)
            {
                foreach (GedcomIndividualEvent individualEvent in attributes)
                {
                    individualEvent.Output(tw);
                }
            }

            foreach (GedcomFamilyLink link in childIn)
            {
                tw.Write(Environment.NewLine);
                tw.Write(levelPlusOne);
                tw.Write(" FAMC ");
                tw.Write("@");
                tw.Write(link.Family);
                tw.Write("@");

                switch (link.Pedigree)
                {
                    case PedigreeLinkageType.Adopted:
                        tw.Write(Environment.NewLine);
                        tw.Write((Level + 2).ToString());
                        tw.Write(" PEDI adopted");
                        break;
                    case PedigreeLinkageType.Birth:
                        tw.Write(Environment.NewLine);
                        tw.Write((Level + 2).ToString());
                        tw.Write(" PEDI birth");
                        break;
                    case PedigreeLinkageType.Foster:
                        tw.Write(Environment.NewLine);
                        tw.Write((Level + 2).ToString());
                        tw.Write(" PEDI foster");
                        break;
                    case PedigreeLinkageType.Sealing:
                        tw.Write(Environment.NewLine);
                        tw.Write((Level + 2).ToString());
                        tw.Write(" PEDI sealing");
                        break;
                    default:
                        // FatherAdopted / MotherAdopted are
                        // non standard, we will put them out in
                        // the family record as _FREL/_MREL
                        // like Family Tree Maker
                        break;
                }
            }

            GedcomFamilyLink prefSpouse = SpouseIn.FirstOrDefault(s => s.PreferedSpouse == true);
            if (prefSpouse != null)
            {
                tw.Write(Environment.NewLine);
                tw.Write(levelPlusOne);
                tw.Write(" FAMS ");
                tw.Write("@");
                tw.Write(prefSpouse.Family);
                tw.Write("@");
            }

            foreach (GedcomFamilyLink link in spouseIn)
            {
                if (link != prefSpouse)
                {
                    tw.Write(Environment.NewLine);
                    tw.Write(levelPlusOne);
                    tw.Write(" FAMS ");
                    tw.Write("@");
                    tw.Write(link.Family);
                    tw.Write("@");
                }
            }

            if (submitterRecords != null)
            {
                foreach (string submitter in SubmitterRecords)
                {
                    tw.Write(Environment.NewLine);
                    tw.Write(levelPlusOne);
                    tw.Write(" SUBM ");
                    tw.Write("@");
                    tw.Write(submitter);
                    tw.Write("@");
                }
            }

            foreach (GedcomAssociation association in Associations)
            {
                association.Output(tw);
            }

            if (alia != null)
            {
                foreach (string aliaValue in Alia)
                {
                    tw.Write(Environment.NewLine);
                    tw.Write(levelPlusOne);
                    tw.Write(" ALIA ");
                    tw.Write("@");
                    tw.Write(aliaValue);
                    tw.Write("@");
                }
            }

            if (anci != null)
            {
                foreach (string anciValue in Anci)
                {
                    tw.Write(Environment.NewLine);
                    tw.Write(levelPlusOne);
                    tw.Write(" ANCI ");
                    tw.Write("@");
                    tw.Write(anciValue);
                    tw.Write("@");
                }
            }

            if (desi != null)
            {
                foreach (string desiValue in Desi)
                {
                    tw.Write(Environment.NewLine);
                    tw.Write(levelPlusOne);
                    tw.Write(" DESI ");
                    tw.Write("@");
                    tw.Write(desiValue);
                    tw.Write("@");
                }
            }

            if (!string.IsNullOrEmpty(PermanentRecordFileNumber))
            {
                tw.Write(Environment.NewLine);
                tw.Write(levelPlusOne);
                tw.Write(" RFN ");
                string line = PermanentRecordFileNumber.Replace("@", "@@");
                tw.Write(line);
            }

            if (!string.IsNullOrEmpty(AncestralFileNumber))
            {
                tw.Write(Environment.NewLine);
                tw.Write(levelPlusOne);
                tw.Write(" AFN ");
                string line = AncestralFileNumber.Replace("@", "@@");
                tw.Write(line);
            }

            // NOTE: this should always be NULL, it is here as
            // a hack to load data from family tree maker.
            // the address will have been converted to a RESI event upon loading
            if (Address != null)
            {
                Address.Output(tw, Level + 1);
            }
        }

        private int CompareEvents(GedcomRecordList<GedcomIndividualEvent> thisEvents, GedcomRecordList<GedcomIndividualEvent> otherEvents)
        {
            // TODO: This is a long winded and non-reusable way of comparing two lists. Can we use a generic comparer?
            if (thisEvents.Count > otherEvents.Count)
            {
                return 1;
            }

            if (otherEvents.Count > thisEvents.Count)
            {
                return -1;
            }

            for (int i = 0; i < otherEvents.Count(); i++)
            {
                var otherEvent = otherEvents.ElementAt(i);
                var thisEvent = thisEvents.ElementAt(i);

                var compare = GedcomGenericComparer.SafeCompareOrder(thisEvent, otherEvent);
                if (compare != 0)
                {
                    return compare;
                }
            }

            return 0;
        }

        private int CompareNotes(GedcomIndividualRecord individual)
        {
            // TODO: This is a long winded and non-reusable way of comparing two lists. Can we use a generic comparer and place this into the GedcomNote class?
            if (Notes.Count > individual.Notes.Count)
            {
                return 1;
            }

            if (Notes.Count < individual.Notes.Count)
            {
                return -1;
            }

            var indiList = individual.Notes.OrderBy(noteXRef => noteXRef.GetHashCode());
            var noteList = Notes.OrderBy(noteXRef => noteXRef.GetHashCode());
            for (int i = 0; i < noteList.Count(); i++)
            {
                var indiNoteXref = indiList.ElementAt(i);
                var noteXref = noteList.ElementAt(i);
                var indiNote = individual.Database[indiNoteXref] as GedcomNoteRecord;
                var note = Database[noteXref] as GedcomNoteRecord;

                if (indiNote == null && note == null)
                {
                    return 0;
                }

                if (indiNote == null)
                {
                    return -1;
                }

                if (note == null)
                {
                    return 1;
                }

                var indiNoteUtf8 = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.Default.GetBytes(indiNote.Text));
                var compare = string.Compare(note.Text, indiNoteUtf8);
                if (compare != 0)
                {
                    return compare;
                }
            }

            return 0;
        }
    }
}
