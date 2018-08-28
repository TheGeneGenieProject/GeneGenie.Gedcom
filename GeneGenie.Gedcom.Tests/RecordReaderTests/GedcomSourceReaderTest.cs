// <copyright file="GedcomSourceReaderTest.cs" company="GeneGenie.com">
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
    using System.Linq;
    using Parser;
    using Xunit;

    /// <summary>
    /// Tests that source records are read in for the varying record types.
    /// </summary>
    public class GedcomSourceReaderTest
    {
        [Fact]
        private void Correct_number_of_sources_loaded_for_individual()
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\multiple-sources.ged");
            string personId = reader.Parser.XrefCollection["P1"];

            var individual = reader.Database.Individuals.First(i => i.XRefID == personId);

            Assert.Single(individual.Birth.Sources);
            Assert.Single(individual.Death.Sources);
        }
    }
}
