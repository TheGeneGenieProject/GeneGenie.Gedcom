// <copyright file="GedcomVariation.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System;

    /// <summary>
    /// TODO: Doc
    /// </summary>
    public class GedcomVariation : IComparable<GedcomVariation>, IComparable, IEquatable<GedcomVariation>
    {
        ///// <summary>
        ///// TODO: Doc, why such a vague type?
        ///// </summary>
        ///// protected object data;

        private GedcomDatabase database;

        private string variationValue;
        private string variationType;

        private GedcomChangeDate changeDate;

        // TODO: at least for GedcomName variations we need to support
        // personal name pieces here

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomVariation"/> class.
        /// </summary>
        public GedcomVariation()
        {
        }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public GedcomDatabase Database
        {
            get { return database; }
            set { database = value; }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value
        {
            get
            {
                return variationValue;
            }

            set
            {
                if (value != variationValue)
                {
                    variationValue = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the variation.
        /// </summary>
        /// <value>
        /// The type of the variation.
        /// </value>
        public string VariationType
        {
            get
            {
                return variationType;
            }

            set
            {
                if (value != variationType)
                {
                    variationType = value;
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
        public GedcomChangeDate ChangeDate
        {
            get { return changeDate; }
            set { changeDate = value; }
        }

        /// <summary>
        /// Compares the current and passed-in GedcomVariation to see if they are the same.
        /// </summary>
        /// <param name="other">The GedcomVariation to compare the current instance against.</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the value parameter.</returns>
        public int CompareTo(GedcomVariation other)
        {
            if (other == null)
            {
                return 1;
            }

            var compare = string.Compare(Value, other.Value);
            if (compare != 0)
            {
                return compare;
            }

            compare = string.Compare(VariationType, other.VariationType);
            if (compare != 0)
            {
                return compare;
            }

            return compare;
        }

        /// <summary>
        /// Compares the current and passed-in object to see if they are the same.
        /// </summary>
        /// <param name="obj">The object to compare the current instance against.</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the value parameter.</returns>
        public int CompareTo(object obj)
        {
            return CompareTo(obj as GedcomVariation);
        }

        /// <summary>
        /// Compares the current and passed-in GedcomVariation to see if they are the same.
        /// </summary>
        /// <param name="other">The GedcomVariation to compare the current instance against.</param>
        /// <returns>True if they match, False otherwise.</returns>
        public bool Equals(GedcomVariation other)
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
            return Equals(obj as GedcomVariation);
        }

        public override int GetHashCode()
        {
            return new
            {
                Value,
                VariationType,
            }.GetHashCode();
        }

        /// <summary>
        /// Changeds this instance.
        /// </summary>
        protected virtual void Changed()
        {
            if (database == null)
            {
                // System.Console.WriteLine("Changed() called on record with no database set");
                //
                //              System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
                //              foreach (System.Diagnostics.StackFrame f in trace.GetFrames())
                //              {
                //                  System.Console.WriteLine(f);
                //              }
            }
            else if (!database.Loading)
            {
                if (ChangeDate == null)
                {
                    ChangeDate = new GedcomChangeDate(database); // TODO: what level?
                }

                DateTime now = DateTime.Now;

                ChangeDate.Date1 = now.ToString("dd MMM yyyy");
                ChangeDate.Time = now.ToString("hh:mm:ss");
            }
        }
    }
}
