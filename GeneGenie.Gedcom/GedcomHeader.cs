// <copyright file="GedcomHeader.cs" company="GeneGenie.com">
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

    /// <summary>
    /// The header from / for a GEDCOM file.
    /// </summary>
    public class GedcomHeader : GedcomRecord
    {
        private GedcomNoteRecord contentDescription;

        private string submitterXRefID;

        private GedcomDate transmissionDate;

        private string copyright;

        private string language;

        private string filename;

        private bool test;

        private string applicationName = string.Empty;
        private string applicationVersion = string.Empty;
        private string applicationSystemID = "GeneGenie.Gedcom";
        private string corporation = string.Empty;

        private GedcomAddress corporationAddress;

        private string sourceName = string.Empty;
        private GedcomDate sourceDate;
        private string sourceCopyright;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomHeader"/> class.
        /// </summary>
        public GedcomHeader()
        {
        }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        /// <exception cref="Exception">Database can only have one header</exception>
        public override GedcomDatabase Database
        {
            get
            {
                return base.Database;
            }

            set
            {
                base.Database = value;
                if (Database != null)
                {
                    if (Database.Header != null)
                    {
                        throw new Exception("Database can only have one header");
                    }

                    Database.Header = this;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GedcomHeader"/> is test.
        /// </summary>
        /// <value>
        ///   <c>true</c> if test; otherwise, <c>false</c>.
        /// </value>
        public bool Test
        {
            get { return test; }
            set { test = value; }
        }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        public string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        public string ApplicationVersion
        {
            get { return applicationVersion; }
            set { applicationVersion = value; }
        }

        /// <summary>
        /// Gets or sets the application system identifier.
        /// </summary>
        /// <value>
        /// The application system identifier.
        /// </value>
        public string ApplicationSystemID
        {
            get { return applicationSystemID; }
            set { applicationSystemID = value; }
        }

        /// <summary>
        /// Gets or sets the corporation.
        /// </summary>
        /// <value>
        /// The corporation.
        /// </value>
        public string Corporation
        {
            get { return corporation; }
            set { corporation = value; }
        }

        /// <summary>
        /// Gets or sets the corporation address.
        /// </summary>
        /// <value>
        /// The corporation address.
        /// </value>
        public GedcomAddress CorporationAddress
        {
            get { return corporationAddress; }
            set { corporationAddress = value; }
        }

        /// <summary>
        /// Gets or sets the content description.
        /// </summary>
        /// <value>
        /// The content description.
        /// </value>
        public GedcomNoteRecord ContentDescription
        {
            get
            {
                return contentDescription;
            }

            set
            {
                if (value != contentDescription)
                {
                    contentDescription = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the submitter x reference identifier.
        /// </summary>
        /// <value>
        /// The submitter x reference identifier.
        /// </value>
        public string SubmitterXRefID
        {
            get
            {
                return submitterXRefID;
            }

            set
            {
                if (submitterXRefID != value)
                {
                    if (!string.IsNullOrEmpty(submitterXRefID))
                    {
                        Submitter.Delete();
                    }

                    submitterXRefID = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the submitter.
        /// </summary>
        /// <value>
        /// The submitter.
        /// </value>
        public GedcomSubmitterRecord Submitter
        {
            get
            {
                return Database[SubmitterXRefID] as GedcomSubmitterRecord;
            }

            set
            {
                if (value == null)
                {
                    SubmitterXRefID = null;
                }
                else
                {
                    SubmitterXRefID = value.XRefID;
                }
            }
        }

        /// <summary>
        /// Gets or sets the transmission date.
        /// </summary>
        /// <value>
        /// The transmission date.
        /// </value>
        public GedcomDate TransmissionDate
        {
            get
            {
                return transmissionDate;
            }

            set
            {
                if (transmissionDate != value)
                {
                    transmissionDate = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the copyright.
        /// </summary>
        /// <value>
        /// The copyright.
        /// </value>
        public string Copyright
        {
            get
            {
                return copyright;
            }

            set
            {
                if (copyright != value)
                {
                    copyright = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language
        {
            get
            {
                return language;
            }

            set
            {
                if (language != value)
                {
                    language = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        /// <summary>
        /// Gets or sets the name of the source.
        /// </summary>
        /// <value>
        /// The name of the source.
        /// </value>
        public string SourceName
        {
            get
            {
                return sourceName;
            }

            set
            {
                if (sourceName != value)
                {
                    sourceName = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the source date.
        /// </summary>
        /// <value>
        /// The source date.
        /// </value>
        public GedcomDate SourceDate
        {
            get
            {
                return sourceDate;
            }

            set
            {
                if (sourceDate != value)
                {
                    sourceDate = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the source copyright.
        /// </summary>
        /// <value>
        /// The source copyright.
        /// </value>
        public string SourceCopyright
        {
            get
            {
                return sourceCopyright;
            }

            set
            {
                if (sourceCopyright != value)
                {
                    sourceCopyright = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets the type of the record.
        /// </summary>
        /// <value>
        /// The type of the record.
        /// </value>
        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Header; }
        }

        /// <summary>
        /// Outputs the specified sw.
        /// </summary>
        /// <param name="sw">The sw.</param>
        public override void Output(TextWriter sw)
        {
            sw.Write("0 HEAD");

            sw.Write(Environment.NewLine);
            sw.Write("1 SOUR {0}", ApplicationSystemID);

            if (!string.IsNullOrEmpty(ApplicationVersion))
            {
                sw.Write(Environment.NewLine);
                sw.Write("2 VERS {0}", ApplicationVersion);
            }

            if (!string.IsNullOrEmpty(ApplicationName))
            {
                sw.Write(Environment.NewLine);
                sw.Write("2 NAME {0}", ApplicationName);
            }

            if (!string.IsNullOrEmpty(Corporation))
            {
                sw.Write(Environment.NewLine);
                sw.Write("2 CORP {0}", Corporation);
            }

            if (CorporationAddress != null)
            {
                CorporationAddress.Output(sw, 3);
            }

            DateTime date = DateTime.Today;
            if (test)
            {
                date = new DateTime(2007, 1, 1);
            }

            if (!string.IsNullOrEmpty(SourceName) ||
                !string.IsNullOrEmpty(SourceCopyright) ||
                SourceDate != null)
            {
                sw.Write(Environment.NewLine);
                sw.Write("2 DATA");
                if (!string.IsNullOrEmpty(SourceName))
                {
                    sw.Write(" ");
                    sw.Write(SourceName);
                }

                if (!string.IsNullOrEmpty(SourceCopyright))
                {
                    sw.Write(Environment.NewLine);
                    sw.Write("3 COPR ");
                    sw.Write(SourceCopyright);
                }

                if (SourceDate != null)
                {
                    SourceDate.Output(sw);
                }
            }

            sw.Write(Environment.NewLine);
            sw.Write("1 DATE {0:dd MMM yyyy}", date);

            bool hasSubmitter = !string.IsNullOrEmpty(submitterXRefID);

            if (hasSubmitter)
            {
                sw.Write(Environment.NewLine);
                sw.Write("1 SUBM ");
                sw.Write(submitterXRefID);
            }

            if (ContentDescription != null)
            {
                ContentDescription.Output(sw);
            }

            sw.Write(Environment.NewLine);
            sw.Write("1 CHAR UTF-8");

            sw.Write(Environment.NewLine);
            sw.Write("1 FILE {0}", Filename);

            sw.Write(Environment.NewLine);
            sw.Write("1 GEDC");

            sw.Write(Environment.NewLine);
            sw.Write("2 VERS 5.5");

            sw.Write(Environment.NewLine);
            sw.Write("2 FORM LINEAGE-LINKED");
        }
    }
}
