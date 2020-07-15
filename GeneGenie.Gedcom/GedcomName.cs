// <copyright file="GedcomName.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System;
    using System.IO;
    using System.Text;
    using GeneGenie.Gedcom.Enums;

    /// <summary>
    /// A name for a given individual, allowing different variations to be
    /// stored.
    /// </summary>
    public class GedcomName : GedcomRecord, IComparable<GedcomName>, IComparable, IEquatable<GedcomName>
    {
        private string type;

        // name pieces
        private string prefix;
        private string given; // not same as firstname, includes middle etc.
        private string surnamePrefix;

        // already got surname
        private string suffix;
        private string nick;

        private GedcomRecordList<GedcomVariation> phoneticVariations;
        private GedcomRecordList<GedcomVariation> romanizedVariations;

        // cached surname / firstname split, this is expensive
        // when trying to filter a list of individuals, so do it
        // upon setting the name
        private string surname;

        private string surnameSoundex;
        private string firstnameSoundex;

        private StringBuilder builtName;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomName"/> class.
        /// </summary>
        public GedcomName()
        {
            prefix = string.Empty;
            given = string.Empty;
            surnamePrefix = string.Empty;
            suffix = string.Empty;
            nick = string.Empty;
            surname = string.Empty;
            surnameSoundex = string.Empty;
            firstnameSoundex = string.Empty;
        }

        /// <summary>
        /// Gets the type of the record.
        /// </summary>
        /// <value>
        /// The type of the record.
        /// </value>
        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Name; }
        }

        /// <summary>
        /// Gets the GEDCOM tag.
        /// </summary>
        /// <value>
        /// The GEDCOM tag.
        /// </value>
        public override string GedcomTag
        {
            get { return "NAME"; }
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
                if (builtName == null)
                {
                    builtName = BuildName();
                }

                return builtName.ToString();
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

                    name = name is null ? string.Empty : name.Trim();

                    int surnameStartPos = name.IndexOf("/");
                    int surnameLength = 0;
                    if (surnameStartPos == -1)
                    {
                        surnameStartPos = name.LastIndexOf(" ");
                        if (surnameStartPos != -1)
                        {
                            surnameLength = name.Length - surnameStartPos - 1;
                            Surname = Database.NameCollection[name, surnameStartPos + 1, surnameLength];
                        }
                        else
                        {
                            Surname = string.Empty;
                            surnameStartPos = name.Length; // No surname, must just be a given name only.
                        }
                    }
                    else
                    {
                        int surnameEndPos = name.IndexOf("/", surnameStartPos + 1);
                        if (surnameEndPos == -1)
                        {
                            surnameLength = name.Length - surnameStartPos - 1;
                            Surname = Database.NameCollection[name, surnameStartPos + 1, surnameLength];
                        }
                        else
                        {
                            surnameLength = surnameEndPos - surnameStartPos - 1;
                            Surname = Database.NameCollection[name, surnameStartPos + 1, surnameLength];
                        }
                    }

                    if (surnameStartPos != -1)
                    {
                        // given is everything up to the surname, not right
                        // but will do for now
                        Given = Database.NameCollection[name, 0, surnameStartPos];

                        // prefix is foo.  e.g.  Prof.  Dr.  Lt. Cmd.
                        // strip it from the given name
                        // prefix must be > 2 chars so we avoid initials
                        // being treated as prefixes
                        int l = given.IndexOf(".");
                        int n = given.IndexOf(" ");
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

                                    o = given.IndexOf(".", o + 1);
                                    p = given.IndexOf(" ", p + 1);
                                }
                                while (o != -1 && (p != -1 && o < p));

                                Prefix = Database.NameCollection[given, 0, l + 1];
                                Given = Database.NameCollection[given, l + 1, given.Length - l - 1];
                            }
                        }

                        // get surname prefix, everything before the last space
                        // is part of the surname prefix
                        int m = surname.LastIndexOf(" ");
                        if (m != -1)
                        {
                            SurnamePrefix = Database.NameCollection[surname, 0, m];
                            Surname = Database.NameCollection[surname, m + 1, surname.Length - m - 1];
                        }
                        else
                        {
                            SurnamePrefix = string.Empty;
                        }

                        // TODO: anything after surname is suffix, again not right
                        // but works for now
                        int offset = surnameStartPos + 1 + surnameLength + 1;
                        if (!string.IsNullOrEmpty(surnamePrefix))
                        {
                            offset += surnamePrefix.Length + 1;
                        }

                        if (offset < name.Length)
                        {
                            Suffix = Database.NameCollection[name, offset, name.Length - offset];
                        }
                        else
                        {
                            Suffix = string.Empty;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type
        {
            get
            {
                return type;
            }

            set
            {
                if (value != type)
                {
                    type = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets the phonetic variations.
        /// </summary>
        /// <value>
        /// The phonetic variations.
        /// </value>
        public GedcomRecordList<GedcomVariation> PhoneticVariations
        {
            get
            {
                if (phoneticVariations == null)
                {
                    phoneticVariations = new GedcomRecordList<GedcomVariation>();
                    phoneticVariations.CollectionChanged += ListChanged;
                }

                return phoneticVariations;
            }
        }

        /// <summary>
        /// Gets the romanized variations.
        /// </summary>
        /// <value>
        /// The romanized variations.
        /// </value>
        public GedcomRecordList<GedcomVariation> RomanizedVariations
        {
            get
            {
                if (romanizedVariations == null)
                {
                    romanizedVariations = new GedcomRecordList<GedcomVariation>();
                    romanizedVariations.CollectionChanged += ListChanged;
                }

                return romanizedVariations;
            }
        }

        /// <summary>
        /// Gets or sets the surname.
        /// </summary>
        /// <value>
        /// The surname.
        /// </value>
        public string Surname
        {
            get
            {
                return surname;
            }

            set
            {
                if (surname != value)
                {
                    surname = value;
                    surnameSoundex = Util.GenerateSoundex(surname);
                    builtName = null;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets the surname soundex.
        /// </summary>
        /// <value>
        /// The surname soundex.
        /// </value>
        public string SurnameSoundex
        {
            get { return surnameSoundex; }
        }

        /// <summary>
        /// Gets the firstname soundex.
        /// </summary>
        /// <value>
        /// The firstname soundex.
        /// </value>
        public string FirstnameSoundex
        {
            get { return firstnameSoundex; }
        }

        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        /// <value>
        /// The prefix.
        /// </value>
        public string Prefix
        {
            get
            {
                return prefix;
            }

            set
            {
                if (prefix != value)
                {
                    prefix = value;
                    builtName = null;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the given.
        /// </summary>
        /// <value>
        /// The given.
        /// </value>
        public string Given
        {
            get
            {
                return given;
            }

            set
            {
                if (given != value)
                {
                    given = value;
                    firstnameSoundex = Util.GenerateSoundex(given);
                    builtName = null;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the surname prefix.
        /// </summary>
        /// <value>
        /// The surname prefix.
        /// </value>
        public string SurnamePrefix
        {
            get
            {
                return surnamePrefix;
            }

            set
            {
                if (surnamePrefix != value)
                {
                    surnamePrefix = value;
                    builtName = null;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the suffix.
        /// </summary>
        /// <value>
        /// The suffix.
        /// </value>
        public string Suffix
        {
            get
            {
                return suffix;
            }

            set
            {
                if (suffix != value)
                {
                    suffix = value;
                    builtName = null;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the nick.
        /// </summary>
        /// <value>
        /// The nick.
        /// </value>
        public string Nick
        {
            get
            {
                return nick;
            }

            set
            {
                if (value != nick)
                {
                    nick = value;
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
                if (phoneticVariations != null)
                {
                    foreach (GedcomVariation variation in phoneticVariations)
                    {
                        childChangeDate = variation.ChangeDate;
                        if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                        {
                            realChangeDate = childChangeDate;
                        }
                    }
                }

                if (romanizedVariations != null)
                {
                    foreach (GedcomVariation variation in romanizedVariations)
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

        /// <summary>
        /// Gets or sets a value indicating whether this is the individuals preferred name.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [preferred name]; otherwise, <c>false</c>.
        /// </value>
        public bool PreferredName { get; set; }

        private bool IsSet
        {
            get
            {
                return (!string.IsNullOrEmpty(prefix)) ||
                        (!string.IsNullOrEmpty(given)) ||
                        (!string.IsNullOrEmpty(nick)) ||
                        (!string.IsNullOrEmpty(surnamePrefix)) ||
                        (!string.IsNullOrEmpty(surname)) ||
                        (!string.IsNullOrEmpty(suffix));
            }
        }

        /// <summary>
        /// Compares two GedcomName instances by using the full name.
        /// </summary>
        /// <param name="other">The name to compare against this instance.</param>
        /// <returns>An integer specifying the relative sort order.</returns>
        public int CompareTo(GedcomName other)
        {
            if (other == null)
            {
                return 1;
            }

            var compare = string.Compare(Type, other.Type);
            if (compare != 0)
            {
                return compare;
            }

            compare = GedcomGenericListComparer.CompareListOrder(PhoneticVariations, other.PhoneticVariations);
            if (compare != 0)
            {
                return compare;
            }

            compare = GedcomGenericListComparer.CompareListOrder(RomanizedVariations, other.RomanizedVariations);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Surname, other.Surname);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Prefix, other.Prefix);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Given, other.Given);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(SurnamePrefix, other.SurnamePrefix);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Suffix, other.Suffix);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Nick, other.Nick);
            if (compare != 0)
            {
                return compare;
            }

            compare = PreferredName.CompareTo(other.PreferredName);
            if (compare != 0)
            {
                return compare;
            }

            return compare;
        }

        /// <summary>
        /// Compares two GedcomName instances by using the full name.
        /// </summary>
        /// <param name="obj">The name to compare against this instance.</param>
        /// <returns>An integer specifying the relative sort order.</returns>
        public int CompareTo(object obj)
        {
            return CompareTo(obj as GedcomName);
        }

        /// <summary>
        /// Compare the user-entered data against the passed instance for similarity.
        /// </summary>
        /// <param name="other">The GedcomName to compare this instance against.</param>
        /// <returns>
        /// True if instance matches user data, otherwise False.
        /// </returns>
        public bool Equals(GedcomName other)
        {
            return CompareTo(other) == 0;
        }

        /// <summary>
        /// Compare the user-entered data against the passed instance for similarity.
        /// </summary>
        /// <param name="obj">The object to compare this instance against.</param>
        /// <returns>
        /// True if instance matches user data, otherwise False.
        /// </returns>
        public override bool IsEquivalentTo(object obj)
        {
            return CompareTo(obj as GedcomName) == 0;
        }

        /// <summary>
        /// Returns a percentage based score on how similar the passed record is to the current instance.
        /// </summary>
        /// <param name="name">The event to compare against this instance.</param>
        /// <returns>A score from 0 to 100 representing the percentage match.</returns>
        public decimal CalculateSimilarityScore(GedcomName name)
        {
            var match = decimal.Zero;

            int parts = 0;

            // TODO: perform soundex check as well?
            // how would that effect returning a % match?
            var matches = decimal.Zero;

            bool surnameMatched = false;

            if (!(string.IsNullOrEmpty(name.Prefix) && string.IsNullOrEmpty(prefix)))
            {
                parts++;
                if (name.Prefix == prefix)
                {
                    matches++;
                }
            }

            if (!(string.IsNullOrEmpty(name.Given) && string.IsNullOrEmpty(given)))
            {
                parts++;
                if (name.Given == given)
                {
                    matches++;
                }
            }

            if (!(string.IsNullOrEmpty(name.Surname) && string.IsNullOrEmpty(surname)))
            {
                if ((name.Surname == "?" && surname == "?") ||
                    ((string.Compare(name.Surname, "unknown", true) == 0) &&
                     (string.Compare(surname, "unknown", true) == 0)))
                {
                    // not really matched, surname isn't known,
                    // don't count as part being checked, and don't penalize
                    surnameMatched = true;
                }
                else
                {
                    parts++;
                    if (name.Surname == surname)
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

            if (!(string.IsNullOrEmpty(name.SurnamePrefix) && string.IsNullOrEmpty(surnamePrefix)))
            {
                parts++;
                if (name.SurnamePrefix == surnamePrefix)
                {
                    matches++;
                }
            }

            if (!(string.IsNullOrEmpty(name.Suffix) && string.IsNullOrEmpty(suffix)))
            {
                parts++;
                if (name.Suffix == suffix)
                {
                    matches++;
                }
            }

            if (!(string.IsNullOrEmpty(name.Nick) && string.IsNullOrEmpty(nick)))
            {
                parts++;
                if (name.Nick == nick)
                {
                    matches++;
                }
            }

            match = (matches / parts) * 100m;

            // TODO: heavily penalise the surname not matching
            // for this to work correctly better matching needs to be
            // performed, not just string comparison
            if (!surnameMatched)
            {
                match *= 0.25m;
            }

            return match;
        }

        /// <summary>
        /// Outputs this instance as a GEDCOM record.
        /// </summary>
        /// <param name="sw">The writer to output to.</param>
        public override void Output(TextWriter sw)
        {
            sw.Write(Environment.NewLine);
            sw.Write(Level.ToString());
            sw.Write(" NAME ");
            sw.Write(Name);

            string levelPlusOne = null;
            string levelPlusTwo = null;

            if (!string.IsNullOrEmpty(Type))
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (Level + 1).ToString();
                }

                sw.Write(Environment.NewLine);
                sw.Write(levelPlusOne);
                sw.Write(" TYPE ");
                string line = Type.Replace("@", "@@");
                sw.Write(line);
            }

            // Gedcom 5.5.5 spec says to always output these fields, even if blank.
            OutputNamePart(sw, "NPFX", Prefix, Level + 1);
            OutputNamePart(sw, "GIVN", Given, Level + 1);
            OutputNamePart(sw, "NICK", Nick, Level + 1);
            OutputNamePart(sw, "SPFX", SurnamePrefix, Level + 1);
            OutputNamePart(sw, "SURN", Surname, Level + 1);
            OutputNamePart(sw, "NSFX", Suffix, Level + 1);

            OutputStandard(sw);

            if (phoneticVariations != null)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (Level + 1).ToString();
                }

                if (levelPlusTwo == null)
                {
                    levelPlusTwo = (Level + 2).ToString();
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

            if (romanizedVariations != null)
            {
                if (levelPlusOne == null)
                {
                    levelPlusOne = (Level + 1).ToString();
                }

                if (levelPlusTwo == null)
                {
                    levelPlusTwo = (Level + 2).ToString();
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

        private void OutputNamePart(TextWriter sw, string tagName, string tagValue, int level)
        {
            sw.Write(Environment.NewLine);
            sw.Write(level.ToString());
            sw.Write(" " + tagName + " ");
            var line = tagValue.Replace("@", "@@");
            sw.Write(line);
        }

        private StringBuilder BuildName()
        {
            int capacity = 0;
            if (!string.IsNullOrEmpty(prefix))
            {
                capacity += prefix.Length;
            }

            if (!string.IsNullOrEmpty(given))
            {
                capacity += given.Length;
            }

            if (!string.IsNullOrEmpty(surnamePrefix))
            {
                capacity += surnamePrefix.Length;
            }

            if (!string.IsNullOrEmpty(surname))
            {
                capacity += surname.Length;
            }

            if (!string.IsNullOrEmpty(suffix))
            {
                capacity += suffix.Length;
            }

            // for the // surrounding surname + potential spaces
            capacity += 4;

            StringBuilder name = new StringBuilder(capacity);

            if (!string.IsNullOrEmpty(prefix))
            {
                name.Append(prefix);
            }

            if (!string.IsNullOrEmpty(given))
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append(given);
            }

            // ALWAYS output a surname, even if it is empty
            if (name.Length != 0)
            {
                name.Append(" ");
            }

            name.Append("/");
            if (!string.IsNullOrEmpty(surnamePrefix))
            {
                name.Append(surnamePrefix);
                name.Append(" ");
            }

            if (!string.IsNullOrEmpty(surname))
            {
                name.Append(surname);
            }

            name.Append("/");

            if (!string.IsNullOrEmpty(suffix))
            {
                // some data in test set has ,foobar on the end,
                // in this instance don't append a space.
                if (!suffix.StartsWith(","))
                {
                    if (name.Length != 0)
                    {
                        name.Append(" ");
                    }
                }

                name.Append(suffix);
            }

            return name;
        }
    }
}
