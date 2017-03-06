// <copyright file="GedcomGenericComparer.cs" company="GeneGenie.com">
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
            else if (item2 == null)
            {
                return -1;
            }
            else if (item1 == null)
            {
                return 1;
            }
            else
            {
                return item1.CompareTo(item2);
            }
        }
    }
}
