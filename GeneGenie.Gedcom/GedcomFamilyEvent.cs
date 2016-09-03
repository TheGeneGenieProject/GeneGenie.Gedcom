// <copyright file="GedcomFamilyEvent.cs" company="GeneGenie.com">
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
    /// An event relating to a given family
    /// </summary>
    public class GedcomFamilyEvent : GedcomEvent
    {
        private GedcomAge husbandAge;
        private GedcomAge wifeAge;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomFamilyEvent"/> class.
        /// </summary>
        public GedcomFamilyEvent()
        {
        }

        /// <summary>
        /// Gets the type of the record.
        /// </summary>
        /// <value>
        /// The type of the record.
        /// </value>
        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.FamilyEvent; }
        }

        /// <summary>
        /// Gets or sets the husband age.
        /// </summary>
        /// <value>
        /// The husband age.
        /// </value>
        public GedcomAge HusbandAge
        {
            get
            {
                return husbandAge;
            }

            set
            {
                if (value != husbandAge)
                {
                    husbandAge = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the wife age.
        /// </summary>
        /// <value>
        /// The wife age.
        /// </value>
        public GedcomAge WifeAge
        {
            get
            {
                return wifeAge;
            }

            set
            {
                if (value != wifeAge)
                {
                    wifeAge = value;
                    Changed();
                }
            }
        }

        // util backpointer to the family record
        // this event belongs in

        /// <summary>
        /// Gets or sets the fam record.
        /// </summary>
        /// <value>
        /// The fam record.
        /// </value>
        /// <exception cref="Exception">Must set a GedcomFamilyRecord on a GedcomFamilyEvent</exception>
        public GedcomFamilyRecord FamRecord
        {
            get
            {
                return (GedcomFamilyRecord)Record;
            }

            set
            {
                if (value != Record)
                {
                    Record = value;
                    if (Record != null)
                    {
                        if (Record.RecordType != GedcomRecordType.Family)
                        {
                            throw new Exception("Must set a GedcomFamilyRecord on a GedcomFamilyEvent");
                        }

                        Database = Record.Database;
                    }
                    else
                    {
                        Database = null;
                    }

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
                if (husbandAge != null)
                {
                    childChangeDate = husbandAge.ChangeDate;
                    if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                    {
                        realChangeDate = childChangeDate;
                    }
                }

                if (wifeAge != null)
                {
                    childChangeDate = wifeAge.ChangeDate;
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
            base.Output(sw);

            string levelPlusOne = null;

            if (HusbandAge != null)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" HUSB ");

                HusbandAge.Output(sw, Level + 2);
            }

            if (WifeAge != null)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" WIFE ");

                WifeAge.Output(sw, Level + 2);
            }
        }
    }
}
