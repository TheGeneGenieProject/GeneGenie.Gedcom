/*
 *  $Id: GedcomSubmitterRecord.cs 200 2008-11-30 14:34:07Z davek $
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

    public class GedcomSubmitterRecord : GedcomRecord
    {
        private string _Name;
        private GedcomAddress _Address;
        private string[] _LanguagePreferences;
        private string _RegisteredRFN;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomSubmitterRecord"/> class.
        /// </summary>
        public GedcomSubmitterRecord()
        {
            _LanguagePreferences = new string[3];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomSubmitterRecord"/> class.
        /// </summary>
        /// <param name="database"></param>
        public GedcomSubmitterRecord(GedcomDatabase database)
            : this()
        {
            Database = database;

            Level = 0;
            XRefID = database.GenerateXref("S");

            database.Add(XRefID, this);
        }

        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Submitter; }
        }

        public override string GedcomTag
        {
            get { return "SUBM"; }
        }

        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                if (value != _Name)
                {
                    _Name = value;
                    Changed();
                }
            }
        }

        public GedcomAddress Address
        {
            get
            {
                return _Address;
            }

            set
            {
                if (value != _Address)
                {
                    _Address = value;
                    Changed();
                }
            }
        }

        public string[] LanguagePreferences
        {
            get
            {
                return _LanguagePreferences;
            }

            set
            {
                if (value != _LanguagePreferences)
                {
                    _LanguagePreferences = value;
                    Changed();
                }
            }
        }

        public string RegisteredRFN
        {
            get
            {
                return _RegisteredRFN;
            }

            set
            {
                if (value != _RegisteredRFN)
                {
                    _RegisteredRFN = value;
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

        public override void Output(TextWriter sw)
        {
            sw.Write(Environment.NewLine);
            sw.Write(Util.IntToString(Level));
            sw.Write(" ");

            if (!string.IsNullOrEmpty(_XrefID))
            {
                sw.Write("@");
                sw.Write(_XrefID);
                sw.Write("@ ");
            }

            sw.Write(GedcomTag);

            string levelPlusOne = Util.IntToString(Level + 1);

            string name = Name;
            if (string.IsNullOrEmpty(name))
            {
                name = "Unknown";
            }

            sw.Write(Environment.NewLine);
            sw.Write(levelPlusOne);
            sw.Write(" NAME ");
            sw.Write(name);

            if (Address != null)
            {
                Address.Output(sw, Level + 1);
            }

            foreach (string languagePreference in LanguagePreferences)
            {
                if (!string.IsNullOrEmpty(languagePreference))
                {
                    sw.Write(Environment.NewLine);
                    sw.Write(levelPlusOne);
                    sw.Write(" LANG ");
                    sw.Write(languagePreference);
                }
            }

            if (!string.IsNullOrEmpty(RegisteredRFN))
            {
                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" RFN ");
                sw.Write(RegisteredRFN);
            }

            OutputStandard(sw);
        }
    }
}
