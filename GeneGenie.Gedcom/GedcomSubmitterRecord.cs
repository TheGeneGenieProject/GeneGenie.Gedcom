// <copyright file="GedcomSubmitterRecord.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using GeneGenie.Gedcom.Enums;

    /// <summary>
    /// An individual or organization who contributes genealogical data to a file or transfers it to someone else.
    /// </summary>
    /// <seealso cref="GedcomRecord" />
    public class GedcomSubmitterRecord : GedcomRecord, IEquatable<GedcomSubmitterRecord>
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
        /// Gets the GEDCOM tag for a submitter record.
        /// </summary>
        /// <value>
        /// The GEDCOM tag.
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
        /// Outputs this submitter record as a GEDCOM record.
        /// </summary>
        /// <param name="sw">The writer to output to.</param>
        public override void Output(TextWriter sw)
        {
            sw.Write(Environment.NewLine);
            sw.Write(Level.ToString());
            sw.Write(" ");

            if (!string.IsNullOrEmpty(XrefId))
            {
                sw.Write("@");
                sw.Write(XrefId);
                sw.Write("@ ");
            }

            sw.Write(GedcomTag);

            string levelPlusOne = (Level + 1).ToString();

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

        /// <summary>
        /// Compare the user entered data against the passed instance for similarity.
        /// </summary>
        /// <param name="obj">The object to compare this instance against.</param>
        /// <returns>
        /// True if instance matches user data, otherwise false.
        /// </returns>
        public override bool IsEquivalentTo(object obj)
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

        /// <summary>
        /// Compare the user entered data against the passed instance for similarity.
        /// </summary>
        /// <param name="other">The GedcomSubmitterRecord to compare this instance against.</param>
        /// <returns>
        /// True if instance matches user data, otherwise false.
        /// </returns>
        public bool Equals(GedcomSubmitterRecord other)
        {
            return IsEquivalentTo(other);
        }
    }
}
