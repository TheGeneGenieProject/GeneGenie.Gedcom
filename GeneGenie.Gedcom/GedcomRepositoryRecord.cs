// <copyright file="GedcomRepositoryRecord.cs" company="GeneGenie.com">
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
    using System.IO;
    using System.Xml;
    using Enums;

    /// <summary>
    /// TODO: Doc
    /// </summary>
    /// <seealso cref="GedcomRecord" />
    /// <seealso cref="System.IComparable" />
    public class GedcomRepositoryRecord : GedcomRecord, IComparable
    {
        private string name;
        private GedcomAddress address;

        private GedcomRecordList<GedcomRepositoryCitation> citations;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomRepositoryRecord"/> class.
        /// </summary>
        public GedcomRepositoryRecord()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomRepositoryRecord"/> class.
        /// </summary>
        /// <param name="database">The database to associate with this record.</param>
        public GedcomRepositoryRecord(GedcomDatabase database)
            : this()
        {
            Database = database;
            Level = 0;

            Name = "New Repository";

            XRefID = database.GenerateXref("REPO");
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
            get { return GedcomRecordType.Repository; }
        }

        /// <summary>
        /// Gets the gedcom tag.
        /// </summary>
        /// <value>
        /// The gedcom tag.
        /// </value>
        public override string GedcomTag
        {
            get { return "REPO"; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (value != name)
                {
                    name = value;
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
        /// Gets the citations.
        /// </summary>
        /// <value>
        /// The citations.
        /// </value>
        public GedcomRecordList<GedcomRepositoryCitation> Citations
        {
            get
            {
                if (citations == null)
                {
                    citations = new GedcomRecordList<GedcomRepositoryCitation>();
                    citations.Changed += ListChanged;
                }

                return citations;
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

        /// <summary>
        /// Compares the names of the passed records.
        /// </summary>
        /// <param name="repoA">The repo a.</param>
        /// <param name="repoB">The repo b.</param>
        /// <returns>TODO: Doc</returns>
        public static int CompareByName(GedcomRepositoryRecord repoA, GedcomRepositoryRecord repoB)
        {
            return string.Compare(repoA.Name, repoB.Name);
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="repoB">The repo b.</param>
        /// <returns>TODO: Doc</returns>
        public int CompareTo(object repoB)
        {
            return GedcomRepositoryRecord.CompareByName(this, (GedcomRepositoryRecord)repoB);
        }

        /// <summary>
        /// Generates the XML.
        /// </summary>
        /// <param name="root">The root.</param>
        public override void GenerateXML(XmlNode root)
        {
            XmlDocument doc = root.OwnerDocument;

            XmlNode node = doc.CreateElement("Repository");
            XmlAttribute attr;

            attr = doc.CreateAttribute("Id");
            attr.Value = XRefID;

            node.Attributes.Append(attr);

            // TODO:  Type attribute comes from where?
            if (!string.IsNullOrEmpty(Name))
            {
                XmlNode name = doc.CreateElement("Name");
                name.AppendChild(doc.CreateTextNode(Name));
            }

            if (Address != null)
            {
                Address.GenerateXML(node);
            }

            GenerateNoteXML(node);
            GenerateChangeDateXML(node);

            root.AppendChild(node);
        }

        /// <summary>
        /// Outputs the specified sw.
        /// </summary>
        /// <param name="sw">The sw.</param>
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

        /// <summary>
        /// Compare the user entered data against the passed instance for similarity.
        /// </summary>
        /// <param name="obj">The object to compare this instance against.</param>
        /// <returns>
        /// True if instance matches user data, otherwise false.
        /// </returns>
        public override bool IsSimilar(object obj)
        {
            var repository = obj as GedcomRepositoryRecord;

            if (repository == null)
            {
                return false;
            }

            if (!Equals(Address, repository.Address))
            {
                return false;
            }

            if (!Equals(ChangeDate, repository.ChangeDate))
            {
                return false;
            }

            if (!Equals(Citations, repository.Citations))
            {
                return false;
            }

            if (!Equals(Name, repository.Name))
            {
                return false;
            }

            return true;
        }
    }
}
