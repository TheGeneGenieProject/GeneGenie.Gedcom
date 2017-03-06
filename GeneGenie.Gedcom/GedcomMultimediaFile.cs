// <copyright file="GedcomMultimediaFile.cs" company="GeneGenie.com">
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

    /// <summary>
    /// A multimedia file
    /// </summary>
    public class GedcomMultimediaFile : IComparable, IComparable<GedcomMultimediaFile>, IEquatable<GedcomMultimediaFile>
    {
        private GedcomDatabase database;

        private string filename;
        private string format;
        private string sourceMediaType;

        private GedcomChangeDate changeDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomMultimediaFile"/> class.
        /// </summary>
        public GedcomMultimediaFile()
        {
        }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public GedcomDatabase Database
        {
            get { return database; }
            set { database = value; }
        }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        public string Filename
        {
            get
            {
                return filename;
            }

            set
            {
                if (value != filename)
                {
                    filename = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        public string Format
        {
            get
            {
                return format;
            }

            set
            {
                if (value != format)
                {
                    format = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the source media.
        /// </summary>
        /// <value>
        /// The type of the source media.
        /// </value>
        public string SourceMediaType
        {
            get
            {
                return sourceMediaType;
            }

            set
            {
                if (value != sourceMediaType)
                {
                    sourceMediaType = value;
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
        public GedcomChangeDate ChangeDate
        {
            get { return changeDate; }
            set { changeDate = value; }
        }

        /// <summary>
        /// Compares an object to this GedcomMultimediaFile to determine sort order.
        /// </summary>
        /// <param name="obj">The object to compare to the current instance</param>
        /// <returns>Returns a value determine the sort order of the compared objects.</returns>
        public int CompareTo(object obj)
        {
            return CompareTo(obj as GedcomMultimediaFile);
        }

        /// <summary>
        /// Compares two GedcomMultimediaFile instances to determine sort order.
        /// </summary>
        /// <param name="other">The GedcomMultimediaFile to compare to the current instance</param>
        /// <returns>Returns a value determine the sort order of the compared GedcomMultimediaFile objects.</returns>
        public int CompareTo(GedcomMultimediaFile other)
        {
            if (other == null)
            {
                return 1;
            }

            var compare = string.Compare(Filename, other.Filename);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Format, other.Format);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(SourceMediaType, other.SourceMediaType);
            if (compare != 0)
            {
                return compare;
            }

            return compare;
        }

        /// <summary>
        /// Compares two instances of GedcomMultimediaFile to determine equality.
        /// </summary>
        /// <param name="other">The GedcomMultimediaFile to compare to the current instance</param>
        /// <returns>True if equal, otherwise False</returns>
        public bool Equals(GedcomMultimediaFile other)
        {
            return CompareTo(other) == 0;
        }

        /// <summary>
        /// Compares an object to this GedcomMultimediaFile to determine equality.
        /// </summary>
        /// <param name="obj">The object to compare to the current instance</param>
        /// <returns>True if equal, otherwise False</returns>
        public override bool Equals(object obj)
        {
            return CompareTo(obj as GedcomMultimediaFile) == 0;
        }

        /// <summary>
        /// Updates the change date and time.
        /// </summary>
        protected virtual void Changed()
        {
            if (database == null)
            {
                // System.Console.WriteLine("Changed() called on record with no database set");
                //
                // System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
                // foreach (System.Diagnostics.StackFrame f in trace.GetFrames())
                // {
                //     System.Console.WriteLine(f);
                // }
            }
            else if (!database.Loading)
            {
                if (changeDate == null)
                {
                    changeDate = new GedcomChangeDate(database); // TODO: what level?
                }

                DateTime now = DateTime.Now;

                changeDate.Date1 = now.ToString("dd MMM yyyy");
                changeDate.Time = now.ToString("hh:mm:ss");
            }
        }
    }
}
