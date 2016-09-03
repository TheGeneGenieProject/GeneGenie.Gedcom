/*
 *  $Id: GedcomName.cs 201 2008-12-01 20:00:26Z davek $
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
    using System.Text;

    /// <summary>
    /// A name for a given individual, allowing different variations to be
    /// stored.
    /// </summary>
    public class GedcomName : GedcomRecord
    {
        private string _Type;

        // name pieces
        private string _prefix;
        private string _given; // not same as firstname, includes middle etc.
        private string _surnamePrefix;
        // already got surname
        private string _suffix;
        private string _nick;

        private GedcomRecordList<GedcomVariation> _PhoneticVariations;
        private GedcomRecordList<GedcomVariation> _RomanizedVariations;

        private bool _preferedName;

        // cached surname / firstname split, this is expensive
        // when trying to filter a list of individuals, so do it
        // upon setting the name
        private string _Surname;

        private string _SurnameSoundex;
        private string _FirstnameSoundex;

        private StringBuilder _builtName;

        public GedcomName()
        {
            _prefix = string.Empty;
            _given = string.Empty;
            _surnamePrefix = string.Empty;
            _suffix = string.Empty;
            _nick = string.Empty;
            _Surname = string.Empty;
            _SurnameSoundex = string.Empty;
            _FirstnameSoundex = string.Empty;
        }

        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Name; }
        }

        public override string GedcomTag
        {
            get { return "NAME"; }
        }

        private bool IsSet
        {
            get
            {
                return (!string.IsNullOrEmpty(_prefix)) ||
                        (!string.IsNullOrEmpty(_given)) ||
                        (!string.IsNullOrEmpty(_surnamePrefix)) ||
                        (!string.IsNullOrEmpty(_Surname)) ||
                        (!string.IsNullOrEmpty(_suffix));
            }
        }

        public string Name
        {
            get
            {
                if (_builtName == null)
                {
                    _builtName = BuildName();
                }

                return _builtName.ToString();
            }
            set
            {
                // check to see if a name has been set before
                // checking if it has changed, otherwise
                // we end up building a name string up twice
                // while loading.  IsSet is true if any name
                // parts are non null
                if ((!IsSet && !string.IsNullOrEmpty(value)) || (value != Name))
                {
                    string name = value;

                    name = name.Trim();

                    int i = name.IndexOf("/");
                    if (i == -1)
                    {
                        i = name.LastIndexOf(" ");
                        if (i != -1)
                        {
                            Surname = _database.NameCollection[name, i + 1, name.Length - i - 1];
                        }
                        else
                        {
                            Surname = string.Empty;
                        }
                    }
                    else
                    {
                        int j = name.IndexOf("/", i + 1);
                        if (j == -1)
                        {
                            Surname = _database.NameCollection[name, i + 1, name.Length - i - 1];
                        }
                        else
                        {
                            Surname = _database.NameCollection[name, i + 1, j - i - 1];
                        }
                    }

                    if (i != -1)
                    {
                        // given is everything up to the surname, not right
                        // but will do for now
                        Given = _database.NameCollection[name, 0, i];

                        // prefix is foo.  e.g.  Prof.  Dr.  Lt. Cmd.
                        // strip it from the given name
                        // prefix must be > 2 chars so we avoid initials
                        // being treated as prefixes
                        int l = _given.IndexOf(".");
                        int n = _given.IndexOf(" ");
                        if (l > 2)
                        {
                            if (n != -1 && l < n)
                            {
                                int o = l;
                                int p = n;

                                do
                                {
                                    l = o;
                                    n = p;

                                    o = _given.IndexOf(".", o + 1);
                                    p = _given.IndexOf(" ", p + 1);
                                }
                                while (o != -1 && (p != -1 && o < p));

                                Prefix = _database.NameCollection[_given, 0, l + 1];
                                Given = _database.NameCollection[_given, l + 1, _given.Length - l - 1];
                            }
                        }

                        // get surname prefix, everything before the last space
                        // is part of the surname prefix
                        int m = _Surname.LastIndexOf(" ");
                        if (m != -1)
                        {
                            SurnamePrefix = _database.NameCollection[_Surname, 0, m];
                            Surname = _database.NameCollection[_Surname, m + 1, _Surname.Length - m - 1];
                        }
                        else
                        {
                            SurnamePrefix = string.Empty;
                        }

                        // FIXME: anything after surname is suffix, again not right
                        // but works for now
                        int offset = i + 1 + _Surname.Length + 1;
                        if (!string.IsNullOrEmpty(_surnamePrefix))
                        {
                            offset += _surnamePrefix.Length + 1;
                        }

                        if (offset < name.Length)
                        {
                            Suffix = _database.NameCollection[name, offset, name.Length - offset];
                        }
                        else
                        {
                            Suffix = string.Empty;
                        }
                    }
                }
            }
        }

        public string Type
        {
            get
            {
                return _Type;
            }

            set
            {
                if (value != _Type)
                {
                    _Type = value;
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

        public string Surname
        {
            get
            {
                return _Surname;
            }

            set
            {
                if (_Surname != value)
                {
                    _Surname = value;
                    _SurnameSoundex = Util.GenerateSoundex(_Surname);
                    _builtName = null;
                    Changed();
                }
            }
        }

        public string SurnameSoundex
        {
            get { return _SurnameSoundex; }
        }

        public string FirstnameSoundex
        {
            get { return _FirstnameSoundex; }
        }

        public string Prefix
        {
            get
            {
                return _prefix;
            }

            set
            {
                if (_prefix != value)
                {
                    _prefix = value;
                    _builtName = null;
                    Changed();
                }
            }
        }

        public string Given
        {
            get
            {
                return _given;
            }

            set
            {
                if (_given != value)
                {
                    _given = value;
                    _FirstnameSoundex = Util.GenerateSoundex(_given);
                    _builtName = null;
                    Changed();
                }
            }
        }

        public string SurnamePrefix
        {
            get
            {
                return _surnamePrefix;
            }

            set
            {
                if (_surnamePrefix != value)
                {
                    _surnamePrefix = value;
                    _builtName = null;
                    Changed();
                }
            }
        }

        public string Suffix
        {
            get
            {
                return _suffix;
            }

            set
            {
                if (_suffix != value)
                {
                    _suffix = value;
                    _builtName = null;
                    Changed();
                }
            }
        }

        public string Nick
        {
            get
            {
                return _nick;
            }

            set
            {
                if (value != _nick)
                {
                    _nick = value;
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

        public bool PreferedName
        {
            get { return _preferedName; }
            set { _preferedName = value; }
        }

        public float IsMatch(GedcomName name)
        {
            float match = 0F;

            int parts = 0;

            // FIXME: perform soundex check as well?
            // how would that effect returning a % match?
            float matches = 0;

            bool surnameMatched = false;

            if (!(string.IsNullOrEmpty(name.Prefix) && string.IsNullOrEmpty(_prefix)))
            {
                parts++;
                if (name.Prefix == _prefix)
                {
                    matches++;
                }
            }

            if (!(string.IsNullOrEmpty(name.Given) && string.IsNullOrEmpty(_given)))
            {
                parts++;
                if (name.Given == _given)
                {
                    matches++;
                }
            }

            if (!(string.IsNullOrEmpty(name.Surname) && string.IsNullOrEmpty(_Surname)))
            {
                if ((name.Surname == "?" && _Surname == "?") ||
                    ((string.Compare(name.Surname, "unknown", true) == 0) &&
                     (string.Compare(_Surname, "unknown", true) == 0)))
                {
                    // not really matched, surname isn't known,
                    // don't count as part being checked, and don't penalize
                    surnameMatched = true;
                }
                else
                {
                    parts++;
                    if (name.Surname == _Surname)
                    {
                        matches++;
                        surnameMatched = true;
                    }
                }
            }
            else
            {
                // pretend the surname matches
                surnameMatched = true;
            }

            if (!(string.IsNullOrEmpty(name.SurnamePrefix) && string.IsNullOrEmpty(_surnamePrefix)))
            {
                parts++;
                if (name.SurnamePrefix == _surnamePrefix)
                {
                    matches++;
                }
            }

            if (!(string.IsNullOrEmpty(name.Suffix) && string.IsNullOrEmpty(_suffix)))
            {
                parts++;
                if (name.Suffix == _suffix)
                {
                    matches++;
                }
            }

            if (!(string.IsNullOrEmpty(name.Nick) && string.IsNullOrEmpty(_nick)))
            {
                parts++;
                if (name.Nick == _nick)
                {
                    matches++;
                }
            }

            match = (matches / parts) * 100.0F;

            // FIXME: heavily penalise the surname not matching
            // for this to work correctly better matching needs to be
            // performed, not just string comparison
            if (!surnameMatched)
            {
                match *= 0.25F;
            }

            return match;
        }

        private StringBuilder BuildName()
        {
            int capacity = 0;
            if (!string.IsNullOrEmpty(_prefix))
            {
                capacity += _prefix.Length;
            }

            if (!string.IsNullOrEmpty(_given))
            {
                capacity += _given.Length;
            }

            if (!string.IsNullOrEmpty(_surnamePrefix))
            {
                capacity += _surnamePrefix.Length;
            }

            if (!string.IsNullOrEmpty(_Surname))
            {
                capacity += _Surname.Length;
            }

            if (!string.IsNullOrEmpty(_suffix))
            {
                capacity += _suffix.Length;
            }

            // for the // surrounding surname + potential spaces
            capacity += 4;

            StringBuilder name = new StringBuilder(capacity);

            if (!string.IsNullOrEmpty(_prefix))
            {
                name.Append(_prefix);
            }

            if (!string.IsNullOrEmpty(_given))
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append(_given);
            }

            // ALWAYS output a surname, even if it is empty
            if (name.Length != 0)
            {
                name.Append(" ");
            }

            name.Append("/");
            if (!string.IsNullOrEmpty(_surnamePrefix))
            {
                name.Append(_surnamePrefix);
                name.Append(" ");
            }

            if (!string.IsNullOrEmpty(_Surname))
            {
                name.Append(_Surname);
            }

            name.Append("/");

            if (!string.IsNullOrEmpty(_suffix))
            {
                // some data in test set has ,foobar on the end,
                // in this instance don't append a space.
                if (!_suffix.StartsWith(","))
                {
                    if (name.Length != 0)
                    {
                        name.Append(" ");
                    }
                }

                name.Append(_suffix);
            }

            return name;
        }

        public override void Output(TextWriter sw)
        {
            // FIXME: should output name parts?  not well supported by other
            // apps?
            sw.Write(Environment.NewLine);
            sw.Write(Util.IntToString(Level));
            sw.Write(" NAME ");
            sw.Write(Name);

            string levelPlusOne = null;
            string levelPlusTwo = null;

            if (!string.IsNullOrEmpty(Type))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = Util.IntToString(Level + 1);
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" TYPE ");
                string line = Type.Replace("@", "@@");
                sw.Write(line);
            }

            OutputStandard(sw);

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
                    sw.Write(levelPlusOne);
                    sw.Write(" FONE ");
                    string line = variation.Value.Replace("@", "@@");
                    sw.Write(line);
                    sw.Write(Environment.NewLine);
                    if (!string.IsNullOrEmpty(variation.VariationType))
                    {
                        sw.Write(levelPlusTwo);
                        sw.Write(" TYPE ");
                        line = variation.VariationType.Replace("@", "@@");
                        sw.Write(line);
                        sw.Write(Environment.NewLine);
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
                    sw.Write(levelPlusOne);
                    sw.Write(" ROMN ");
                    string line = variation.Value.Replace("@", "@@");
                    sw.Write(variation.Value);
                    sw.Write(Environment.NewLine);
                    if (!string.IsNullOrEmpty(variation.VariationType))
                    {
                        sw.Write(levelPlusTwo);
                        sw.Write(" TYPE ");
                        line = variation.VariationType.Replace("@", "@@");
                        sw.Write(line);
                        sw.Write(Environment.NewLine);
                    }
                }
            }
        }
    }
}
