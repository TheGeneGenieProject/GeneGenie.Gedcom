// <copyright file="GedcomNameListComparer.cs" company="GeneGenie.com">
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
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Compares two lists of names for equality.
    /// TODO: Can probably make this a generic list comparer. Pass in lambda for orderby and use generic Equals T.
    /// </summary>
    [Obsolete]
    public class GedcomNameListComparer : IComparer<List<GedcomName>>
    {
        /// <summary>
        /// Compares two lists of names to see if they are equal.
        /// </summary>
        /// <param name="list1">The first list of names.</param>
        /// <param name="list2">The second list of names.</param>
        /// <returns>Returns an integer that indicates their relative position in the sort order.</returns>
        [Obsolete]
        public static int CompareNames(List<GedcomName> list1, List<GedcomName> list2)
        {
            if (list1.Count > list2.Count)
            {
                return 1;
            }

            if (list1.Count < list2.Count)
            {
                return -1;
            }

            var sortedList1 = list1.OrderBy(n => n.Name).ToList();
            var sortedList2 = list2.OrderBy(n => n.Name).ToList();
            for (int i = 0; i < sortedList1.Count; i++)
            {
                var compare = GedcomName.CompareByName(sortedList1.ElementAt(i), sortedList2.ElementAt(i));
                if (compare != 0)
                {
                    return compare;
                }
            }

            return 0;
        }

        /// <summary>
        /// Compares two lists of names to see if they are equal.
        /// </summary>
        /// <param name="list1">The first list of names.</param>
        /// <param name="list2">The second list of names.</param>
        /// <returns>Returns an integer that indicates their relative position in the sort order.</returns>
        [Obsolete]
        public int Compare(List<GedcomName> list1, List<GedcomName> list2)
        {
            return CompareNames(list1, list2);
        }
    }
}
