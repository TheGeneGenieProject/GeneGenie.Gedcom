// <copyright file="GedcomRepositoryRecord.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using GeneGenie.Gedcom.Enums;

    /// <summary>
    /// An institution or person that has the specified item as part of their collection(s).
    /// </summary>
    /// <seealso cref="GedcomRecord" />
    /// <seealso cref="System.IComparable" />
    public class GedcomRepositoryRecord : GedcomRecord, IComparable, IComparable<GedcomRepositoryRecord>, IEquatable<GedcomRepositoryRecord>
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
        /// Gets the GEDCOM tag for a repository record.
        /// </summary>
        /// <value>
        /// The GEDCOM tag.
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
                    citations.CollectionChanged += ListChanged;
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
        /// <param name="repoA">The first repository record.</param>
        /// <param name="repoB">The second repository record.</param>
        /// <returns>
        /// &lt;0 if the first record's name precedes the second in the sort order;
        /// &gt;0 if the second record's name precedes the first;
        /// 0 if the names are equal
        /// </returns>
        public static int CompareByName(GedcomRepositoryRecord repoA, GedcomRepositoryRecord repoB)
        {
            return string.Compare(repoA.Name, repoB.Name);
        }

        /// <summary>
        /// Compares this repository record to another record.
        /// </summary>
        /// <param name="repoB">A repository record.</param>
        /// <returns>
        /// &lt;0 if this record precedes the other in the sort order;
        /// &gt;0 if the other record precedes this one;
        /// 0 if the records are equal
        /// </returns>
        public int CompareTo(object repoB)
        {
            return CompareTo(repoB as GedcomRepositoryRecord);
        }

        /// <summary>
        /// Generates the XML.
        /// </summary>
        /// <param name="root">The root node.</param>
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
        /// Outputs this repository record as a GEDCOM record.
        /// </summary>
        /// <param name="sw">The writer to output to.</param>
        public override void Output(TextWriter sw)
        {
            base.Output(sw);

            if (!string.IsNullOrEmpty(Name))
            {
                sw.Write(Environment.NewLine);
                sw.Write((Level + 1).ToString());
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
        /// True if instance matches user data, otherwise False.
        /// </returns>
        public override bool IsEquivalentTo(object obj)
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

            if (!Citations.All(repository.Citations.Contains))
            {
                return false;
            }

            if (!Equals(Name, repository.Name))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Compares this repository record to another record.
        /// </summary>
        /// <param name="other">A repository record.</param>
        /// <returns>
        /// &lt;0 if this record precedes the other in the sort order;
        /// &gt;0 if the other record precedes this one;
        /// 0 if the records are equal
        /// </returns>
        public int CompareTo(GedcomRepositoryRecord other)
        {
            return CompareByName(this, other);
        }

        /// <summary>
        /// Compare the user entered data against the passed instance for similarity.
        /// </summary>
        /// <param name="other">The GedcomRepositoryRecord to compare this instance against.</param>
        /// <returns>
        /// True if instance matches user data, otherwise False.
        /// </returns>
        public bool Equals(GedcomRepositoryRecord other)
        {
            return IsEquivalentTo(other);
        }
    }
}
