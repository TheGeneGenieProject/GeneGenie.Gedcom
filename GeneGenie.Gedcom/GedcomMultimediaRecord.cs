// <copyright file="GedcomMultimediaRecord.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System;
    using System.IO;
    using System.Text;
    using GeneGenie.Gedcom.Enums;

    /// <summary>
    /// A multimedia record, this can consist of any number of files
    /// of varying types
    /// </summary>
    public class GedcomMultimediaRecord : GedcomRecord, IEquatable<GedcomMultimediaRecord>
    {
        private GedcomRecordList<GedcomMultimediaFile> files;

        private string title;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomMultimediaRecord"/> class.
        /// </summary>
        public GedcomMultimediaRecord()
        {
            files = new GedcomRecordList<GedcomMultimediaFile>();
            files.CollectionChanged += ListChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomMultimediaRecord"/> class.
        /// </summary>
        /// <param name="database">The database to associate with this record.</param>
        public GedcomMultimediaRecord(GedcomDatabase database)
            : this()
        {
            Database = database;
            Level = 0;

            XRefID = database.GenerateXref("OBJE");
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
            get { return GedcomRecordType.Multimedia; }
        }

        /// <summary>
        /// Gets the GEDCOM tag for a multimedia record.
        /// </summary>
        /// <value>
        /// The GEDCOM tag.
        /// </value>
        public override string GedcomTag
        {
            get { return "OBJE"; }
        }

        /// <summary>
        /// Gets the multimedia files.
        /// </summary>
        /// <value>
        /// The multimedia files.
        /// </value>
        public GedcomRecordList<GedcomMultimediaFile> Files
        {
            get { return files; }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(title))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (GedcomMultimediaFile file in files)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(", ");
                        }

                        sb.Append(file.Filename);
                    }

                    title = sb.ToString();
                }

                return title;
            }

            set
            {
                if (value != title)
                {
                    title = value;
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
                foreach (GedcomMultimediaFile file in Files)
                {
                    childChangeDate = file.ChangeDate;
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
        /// Compares the two passed records by title.
        /// </summary>
        /// <param name="mediaA">The first multimedia record.</param>
        /// <param name="mediaB">The second multimedia record.</param>
        /// <returns>
        /// &lt;0 if the first record's title precedes the second in the sort order;
        /// &gt;0 if the second record's title precedes the first;
        /// 0 if the titles are equal
        /// </returns>
        public static int CompareByTitle(GedcomMultimediaRecord mediaA, GedcomMultimediaRecord mediaB)
        {
            return string.Compare(mediaA.Title, mediaB.Title);
        }

        /// <summary>
        /// Adds the multimedia file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void AddMultimediaFile(string filename)
        {
            FileInfo info = new FileInfo(filename);

            GedcomMultimediaFile file = new GedcomMultimediaFile();
            file.Database = Database;

            file.Filename = filename;
            file.Format = info.Extension;

            files.Add(file);
        }

        /// <summary>
        /// Outputs this instance as a GEDCOM record.
        /// </summary>
        /// <param name="sw">The writer to output to.</param>
        public override void Output(TextWriter sw)
        {
            base.Output(sw);

            string levelPlusOne = null;
            string levelPlusTwo = null;

            foreach (GedcomMultimediaFile file in files)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (Level + 1).ToString();
                    levelPlusTwo = (Level + 2).ToString();
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" FILE ");

                // TODO: we don't support BLOB so we can end up without a filename
                if (!string.IsNullOrEmpty(file.Filename))
                {
                    sw.Write(file.Filename);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusTwo);
                sw.Write(" FORM ");
                if (!string.IsNullOrEmpty(file.Format))
                {
                    sw.Write(file.Format);
                }
                else
                {
                    sw.Write("Unknown");
                }

                if (!string.IsNullOrEmpty(file.SourceMediaType))
                {
                    sw.Write(Environment.NewLine);
                    sw.Write(levelPlusTwo);
                    sw.Write(" MEDI ");
                    sw.Write(file.SourceMediaType);
                }
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
            var media = obj as GedcomMultimediaRecord;

            if (media == null)
            {
                return false;
            }

            if (!GedcomGenericListComparer.CompareLists(Files, media.Files))
            {
                return false;
            }

            if (!Equals(Title, media.Title))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Compare the user entered data against the passed instance for similarity.
        /// </summary>
        /// <param name="other">The GedcomMultimediaRecord to compare this instance against.</param>
        /// <returns>
        /// True if instance matches user data, otherwise false.
        /// </returns>
        public bool Equals(GedcomMultimediaRecord other)
        {
            return IsEquivalentTo(other);
        }

        /// <summary>
        /// Compare the user entered data against the passed instance for similarity.
        /// </summary>
        /// <param name="obj">The object to compare this instance against.</param>
        /// <returns>
        /// True if instance matches user data, otherwise false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return IsEquivalentTo(obj);
        }

        public override int GetHashCode()
        {
            return new
            {
                Files,
                Title,
            }.GetHashCode();
        }
    }
}
