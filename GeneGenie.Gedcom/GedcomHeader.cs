/*
 *  $Id: GedcomHeader.cs 200 2008-11-30 14:34:07Z davek $
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

    /// <summary>
    /// The header from / for a GEDCOM file.
    /// </summary>
    public class GedcomHeader : GedcomRecord
    {
        private GedcomNoteRecord _ContentDescription;

        private string _submitterXRefID;

        private GedcomDate _transmissionDate;

        private string _copyright;

        private string _language;

        private string _filename;

        private bool _test;

        private string _applicationName = string.Empty;
        private string _applicationVersion = string.Empty;
        private string _applicationSystemID = "Gedcom.NET";
        private string _corporation = string.Empty;

        private GedcomAddress _corporationAddress;

        private string _sourceName = string.Empty;
        private GedcomDate _sourceDate;
        private string _sourceCopyright;

        public GedcomHeader()
        {
        }

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

        public bool Test
        {
            get { return _test; }
            set { _test = value; }
        }

        public string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        public string ApplicationVersion
        {
            get { return _applicationVersion; }
            set { _applicationVersion = value; }
        }

        public string ApplicationSystemID
        {
            get { return _applicationSystemID; }
            set { _applicationSystemID = value; }
        }

        public string Corporation
        {
            get { return _corporation; }
            set { _corporation = value; }
        }

        public GedcomAddress CorporationAddress
        {
            get { return _corporationAddress; }
            set { _corporationAddress = value; }
        }

        public GedcomNoteRecord ContentDescription
        {
            get
            {
                return _ContentDescription;
            }

            set
            {
                if (value != _ContentDescription)
                {
                    _ContentDescription = value;
                    Changed();
                }
            }
        }

        public string SubmitterXRefID
        {
            get
            {
                return _submitterXRefID;
            }

            set
            {
                if (_submitterXRefID != value)
                {
                    if (!string.IsNullOrEmpty(_submitterXRefID))
                    {
                        Submitter.Delete();
                    }

                    _submitterXRefID = value;
                    Changed();
                }
            }
        }

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

        public GedcomDate TransmissionDate
        {
            get
            {
                return _transmissionDate;
            }

            set
            {
                if (_transmissionDate != value)
                {
                    _transmissionDate = value;
                    Changed();
                }
            }
        }

        public string Copyright
        {
            get
            {
                return _copyright;
            }

            set
            {
                if (_copyright != value)
                {
                    _copyright = value;
                    Changed();
                }
            }
        }

        public string Language
        {
            get
            {
                return _language;
            }

            set
            {
                if (_language != value)
                {
                    _language = value;
                    Changed();
                }
            }
        }

        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        public string SourceName
        {
            get
            {
                return _sourceName;
            }

            set
            {
                if (_sourceName != value)
                {
                    _sourceName = value;
                    Changed();
                }
            }
        }

        public GedcomDate SourceDate
        {
            get
            {
                return _sourceDate;
            }

            set
            {
                if (_sourceDate != value)
                {
                    _sourceDate = value;
                    Changed();
                }
            }
        }

        public string SourceCopyright
        {
            get
            {
                return _sourceCopyright;
            }

            set
            {
                if (_sourceCopyright != value)
                {
                    _sourceCopyright = value;
                    Changed();
                }
            }
        }

        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Header; }
        }

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
            if (_test)
            {
                date = new DateTime(2007,1,1);
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

            bool hasSubmitter = !string.IsNullOrEmpty(_submitterXRefID);

            if (hasSubmitter)
            {
                sw.Write(Environment.NewLine);
                sw.Write("1 SUBM ");
                sw.Write(_submitterXRefID);
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
