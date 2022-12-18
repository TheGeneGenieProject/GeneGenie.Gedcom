// <copyright file="GedcomRecordCountTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Parser
{
    using Xunit;

    /// <summary>
    /// Tests to ensure that the correct number of families and individuals are read.
    /// </summary>
    public class GedcomRecordCountTest
    {
        [Theory]
        [InlineData("./Data/simple.ged", 1)]
        [InlineData("./Data/presidents.ged", 1042)]
        private void Correct_family_count_can_be_read(string file, int familyCount)
        {
            var reader = GedcomRecordReader.CreateReader(file);

            Assert.Equal(familyCount, reader.Database.Families.Count);
        }

        [Theory]
        [InlineData("./Data/simple.ged", 3)]
        [InlineData("./Data/presidents.ged", 2145)]
        private void Correct_individual_count_can_be_read(string file, int individualCount)
        {
            var reader = GedcomRecordReader.CreateReader(file);

            Assert.Equal(individualCount, reader.Database.Individuals.Count);
        }

        [Theory]
        [InlineData("./Data/allged.ged", "(C) 1997-2000 by H. Eichmann. You can use and distribute this file freely as long as you do not charge for it")]
        private void Copyright_can_be_read(string file, string expectedCopyright)
        {
            var reader = GedcomRecordReader.CreateReader(file);

            Assert.Equal(expectedCopyright, reader.Database.Header.Copyright);
        }

        [Theory]
        [InlineData("./Data/allged.ged", "/Submitter-Name/")]
        private void Submitter_name_can_be_read(string file, string expectedName)
        {
            var reader = GedcomRecordReader.CreateReader(file);

            Assert.Equal(expectedName, reader.Database.Header.Submitter.Name);
        }

        [Theory]
        [InlineData("./Data/allged.ged", "Corporation address line 1\r\nCorporation address line 2\r\nCorporation address line 3\r\nCorporation address line 4")]
        private void Corporation_address_can_be_read(string file, string expected)
        {
            var reader = GedcomRecordReader.CreateReader(file);

            Assert.Equal(expected, reader.Database.Header.CorporationAddress.AddressLine);
        }
    }
}
