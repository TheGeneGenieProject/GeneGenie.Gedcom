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
    public class GedcomMultimediaFile
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
        /// Changeds this instance.
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
