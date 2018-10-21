// <copyright file="GedcomNameParsingTest.cs" company="GeneGenie.com">
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
    }
}
