// <copyright file="GedcomGenericListComparer.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Compares two lists of objects that inherit from GedcomRecord for equality.
    /// </summary>
    public class GedcomGenericListComparer
    {
        /// <summary>
        /// Compares two lists of records to see if they are equal.
        /// Uses the automated record id from the base class for sorting.
        /// </summary>
        /// <typeparam name="T">A class that inherits from <see cref="GedcomRecord"/> and implements Equals/GetHashCode.</typeparam>
        /// <param name="list1">The first list of records.</param>
        /// <param name="list2">The second list of records.</param>
        /// <returns>
        /// True if they match, otherwise false.
        /// </returns>
        public static bool CompareGedcomRecordLists<T>(ICollection<T> list1, ICollection<T> list2)
            where T : GedcomRecord
        {
            if (list1.Count != list2.Count)
            {
                return false;
            }

            var sortedList1 = list1.OrderBy(n => n.GetHashCode()).ToList();
            var sortedList2 = list2.OrderBy(n => n.GetHashCode()).ToList();
            for (int i = 0; i < sortedList1.Count; i++)
            {
                if (!Equals(sortedList1.ElementAt(i), sortedList2.ElementAt(i)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares two lists to see if they are equal.
        /// Relies on the sorting of the generic type used.
        /// </summary>
        /// <typeparam name="T">Any old object that can be compared.</typeparam>
        /// <param name="list1">The first list of records.</param>
        /// <param name="list2">The second list of records.</param>
        /// <returns>
        /// True if they match, otherwise false.
        /// </returns>
        public static bool CompareLists<T>(ICollection<T> list1, ICollection<T> list2)
        {
            if (list1.Count != list2.Count)
            {
                return false;
            }

            var sortedList1 = list1.OrderBy(n => n.GetHashCode()).ToList();
            var sortedList2 = list2.OrderBy(n => n.GetHashCode()).ToList();
            for (int i = 0; i < sortedList1.Count; i++)
            {
                if (!Equals(sortedList1.ElementAt(i), sortedList2.ElementAt(i)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares two lists of records to see if they are equal.
        /// The records must implement IComparable and inherit from GedcomRecord.
        /// </summary>
        /// <typeparam name="T">A type that inherits from GedcomRecord and implements IComparable.</typeparam>
        /// <param name="list1">The first list of records.</param>
        /// <param name="list2">The second list of records.</param>
        /// <returns>
        /// Returns an integer that indicates their relative position in the sort order.
        /// </returns>
        public static int CompareListSortOrders<T>(ICollection<T> list1, ICollection<T> list2)
            where T : GedcomRecord, IComparable<T>
        {
            if (list1.Count > list2.Count)
            {
                return 1;
            }

            if (list1.Count < list2.Count)
            {
                return -1;
            }

            var sortedList1 = list1.OrderBy(n => n.GetHashCode()).ToList();
            var sortedList2 = list2.OrderBy(n => n.GetHashCode()).ToList();
            for (int i = 0; i < sortedList1.Count; i++)
            {
                var compare = sortedList1.ElementAt(i).CompareTo(sortedList2.ElementAt(i));
                if (compare != 0)
                {
                    return compare;
                }
            }

            return 0;
        }

        /// <summary>
        /// Compares two lists of records to see if they are equal.
        /// The records must implement IComparable.
        /// </summary>
        /// <typeparam name="T">A type that implements IComparable.</typeparam>
        /// <param name="list1">The first list of records.</param>
        /// <param name="list2">The second list of records.</param>
        /// <returns>
        /// Returns an integer that indicates their relative position in the sort order.
        /// </returns>
        public static int CompareListOrder<T>(ICollection<T> list1, ICollection<T> list2)
            where T : IComparable<T>
        {
            if (list1.Count > list2.Count)
            {
                return 1;
            }

            if (list1.Count < list2.Count)
            {
                return -1;
            }

            var sortedList1 = list1.OrderBy(n => n.GetHashCode()).ToList();
            var sortedList2 = list2.OrderBy(n => n.GetHashCode()).ToList();
            for (int i = 0; i < sortedList1.Count; i++)
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
