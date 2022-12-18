// <copyright file="GedcomNameParsingTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Parser
{
    using System.Linq;
    using GeneGenie.Gedcom.Tests.DataHelperExtensions;
    using Xunit;

    /// <summary>
    /// Tests for name parsing.
    /// </summary>
    public class GedcomNameParsingTest
    {
        private readonly GedcomDatabase gedcomDb;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomNameParsingTest"/> class.
        /// </summary>
        public GedcomNameParsingTest()
        {
            gedcomDb = new GedcomDatabase();
        }

        [Fact]
        private void Surname_can_be_added_to_individual()
        {
            var person = gedcomDb.NamedPerson("Ryan", "/O'Neill/");

            Assert.Equal("/O'Neill/", person.Names.First().Surname);
        }

        [Fact]
        private void Surname_can_be_added_to_individual_without_delimiters()
        {
            var person = gedcomDb.NamedPerson("Ryan", "O'Neill");

            Assert.Equal("O'Neill", person.Names.First().Surname);
        }

        [Fact]
        private void Single_string_is_parsed_as_given_name()
        {
            var individual = gedcomDb.NamedPerson("Ryan");

            Assert.Equal("Ryan", individual.Names.Single().Given);
        }

        [Fact]
        private void Single_string_with_surname_delimiter_is_parsed_as_surname()
        {
            var individual = gedcomDb.NamedPerson("/O'Neill/");

            Assert.Equal("O'Neill", individual.Names.Single().Surname);
        }

        [Fact]
        private void Surname_with_leading_space_is_parsed_and_trimmed()
        {
            var sourceFile = "./Data/name-spaced.ged";
            var reader = GedcomRecordReader.CreateReader(sourceFile);

            var individual = reader.Database.Individuals.Single();

            Assert.Equal("Olsen", individual.Names.Single().Surname);
        }

        [Fact]
        private void Given_name_with_leading_space_can_be_parsed()
        {
            var sourceFile = "./Data/name-spaced.ged";
            var reader = GedcomRecordReader.CreateReader(sourceFile);

            var individual = reader.Database.Individuals.Single();

            Assert.Equal("Peter /Olsen/", individual.Names.Single().Name);
        }
    }
}
