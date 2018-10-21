// <copyright file="GedcomIndividualEvent.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

#define XML_NODE_UNDEFINED

namespace GeneGenie.Gedcom
{
#if !XML_NODE_UNDEFINED
    using System.Xml;
#endif

    using System;
    using System.IO;
    using GeneGenie.Gedcom.Enums;

    /// <summary>
    /// An event relating to a given individual
    /// </summary>
    public class GedcomIndividualEvent : GedcomEvent
    {
        private GedcomAge age;
        private string famc;

        private GedcomAdoptionType adoptedBy;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomIndividualEvent"/> class.
        /// </summary>
        public GedcomIndividualEvent()
        {
        }

        /// <summary>
        /// Gets the type of the record.
        /// </summary>
        /// <value>
        /// The type of the record.
        /// </value>
        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.IndividualEvent; }
        }

        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value>
        /// The age.
        /// </value>
        public GedcomAge Age
        {
            get
            {
                return age;
            }

            set
            {
                if (value != age)
                {
                    age = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the family in which an individual appears as a child.
        /// </summary>
        public string Famc
        {
            get
            {
                return famc;
            }

            set
            {
                if (value != famc)
                {
                    famc = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the adoption type.
        /// </summary>
        public GedcomAdoptionType AdoptedBy
        {
            get
            {
                return adoptedBy;
            }

            set
            {
                if (value != adoptedBy)
                {
                    adoptedBy = value;
                    Changed();
                }
            }
        }

        // util backpointer to the individual for this event

        /// <summary>
        /// Gets or sets the individual's record.
        /// </summary>
        /// <exception cref="Exception">Must set a GedcomIndividualRecord on a GedcomIndividualEvent</exception>
        public GedcomIndividualRecord IndiRecord
        {
            get
            {
                return (GedcomIndividualRecord)Record;
            }

            set
            {
                if (value != Record)
                {
                    Record = value;
                    if (Record != null)
                    {
                        if (Record.RecordType != GedcomRecordType.Individual)
                        {
                            throw new Exception("Must set a GedcomIndividualRecord on a GedcomIndividualEvent");
                        }

                        Database = Record.Database;
                    }
                    else
                    {
                        Database = null;
                    }

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
                if (age != null)
                {
                    childChangeDate = age.ChangeDate;
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

#if !XML_NODE_UNDEFINED
        /// <summary>
        /// Generates the pers information XML.
        /// </summary>
        /// <param name="root">The root node.</param>
        public void GeneratePersInfoXML(XmlNode root)
        {
            XmlDocument doc = root.OwnerDocument;

            XmlNode node;
            XmlAttribute attr;

            XmlNode persInfoNode = doc.CreateElement("PersInfo");
            attr = doc.CreateAttribute("Type");

            string type = string.Empty;
            if (EventType == GedcomEventType.GenericEvent ||
                EventType == GedcomEventType.GenericFact)
            {
                type = EventName;
            }
            else
            {
                type = TypeToReadable(EventType);
            }

            attr.Value = type;
            persInfoNode.Attributes.Append(attr);

            if (!string.IsNullOrEmpty(Classification))
            {
                node = doc.CreateElement("Information");
                node.AppendChild(doc.CreateTextNode(Classification));
                persInfoNode.AppendChild(node);
            }

            if (Date != null)
            {
                node = doc.CreateElement("Date");
                node.AppendChild(doc.CreateTextNode(Date.DateString));
                persInfoNode.AppendChild(node);
            }

            if (Place != null)
            {
                node = doc.CreateElement("Place");
                node.AppendChild(doc.CreateTextNode(Place.Name));
                persInfoNode.AppendChild(node);
            }

            root.AppendChild(persInfoNode);
        }
#endif

        /// <summary>
        /// Output GEDCOM format for this instance.
        /// </summary>
        /// <param name="sw">Where to output the data to.</param>
        public override void Output(TextWriter sw)
        {
            base.Output(sw);

            if (Age != null)
            {
                Age.Output(sw, Level + 1);
            }
        }
    }
}
