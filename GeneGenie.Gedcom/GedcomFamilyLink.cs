// <copyright file="GedcomFamilyLink.cs" company="GeneGenie.com">
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
    using GeneGenie.Gedcom.Enums;

    /// <summary>
    /// How an individal is linked to a family
    /// </summary>
    public class GedcomFamilyLink : GedcomRecord
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
        /// Compare the user entered data against the passed instance for similarity.
        /// </summary>
        /// <param name="obj">The object to compare this instance against.</param>
        /// <returns>
        /// True if instance matches user data, otherwise false.
        /// </returns>
        public override bool IsSimilar(object obj)
        {
            var link = obj as GedcomFamilyLink;

            if (link == null)
            {
                return false;
            }

            if (!Equals(Family, link.Family))
            {
                return false;
            }

            if (!Equals(FatherPedigree, link.FatherPedigree))
            {
                return false;
            }

            if (!Equals(Individual, link.Individual))
            {
                return false;
            }

            if (!Equals(MotherPedigree, link.MotherPedigree))
            {
                return false;
            }

            if (!Equals(Pedigree, link.Pedigree))
            {
                return false;
            }

            if (!Equals(PreferedSpouse, link.PreferedSpouse))
            {
                return false;
            }

            if (!Equals(Status, link.Status))
            {
                return false;
            }

            return true;
        }
    }
}
