// <copyright file="GedcomNameComparisonTest.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Parser
{
    using System.Collections.Generic;
    using Tests.DataHelperExtensions;
    using Xunit;

    /// <summary>
    /// Tests for equality of names and name lists.
    /// </summary>
    public class GedcomNameComparisonTest
    {
        private readonly GedcomDatabase gedcomDb;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomNameComparisonTest"/> class.
        /// </summary>
        public GedcomNameComparisonTest()
        {
            gedcomDb = new GedcomDatabase();
        }

        [Fact]
        private void Longer_list_of_names_is_sorted_after_smaller_list()
        {
            var list1 = new List<GedcomName> { new GedcomName() };
            var list2 = new List<GedcomName> { };

            Assert.Equal(1, GedcomGenericListComparer.CompareListSortOrders(list1, list2));
        }

        [Fact]
        private void Smaller_list_of_names_is_sorted_before_larger_list()
        {
            var list1 = new List<GedcomName> { };
            var list2 = new List<GedcomName> { new GedcomName() };

            Assert.Equal(-1, GedcomGenericListComparer.CompareListSortOrders(list1, list2));
        }

        [Fact]
        private void Two_lists_of_same_count_and_null_names_are_equal()
        {
            var list1 = new List<GedcomName> { new GedcomName() };
            var list2 = new List<GedcomName> { new GedcomName() };

            Assert.Equal(0, GedcomGenericListComparer.CompareListSortOrders(list1, list2));
        }

        [Fact]
        private void Two_empty_names_are_equal()
        {
            var name1 = gedcomDb.NamedPerson(null, null);
            var name2 = gedcomDb.NamedPerson(null, null);

            Assert.Equal(name1, name2);
        }

        [Fact]
        private void Different_casing_of_given_names_are_not_equal()
        {
            Assert.NotEqual(gedcomDb.NamedPerson("Ryan", null), gedcomDb.NamedPerson("ryan", null));
        }

        [Fact]
        private void Individuals_with_different_given_names_and_same_surname_are_not_equal()
        {
            Assert.NotEqual(gedcomDb.NamedPerson("Ryan", "/unknown/"), gedcomDb.NamedPerson("David", "/unknown/"));
        }

        [Fact]
        private void Different_given_names_are_not_equal()
        {
            Assert.NotEqual(gedcomDb.NamedPerson("Ryan", null), gedcomDb.NamedPerson("David", null));
        }

        [Fact]
        private void Same_given_names_are_equal()
        {
            Assert.Equal(gedcomDb.NamedPerson("Ryan", null), gedcomDb.NamedPerson("Ryan", null));
        }
    }
}
