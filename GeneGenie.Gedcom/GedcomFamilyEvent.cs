/*
 *  $Id: GedcomFamilyEvent.cs 200 2008-11-30 14:34:07Z davek $
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
    /// An event relating to a given family
    /// </summary>
    public class GedcomFamilyEvent : GedcomEvent
    {
        private GedcomAge _HusbandAge;
        private GedcomAge _WifeAge;

        public GedcomFamilyEvent()
        {
        }

        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.FamilyEvent; }
        }

        public GedcomAge HusbandAge
        {
            get
            {
                return _HusbandAge;
            }

            set
            {
                if (value != _HusbandAge)
                {
                    _HusbandAge = value;
                    Changed();
                }
            }
        }

        public GedcomAge WifeAge
        {
            get
            {
                return _WifeAge;
            }

            set
            {
                if (value != _WifeAge)
                {
                    _WifeAge = value;
                    Changed();
                }
            }
        }

        // util backpointer to the family record
        // this event belongs in
        public GedcomFamilyRecord FamRecord
        {
            get
            {
                return (GedcomFamilyRecord)_Record;
            }

            set
            {
                if (value != _Record)
                {
                    _Record = value;
                    if (_Record != null)
                    {
                        if (_Record.RecordType != GedcomRecordType.Family)
                        {
                            throw new Exception("Must set a GedcomFamilyRecord on a GedcomFamilyEvent");
                        }

                        Database = _Record.Database;
                    }
                    else
                    {
                        Database = null;
                    }

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
                if (_HusbandAge != null)
                {
                    childChangeDate = _HusbandAge.ChangeDate;
                    if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                    {
                        realChangeDate = childChangeDate;
                    }
                }

                if (_WifeAge != null)
                {
                    childChangeDate = _WifeAge.ChangeDate;
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
