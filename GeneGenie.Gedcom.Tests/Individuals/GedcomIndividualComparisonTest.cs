// <copyright file="GedcomIndividualComparisonTest.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Parser
{
    using System.Collections.Generic;
    using Tests.DataHelperExtensions;
    using Xunit;

    /// <summary>
    /// Tests for equality of individuals.
    /// </summary>
    public class GedcomIndividualComparisonTest
    {
        private readonly GedcomDatabase gedcomDb;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomIndividualComparisonTest"/> class.
        /// </summary>
        public GedcomIndividualComparisonTest()
        {
            gedcomDb = new GedcomDatabase();
        }

        [Fact]
        private void Different_count_of_individuals_is_not_equal()
        {
            var list1 = new List<GedcomIndividualRecord> { new GedcomIndividualRecord() };
            var list2 = new List<GedcomIndividualRecord> { };

            Assert.NotEqual(list1, list2);
        }

        [Fact]
        private void Check_same_count_of_null_individuals_is_equal()
        {
            var list1 = new List<GedcomIndividualRecord> { new GedcomIndividualRecord() };
            var list2 = new List<GedcomIndividualRecord> { new GedcomIndividualRecord() };

            Assert.Equal(list1, list2);
        }

        [Fact]
        private void Two_lists_with_the_same_count_but_different_individuals_are_not_equal()
        {
            var list1 = new List<GedcomIndividualRecord> { gedcomDb.NamedPerson("Ryan") };
            var list2 = new List<GedcomIndividualRecord> { gedcomDb.NamedPerson("David") };

            Assert.NotEqual(list1, list2);
        }

        [Fact]
        private void Individual_is_not_similar_to_null()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");

            Assert.False(person1.IsSimilar(null));
        }

        [Fact]
        private void Individual_is_not_equal_to_null()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");

            Assert.False(Equals(null, person1));
        }

        [Fact]
        private void Individual_is_sorted_after_null()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");

            var sortOrder = person1.CompareTo(null);

            Assert.Equal(1, sortOrder);
        }

        [Fact]
        private void Individuals_with_same_facts_are_similar()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            Assert.True(person1.IsSimilar(person2));
        }

        [Fact]
        private void Individuals_with_same_facts_are_sorted_equally()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            var sortOrder = person1.CompareTo(person2);

            Assert.Equal(0, sortOrder);
        }

        [Fact]
        private void Individuals_with_different_sex_are_not_similar()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");
            person2.Sex = Gedcom.Enums.GedcomSex.Male;

            Assert.False(person1.IsSimilar(person2));
        }

        [Fact]
        private void Individuals_with_different_sex_are_not_equal()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");
            person2.Sex = Gedcom.Enums.GedcomSex.Male;

            Assert.NotEqual(person1, person2);
        }

        [Fact]
        private void Individual_are_sorted_alphanumerically()
        {
            var person1 = gedcomDb.NamedPerson("1");
            var person2 = gedcomDb.NamedPerson("2");

            var sortOrder = person1.CompareTo(person2);

            Assert.Equal(-1, sortOrder);
        }

        [Fact]
        private void Individual_are_sorted_alphanumerically_when_reversed()
        {
            var person1 = gedcomDb.NamedPerson("1");
            var person2 = gedcomDb.NamedPerson("2");

            var sortOrder = person2.CompareTo(person1);

            Assert.Equal(1, sortOrder);
        }
    }
}
