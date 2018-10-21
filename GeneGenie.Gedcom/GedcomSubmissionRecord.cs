// <copyright file="GedcomSubmissionRecord.cs" company="GeneGenie.com">
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
    /// TODO: Doc
    /// </summary>
    /// <seealso cref="GedcomRecord" />
    public class GedcomSubmissionRecord : GedcomRecord, IEquatable<GedcomSubmissionRecord>
    {
        /// <summary>
        /// The submitter
        /// </summary>
        private string submitter;

        /// <summary>
        /// The family file
        /// </summary>
        private string familyFile;

        /// <summary>
        /// The temple code
        /// </summary>
        private string templeCode;

        /// <summary>
        /// The generations of ancestors
        /// </summary>
        private int generationsOfAncestors;

        /// <summary>
        /// The generations of decendants
        /// </summary>
        private int generationsOfDecendants;

        /// <summary>
        /// The ordinance process flag
        /// </summary>
        private bool ordinanceProcessFlag;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomSubmissionRecord"/> class.
        /// </summary>
        public GedcomSubmissionRecord()
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
            get { return GedcomRecordType.Submission; }
        }

        /// <summary>
        /// Gets the GEDCOM tag for a submission record.
        /// </summary>
        /// <value>
        /// The GEDCOM tag.
        /// </value>
        public override string GedcomTag
        {
            get { return "SUBN"; }
        }

        /// <summary>
        /// Gets or sets the submitter.
        /// </summary>
        /// <value>
        /// The submitter.
        /// </value>
        public string Submitter
        {
            get
            {
                return submitter;
            }

            set
            {
                if (value != submitter)
                {
                    submitter = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the family file.
        /// </summary>
        /// <value>
        /// The family file.
        /// </value>
        public string FamilyFile
        {
            get
            {
                return familyFile;
            }

            set
            {
                if (value != familyFile)
                {
                    familyFile = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the temple code.
        /// </summary>
        /// <value>
        /// The temple code.
        /// </value>
        public string TempleCode
        {
            get
            {
                return templeCode;
            }

            set
            {
                if (value != templeCode)
                {
                    templeCode = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the generations of ancestors.
        /// </summary>
        /// <value>
        /// The generations of ancestors.
        /// </value>
        public int GenerationsOfAncestors
        {
            get
            {
                return generationsOfAncestors;
            }

            set
            {
                if (value != generationsOfAncestors)
                {
                    generationsOfAncestors = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the generations of decendants.
        /// </summary>
        /// <value>
        /// The generations of decendants.
        /// </value>
        public int GenerationsOfDecendants
        {
            get
            {
                return generationsOfDecendants;
            }

            set
            {
                if (value != generationsOfDecendants)
                {
                    generationsOfDecendants = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [ordinance process flag].
        /// </summary>
        /// <value>
        /// <c>true</c> if [ordinance process flag]; otherwise, <c>false</c>.
        /// </value>
        public bool OrdinanceProcessFlag
        {
            get
            {
                return ordinanceProcessFlag;
            }

            set
            {
                if (value != ordinanceProcessFlag)
                {
                    ordinanceProcessFlag = value;
                    Changed();
                }
            }
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
            var submission = obj as GedcomSubmissionRecord;

            if (submission == null)
            {
                return false;
            }

            if (!Equals(FamilyFile, submission.FamilyFile))
            {
                return false;
            }

            if (!Equals(GenerationsOfAncestors, submission.GenerationsOfAncestors))
            {
                return false;
            }

            if (!Equals(GenerationsOfDecendants, submission.GenerationsOfDecendants))
            {
                return false;
            }

            if (!Equals(OrdinanceProcessFlag, submission.OrdinanceProcessFlag))
            {
                return false;
            }

            if (!Equals(Submitter, submission.Submitter))
            {
                return false;
            }

            if (!Equals(TempleCode, submission.TempleCode))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Compare the user entered data against the passed instance for similarity.
        /// </summary>
        /// <param name="other">The GedcomSubmissionRecord to compare this instance against.</param>
        /// <returns>
        /// True if instance matches user data, otherwise false.
        /// </returns>
        public bool Equals(GedcomSubmissionRecord other)
        {
            return IsEquivalentTo(other);
        }

        // TODO: add output method
    }
}
