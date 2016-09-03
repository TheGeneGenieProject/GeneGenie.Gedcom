// <copyright file="GedcomEvent.cs" company="GeneGenie.com">
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
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Defines a generic event or fact
    /// </summary>
    public class GedcomEvent : GedcomRecord, IComparable
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
            "_UNKN"
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
            "Custom"
        };

        /// <summary>
        /// TODO: Doc
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
        /// Types to readable.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>TODO: Doc</returns>
        public static string TypeToReadable(GedcomEventType eventType)
        {
            return typeDescriptions[(int)eventType];
        }

        /// <summary>
        /// Types to tag.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>TODO: Doc</returns>
        public static string TypeToTag(GedcomEventType eventType)
        {
            return typeStrings[(int)eventType];
        }

        /// <summary>
        /// Compares the by date.
        /// </summary>
        /// <param name="eventA">The event a.</param>
        /// <param name="eventB">The event b.</param>
        /// <returns>TODO: Doc</returns>
        public static int CompareByDate(GedcomEvent eventA, GedcomEvent eventB)
        {
            int ret = -1;

            if (eventA != null && eventB != null)
            {
                GedcomDate dateA = eventA.Date;
                GedcomDate dateB = eventB.Date;

                if (dateA != null && dateB != null)
                {
                    DateTime dateTimeA;
                    DateTime dateTimeB;

                    if (DateTime.TryParse(dateA.Date1, out dateTimeA) && DateTime.TryParse(dateB.Date1, out dateTimeB))
                    {
                        ret = DateTime.Compare(dateTimeA, dateTimeB);
                    }

                    if (ret == 0)
                    {
                        ret = string.Compare(eventA.eventName, eventB.eventName);
                    }
                }
                else if (dateA != null)
                {
                    ret = 1;
                }
            }
            else if (eventA != null)
            {
                ret = 1;
            }

            return ret;
        }

        /// <summary>
        /// Attempts to determine a standard event type from a textual
        /// description.  Always returns GenericEvent if one can't be found
        /// even though where the string came from maybe a FACT
        /// </summary>
        /// <param name="readable">The type name as a string.</param>
        /// <returns>
        /// TODO: Doc
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
        /// Compares to.
        /// </summary>
        /// <param name="eventB">The event b.</param>
        /// <returns>TODO: Doc</returns>
        public int CompareTo(object eventB)
        {
            return GedcomEvent.CompareByDate(this, (GedcomEvent)eventB);
        }

        /// <summary>
        /// Determines whether the specified ev is match.
        /// </summary>
        /// <param name="ev">The ev.</param>
        /// <returns>TODO: Doc</returns>
        public float IsMatch(GedcomEvent ev)
        {
            float match = 0F;

            if (ev.EventType == EventType)
            {
                // match date
                float dateMatch = 0;
                if (Date == null && ev.Date == null)
                {
                    dateMatch = 100.0F;
                }
                else if (Date != null && ev.Date != null)
                {
                    dateMatch = Date.IsMatch(ev.Date);
                }

                // match location
                float locMatch = 0;
                if (Place == null && ev.Place == null)
                {
                    locMatch = 100.0F;
                }
                else if (Place != null && ev.Place != null)
                {
                    if (Place.Name == ev.Place.Name)
                    {
                        locMatch = 100.0F;
                    }
                }

                match = (dateMatch + locMatch) / 2.0F;
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
            sw.Write(Util.IntToString(Level));
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
                    levelPlusOne = Util.IntToString(Level + 1);
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
                    levelPlusOne = Util.IntToString(Level + 1);
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
                    levelPlusOne = Util.IntToString(Level + 1);
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
                    levelPlusOne = Util.IntToString(Level + 1);
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
                    levelPlusOne = Util.IntToString(Level + 1);
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
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" QUAY ");
                sw.Write(Util.IntToString((int)Certainty));
            }
        }
    }
}
