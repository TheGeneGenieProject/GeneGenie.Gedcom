// <copyright file="GedcomRecordedEvent.cs" company="GeneGenie.com">
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
    using System.Linq;
    using Enums;

    /// <summary>
    /// TODO: Doc
    /// </summary>
    public class GedcomRecordedEvent : IComparable<GedcomRecordedEvent>, IComparable, IEquatable<GedcomRecordedEvent>
    {
        private GedcomDatabase database;

        private GedcomRecordList<GedcomEventType> types;
        private GedcomDate date;
        private GedcomPlace place;

        private GedcomChangeDate changeDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomRecordedEvent"/> class.
        /// </summary>
        public GedcomRecordedEvent()
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
        /// Gets or sets the types.
        /// </summary>
        /// <value>
        /// The types.
        /// </value>
        public GedcomRecordList<GedcomEventType> Types
        {
            get
            {
                if (types == null)
                {
                    types = new GedcomRecordList<GedcomEventType>();
                }

                return types;
            }

            set
            {
                if (types != value)
                {
                    types = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
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
        /// Gets or sets the place.
        /// </summary>
        /// <value>
        /// The place.
        /// </value>
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
        /// Gets or sets the change date.
        /// </summary>
        /// <value>
        /// The change date.
        /// </value>
        public GedcomChangeDate ChangeDate
        {
            get
            {
                GedcomChangeDate realChangeDate = null;
                GedcomChangeDate childChangeDate;
                if (date != null)
                {
                    childChangeDate = date.ChangeDate;
                    if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                    {
                        realChangeDate = childChangeDate;
                    }
                }

                if (place != null)
                {
                    childChangeDate = place.ChangeDate;
                    if (childChangeDate != null && realChangeDate != null && childChangeDate > realChangeDate)
                    {
                        realChangeDate = childChangeDate;
                    }
                }

                return realChangeDate;
            }

            set
            {
                changeDate = value;
            }
        }

        /// <summary>
        /// Compares this event to another record.
        /// </summary>
        /// <param name="other">A recorded event.</param>
        /// <returns>
        /// &lt;0 if the first event precedes the second in the sort order;
        /// &gt;0 if the second event precedes the first;
        /// 0 if the events are equal
        /// </returns>
        public int CompareTo(GedcomRecordedEvent other)
        {
            if (other == null)
            {
                return 1;
            }

            var compare = CompareEvents(Types, other.Types);
            if (compare != 0)
            {
                return compare;
            }

            compare = GedcomGenericComparer.SafeCompareOrder(Date, other.Date);
            if (compare != 0)
            {
                return compare;
            }

            compare = GedcomGenericComparer.SafeCompareOrder(Place, other.Place);
            if (compare != 0)
            {
                return compare;
            }

            return compare;
        }

        /// <summary>
        /// Compares this event to another record.
        /// </summary>
        /// <param name="obj">A recorded event.</param>
        /// <returns>
        /// &lt;0 if the first event precedes the second in the sort order;
        /// &gt;0 if the second event precedes the first;
        /// 0 if the events are equal
        /// </returns>
        public int CompareTo(object obj)
        {
            return CompareTo(obj as GedcomRecordedEvent);
        }

        /// <summary>
        /// Compare the GedcomRecordedEvent against the passed instance for similarity.
        /// </summary>
        /// <param name="other">The other instance to compare this instance against.</param>
        /// <returns>
        /// True if other instance matches this instance, otherwise False.
        /// </returns>
        public bool Equals(GedcomRecordedEvent other)
        {
            return CompareTo(other) == 0;
        }

        /// <summary>
        /// Compare the GedcomRecordedEvent against the passed instance for similarity.
        /// </summary>
        /// <param name="obj">The other instance to compare this instance against.</param>
        /// <returns>
        /// True if other instance matches this instance, otherwise False.
        /// </returns>
        public override bool Equals(object obj)
        {
            return CompareTo(obj as GedcomRecordedEvent) == 0;
        }

        /// <summary>
        /// Updates the changed date and time.
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
                if (changeDate == null)
                {
                    changeDate = new GedcomChangeDate(database); // TODO: what level?
                }

                DateTime now = DateTime.Now;

                changeDate.Date1 = now.ToString("dd MMM yyyy");
                changeDate.Time = now.ToString("hh:mm:ss");
            }
        }

        private static int CompareEvents(GedcomRecordList<GedcomEventType> list1, GedcomRecordList<GedcomEventType> list2)
        {
            if (list1.Count > list2.Count)
            {
                return 1;
            }

            if (list1.Count < list2.Count)
            {
                return -1;
            }

            var sortedList1 = list1.OrderBy(n => n).ToList();
            var sortedList2 = list2.OrderBy(n => n).ToList();
            for (var i = 0; i < sortedList1.Count; i++)
            {
                var compare = sortedList1.ElementAt(i).CompareTo(sortedList2.ElementAt(i));
                if (compare != 0)
                {
                    return compare;
                }
            }

            return 0;
        }
    }
}
