// <copyright file="GedcomEvent.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using GeneGenie.Gedcom.Enums;

    /// <summary>
    /// Defines a generic event or fact
    /// </summary>
    public class GedcomEvent : GedcomRecord, IComparable, IComparable<GedcomEvent>, IEquatable<GedcomEvent>
    {
        private static string[] typeStrings = new string[]
        {
            "EVEN",

            // Family Events
            "ANUL",
            "CENS",
            "DIV",
            "DIVF",
            "ENGA",
            "MARB",
            "MARC",
            "MARR",
            "MARL",
            "MARS",
            "RESI",

            // Individual Events
            "BIRT",
            "CHR",
            "DEAT",
            "BURI",
            "CREM",
            "ADOP",
            "BAPM",
            "BARM",
            "BASM",
            "BLES",
            "CHRA",
            "CONF",
            "FCOM",
            "ORDN",
            "NATU",
            "EMIG",
            "IMMI",
            "CENS",
            "PROB",
            "WILL",
            "GRAD",
            "RETI",

            // Facts
            "FACT",
            "CAST",
            "DSCR",
            "EDUC",
            "IDNO",
            "NATI",
            "NCHI",
            "NMR",
            "OCCU",
            "PROP",
            "RELI",
            "RESI",
            "SSN",
            "TITL",

            // GEDCOM allows custom records, beginging with _
            "_UNKN",
        };

        private static List<string> typeDescriptions = new List<string>()
        {
            "Other Event",
            "Annulment",
            "Census",
            "Divorce",
            "Divorce Filed",
            "Engagement",
            "Marriage Bann",
            "Marriage Contract",
            "Marriage",
            "Marriage License",
            "Marriage Settlement",
            "Residence",
            "Birth",
            "Christening",
            "Death",
            "Burial",
            "Cremation",
            "Adoption",
            "Baptism",
            "Bar Mitzvah",
            "Bas Mitzvah",
            "Blessing",
            "Adult Christening",
            "Confirmation",
            "First Communion",
            "Ordination",
            "Naturalization",
            "Emigration",
            "Immigration",
            "Census",
            "Probate",
            "Will",
            "Graduation",
            "Retirement",
            "Other Fact",
            "Caste",
            "Physical Description",
            "Education",
            "Identification Number",
            "Nationality",
            "Number of Children",
            "Number of Marriages",
            "Occupation",
            "Property",
            "Religion",
            "Residence",
            "Social Security Number",
            "Title",
            "Custom",
        };

        /// <summary>
        /// The GEDCOM event type
        /// </summary>
        private GedcomEventType eventType;

        /// <summary>
        /// The classification
        /// </summary>
        private string classification;

        /// <summary>
        /// The certainty
        /// </summary>
        private GedcomCertainty certainty = GedcomCertainty.Unknown;

        /// <summary>
        /// The record
        /// </summary>
        private GedcomRecord record;

        /// <summary>
        /// Used for Gedcom 6 XML output
        /// </summary>
        private string eventXRefID;

        /// <summary>
        /// The event name
        /// </summary>
        private string eventName;

        /// <summary>
        /// The date
        /// </summary>
        private GedcomDate date;

        /// <summary>
        /// The place
        /// </summary>
        private GedcomPlace place;

        /// <summary>
        /// The address
        /// </summary>
        private GedcomAddress address;

        /// <summary>
        /// The responsible agency
        /// </summary>
        private string responsibleAgency;

        /// <summary>
        /// The religious affiliation
        /// </summary>
        private string religiousAffiliation;

        /// <summary>
        /// The cause
        /// </summary>
        private string cause;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomEvent"/> class.
        /// </summary>
        public GedcomEvent()
        {
            // default event type is generic, need to set event name
            // or it will not be set if the record actually is a generic event
        }

        /// <summary>
        /// Gets the type of the record.
        /// </summary>
        /// <value>
        /// The type of the record.
        /// </value>
        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Event; }
        }

        /// <summary>
        /// Gets the gedcom tag.
        /// </summary>
        /// <value>
        /// The gedcom tag.
        /// </value>
        public override string GedcomTag
        {
            get { return GedcomEvent.TypeToTag(EventType); }
        }

        /// <summary>
        /// Gets or sets the type of the event.
        /// </summary>
        /// <value>
        /// The type of the event.
        /// </value>
        public GedcomEventType EventType
        {
            get
            {
                return eventType;
            }

            set
            {
                if (value != eventType)
                {
                    eventType = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the event.
        /// </summary>
        /// <value>
        /// The name of the event.
        /// </value>
        public string EventName
        {
            get
            {
                return eventName;
            }

            set
            {
                if (value != eventName)
                {
                    eventName = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the classification.
        /// </summary>
        /// <value>
        /// The classification.
        /// </value>
        public string Classification
        {
            get
            {
                return classification;
            }

            set
            {
                if (value != classification)
                {
                    classification = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public GedcomDate Date
        {
            get
            {
                return date;
            }

            set
            {
                if (value != date)
                {
                    date = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the place.
        /// </summary>
        /// <value>
        /// The place.
        /// </value>
        public GedcomPlace Place
        {
            get
            {
                return place;
            }

            set
            {
                if (value != place)
                {
                    place = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the address.
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

        /// <summary>
        /// Gets or sets the responsible agency.
        /// </summary>
        /// <value>
        /// The responsible agency.
        /// </value>
        public string ResponsibleAgency
        {
            get
            {
                return responsibleAgency;
            }

            set
            {
                if (value != responsibleAgency)
                {
                    responsibleAgency = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the religious affiliation.
        /// </summary>
        /// <value>
        /// The religious affiliation.
        /// </value>
        public string ReligiousAffiliation
        {
            get
            {
                return religiousAffiliation;
            }

            set
            {
                if (value != religiousAffiliation)
                {
                    religiousAffiliation = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the cause.
        /// </summary>
        /// <value>
        /// The cause.
        /// </value>
        public string Cause
        {
            get
            {
                return cause;
            }

            set
            {
                if (value != cause)
                {
                    cause = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the certainty.
        /// </summary>
        /// <value>
        /// The certainty.
        /// </value>
        public GedcomCertainty Certainty
        {
            get
            {
                return certainty;
            }

            set
            {
                if (value != certainty)
                {
                    certainty = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the event x reference identifier.
        /// </summary>
        /// <value>
        /// The event x reference identifier.
        /// </value>
        public string EventXRefID
        {
            get
            {
                return eventXRefID;
            }

            set
            {
                if (value != eventXRefID)
                {
                    eventXRefID = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the record.
        /// </summary>
        /// <value>
        /// The record.
        /// </value>
        public GedcomRecord Record
        {
            get
            {
                return record;
            }

            set
            {
                if (value != record)
                {
                    record = value;
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
        public override GedcomChangeDate ChangeDate
        {
            get
            {
                GedcomChangeDate realChangeDate = base.ChangeDate;
                GedcomChangeDate childChangeDate;
                if (Address != null)
                {
                    childChangeDate = Address.ChangeDate;
                    if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                    {
                        realChangeDate = childChangeDate;
                    }
                }

                if (Place != null)
                {
                    childChangeDate = Place.ChangeDate;
                    if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                    {
                        realChangeDate = childChangeDate;
                    }
                }

                if (Date != null)
                {
                    childChangeDate = Date.ChangeDate;
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
        /// Get the user-friendly textual description for a GedcomEventType.
        /// </summary>
        /// <param name="eventType">A GEDCOM event type.</param>
        /// <returns>
        /// The textual description for a given GedcomEventType.
        /// </returns>
        public static string TypeToReadable(GedcomEventType eventType)
        {
            return typeDescriptions[(int)eventType];
        }

        /// <summary>
        /// Get the tag for a GedcomEventType.
        /// </summary>
        /// <param name="eventType">A GEDCOM event type.</param>
        /// <returns>
        /// The tag for a given GedcomEventType.
        /// </returns>
        public static string TypeToTag(GedcomEventType eventType)
        {
            return typeStrings[(int)eventType];
        }

        /// <summary>
        /// Attempts to determine a standard event type from a textual
        /// description.  Always returns GenericEvent if one can't be found
        /// even though where the string came from maybe a FACT
        /// </summary>
        /// <param name="readable">The type name as a string.</param>
        /// <returns>
        /// A GedcomEventType matching the textual description, or GenericEvent if no match was found.
        /// </returns>
        public static GedcomEventType ReadableToType(string readable)
        {
            GedcomEventType ret = GedcomEventType.GenericEvent;

            int i = typeDescriptions.IndexOf(readable);
            if (i != -1)
            {
                ret = (GedcomEventType)i;
            }

            return ret;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public override void Delete()
        {
            base.Delete();

            if (date != null)
            {
                date.Delete();
            }

            if (place != null)
            {
                place.Delete();
            }
        }

        /// <summary>
        /// Compares two events to see if the date and place are the same.
        /// </summary>
        /// <param name="obj">The event instance to compare against.</param>
        /// <returns>Relative position in the sort order.</returns>
        public int CompareTo(object obj)
        {
            return CompareTo(obj as GedcomEvent);
        }

        /// <summary>
        /// Compares two events to see if the date and place are the same.
        /// </summary>
        /// <param name="eventToCompare">The event instance to compare against.</param>
        /// <returns>Relative position in the sort order.</returns>
        public int CompareTo(GedcomEvent eventToCompare)
        {
            if (eventToCompare == null)
            {
                return -1;
            }

            if (eventToCompare.Date == null && Date != null)
            {
                return -1;
            }

            if (Date == null && eventToCompare.Date != null)
            {
                return -1;
            }

            var compare = GedcomDate.CompareByDate(Date, eventToCompare.Date);
            if (compare != 0)
            {
                return compare;
            }

            return string.Compare(eventName, eventToCompare.eventName);
        }

        /// <summary>
        /// Returns a percentage based score on how similar the passed record is to the current instance.
        /// </summary>
        /// <param name="ev">The event to compare against this instance.</param>
        /// <returns>A score from 0 to 100 representing the percentage match.</returns>
        public decimal CalculateSimilarityScore(GedcomEvent ev)
        {
            var match = decimal.Zero;

            if (ev.EventType == EventType)
            {
                // match date
                var dateMatch = decimal.Zero;
                if (Date == null && ev.Date == null)
                {
                    dateMatch = 100m;
                }
                else if (Date != null && ev.Date != null)
                {
                    dateMatch = Date.CalculateSimilarityScore(ev.Date);
                }

                // match location
                var locMatch = decimal.Zero;
                if (Place == null && ev.Place == null)
                {
                    locMatch = 100m;
                }
                else if (Place != null && ev.Place != null)
                {
                    if (Place.Name == ev.Place.Name)
                    {
                        locMatch = 100m;
                    }
                }

                match = (dateMatch + locMatch) / 2m;
            }

            return match;
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

            XmlNode eventNode = doc.CreateElement("EventRec");
            attr = doc.CreateAttribute("Id");
            attr.Value = EventXRefID;
            eventNode.Attributes.Append(attr);

            attr = doc.CreateAttribute("Type");
            attr.Value = GedcomEvent.TypeToReadable(EventType);
            eventNode.Attributes.Append(attr);

            // TODO: VitalType attribute
            // (marriage | befmarriage | aftmarriage |
            // birth | befbirth | aftbirth |
            // death | befdeath | aftdeath)
            if (RecordType == GedcomRecordType.FamilyEvent)
            {
                GedcomFamilyEvent famEvent = this as GedcomFamilyEvent;
                GedcomFamilyRecord family = famEvent.FamRecord;

                // TODO: <Participant>s
                // probably not right, but always stick husband/wife in as
                // participants
                bool added = false;

                if (!string.IsNullOrEmpty(family.Husband))
                {
                    GedcomIndividualRecord husb = Database[family.Husband] as GedcomIndividualRecord;
                    if (husb != null)
                    {
                        node = doc.CreateElement("Participant");

                        XmlNode linkNode = doc.CreateElement("Link");

                        attr = doc.CreateAttribute("Target");
                        attr.Value = "IndividualRec";
                        linkNode.Attributes.Append(attr);

                        attr = doc.CreateAttribute("Ref");
                        attr.Value = family.Husband;
                        linkNode.Attributes.Append(attr);

                        node.AppendChild(linkNode);

                        eventNode.AppendChild(node);
                        added = true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Pointer to non existant husband");
                    }
                }

                if (!string.IsNullOrEmpty(family.Wife))
                {
                    GedcomIndividualRecord wife = Database[family.Wife] as GedcomIndividualRecord;
                    if (wife != null)
                    {
                        node = doc.CreateElement("Participant");

                        XmlNode linkNode = doc.CreateElement("Link");

                        attr = doc.CreateAttribute("Target");
                        attr.Value = "IndividualRec";
                        linkNode.Attributes.Append(attr);

                        attr = doc.CreateAttribute("Ref");
                        attr.Value = family.Wife;
                        linkNode.Attributes.Append(attr);

                        node.AppendChild(linkNode);

                        eventNode.AppendChild(node);
                        added = true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Pointer to non existant wife");
                    }
                }

                if (!added)
                {
                    // TODO: no husband or wife now what?  XML will be invalid
                    // without a participant
                }
            }
            else
            {
                GedcomIndividualRecord indi = (this as GedcomIndividualEvent).IndiRecord;

                node = doc.CreateElement("Participant");

                XmlNode linkNode = doc.CreateElement("Link");

                attr = doc.CreateAttribute("Target");
                attr.Value = "IndividualRec";
                linkNode.Attributes.Append(attr);

                attr = doc.CreateAttribute("Ref");
                attr.Value = indi.XRefID;
                linkNode.Attributes.Append(attr);

                XmlNode roleNode = doc.CreateElement("Role");
                if (this == indi.Birth)
                {
                    roleNode.AppendChild(doc.CreateTextNode("child"));
                }
                else
                {
                    roleNode.AppendChild(doc.CreateTextNode("principle"));
                }

                linkNode.AppendChild(roleNode);

                node.AppendChild(linkNode);

                eventNode.AppendChild(node);
            }

            if (Date != null)
            {
                node = doc.CreateElement("Date");
                node.AppendChild(doc.CreateTextNode(Date.DateString));
                eventNode.AppendChild(node);
            }

            if (Place != null)
            {
                node = doc.CreateElement("Place");
                node.AppendChild(doc.CreateTextNode(Place.Name));
                eventNode.AppendChild(node);
            }

            GenerateNoteXML(eventNode);
            GenerateCitationsXML(eventNode);
            GenerateMultimediaXML(eventNode);

            GenerateChangeDateXML(eventNode);

            root.AppendChild(eventNode);
        }

        /// <summary>
        /// Outputs the specified sw.
        /// </summary>
        /// <param name="sw">The sw.</param>
        public override void Output(TextWriter sw)
        {
            sw.Write(Environment.NewLine);
            sw.Write(Level.ToString());
            sw.Write(" ");

            sw.Write(GedcomTag);

            if (!string.IsNullOrEmpty(eventName))
            {
                sw.Write(" ");
                sw.Write(eventName);
            }

            OutputStandard(sw);

            string levelPlusOne = null;

            if (!string.IsNullOrEmpty(classification))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (Level + 1).ToString();
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" TYPE ");
                sw.Write(classification);
            }

            if (date != null)
            {
                date.Output(sw);
            }

            if (place != null)
            {
                place.Output(sw);
            }

            if (address != null)
            {
                address.Output(sw, Level + 1);
            }

            if (!string.IsNullOrEmpty(responsibleAgency))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (Level + 1).ToString();
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" AGNC ");
                string line = responsibleAgency.Replace("@", "@@");
                sw.Write(line);
            }

            if (!string.IsNullOrEmpty(religiousAffiliation))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (Level + 1).ToString();
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" RELI ");
                string line = religiousAffiliation.Replace("@", "@@");
                sw.Write(line);
            }

            if (!string.IsNullOrEmpty(cause))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (Level + 1).ToString();
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" CAUS ");
                string line = cause.Replace("@", "@@");
                sw.Write(line);
            }

            if (RestrictionNotice != GedcomRestrictionNotice.None)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (Level + 1).ToString();
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" RESN ");
                sw.Write(RestrictionNotice.ToString());
            }

            // Quality of data should only be on source citations according to
            // the spec.
            // We output it on events as well as it has been seen in GEDCOM
            // files from other apps
            if (Certainty != GedcomCertainty.Unknown)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (Level + 1).ToString();
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" QUAY ");
                sw.Write(((int)Certainty).ToString());
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
            var eventRecord = obj as GedcomEvent;

            if (eventRecord == null)
            {
                return false;
            }

            if (!GedcomGenericComparer.SafeEqualityCheck(Address, eventRecord.Address))
            {
                return false;
            }

            if (!Equals(Cause, eventRecord.Cause))
            {
                return false;
            }

            if (!Equals(Certainty, eventRecord.Certainty))
            {
                return false;
            }

            if (!Equals(Classification, eventRecord.Classification))
            {
                return false;
            }

            if (!Equals(Date, eventRecord.Date))
            {
                return false;
            }

            if (!Equals(EventName, eventRecord.EventName))
            {
                return false;
            }

            if (!Equals(EventType, eventRecord.EventType))
            {
                return false;
            }

            if (!Equals(Place, eventRecord.Place))
            {
                return false;
            }

            if (!Equals(ReligiousAffiliation, eventRecord.ReligiousAffiliation))
            {
                return false;
            }

            if (!Equals(ResponsibleAgency, eventRecord.ResponsibleAgency))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Compare the user entered data against the passed instance for similarity.
        /// </summary>
        /// <param name="other">The GedcomEvent to compare this instance against.</param>
        /// <returns>
        /// True if instance matches user data, otherwise false.
        /// </returns>
        public bool Equals(GedcomEvent other)
        {
            return IsEquivalentTo(other);
        }
    }
}
