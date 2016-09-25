// <copyright file="GedcomSubmitterRecord.cs" company="GeneGenie.com">
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
    using System.Collections.Generic;
    using System.IO;
    using Enums;

    /// <summary>
    /// TODO: Doc
    /// </summary>
    /// <seealso cref="GedcomRecord" />
    public class GedcomSubmitterRecord : GedcomRecord
    {
        private string name;
        private GedcomAddress address;
        private List<string> languagePreferences;
        private string registeredRFN;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomSubmitterRecord"/> class.
        /// </summary>
        public GedcomSubmitterRecord()
        {
            LanguagePreferences = new List<string>(3);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomSubmitterRecord"/> class.
        /// </summary>
        /// <param name="database">The database to associate with this record.</param>
        public GedcomSubmitterRecord(GedcomDatabase database)
            : this()
        {
            Database = database;

            Level = 0;
            XRefID = database.GenerateXref("S");

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
            get { return GedcomRecordType.Submitter; }
        }

        /// <summary>
        /// Gets the gedcom tag.
        /// </summary>
        /// <value>
        /// The gedcom tag.
        /// </value>
        public override string GedcomTag
        {
            get { return "SUBM"; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (value != name)
                {
                    name = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public GedcomAddress Address
        {
            get
            {
                return address;
            }

            set
            {
                if (value != address)
                {
                    address = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the language preferences.
        /// </summary>
        /// <value>
        /// The language preferences.
        /// </value>
        public List<string> LanguagePreferences
        {
            get
            {
                return languagePreferences;
            }

            set
            {
                if (value != languagePreferences)
                {
                    languagePreferences = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the registered RFN.
        /// </summary>
        /// <value>
        /// The registered RFN.
        /// </value>
        public string RegisteredRFN
        {
            get
            {
                return registeredRFN;
            }

            set
            {
                if (value != registeredRFN)
                {
                    registeredRFN = value;
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

        /// <summary>
        /// Outputs the specified sw.
        /// </summary>
        /// <param name="sw">The sw.</param>
        public override void Output(TextWriter sw)
        {
            sw.Write(Environment.NewLine);
            sw.Write(Util.IntToString(Level));
            sw.Write(" ");

            if (!string.IsNullOrEmpty(XrefId))
            {
                sw.Write("@");
                sw.Write(XrefId);
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

        public override bool IsSimilar(object obj)
        {
            var submitter = obj as GedcomSubmitterRecord;

            if (submitter == null)
            {
                return false;
            }

            if (!Equals(Address, submitter.Address))
            {
                return false;
            }

            if (!Equals(ChangeDate, submitter.ChangeDate))
            {
                return false;
            }

            if (!GedcomGenericListComparer.CompareLists(LanguagePreferences, submitter.LanguagePreferences))
            {
                return false;
            }

            if (!Equals(Name, submitter.Name))
            {
                return false;
            }

            if (!Equals(RegisteredRFN, submitter.RegisteredRFN))
            {
                return false;
            }

            return true;
        }
    }
}
