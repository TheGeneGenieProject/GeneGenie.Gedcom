// <copyright file="GedcomNameComparisonTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Parser
{
    using System.Collections.Generic;
    using System.Linq;
    using GeneGenie.Gedcom.Tests.DataHelperExtensions;
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
        private void Nickname_part_can_be_roundtripped_to_file()
        {
            var rewrittenReader = RewrittenFile();

            var person = rewrittenReader.Database.Individuals.Single();
            Assert.Single(person.Names, n => n.Nick == "Joe");
        }

        [Fact]
        private void Name_prefix_part_can_be_roundtripped_to_file()
        {
            var rewrittenReader = RewrittenFile();

            var person = rewrittenReader.Database.Individuals.Single();
            Assert.Single(person.Names, n => n.Prefix == "Dr.");
        }

        [Fact]
        private void Given_name_part_can_be_roundtripped_to_file()
        {
            var rewrittenReader = RewrittenFile();

            var person = rewrittenReader.Database.Individuals.Single();
            Assert.Single(person.Names, n => n.Given == "Joseph");
        }

        [Fact]
        private void Surname_prefix_name_part_can_be_roundtripped_to_file()
        {
            var rewrittenReader = RewrittenFile();

            var person = rewrittenReader.Database.Individuals.Single();
            Assert.Single(person.Names, n => n.SurnamePrefix == "Le");
        }

        [Fact]
        private void Surname_name_part_can_be_roundtripped_to_file()
        {
            var rewrittenReader = RewrittenFile();

            var person = rewrittenReader.Database.Individuals.Single();
            Assert.Single(person.Names, n => n.Surname == "Einstein");
        }

        [Fact]
        private void Surname_suffix_name_part_can_be_roundtripped_to_file()
        {
            var rewrittenReader = RewrittenFile();

            var person = rewrittenReader.Database.Individuals.Single();
            Assert.Single(person.Names, n => n.Suffix == "Jr.");
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
            var name = new GedcomName
            {
                Type = "aka",
                Prefix = "Miss",
                Given = "Mary",
                SurnamePrefix = "Neu",
                Surname = "Neumann",
                Suffix = "Jr",
                Nick = "Polly",
                PreferredName = true,
            };

            name.PhoneticVariations.Add(new GedcomVariation { Value = "ma-rē", VariationType = "unknown" });
            name.PhoneticVariations.Add(new GedcomVariation { Value = "mer-ē", VariationType = "unknown" });

            name.RomanizedVariations.Add(new GedcomVariation { Value = "Miriam" });
            name.RomanizedVariations.Add(new GedcomVariation { Value = "Maria" });

            return name;
        }

        private GedcomRecordReader RewrittenFile()
        {
            var sourceFile = ".\\Data\\name.ged";
            var originalReader = GedcomRecordReader.CreateReader(sourceFile);
            var rewrittenPath = sourceFile + ".rewritten";
            GedcomRecordWriter.OutputGedcom(originalReader.Database, rewrittenPath);

            return GedcomRecordReader.CreateReader(rewrittenPath);
        }
    }
}
