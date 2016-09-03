/*
 *  $Id: GedcomPlace.cs 200 2008-11-30 14:34:07Z davek $
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

    public class GedcomPlace : GedcomRecord
    {
        private string _Name;
        private string _Form;

        private GedcomRecordList<GedcomVariation> _PhoneticVariations;
        private GedcomRecordList<GedcomVariation> _RomanizedVariations;

        private string _Latitude;
        private string _Longitude;

        public GedcomPlace()
        {
        }

        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Place; }
        }

        public override string GedcomTag
        {
            get { return "PLAC"; }
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

        public string Form
        {
            get
            {
                return _Form;
            }

            set
            {
                if (value != _Form)
                {
                    _Form = value;
                    Changed();
                }
            }
        }

        public GedcomRecordList<GedcomVariation> PhoneticVariations
        {
            get
            {
                if (_PhoneticVariations == null)
                {
                    _PhoneticVariations = new GedcomRecordList<GedcomVariation>();
                    _PhoneticVariations.Changed += ListChanged;
                }

                return _PhoneticVariations;
            }
        }

        public GedcomRecordList<GedcomVariation> RomanizedVariations
        {
            get
            {
                if (_RomanizedVariations == null)
                {
                    _RomanizedVariations = new GedcomRecordList<GedcomVariation>();
                    _RomanizedVariations.Changed += ListChanged;
                }

                return _RomanizedVariations;
            }
        }

        public string Latitude
        {
            get
            {
                return _Latitude;
            }

            set
            {
                if (value != _Latitude)
                {
                    _Latitude = value;
                    Changed();
                }
            }
        }

        public string Longitude
        {
            get
            {
                return _Longitude;
            }

            set
            {
                if (value != _Longitude)
                {
                    _Longitude = value;
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
                if (_PhoneticVariations != null)
                {
                    foreach (GedcomVariation variation in _PhoneticVariations)
                    {
                        childChangeDate = variation.ChangeDate;
                        if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                        {
                            realChangeDate = childChangeDate;
                        }
                    }
                }

                if (_RomanizedVariations != null)
                {
                    foreach (GedcomVariation variation in _RomanizedVariations)
                    {
                        childChangeDate = variation.ChangeDate;
                        if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                        {
                            realChangeDate = childChangeDate;
                        }
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
            sw.Write(" PLAC ");

            if (!string.IsNullOrEmpty(Name))
            {
                string line = Name.Replace("@", "@@");
                sw.Write(line);
            }

            OutputStandard(sw);

            string levelPlusOne = null;
            string levelPlusTwo = null;

            if (!string.IsNullOrEmpty(Form))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                string line = Form.Replace("@", "@@");
                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" FORM ");
                sw.Write(line);
            }

            if (_PhoneticVariations != null)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                if (levelPlusTwo == null)
                {
                    levelPlusTwo = Util.IntToString(Level + 2);
                }

                foreach (GedcomVariation variation in PhoneticVariations)
                {
                    sw.Write(Environment.NewLine);
                    sw.Write(levelPlusOne);
                    sw.Write(" FONE ");
                    string line = variation.Value.Replace("@", "@@");
                    sw.Write(line);
                    if (!string.IsNullOrEmpty(variation.VariationType))
                    {
                        sw.Write(Environment.NewLine);
                        sw.Write(levelPlusTwo);
                        sw.Write(" TYPE ");
                        line = variation.VariationType.Replace("@", "@@");
                        sw.Write(line);
                    }
                }
            }

            if (_RomanizedVariations != null)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                if (levelPlusTwo == null)
                {
                    levelPlusTwo = Util.IntToString(Level + 2);
                }

                foreach (GedcomVariation variation in RomanizedVariations)
                {
                    sw.Write(Environment.NewLine);
                    sw.Write(levelPlusOne);
                    sw.Write(" ROMN ");
                    string line = variation.Value.Replace("@", "@@");
                    sw.Write(line);
                    if (!string.IsNullOrEmpty(variation.VariationType))
                    {
                        sw.Write(Environment.NewLine);
                        sw.Write(levelPlusTwo);
                        sw.Write(" TYPE ");
                        line = variation.VariationType.Replace("@", "@@");
                        sw.Write(variation.VariationType);
                    }
                }
            }

            if (!string.IsNullOrEmpty(Latitude) || !string.IsNullOrEmpty(Longitude))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" MAP");
                if (!string.IsNullOrEmpty(Latitude))
                {
                    if (levelPlusTwo == null)
                    {
                        levelPlusTwo = Util.IntToString(Level + 2);
                    }

                    sw.Write(Environment.NewLine);
                    sw.Write(levelPlusTwo);
                    sw.Write(" LATI ");
                    string line = Latitude.Replace("@", "@@");
                    sw.Write(line);
                }

                if (!string.IsNullOrEmpty(Longitude))
                {
                    if (levelPlusTwo == null)
                    {
                        levelPlusTwo = Util.IntToString(Level + 2);
                    }

                    sw.Write(Environment.NewLine);
                    sw.Write(levelPlusTwo);
                    sw.Write(" LONG ");
                    string line = Longitude.Replace("@", "@@");
                    sw.Write(line);
                }
            }
        }
    }
}
