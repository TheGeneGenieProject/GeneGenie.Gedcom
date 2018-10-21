// <copyright file="GedcomHeader.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System;
    using System.IO;
    using GeneGenie.Gedcom.Enums;

    /// <summary>
    /// The header from / for a GEDCOM file.
    /// </summary>
    public class GedcomHeader : GedcomRecord, IEquatable<GedcomHeader>
    {
        private GedcomNoteRecord contentDescription;

        private string submitterXRefID;

        private GedcomDate transmissionDate;

        private string copyright;

        private string language;

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
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        public string ApplicationName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        public string ApplicationVersion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the application system identifier.
        /// </summary>
        /// <value>
        /// The application system identifier.
        /// </value>
        public string ApplicationSystemId { get; set; } = "GeneGenie.Gedcom";

        /// <summary>
        /// Gets or sets the corporation.
        /// </summary>
        /// <value>
        /// The corporation.
        /// </value>
        public string Corporation { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the corporation address.
        /// </summary>
        /// <value>
        /// The corporation address.
        /// </value>
        public GedcomAddress CorporationAddress { get; set; }

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
        public string Filename { get; set; }

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
        /// Output GEDCOM format for this instance.
        /// </summary>
        /// <param name="sw">Where to output the data to.</param>
        public override void Output(TextWriter sw)
        {
            sw.Write("0 HEAD");

            sw.Write(Environment.NewLine);
            sw.Write("1 SOUR {0}", ApplicationSystemId);

            if (!string.IsNullOrEmpty(ApplicationName))
            {
                sw.Write(Environment.NewLine);
                sw.Write("2 NAME {0}", ApplicationName);
            }

            if (!string.IsNullOrEmpty(ApplicationVersion))
            {
                sw.Write(Environment.NewLine);
                sw.Write("2 VERS {0}", ApplicationVersion);
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

            if (TransmissionDate != null)
            {
                TransmissionDate.Output(sw);
            }

            sw.Write(Environment.NewLine);
            sw.Write("1 FILE {0}", Filename);

            if (ContentDescription != null)
            {
                ContentDescription.Output(sw);
            }

            sw.Write(Environment.NewLine);
            sw.Write("1 GEDC");

            sw.Write(Environment.NewLine);
            sw.Write("2 VERS 5.5.1");

            sw.Write(Environment.NewLine);
            sw.Write("2 FORM LINEAGE-LINKED");

            sw.Write(Environment.NewLine);
            sw.Write("1 CHAR UTF-8");

            sw.Write(Environment.NewLine);
            if (!string.IsNullOrWhiteSpace(Language))
            {
                sw.Write($"1 LANG {Language}");
            }

            bool hasSubmitter = !string.IsNullOrEmpty(submitterXRefID);
            if (hasSubmitter)
            {
                sw.Write(Environment.NewLine);
                sw.Write($"1 SUBM @{submitterXRefID}@");
            }
        }

        /// <summary>
        /// Checks if the passed header is equal in terms of user content to the current instance.
        /// If new fields are added to the header they should also be added in here for comparison.
        /// </summary>
        /// <param name="obj">The object to compare against this instance.</param>
        /// <returns>Returns true if headers match in user entered content, otherwise false.</returns>
        public override bool IsEquivalentTo(object obj)
        {
            var header = obj as GedcomHeader;

            if (header == null)
            {
                return false;
            }

            if (!Equals(ApplicationName, header.ApplicationName))
            {
                return false;
            }

            if (!Equals(ApplicationSystemId, header.ApplicationSystemId))
            {
                return false;
            }

            if (!Equals(ApplicationVersion, header.ApplicationVersion))
            {
                return false;
            }

            if (!Equals(ContentDescription, header.ContentDescription))
            {
                return false;
            }

            if (!Equals(Copyright, header.Copyright))
            {
                return false;
            }

            if (!Equals(Corporation, header.Corporation))
            {
                return false;
            }

            if (!Equals(CorporationAddress, header.CorporationAddress))
            {
                return false;
            }

            if (!Equals(Filename, header.Filename))
            {
                return false;
            }

            if (!Equals(Language, header.Language))
            {
                return false;
            }

            if (!Equals(SourceCopyright, header.SourceCopyright))
            {
                return false;
            }

            if (!Equals(SourceDate, header.SourceDate))
            {
                return false;
            }

            if (!Equals(SourceName, header.SourceName))
            {
                return false;
            }

            if (!Equals(TransmissionDate, header.TransmissionDate))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the passed header is equal in terms of user content to the current instance.
        /// If new fields are added to the header they should also be added in here for comparison.
        /// </summary>
        /// <param name="other">The GedcomHeader to compare against this instance.</param>
        /// <returns>Returns true if headers match in user entered content, otherwise false.</returns>
        public bool Equals(GedcomHeader other)
        {
            return IsEquivalentTo(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            // Overflow is fine, just wrap.
            unchecked
            {
                int hash = 17;

                hash *= 23 + ApplicationName.GetHashCode();
                hash *= 23 + ApplicationSystemId.GetHashCode();
                hash *= 23 + ApplicationVersion.GetHashCode();
                hash *= 23 + ContentDescription.GetHashCode();
                hash *= 23 + Copyright.GetHashCode();
                hash *= 23 + Corporation.GetHashCode();
                hash *= 23 + CorporationAddress.GetHashCode();
                hash *= 23 + Filename.GetHashCode();
                hash *= 23 + Language.GetHashCode();
                hash *= 23 + SourceCopyright.GetHashCode();
                hash *= 23 + SourceDate.GetHashCode();
                hash *= 23 + SourceName.GetHashCode();
                hash *= 23 + TransmissionDate.GetHashCode();

                return hash;
            }
        }
    }
}
