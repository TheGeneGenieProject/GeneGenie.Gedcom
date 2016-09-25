// <copyright file="GedcomMultimediaRecord.cs" company="GeneGenie.com">
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
    using System.Text;
    using Enums;

    /// <summary>
    /// A multimedia record, this can consist of any number of files
    /// of varying types
    /// </summary>
    public class GedcomMultimediaRecord : GedcomRecord
    {
        private GedcomRecordList<GedcomMultimediaFile> files;

        private string title;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomMultimediaRecord"/> class.
        /// </summary>
        public GedcomMultimediaRecord()
        {
            files = new GedcomRecordList<GedcomMultimediaFile>();
            files.Changed += ListChanged;
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
        /// Gets the gedcom tag.
        /// </summary>
        /// <value>
        /// The gedcom tag.
        /// </value>
        public override string GedcomTag
        {
            get { return "OBJE"; }
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>
        /// The files.
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
        /// <param name="mediaA">The media a.</param>
        /// <param name="mediaB">The media b.</param>
        /// <returns>TODO: Doc</returns>
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
        /// Outputs the specified sw.
        /// </summary>
        /// <param name="sw">The sw.</param>
        public override void Output(TextWriter sw)
        {
            base.Output(sw);

            string levelPlusOne = null;
            string levelPlusTwo = null;

            foreach (GedcomMultimediaFile file in files)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                    levelPlusTwo = Util.IntToString(Level + 2);
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

        public override bool IsSimilar(object obj)
        {
            var media = obj as GedcomMultimediaRecord;

            if (media == null)
            {
                return false;
            }

            if (!Equals(ChangeDate, media.ChangeDate))
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
    }
}
