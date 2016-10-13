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
        private void Name_is_not_equal_to_null()
        {
            GedcomName name1 = new GedcomName();
            GedcomName name2 = null;

            Assert.NotEqual(name1, name2);
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

        private void Names_with_same_facts_are_equal()
        {
            var name1 = GenerateCompleteName();
            var name2 = GenerateCompleteName();

            Assert.Equal(name1, name2);
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
        private void Different_types_are_not_equal()
        {
            var name1 = GenerateCompleteName();
            var name2 = GenerateCompleteName();

            name2.Type = string.Empty;

            Assert.NotEqual(name1, name2);
        }

        [Fact]
        private void Different_phonetic_variations_are_not_equal()
        {
            var name1 = GenerateCompleteName();
            var name2 = GenerateCompleteName();

            name1.PhoneticVariations.Clear();

            Assert.NotEqual(name1, name2);
        }

        [Fact]
        private void Different_romanized_variations_are_not_equal()
        {
            var name1 = GenerateCompleteName();
            var name2 = GenerateCompleteName();

            name1.RomanizedVariations.Clear();

            Assert.NotEqual(name1, name2);
        }

        [Fact]
        private void Different_surnames_are_not_equal()
        {
            var name1 = GenerateCompleteName();
            var name2 = GenerateCompleteName();

            name1.Surname = "Smith";
            name2.Surname = "Jones";

            Assert.NotEqual(name1, name2);
        }

        [Fact]
        private void Different_prefixes_are_not_equal()
        {
            var name1 = GenerateCompleteName();
            var name2 = GenerateCompleteName();

            name1.Prefix = "Miss";
            name2.Prefix = "Mrs";

            Assert.NotEqual(name1, name2);
        }

        [Fact]
        private void Different_given_names_are_not_equal()
        {
            var name1 = GenerateCompleteName();
            var name2 = GenerateCompleteName();

            name1.Given = "Mary";
            name2.Given = "Miriam";

            Assert.NotEqual(name1, name2);
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
        private void Same_given_names_are_equal()
        {
            Assert.Equal(gedcomDb.NamedPerson("Ryan", null), gedcomDb.NamedPerson("Ryan", null));
        }

        [Fact]
        private void Different_surname_prefixes_are_not_equal()
        {
            var name1 = GenerateCompleteName();
            var name2 = GenerateCompleteName();

            name1.Surname = "Neu";
            name2.Surname = string.Empty;

            Assert.NotEqual(name1, name2);
        }

        [Fact]
        private void Different_suffixes_are_not_equal()
        {
            var name1 = GenerateCompleteName();
            var name2 = GenerateCompleteName();

            name1.Surname = "Smith";
            name2.Surname = "Jones";

            Assert.NotEqual(name1, name2);
        }

        [Fact]
        private void Different_nicks_are_not_equal()
        {
            var name1 = GenerateCompleteName();
            var name2 = GenerateCompleteName();

            name1.Nick = "Polly";
            name2.Nick = "Molly";

            Assert.NotEqual(name1, name2);
        }

        [Fact]
        private void Different_preferred_flags_are_not_equal()
        {
            var name1 = GenerateCompleteName();
            var name2 = GenerateCompleteName();

            name1.PreferredName = true;
            name2.PreferredName = false;

            Assert.NotEqual(name1, name2);
        }

        private GedcomName GenerateCompleteName()
        {
            var phoneticVariations =
                new GedcomRecordList<GedcomVariation>
                {
                    new GedcomVariation { Value = "ma-rē", VariationType = "unknown" },
                    new GedcomVariation { Value = "mer-ē", VariationType = "unknown" }
                };

            var romanizedVariations =
                new GedcomRecordList<GedcomVariation>
                {
                    new GedcomVariation { Value = "Miriam" },
                    new GedcomVariation { Value = "Maria" }
                };

            var name = new GedcomName
            {
                Type = "aka",
                Prefix = "Miss",
                Given = "Mary",
                SurnamePrefix = "Neu",
                Surname = "Neumann",
                Suffix = "Jr",
                Nick = "Polly",
                PreferredName = true
            };

            name.PhoneticVariations.AddRange(phoneticVariations);
            name.RomanizedVariations.AddRange(romanizedVariations);

            return name;
        }
    }
}
