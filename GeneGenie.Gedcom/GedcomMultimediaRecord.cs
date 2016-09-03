/*
 *  $Id: GedcomMultimediaRecord.cs 200 2008-11-30 14:34:07Z davek $
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
    using System.Text;

    /// <summary>
    /// A multimedia record, this can consist of any number of files
    /// of varying types
    /// </summary>
    public class GedcomMultimediaRecord : GedcomRecord
    {
        private GedcomRecordList<GedcomMultimediaFile> _Files;

        private string _Title;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomMultimediaRecord"/> class.
        /// </summary>
        public GedcomMultimediaRecord()
        {
            _Files = new GedcomRecordList<GedcomMultimediaFile>();
            _Files.Changed += ListChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomMultimediaRecord"/> class.
        /// </summary>
        /// <param name="database"></param>
        public GedcomMultimediaRecord(GedcomDatabase database)
            : this()
        {
            Database = database;
            Level = 0;

            XRefID = database.GenerateXref("OBJE");
            database.Add(XRefID, this);
        }

        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Multimedia; }
        }

        public override string GedcomTag
        {
            get { return "OBJE"; }
        }

        public GedcomRecordList<GedcomMultimediaFile> Files
        {
            get { return _Files; }
        }

        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(_Title))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (GedcomMultimediaFile file in _Files)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(", ");
                        }

                        sb.Append(file.Filename);
                    }

                    _Title = sb.ToString();
                }

                return _Title;
            }
            set
            {
                if (value != _Title)
                {
                    _Title = value;
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

                public static int CompareByTitle(GedcomMultimediaRecord mediaA, GedcomMultimediaRecord mediaB)
        {
            return string.Compare(mediaA.Title,mediaB.Title);
        }

        public void AddMultimediaFile(string filename)
        {
            FileInfo info = new FileInfo(filename);

            GedcomMultimediaFile file = new GedcomMultimediaFile();
            file.Database = _database;

            file.Filename = filename;
            file.Format = info.Extension;

            _Files.Add(file);
        }

        public override void Output(TextWriter sw)
        {
            base.Output(sw);

            string levelPlusOne = null;
            string levelPlusTwo = null;

            foreach (GedcomMultimediaFile file in _Files)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                    levelPlusTwo = Util.IntToString(Level + 2);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" FILE ");
                // FIXME: we don't support BLOB so we can end up without a
                // filename
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
    }
}
