// <copyright file="GedcomSpouseSealingRecord.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2017 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System;
    using System.IO;
    using GeneGenie.Gedcom.Enums;
    using GeneGenie.Gedcom.Helpers;

    /// <summary>
    /// Details the spouse sealing event which can occur between a husband and wife.
    /// Sealing is a ritual performed by Latter Day Saint temples to seal familial relationships and
    /// the promise of family relationships throughout eternity.
    /// </summary>
    public class GedcomSpouseSealingRecord : GedcomRecord, IComparable, IComparable<GedcomSpouseSealingRecord>, IEquatable<GedcomSpouseSealingRecord>
    {
        /// <summary>
        /// The date that this sealing occurred on.
        /// </summary>
        private GedcomDate date;

        /// <summary>
        /// The description for this sealing event.
        /// </summary>
        private string description;

        /// <summary>
        /// The place at which this sealing occurred.
        /// </summary>
        private GedcomPlace place;

        /// <summary>
        /// The status of this sealing.
        /// </summary>
        private SpouseSealingDateStatus status;

        /// <summary>
        /// The date that the status was last changed.
        /// </summary>
        private GedcomChangeDate statusChangeDate;

        /// <summary>
        /// The temple code.
        /// </summary>
        private string templeCode;

        /// <summary>
        /// Gets or sets the date that this sealing occurred on.
        /// </summary>
        public GedcomDate Date
        {
            get
            {
                return date;
            }

            set
            {
                if (value != date)
                {
                    date = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the description for this sealing event.
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
        /// Gets or sets the place that this sealing occurred at.
        /// </summary>
        public GedcomPlace Place
        {
            get
            {
                return place;
            }

            set
            {
                if (value != place)
                {
                    place = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the status of this sealing at a point in time.
        /// </summary>
        public SpouseSealingDateStatus Status
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
        /// Gets or sets the date that the status was last changed.
        /// </summary>
        public GedcomChangeDate StatusChangeDate
        {
            get
            {
                return statusChangeDate;
            }

            set
            {
                if (value != statusChangeDate)
                {
                    statusChangeDate = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the temple code.
        /// </summary>
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
        /// Gets the type of the record.
        /// </summary>
        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.SpouseSealing; }
        }

        /// <summary>
        /// Gets the GEDCOM tag for a spouse sealing record.
        /// </summary>
        public override string GedcomTag
        {
            get { return "SLGS"; }
        }

        /// <summary>
        /// Compare two GEDCOM spouse sealing records.
        /// </summary>
        /// <param name="recorda">First record to compare.</param>
        /// <param name="recordb">Second record to compare.</param>
        /// <returns>0 if equal, -1 if recorda less than recordb, else 1.</returns>
        public static int Compare(GedcomSpouseSealingRecord recorda, GedcomSpouseSealingRecord recordb)
        {
            bool anull = Equals(recorda, null);
            bool bnull = Equals(recordb, null);

            if (anull && bnull)
            {
                return 0;
            }
            else if (anull)
            {
                return -1;
            }
            else if (bnull)
            {
                return 1;
            }

            int ret = recorda.Date.CompareTo(recordb.date);
            if (ret == 0)
            {
                ret = recorda.TempleCode.CompareTo(recordb.TempleCode);
                if (ret == 0)
                {
                    ret = recorda.Description.CompareTo(recordb.Description);
                    if (ret == 0)
                    {
                        ret = recorda.Place.Name.CompareTo(recordb.Place.Name);
                        if (ret == 0)
                        {
                            ret = recorda.Status.CompareTo(recordb.Status);
                            if (ret == 0)
                            {
                                ret = recorda.StatusChangeDate.CompareTo(recordb.StatusChangeDate);
                            }
                        }
                    }
                }
            }

            return ret;
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
            return CompareTo(obj as GedcomSpouseSealingRecord) == 0;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this == (GedcomSpouseSealingRecord)obj;
        }

        /// <summary>
        /// Compares the current and passed-in object to see if they are the same.
        /// </summary>
        /// <param name="obj">The object to compare the current instance against.</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the value parameter.</returns>
        public int CompareTo(object obj)
        {
            return CompareTo(obj as GedcomSpouseSealingRecord);
        }

        /// <summary>
        /// Compares the current and passed-in sealing record to see if they are the same.
        /// </summary>
        /// <param name="otherRecord">The sealing record to compare the current instance against.</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the value parameter.</returns>
        public int CompareTo(GedcomSpouseSealingRecord otherRecord)
        {
            return Compare(this, otherRecord);
        }

        /// <summary>
        /// Compares the current and passed-in sealing record to see if they are the same.
        /// </summary>
        /// <param name="otherRecord">The sealing record to compare the current instance against.</param>
        /// <returns>True if they match, False otherwise.</returns>
        public bool Equals(GedcomSpouseSealingRecord otherRecord)
        {
            return this == otherRecord;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            // Overflow is fine, just wrap.
            unchecked
            {
                int hash = 17;

                hash *= 23 + date.GetHashCode();
                hash *= 23 + templeCode.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Outputs this instance as a GEDCOM record.
        /// </summary>
        /// <param name="tw">The writer to output to.</param>
        public override void Output(TextWriter tw)
        {
            tw.WriteLine();
            tw.Write(Level.ToString());
            tw.Write(" SLGS ");

            if (!string.IsNullOrEmpty(Description))
            {
                tw.Write(Description);
            }

            if (date != null)
            {
                date.Output(tw);
            }

            if (place != null)
            {
                place.Output(tw);
            }

            var levelPlusOne = (Level + 1).ToString();
            if (!string.IsNullOrWhiteSpace(templeCode))
            {
                tw.WriteLine();
                tw.Write(levelPlusOne);
                tw.Write(" TEMP ");
                tw.Write(templeCode);
            }

            if (status != SpouseSealingDateStatus.NotSet)
            {
                tw.WriteLine();
                tw.Write(levelPlusOne);
                tw.Write(" STAT ");
                tw.Write(EnumHelper.ToDescription(status));

                if (StatusChangeDate != null)
                {
                    var levelPlusTwo = (Level + 2).ToString();

                    tw.Write(Environment.NewLine);
                    tw.Write(levelPlusTwo);
                    tw.Write(" CHAN ");
                    StatusChangeDate.Output(tw);
                }
            }

            OutputStandard(tw);
        }
    }
}
