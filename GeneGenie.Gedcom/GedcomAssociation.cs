// <copyright file="GedcomAssociation.cs" company="GeneGenie.com">
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
    using Enums;

    /// <summary>
    /// How the given individual is associated to another.
    /// Each GedcomIndividal contains a list of these.
    /// </summary>
    public class GedcomAssociation : GedcomRecord, IComparable, IComparable<GedcomAssociation>, IEquatable<GedcomAssociation>
    {
        private string description;

        private string individual;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomAssociation"/> class.
        /// </summary>
        public GedcomAssociation()
        {
        }

        /// <summary>
        /// Gets the record type for an association.
        /// </summary>
        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Association; }
        }

        /// <summary>
        /// Gets the GEDCOM tag for an association.
        /// </summary>
        public override string GedcomTag
        {
            get { return "ASSO"; }
        }

        /// <summary>
        /// Gets or sets the description for this association.
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                if (value != description)
                {
                    description = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the individual for this association.
        /// </summary>
        public string Individual
        {
            get
            {
                return individual;
            }

            set
            {
                if (value != individual)
                {
                    individual = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Outputs a GEDCOM format version of this instance.
        /// </summary>
        /// <param name="tw">The writer to output to.</param>
        public override void Output(TextWriter tw)
        {
            tw.Write(Environment.NewLine);
            tw.Write(Level.ToString());
            tw.Write(" ASSO ");
            tw.Write("@");
            tw.Write(Individual);
            tw.Write("@");

            string levelPlusOne = (Level + 1).ToString();

            tw.Write(Environment.NewLine);
            tw.Write(levelPlusOne);
            tw.Write(" RELA ");

            string line = Description.Replace("@", "@@");
            if (line.Length > 25)
            {
                Util.SplitText(tw, line, Level + 1, 25, 1, true);
            }
            else
            {
                tw.Write(line);
            }

            OutputStandard(tw);
        }

        /// <summary>
        /// Compare the user-entered data against the passed instance for similarity.
        /// </summary>
        /// <param name="obj">The object to compare this instance against.</param>
        /// <returns>
        /// True if instance matches user data, otherwise false.
        /// </returns>
        public override bool IsEquivalentTo(object obj)
        {
            return CompareTo(obj as GedcomAssociation) == 0;
        }

        /// <summary>
        /// Compares the current and passed-in object to see if they are the same.
        /// </summary>
        /// <param name="obj">The object to compare the current instance against.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the value parameter.
        /// </returns>
        public int CompareTo(object obj)
        {
            return CompareTo(obj as GedcomAssociation);
        }

        /// <summary>
        /// Compares the current and passed-in association to see if they are the same.
        /// </summary>
        /// <param name="other">The association to compare the current instance against.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the value parameter.
        /// </returns>
        public int CompareTo(GedcomAssociation other)
        {
            if (other == null)
            {
                return 1;
            }

            var compare = string.Compare(Description, other.Description);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(Individual, other.Individual);
            if (compare != 0)
            {
                return compare;
            }

            return compare;
        }

        /// <summary>
        /// Compares the current and passed-in association to see if they are the same.
        /// </summary>
        /// <param name="other">The association to compare the current instance against.</param>
        /// <returns>
        /// True if they match, False otherwise.
        /// </returns>
        public bool Equals(GedcomAssociation other)
        {
            return CompareTo(other) == 0;
        }

        /// <summary>
        /// Compares the current and passed-in object to see if they are the same.
        /// </summary>
        /// <param name="obj">The object to compare the current instance against.</param>
        /// <returns>True if they match, False otherwise.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as GedcomAssociation);
        }

        public override int GetHashCode()
        {
            return new
            {
                Description,
                Individual,
            }.GetHashCode();
        }
    }
}
