// <copyright file="GedcomSourceReaderTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System.Linq;
    using GeneGenie.Gedcom.Parser;
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
