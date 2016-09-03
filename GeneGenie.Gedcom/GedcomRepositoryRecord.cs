/*
 *  $Id: GedcomRepositoryRecord.cs 200 2008-11-30 14:34:07Z davek $
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

    public class GedcomRepositoryRecord : GedcomRecord, IComparable
    {
        private string _Name;
        private GedcomAddress _Address;

        private GedcomRecordList<GedcomRepositoryCitation> _Citations;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomRepositoryRecord"/> class.
        /// </summary>
        public GedcomRepositoryRecord()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomRepositoryRecord"/> class.
        /// </summary>
        /// <param name="database"></param>
        public GedcomRepositoryRecord(GedcomDatabase database)
            : this()
        {
            Database = database;
            Level = 0;

            Name = "New Repository";

            XRefID = database.GenerateXref("REPO");
            database.Add(XRefID, this);
        }

        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Repository; }
        }

        public override string GedcomTag
        {
            get { return "REPO"; }
        }

        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                if (value != _Name)
                {
                    _Name = value;
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

        public GedcomRecordList<GedcomRepositoryCitation> Citations
        {
            get
            {
                if (_Citations == null)
                {
                    _Citations = new GedcomRecordList<GedcomRepositoryCitation>();
                    _Citations.Changed += ListChanged;
                }

                return _Citations;
            }
        }

        public override GedcomChangeDate ChangeDate
        {
            get
            {
                GedcomChangeDate realChangeDate = base.ChangeDate;
                GedcomChangeDate childChangeDate;
                foreach (GedcomRepositoryCitation citation in Citations)
                {
                    childChangeDate = citation.ChangeDate;
                    if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                    {
                        realChangeDate = childChangeDate;
                    }
                }

                if (Address != null)
                {
                    childChangeDate = Address.ChangeDate;
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

        public int CompareTo(object repoB)
        {
            return GedcomRepositoryRecord.CompareByName(this, (GedcomRepositoryRecord)repoB);
        }

        public static int CompareByName(GedcomRepositoryRecord repoA, GedcomRepositoryRecord repoB)
        {
            return string.Compare(repoA.Name, repoB.Name);
        }

        public override void GenerateXML(XmlNode root)
        {
            XmlDocument doc = root.OwnerDocument;

            XmlNode node = doc.CreateElement("Repository");
            XmlAttribute attr;

            attr = doc.CreateAttribute("Id");
            attr.Value = XRefID;

            node.Attributes.Append(attr);

            // FIXME:  Type attribute comes from where?
            if (!string.IsNullOrEmpty(_Name))
            {
                XmlNode name = doc.CreateElement("Name");
                name.AppendChild(doc.CreateTextNode(_Name));
            }

            if (_Address != null)
            {
                _Address.GenerateXML(node);
            }

            GenerateNoteXML(node);
            GenerateChangeDateXML(node);

            root.AppendChild(node);
        }

        public override void Output(TextWriter sw)
        {
            base.Output(sw);

            if (!string.IsNullOrEmpty(Name))
            {
                sw.Write(Environment.NewLine);
                sw.Write(Util.IntToString(Level + 1));
                sw.Write(" NAME ");
                string line = Name.Replace("@", "@@");
                sw.Write(line);
            }

            if (Address != null)
            {
                Address.Output(sw, Level + 1);
            }
        }
    }
}
