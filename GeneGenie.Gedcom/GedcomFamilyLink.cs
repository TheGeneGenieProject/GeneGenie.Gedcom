// <copyright file="GedcomFamilyLink.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System;
    using GeneGenie.Gedcom.Enums;

    /// <summary>
    /// How an individal is linked to a family
    /// </summary>
    public class GedcomFamilyLink : GedcomRecord, IComparable<GedcomFamilyLink>, IComparable, IEquatable<GedcomFamilyLink>
    {
        private string family;
        private string indi;

        private PedigreeLinkageType pedigree;
        private ChildLinkageStatus status;

        private PedigreeLinkageType fatherPedigree;
        private PedigreeLinkageType motherPedigree;

        private bool preferedSpouse;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomFamilyLink"/> class.
        /// </summary>
        public GedcomFamilyLink()
        {
            pedigree = PedigreeLinkageType.Unknown;
        }

        /// <summary>
        /// Gets the type of the record.
        /// </summary>
        /// <value>
        /// The type of the record.
        /// </value>
        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.FamilyLink; }
        }

        /// <summary>
        /// Gets or sets the family.
        /// </summary>
        /// <value>
        /// The family.
        /// </value>
        public string Family
        {
            get
            {
                return family;
            }

            set
            {
                if (value != family)
                {
                    family = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the individual being linked in this family record.
        /// </summary>
        public string Individual
        {
            get
            {
                return indi;
            }

            set
            {
                if (value != indi)
                {
                    indi = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the pedigree.
        /// </summary>
        /// <value>
        /// The pedigree.
        /// </value>
        public PedigreeLinkageType Pedigree
        {
            get
            {
                return pedigree;
            }

            set
            {
                if (value != pedigree)
                {
                    pedigree = value;
                    FatherPedigree = value;
                    MotherPedigree = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the father pedigree.
        /// </summary>
        /// <value>
        /// The father pedigree.
        /// </value>
        public PedigreeLinkageType FatherPedigree
        {
            get
            {
                return fatherPedigree;
            }

            set
            {
                if (value != fatherPedigree)
                {
                    fatherPedigree = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the mother pedigree.
        /// </summary>
        /// <value>
        /// The mother pedigree.
        /// </value>
        public PedigreeLinkageType MotherPedigree
        {
            get
            {
                return motherPedigree;
            }

            set
            {
                if (value != motherPedigree)
                {
                    motherPedigree = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public ChildLinkageStatus Status
        {
            get
            {
                return status;
            }

            set
            {
                if (value != status)
                {
                    status = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [prefered spouse].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [prefered spouse]; otherwise, <c>false</c>.
        /// </value>
        public bool PreferedSpouse
        {
            get { return preferedSpouse; }
            set { preferedSpouse = value; }
        }

        /// <summary>
        /// Compares the current and passed family link to see if they are the same.
        /// </summary>
        /// <param name="obj">The object to compare the current instance against.</param>
        /// <returns>True if they match, False otherwise.</returns>
        public override bool IsEquivalentTo(object obj)
        {
            return CompareTo(obj as GedcomFamilyLink) == 0;
        }

        /// <summary>
        /// Compares the current and passed family link to see if they are the same.
        /// </summary>
        /// <param name="link">The family link to compare the current instance against.</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the value parameter.</returns>
        public int CompareTo(GedcomFamilyLink link)
        {
            /* Family and Individual appear to store XRefId values,
             * which don't seem to contribute to the equality of a family link.
             */

            if (link == null)
            {
                return 1;
            }

            var compare = FatherPedigree.CompareTo(link.FatherPedigree);
            if (compare != 0)
            {
                return compare;
            }

            compare = MotherPedigree.CompareTo(link.MotherPedigree);
            if (compare != 0)
            {
                return compare;
            }

            compare = Pedigree.CompareTo(link.Pedigree);
            if (compare != 0)
            {
                return compare;
            }

            compare = PreferedSpouse.CompareTo(link.PreferedSpouse);
            if (compare != 0)
            {
                return compare;
            }

            compare = Status.CompareTo(link.Status);
            if (compare != 0)
            {
                return compare;
            }

            return compare;
        }

        /// <summary>
        /// Compares the current and passed family link to see if they are the same.
        /// </summary>
        /// <param name="obj">The object to compare the current instance against.</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the value parameter.</returns>
        public int CompareTo(object obj)
        {
            return CompareTo(obj as GedcomFamilyLink);
        }

        /// <summary>
        /// Compares the current and passed family link to see if they are the same.
        /// </summary>
        /// <param name="other">The GedcomFamilyLink to compare the current instance against.</param>
        /// <returns>True if they match, False otherwise.</returns>
        public bool Equals(GedcomFamilyLink other)
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
            return Equals(obj as GedcomFamilyLink);
        }

        public override int GetHashCode()
        {
            return new
            {
                FatherPedigree,
                MotherPedigree,
                Pedigree,
                PreferedSpouse,
                Status,
            }.GetHashCode();
        }
    }
}
