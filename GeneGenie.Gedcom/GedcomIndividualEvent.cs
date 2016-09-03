/*
 *  $Id: GedcomIndividualEvent.cs 200 2008-11-30 14:34:07Z davek $
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

    /// <summary>
    /// An event relating to a given individual
    /// </summary>
    public class GedcomIndividualEvent : GedcomEvent
    {
        private GedcomAge _Age;
        private string _Famc;

        private GedcomAdoptionType _AdoptedBy;

        public GedcomIndividualEvent()
        {
        }

        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.IndividualEvent; }
        }

        public GedcomAge Age
        {
            get
            {
                return _Age;
            }

            set
            {
                if (value != _Age)
                {
                    _Age = value;
                    Changed();
                }
            }
        }

        public string Famc
        {
            get
            {
                return _Famc;
            }

            set
            {
                if (value != _Famc)
                {
                    _Famc = value;
                    Changed();
                }
            }
        }

        public GedcomAdoptionType AdoptedBy
        {
            get
            {
                return _AdoptedBy;
            }

            set
            {
                if (value != _AdoptedBy)
                {
                    _AdoptedBy = value;
                    Changed();
                }
            }
        }

        // util backpointer to the individual for this event
        public GedcomIndividualRecord IndiRecord
        {
            get
            {
                return (GedcomIndividualRecord)_Record;
            }

            set
            {
                if (value != _Record)
                {
                    _Record = value;
                    if (_Record != null)
                    {
                        if (_Record.RecordType != GedcomRecordType.Individual)
                        {
                            throw new Exception("Must set a GedcomIndividualRecord on a GedcomIndividualEvent");
                        }

                        Database = _Record.Database;
                    }
                    else
                    {
                        Database = null;
                    }

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
                if (_Age != null)
                {
                    childChangeDate = _Age.ChangeDate;
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
                type = TypeToReadable(eventType);
            }

            attr.Value = type;
            persInfoNode.Attributes.Append(attr);

            if (!string.IsNullOrEmpty(_Classification))
            {
                node = doc.CreateElement("Information");
                node.AppendChild(doc.CreateTextNode(_Classification));
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
