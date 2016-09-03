// <copyright file="GedcomDatabase.cs" company="GeneGenie.com">
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
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// The database for all the GEDCOM records.
    /// This is currently just in memory.  To implement a "real"
    /// database you should derive from this class and override
    /// the neccesary methods / properties
    /// </summary>
    public class GedcomDatabase : IEnumerator
    {
        private List<GedcomIndividualRecord> individuals;
        private List<GedcomFamilyRecord> families;
        private List<GedcomSourceRecord> sources;
        private List<GedcomRepositoryRecord> repositories;
        private List<GedcomMultimediaRecord> media;
        private List<GedcomNoteRecord> notes;
        private List<GedcomSubmitterRecord> submitters;

        private int xrefCounter = 0;

        private string name;

        private Utility.IndexedKeyCollection nameCollection;
        private Utility.IndexedKeyCollection placeNameCollection;

        // NOTE: having a collection for date strings saves memory
        // but kills GEDCOM reading time to an extent that it isn't worth it
        private Dictionary<string, int> surnames;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomDatabase"/> class.
        /// </summary>
        public GedcomDatabase()
        {
            Table = new Hashtable();
            individuals = new List<GedcomIndividualRecord>();
            families = new List<GedcomFamilyRecord>();
            sources = new List<GedcomSourceRecord>();
            repositories = new List<GedcomRepositoryRecord>();
            media = new List<GedcomMultimediaRecord>();
            notes = new List<GedcomNoteRecord>();
            submitters = new List<GedcomSubmitterRecord>();

            nameCollection = new Utility.IndexedKeyCollection();
            placeNameCollection = new Utility.IndexedKeyCollection();

            surnames = new Dictionary<string, int>();
        }

        /// <summary>
        /// Gets or sets
        /// </summary>
        /// TODO: Docs
        public virtual GedcomHeader Header { get; set; }

        /// <summary>
        /// Gets or sets hashtable of all top level GEDCOM records, key is the XRef.
        /// Top level records are Individuals, Families, Sources, Repositories, and Media
        /// </summary>
        public virtual Hashtable Table { get; set; }

        /// <summary>
        /// Gets total number of top level GEDCOM records in the database.
        /// Top level records are Individuals, Families, Sources, Repositories, and Media
        /// </summary>
        public virtual int Count
        {
            get { return Table.Count; }
        }

        /// <summary>
        /// Gets the current GedcomRecord when enumerating the database.
        /// </summary>
        public virtual object Current
        {
            get { return Table.GetEnumerator().Current; }
        }

        /// <summary>
        /// Gets a list of all the Individuals in the database.
        /// </summary>
        public virtual List<GedcomIndividualRecord> Individuals
        {
            get { return individuals; }
        }

        /// <summary>
        /// Gets a list of all the Families in the database.
        /// </summary>
        public virtual List<GedcomFamilyRecord> Families
        {
            get { return families; }
        }

        /// <summary>
        /// Gets a list of all the sources in the database.
        /// </summary>
        public virtual List<GedcomSourceRecord> Sources
        {
            get { return sources; }
        }

        /// <summary>
        /// Gets a list of all the repositories in the database.
        /// </summary>
        public virtual List<GedcomRepositoryRecord> Repositories
        {
            get { return repositories; }
        }

        /// <summary>
        /// Gets a list of all the media items in the database.
        /// </summary>
        public virtual List<GedcomMultimediaRecord> Media
        {
            get { return media; }
        }

        /// <summary>
        /// Gets a list of all the notes in the database.
        /// </summary>
        public virtual List<GedcomNoteRecord> Notes
        {
            get { return notes; }
        }

        /// <summary>
        /// Gets a list of all the submitters in the database.
        /// </summary>
        public virtual List<GedcomSubmitterRecord> Submitters
        {
            get { return submitters; }
        }

        /// <summary>
        /// Gets or sets the name of the database, this is currently the full filename
        /// of the GEDCOM file the database was read from / saved to,
        /// but could equally be a connection string for a real backend database
        /// </summary>
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets all the names used in the database, used primarily to save
        /// memory by storing names only once
        /// </summary>
        public Utility.IndexedKeyCollection NameCollection
        {
            get { return nameCollection; }
        }

        /// <summary>
        /// Gets all the place names used in the database, used primarily to save
        /// memory by storing names only once
        /// </summary>
        public Utility.IndexedKeyCollection PlaceNameCollection
        {
            get { return placeNameCollection; }
        }

        /// <summary>
        /// Gets or sets utility property providing all the surnames in the database, along with
        /// a count of how many people have that surname.
        /// </summary>
        public virtual Dictionary<string, int> Surnames
        {
            get { return surnames; }
            set { surnames = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the database is being loaded.
        /// </summary>
        public bool Loading { get; set; }

        /// <summary>
        /// Get / Set the GedcomRecord associated with the given XRef.
        /// </summary>
        /// <param name="key">TODO: Doc</param>
        /// TODO: DOC
        public virtual GedcomRecord this[string key]
        {
            get
            {
                return Table[key] as GedcomRecord;
            }

            set
            {
                Remove(key, value);
                Add(key, value);
            }
        }

        /// <summary>
        /// Add the given record to the database with the given XRef
        /// </summary>
        /// <param name="xrefID">
        /// A <see cref="string"/>
        /// </param>
        /// <param name="record">
        /// A <see cref="GedcomRecord"/>
        /// </param>
        public virtual void Add(string xrefID, GedcomRecord record)
        {
            Table.Add(xrefID, record);

            if (record is GedcomIndividualRecord)
            {
                GedcomIndividualRecord indi = (GedcomIndividualRecord)record;

                int pos = individuals.BinarySearch(indi);
                if (pos < 0)
                {
                    pos = ~pos;
                }

                individuals.Insert(pos, indi);
            }
            else if (record is GedcomFamilyRecord)
            {
                families.Add((GedcomFamilyRecord)record);
            }
            else if (record is GedcomSourceRecord)
            {
                GedcomSourceRecord source = (GedcomSourceRecord)record;

                int pos = sources.BinarySearch(source);
                if (pos < 0)
                {
                    pos = ~pos;
                }

                sources.Insert(pos, source);
            }
            else if (record is GedcomRepositoryRecord)
            {
                GedcomRepositoryRecord repo = (GedcomRepositoryRecord)record;

                int pos = repositories.BinarySearch(repo);
                if (pos < 0)
                {
                    pos = ~pos;
                }

                repositories.Insert(pos, repo);
            }
            else if (record is GedcomMultimediaRecord)
            {
                media.Add((GedcomMultimediaRecord)record);
            }
            else if (record is GedcomNoteRecord)
            {
                notes.Add((GedcomNoteRecord)record);
            }
            else if (record is GedcomSubmitterRecord)
            {
                submitters.Add((GedcomSubmitterRecord)record);
            }

            record.Database = this;
        }

        /// <summary>
        /// Builds up the surname list for use with the Surnames property.
        /// </summary>
        public virtual void BuildSurnameList()
        {
            foreach (GedcomIndividualRecord indi in individuals)
            {
                BuildSurnameList(indi);
            }
        }

        /// <summary>
        /// Remove the given record with the given XRef from the database
        /// </summary>
        /// <param name="xrefID">
        /// A <see cref="string"/>
        /// </param>
        /// <param name="record">
        /// A <see cref="GedcomRecord"/>
        /// </param>
        public virtual void Remove(string xrefID, GedcomRecord record)
        {
            if (Table.Contains(xrefID))
            {
                Table.Remove(xrefID);

                if (record is GedcomIndividualRecord)
                {
                    GedcomIndividualRecord indi = (GedcomIndividualRecord)record;

                    individuals.Remove(indi);

                    // remove names from surname cache
                    foreach (GedcomName name in indi.Names)
                    {
                        // TODO: not right, need to include prefix + suffix
                        string surname = name.Surname;

                        if (surnames.ContainsKey(surname))
                        {
                            int count = (int)surnames[surname];
                            count--;
                            if (count > 0)
                            {
                                surnames[surname] = count;
                            }
                            else
                            {
                                surnames.Remove(surname);
                            }
                        }
                    }
                }
                else if (record is GedcomFamilyRecord)
                {
                    families.Remove((GedcomFamilyRecord)record);
                }
                else if (record is GedcomSourceRecord)
                {
                    sources.Remove((GedcomSourceRecord)record);
                }
                else if (record is GedcomRepositoryRecord)
                {
                    repositories.Remove((GedcomRepositoryRecord)record);
                }
                else if (record is GedcomMultimediaRecord)
                {
                    media.Remove((GedcomMultimediaRecord)record);
                }
                else if (record is GedcomNoteRecord)
                {
                    notes.Remove((GedcomNoteRecord)record);
                }
                else if (record is GedcomSubmitterRecord)
                {
                    submitters.Remove((GedcomSubmitterRecord)record);
                }

                // TODO: should we set this to null? part of the deletion
                // methods may still want to access the database
                // record.Database = null;
            }
        }

        /// <summary>
        /// Does the database contain a record with the given XRef
        /// </summary>
        /// <param name="xrefID">
        /// A <see cref="string"/>
        /// </param>
        /// <returns>
        /// A <see cref="bool"/>
        /// </returns>
        public virtual bool Contains(string xrefID)
        {
            return Table.Contains(xrefID);
        }

        /// <summary>
        /// TODO: Doc
        /// </summary>
        /// <returns>TODO: Doc 2</returns>
        public virtual bool MoveNext()
        {
            return Table.GetEnumerator().MoveNext();
        }

        /// <inheritdoc/>
        public virtual void Reset()
        {
            Table.GetEnumerator().Reset();
        }

        /// <summary>
        /// TODO: Doc
        /// </summary>
        /// <returns>TODO: Doc 2</returns>
        public virtual IDictionaryEnumerator GetEnumerator()
        {
            return Table.GetEnumerator();
        }

        /// <summary>
        /// Create a new XRef
        /// </summary>
        /// <param name="prefix">
        /// A <see cref="string"/>
        /// </param>
        /// <returns>
        /// A <see cref="string"/> TODO: Doc
        /// </returns>
        public string GenerateXref(string prefix)
        {
            return string.Format("{0}{1}", prefix, Util.IntToString(++xrefCounter));
        }

        /// <summary>
        /// Combines the given database with this one.
        /// This is literally what it says, no duplicate removal is performed
        /// combine will not take place if there are duplicate xrefs.
        /// </summary>
        /// <param name="database">
        /// A <see cref="GedcomDatabase"/>
        /// </param>
        /// <returns>
        /// A <see cref="bool"/>
        /// </returns>
        public virtual bool Combine(GedcomDatabase database)
        {
            // check the databases can be combined, i.e. unique xrefs
            bool canCombine = true;
            foreach (GedcomRecord record in database.Table.Values)
            {
                if (Contains(record.XRefID))
                {
                    canCombine = false;
                    break;
                }
            }

            if (canCombine)
            {
                foreach (GedcomRecord record in database.Table.Values)
                {
                    Add(record.XRefID, record);
                }
            }

            return canCombine;
        }

        /// <summary>
        /// Add the given individual to the surnames list
        /// </summary>
        /// <param name="indi">
        /// A <see cref="GedcomIndividualRecord"/>
        /// </param>
        protected virtual void BuildSurnameList(GedcomIndividualRecord indi)
        {
            foreach (GedcomName name in indi.Names)
            {
                // TODO: not right, need to include prefix + suffix
                string surname = name.Surname;

                if (!surnames.ContainsKey(surname))
                {
                    surnames[surname] = 1;
                }
                else
                {
                    surnames[surname] = 1 + (int)surnames[surname];
                }
            }
        }
    }
}
