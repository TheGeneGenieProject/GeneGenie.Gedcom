// <copyright file="GedcomFamilyRecord.cs" company="GeneGenie.com">
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
    /// Defines a family, consisting of husband/wife and children, and
    /// family events.
    /// </summary>
    public class GedcomFamilyRecord : GedcomRecord
    {
        private GedcomRecordList<GedcomFamilyEvent> events;

        private string husband;
        private string wife;

        private GedcomRecordList<string> children;

        // not just _Children.Count, may be unknown children
        private int numberOfChildren;

        private GedcomRecordList<string> submitterRecords;

        // TODO
        private object lDSSpouseSealings;

        private MarriageStartStatus startStatus;

        // only used during parsing
        private Dictionary<string, Gedcom.PedegreeLinkageType> linkageTypes;
        private Dictionary<string, Gedcom.PedegreeLinkageType> husbLinkageTypes;
        private Dictionary<string, Gedcom.PedegreeLinkageType> wifeLinkageTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomFamilyRecord"/> class.
        /// </summary>
        public GedcomFamilyRecord()
        {
            events = new GedcomRecordList<GedcomFamilyEvent>();
            events.Changed += ListChanged;
            children = new GedcomRecordList<string>();
            children.Changed += ListChanged;

            startStatus = MarriageStartStatus.Unknown;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomFamilyRecord" /> class.
        /// </summary>
        /// <param name="database">The database to associate with this record.</param>
        /// <param name="indi1">The indi1.</param>
        /// <param name="indi2">The indi2.</param>
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

        /// <summary>
        /// Gets the type of the record.
        /// </summary>
        /// <value>
        /// The type of the record.
        /// </value>
        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Family; }
        }

        /// <summary>
        /// Gets the gedcom tag.
        /// </summary>
        /// <value>
        /// The gedcom tag.
        /// </value>
        public override string GedcomTag
        {
            get { return "FAM"; }
        }

        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        public GedcomRecordList<GedcomFamilyEvent> Events
        {
            get { return events; }
        }

        /// <summary>
        /// Gets or sets the husband.
        /// </summary>
        /// <value>
        /// The husband.
        /// </value>
        public string Husband
        {
            get
            {
                return husband;
            }

            set
            {
                if (value != husband)
                {
                    husband = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the wife.
        /// </summary>
        /// <value>
        /// The wife.
        /// </value>
        public string Wife
        {
            get
            {
                return wife;
            }

            set
            {
                if (value != wife)
                {
                    wife = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public GedcomRecordList<string> Children
        {
            get { return children; }
        }

        /// <summary>
        /// Gets or sets the number of children.
        /// </summary>
        /// <value>
        /// The number of children.
        /// </value>
        public int NumberOfChildren
        {
            get
            {
                return numberOfChildren;
            }

            set
            {
                if (value != numberOfChildren)
                {
                    numberOfChildren = value;
                    Changed();
                }
            }
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

        // Util properties to get marriage event

        /// <summary>
        /// Gets the marriage.
        /// </summary>
        /// <value>
        /// The marriage.
        /// </value>
        public GedcomFamilyEvent Marriage
        {
            get
            {
                GedcomFamilyEvent marriage = null;

                foreach (GedcomFamilyEvent e in events)
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

        /// <summary>
        /// Gets or sets the start status.
        /// </summary>
        /// <value>
        /// The start status.
        /// </value>
        public MarriageStartStatus StartStatus
        {
            get
            {
                return startStatus;
            }

            set
            {
                if (value != startStatus)
                {
                    startStatus = value;
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
                    record = Database[submitterID];
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

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public override void Delete()
        {
            base.Delete();

            if (events != null && RefCount == 0)
            {
                foreach (GedcomEvent ev in events)
                {
                    ev.Delete();
                }
            }
        }

        /// <summary>
        /// Adds the new event.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>TODO: Doc</returns>
        public GedcomFamilyEvent AddNewEvent(GedcomEventType type)
        {
            GedcomFamilyEvent familyEvent = new GedcomFamilyEvent();
            familyEvent.EventType = type;
            familyEvent.Level = Level + 1;
            familyEvent.FamRecord = this;

            Events.Add(familyEvent);

            return familyEvent;
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="indi">The indi.</param>
        /// <returns>TODO: Doc</returns>
        public bool AddChild(GedcomIndividualRecord indi)
        {
            bool added = false;

            if (indi != null && !Children.Contains(indi.XRefID))
            {
                if (string.IsNullOrEmpty(XRefID))
                {
                    XRefID = Database.GenerateXref("FAM");
                    Database.Add(XRefID, this);
                }

                if (!indi.ChildInFamily(XRefID))
                {
                    GedcomFamilyLink link = new GedcomFamilyLink();
                    link.Database = Database;
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

        /// <summary>
        /// Adds the new child.
        /// </summary>
        /// <returns>TODO: Doc</returns>
        public GedcomIndividualRecord AddNewChild()
        {
            GedcomIndividualRecord husband = null;
            GedcomIndividualRecord wife = null;

            if (!string.IsNullOrEmpty(this.husband))
            {
                husband = Database[this.husband] as GedcomIndividualRecord;
            }

            if (!string.IsNullOrEmpty(this.wife))
            {
                wife = Database[this.wife] as GedcomIndividualRecord;
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

            GedcomIndividualRecord indi = new GedcomIndividualRecord(Database, surname);

            // don't care about failure here, won't happen as indi isn't null
            // and they aren't already in the family
            AddChild(indi);

            return indi;
        }

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="child">The child.</param>
        public void RemoveChild(GedcomIndividualRecord child)
        {
            Children.Remove(child.XRefID);

            GedcomFamilyLink link;
            if (child.ChildInFamily(XRefID, out link))
            {
                child.ChildIn.Remove(link);
            }
        }

        /// <summary>
        /// Changes the husband.
        /// </summary>
        /// <param name="indi">The indi.</param>
        public void ChangeHusband(GedcomIndividualRecord indi)
        {
            GedcomIndividualRecord husband = null;
            GedcomIndividualRecord wife = null;

            if (!string.IsNullOrEmpty(this.husband))
            {
                husband = Database[this.husband] as GedcomIndividualRecord;
            }

            if (!string.IsNullOrEmpty(this.wife))
            {
                wife = Database[this.wife] as GedcomIndividualRecord;
            }

            if (string.IsNullOrEmpty(XRefID))
            {
                XRefID = Database.GenerateXref("FAM");
                Database.Add(XRefID, this);
            }

            if (husband != null)
            {
                GedcomFamilyLink link;
                if (husband.SpouseInFamily(XRefID, out link))
                {
                    husband.SpouseIn.Remove(link);
                }
            }

            husband = indi;
            this.husband = string.Empty;

            if (husband != null)
            {
                this.husband = husband.XRefID;

                if (!husband.SpouseInFamily(XRefID))
                {
                    GedcomFamilyLink link = new GedcomFamilyLink();
                    link.Database = Database;
                    link.Family = XRefID;
                    link.Indi = this.husband;
                    husband.SpouseIn.Add(link);
                }
            }

            if (wife != null)
            {
                this.wife = wife.XRefID;

                if (!wife.SpouseInFamily(XRefID))
                {
                    GedcomFamilyLink link = new GedcomFamilyLink();
                    link.Database = Database;
                    link.Family = XRefID;
                    link.Indi = this.wife;
                    wife.SpouseIn.Add(link);
                }
            }
        }

        /// <summary>
        /// Changes the wife.
        /// </summary>
        /// <param name="indi">The indi.</param>
        public void ChangeWife(GedcomIndividualRecord indi)
        {
            GedcomIndividualRecord husband = null;
            GedcomIndividualRecord wife = null;

            if (!string.IsNullOrEmpty(this.husband))
            {
                husband = Database[this.husband] as GedcomIndividualRecord;
            }

            if (!string.IsNullOrEmpty(this.wife))
            {
                wife = Database[this.wife] as GedcomIndividualRecord;
            }

            if (string.IsNullOrEmpty(XRefID))
            {
                XRefID = Database.GenerateXref("FAM");
                Database.Add(XRefID, this);
            }

            if (wife != null)
            {
                GedcomFamilyLink link;
                if (wife.SpouseInFamily(XRefID, out link))
                {
                    wife.SpouseIn.Remove(link);
                }
            }

            wife = indi;
            this.wife = string.Empty;

            if (husband != null)
            {
                this.husband = husband.XRefID;

                if (!husband.SpouseInFamily(XRefID))
                {
                    GedcomFamilyLink link = new GedcomFamilyLink();
                    link.Database = Database;
                    link.Family = XRefID;
                    link.Indi = this.husband;
                    husband.SpouseIn.Add(link);
                }
            }

            if (wife != null)
            {
                this.wife = wife.XRefID;

                if (!wife.SpouseInFamily(XRefID))
                {
                    GedcomFamilyLink link = new GedcomFamilyLink();
                    link.Database = Database;
                    link.Family = XRefID;
                    link.Indi = this.wife;
                    wife.SpouseIn.Add(link);
                }
            }
        }

        /// <summary>
        /// Removes the husband.
        /// </summary>
        /// <param name="indi">The indi.</param>
        public void RemoveHusband(GedcomIndividualRecord indi)
        {
            GedcomFamilyLink link;

            if (husband == indi.XRefID)
            {
                husband = string.Empty;
            }

            if (indi.SpouseInFamily(XRefID, out link))
            {
                indi.SpouseIn.Remove(link);
            }
        }

        /// <summary>
        /// Removes the wife.
        /// </summary>
        /// <param name="indi">The indi.</param>
        public void RemoveWife(GedcomIndividualRecord indi)
        {
            GedcomFamilyLink link;

            if (wife == indi.XRefID)
            {
                wife = string.Empty;
            }

            if (indi.SpouseInFamily(XRefID, out link))
            {
                indi.SpouseIn.Remove(link);
            }
        }

        /// <summary>
        /// Clears the linkage types.
        /// </summary>
        public void ClearLinkageTypes()
        {
            if (linkageTypes != null)
            {
                linkageTypes.Clear();
                linkageTypes = null;
            }

            if (husbLinkageTypes != null)
            {
                husbLinkageTypes.Clear();
                husbLinkageTypes = null;
            }

            if (wifeLinkageTypes != null)
            {
                wifeLinkageTypes.Clear();
                wifeLinkageTypes = null;
            }
        }

        /// <summary>
        /// Sets the type of the linkage.
        /// </summary>
        /// <param name="childXrefID">The child xref identifier.</param>
        /// <param name="type">The type.</param>
        public void SetLinkageType(string childXrefID, Gedcom.PedegreeLinkageType type)
        {
            SetLinkageType(childXrefID, type, Gedcom.GedcomAdoptionType.HusbandAndWife);
        }

        /// <summary>
        /// Sets the type of the linkage.
        /// </summary>
        /// <param name="childXrefID">The child xref identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="to">To.</param>
        public void SetLinkageType(string childXrefID, Gedcom.PedegreeLinkageType type, Gedcom.GedcomAdoptionType to)
        {
            Dictionary<string, PedegreeLinkageType> dict;

            switch (to)
            {
                case Gedcom.GedcomAdoptionType.Husband:
                    if (husbLinkageTypes == null)
                    {
                        husbLinkageTypes = new Dictionary<string, PedegreeLinkageType>();
                    }

                    dict = husbLinkageTypes;
                    break;
                case Gedcom.GedcomAdoptionType.Wife:
                    if (wifeLinkageTypes == null)
                    {
                        wifeLinkageTypes = new Dictionary<string, PedegreeLinkageType>();
                    }

                    dict = wifeLinkageTypes;
                    break;
                case Gedcom.GedcomAdoptionType.HusbandAndWife:
                default:
                    if (linkageTypes == null)
                    {
                        linkageTypes = new Dictionary<string, PedegreeLinkageType>();
                    }

                    dict = linkageTypes;
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

        /// <summary>
        /// Gets the type of the husband linkage.
        /// </summary>
        /// <param name="childXrefID">The child xref identifier.</param>
        /// <returns>TODO: Doc</returns>
        public Gedcom.PedegreeLinkageType GetHusbandLinkageType(string childXrefID)
        {
            Gedcom.PedegreeLinkageType ret = Gedcom.PedegreeLinkageType.Unknown;

            if (husbLinkageTypes != null && husbLinkageTypes.ContainsKey(childXrefID))
            {
                ret = husbLinkageTypes[childXrefID];
            }
            else
            {
                GedcomIndividualRecord child = (GedcomIndividualRecord)Database[childXrefID];
                if (child != null)
                {
                    GedcomFamilyLink link = null;
                    if (child.ChildInFamily(XrefId, out link))
                    {
                        ret = link.FatherPedigree;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Child " + childXrefID + " is not in family " +
                                                           XrefId + " in GetLinkageType");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Attempt to GetLinkageType of unknown child " +
                                                       childXrefID + " in " + XrefId);
                }
            }

            return ret;
        }

        /// <summary>
        /// Gets the type of the wife linkage.
        /// </summary>
        /// <param name="childXrefID">The child xref identifier.</param>
        /// <returns>TODO: Doc</returns>
        public Gedcom.PedegreeLinkageType GetWifeLinkageType(string childXrefID)
        {
            Gedcom.PedegreeLinkageType ret = Gedcom.PedegreeLinkageType.Unknown;

            if (wifeLinkageTypes != null && wifeLinkageTypes.ContainsKey(childXrefID))
            {
                ret = wifeLinkageTypes[childXrefID];
            }
            else
            {
                GedcomIndividualRecord child = (GedcomIndividualRecord)Database[childXrefID];
                if (child != null)
                {
                    GedcomFamilyLink link = null;
                    if (child.ChildInFamily(XrefId, out link))
                    {
                        ret = link.MotherPedigree;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Child " + childXrefID + " is not in family " +
                                                           XrefId + " in GetLinkageType");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Attempt to GetLinkageType of unknown child " +
                                                       childXrefID + " in " + XrefId);
                }
            }

            return ret;
        }

        /// <summary>
        /// Gets the type of the linkage.
        /// </summary>
        /// <param name="childXrefID">The child xref identifier.</param>
        /// <returns>TODO: Doc</returns>
        public Gedcom.PedegreeLinkageType GetLinkageType(string childXrefID)
        {
            Gedcom.PedegreeLinkageType ret = Gedcom.PedegreeLinkageType.Unknown;

            if (linkageTypes != null && linkageTypes.ContainsKey(childXrefID))
            {
                ret = linkageTypes[childXrefID];
            }
            else
            {
                GedcomIndividualRecord child = (GedcomIndividualRecord)Database[childXrefID];
                if (child != null)
                {
                    GedcomFamilyLink link = null;
                    if (child.ChildInFamily(XrefId, out link))
                    {
                        ret = link.Pedigree;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Child " + childXrefID + " is not in family " +
                                                           XrefId + " in GetLinkageType");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Attempt to GetLinkageType of unknown child " +
                                                       childXrefID + " in " + XrefId);
                }
            }

            return ret;
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

            XmlNode famNode = doc.CreateElement("FamilyRec");
            attr = doc.CreateAttribute("Id");
            attr.Value = XRefID;
            famNode.Attributes.Append(attr);

            if (!string.IsNullOrEmpty(Husband))
            {
                GedcomIndividualRecord husb = Database[Husband] as GedcomIndividualRecord;
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
                GedcomIndividualRecord wife = Database[Wife] as GedcomIndividualRecord;
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
                GedcomIndividualRecord indi = Database[child] as GedcomIndividualRecord;
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

                    // TODO: add in <ChildNbr>
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

        /// <summary>
        /// Outputs the specified sw.
        /// </summary>
        /// <param name="sw">The sw.</param>
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

            foreach (GedcomFamilyEvent familyEvent in events)
            {
                familyEvent.Output(sw);
            }

            if (!string.IsNullOrEmpty(husband))
            {
                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" HUSB ");
                sw.Write("@");
                sw.Write(husband);
                sw.Write("@");
            }

            if (!string.IsNullOrEmpty(wife))
            {
                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" WIFE ");
                sw.Write("@");
                sw.Write(wife);
                sw.Write("@");
            }

            string levelPlusTwo = Util.IntToString(Level + 2);
            foreach (string childID in children)
            {
                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" CHIL ");
                sw.Write("@");
                sw.Write(childID);
                sw.Write("@");

                GedcomIndividualRecord child = (GedcomIndividualRecord)Database[childID];
                if (child != null)
                {
                    // only output _FREL / _MREL value here,
                    // real PEDI goes on the FAMC on the INDI tag
                    GedcomFamilyLink link = null;
                    if (child.ChildInFamily(XrefId, out link))
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
                        System.Diagnostics.Debug.WriteLine("Missing child linkage for " + childID + " to family " + XrefId);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Missing child " + childID + " when outputting family " + XrefId);
                }
            }

            if (numberOfChildren != 0)
            {
                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" NCHI ");
                sw.Write("@");
                sw.Write(Util.IntToString(numberOfChildren));
                sw.Write("@");
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