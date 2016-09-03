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
    public class GedcomEvent : GedcomRecord,  IComparable
    {
        /// <summary>
        /// TODO: Doc
        /// </summary>
        protected GedcomEventType eventType;

        protected string eventName;

        protected string _Classification;

        protected GedcomDate _Date;

        protected GedcomPlace _Place;
        protected GedcomAddress _Address;

        protected string _ResponsibleAgency;
        protected string _ReligiousAffiliation;
        protected string _Cause;

        protected GedcomCertainty _Certainty = GedcomCertainty.Unknown;

        protected GedcomRecord _Record;

        // used for Gedcom 6 XML output
        protected string _EventXRefID;

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
            "Nationaility",
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

        public GedcomEvent()
        {
            // default event type is generic, need to set event name
            // or it will not be set if the record actually is a generic event
        }

        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Event; }
        }

        public override string GedcomTag
        {
            get { return GedcomEvent.TypeToTag(EventType); }
        }

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

        public string Classification
        {
            get
            {
                return _Classification;
            }

            set
            {
                if (value != _Classification)
                {
                    _Classification = value;
                    Changed();
                }
            }
        }

        public GedcomDate Date
        {
            get
            {
                return _Date;
            }

            set
            {
                if (value != _Date)
                {
                    _Date = value;
                    Changed();
                }
            }
        }

        public GedcomPlace Place
        {
            get
            {
                return _Place;
            }

            set
            {
                if (value != _Place)
                {
                    _Place = value;
                    Changed();
                }
            }
        }

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

        public string ResponsibleAgency
        {
            get
            {
                return _ResponsibleAgency;
            }

            set
            {
                if (value != _ResponsibleAgency)
                {
                    _ResponsibleAgency = value;
                    Changed();
                }
            }
        }

        public string ReligiousAffiliation
        {
            get
            {
                return _ReligiousAffiliation;
            }

            set
            {
                if (value != _ReligiousAffiliation)
                {
                    _ReligiousAffiliation = value;
                    Changed();
                }
            }
        }

        public string Cause
        {
            get
            {
                return _Cause;
            }

            set
            {
                if (value != _Cause)
                {
                    _Cause = value;
                    Changed();
                }
            }
        }

        public GedcomCertainty Certainty
        {
            get
            {
                return _Certainty;
            }

            set
            {
                if (value != _Certainty)
                {
                    _Certainty = value;
                    Changed();
                }
            }
        }

        public string EventXRefID
        {
            get
            {
                return _EventXRefID;
            }

            set
            {
                if (value != _EventXRefID)
                {
                    _EventXRefID = value;
                    Changed();
                }
            }
        }

        public GedcomRecord Record
        {
            get
            {
                return _Record;
            }

            set
            {
                if (value != _Record)
                {
                    _Record = value;
                    Changed();
                }
            }
        }

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
        /// 
        /// </summary>
        public override void Delete()
        {
            base.Delete();

            if (_Date != null)
            {
                _Date.Delete();
            }

            if (_Place != null)
            {
                _Place.Delete();
            }
        }

        public int CompareTo(object eventB)
        {
            return GedcomEvent.CompareByDate(this, (GedcomEvent)eventB);
        }

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

        public static string TypeToTag(GedcomEventType eventType)
        {
            return typeStrings[(int)eventType];
        }

        public static string TypeToReadable(GedcomEventType eventType)
        {
            return typeDescriptions[(int)eventType];
        }

        /// <summary>
        /// Attempts to determine a standard event type from a textual
        /// description.  Always returns GenericEvent if one can't be found
        /// even though where the string came from maybe a FACT
        /// <param name="readable" />
        /// </summary>
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

            // FIXME: VitalType attribute
            // (marriage | befmarriage | aftmarriage |
            // birth | befbirth | aftbirth |
            // death | befdeath | aftdeath)
            if (RecordType == GedcomRecordType.FamilyEvent)
            {
                GedcomFamilyEvent famEvent = this as GedcomFamilyEvent;
                GedcomFamilyRecord family = famEvent.FamRecord;

                // FIXME: <Participant>s
                // probably not right, but always stick husband/wife in as
                // participants
                bool added = false;

                if (!string.IsNullOrEmpty(family.Husband))
                {
                    GedcomIndividualRecord husb = _database[family.Husband] as GedcomIndividualRecord;
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
                    GedcomIndividualRecord wife = _database[family.Wife] as GedcomIndividualRecord;
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
                    // FIXME: no husband or wife now what?  XML will be invalid
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

            if (!string.IsNullOrEmpty(_Classification))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" TYPE ");
                sw.Write(_Classification);
            }

            if (_Date != null)
            {
                _Date.Output(sw);
            }

            if (_Place != null)
            {
                _Place.Output(sw);
            }

            if (_Address != null)
            {
                _Address.Output(sw, Level + 1);
            }

            if (!string.IsNullOrEmpty(_ResponsibleAgency))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" AGNC ");
                string line = _ResponsibleAgency.Replace("@", "@@");
                sw.Write(line);
            }

            if (!string.IsNullOrEmpty(_ReligiousAffiliation))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" RELI ");
                string line = _ReligiousAffiliation.Replace("@", "@@");
                sw.Write(line);
            }

            if (!string.IsNullOrEmpty(_Cause))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" CAUS ");
                string line = _Cause.Replace("@", "@@");
                sw.Write(line);
            }

            if (_RestrictionNotice != GedcomRestrictionNotice.None)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" RESN ");
                sw.Write(_RestrictionNotice.ToString());
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
