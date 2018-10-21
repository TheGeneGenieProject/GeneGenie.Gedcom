// <copyright file="GedcomGenericComparer.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2017 Ryan O'Neill r@genegenie.com </author>
// <author> Copyright (C) 2016 Grant Winney </author>

namespace GeneGenie.Gedcom
{
    using System;

    /// <summary>
    /// Compares two objects for equality.
    /// </summary>
    public class GedcomGenericComparer
    {
        /// <summary>
        /// Compares two records to see if they are equal.
        /// Safely handles one or both being null.
        /// The records must implement IComparable.
        /// </summary>
        /// <typeparam name="T">A type that implements IComparable.</typeparam>
        /// <param name="item1">The first record.</param>
        /// <param name="item2">The second record.</param>
        /// <returns>
        /// Returns an integer that indicates their relative position in the sort order.
        /// </returns>
        public static int SafeCompareOrder<T>(T item1, T item2)
            where T : IComparable<T>
        {
            if (item1 == null && item2 == null)
            {
                return 0;
            }

            if (item2 == null)
            {
                return -1;
            }

            if (item1 == null)
            {
                return 1;
            }

            return item1.CompareTo(item2);
        }

        /// <summary>
        /// Compares two records to see if they are equal.
        /// Safely handles one or both being null.
        /// The records must implement IComparable.
        /// </summary>
        /// <typeparam name="T">A type that implements IComparable.</typeparam>
        /// <param name="item1">The first record.</param>
        /// <param name="item2">The second record.</param>
        /// <returns>
        /// Returns an integer that indicates their relative position in the sort order.
        /// </returns>
        public static bool SafeEqualityCheck<T>(T item1, T item2)
            where T : IComparable<T>
        {
            if (item1 == null && item2 == null)
            {
                return true;
            }

            if (item1 == null || item2 == null)
            {
                return false;
            }

            return item1.Equals(item2);
        }
    }
}
