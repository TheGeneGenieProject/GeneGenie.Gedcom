// <copyright file="GedcomSpouseSealingTest.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Tests.Individuals
{
    using System.Linq;
    using GeneGenie.Gedcom.Enums;
    using GeneGenie.Gedcom.Parser;
    using Xunit;

    /// <summary>
    /// Tests that the spousal sealing record can be added, removed and round tripped to storage.
    /// </summary>
    public class GedcomSpouseSealingTest
    {
        [Fact]
        private void Sealing_can_be_added_to_family_record_directly()
        {
            var family = new GedcomFamilyRecord();

            family.SpouseSealing = new GedcomSpouseSealingRecord();

            Assert.NotNull(family.SpouseSealing);
        }

        [Fact]
        private void Sealing_can_be_read_from_family_record_in_file()
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\Spouse-sealing.ged");

            var family = reader.Database.Families.Single();

            Assert.NotNull(family.SpouseSealing);
        }

        [Fact]
        private void Sealing_description_can_be_read_from_family_record_in_file()
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\Spouse-sealing.ged");

            var family = reader.Database.Families.Single();

            Assert.Equal("Sealing description", family.SpouseSealing.Description);
        }

        [Fact]
        private void Sealing_date_can_be_read_from_family_record_in_file()
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\Spouse-sealing.ged");

            var family = reader.Database.Families.Single();

            Assert.Equal("10 JAN 2011", family.SpouseSealing.Date.Date1);
        }

        [Fact]
        private void Sealing_place_can_be_read_from_family_record_in_file()
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\Spouse-sealing.ged");

            var family = reader.Database.Families.Single();

            Assert.Equal("Timbuktu", family.SpouseSealing.Place.Name);
        }

        [Fact]
        private void Sealing_temple_code_can_be_read_from_family_record_in_file()
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\Spouse-sealing.ged");

            var family = reader.Database.Families.Single();

            Assert.Equal("Temple Code", family.SpouseSealing.TempleCode);
        }

        [Fact]
        private void Sealing_description_can_be_roundtripped_to_file()
        {
            var rewrittenReader = RewrittenFile();

            var family = rewrittenReader.Database.Families.Single();
            Assert.Equal("Sealing description", family.SpouseSealing.Description);
        }

        [Fact]
        private void Sealing_date_can_be_roundtripped_to_file()
        {
            var rewrittenReader = RewrittenFile();

            var family = rewrittenReader.Database.Families.Single();
            Assert.Equal("10 JAN 2011", family.SpouseSealing.Date.Date1);
        }

        [Fact]
        private void Sealing_place_can_be_roundtripped_to_file()
        {
            var rewrittenReader = RewrittenFile();

            var family = rewrittenReader.Database.Families.Single();
            Assert.Equal("Timbuktu", family.SpouseSealing.Place.Name);
        }

        [Fact]
        private void Sealing_temple_code_can_be_roundtripped_to_file()
        {
            var rewrittenReader = RewrittenFile();

            var family = rewrittenReader.Database.Families.Single();
            Assert.Equal("Temple Code", family.SpouseSealing.TempleCode);
        }

        [Fact]
        private void Sealing_status_with_special_characters_can_be_roundtripped_to_file()
        {
            var rewrittenReader = RewrittenFile();

            var family = rewrittenReader.Database.Families.Single();
            Assert.Equal(SpouseSealingDateStatus.DNS_CAN, family.SpouseSealing.Status);
        }

        [Fact]
        private void Sealing_status_date_can_be_roundtripped_to_file()
        {
            var rewrittenReader = RewrittenFile();

            var family = rewrittenReader.Database.Families.Single();
            Assert.Equal("11 FEB 2012", family.SpouseSealing.StatusChangeDate.Date1);
        }

        [Fact]
        private void Sealing_notes_can_be_roundtripped_to_file()
        {
            var rewrittenReader = RewrittenFile();

            var sealing = rewrittenReader.Database.Families.Single().SpouseSealing;
            Assert.True(sealing.Notes.Any());
        }

        [Fact]
        private void Sealing_sources_can_be_roundtripped_to_file()
        {
            var rewrittenReader = RewrittenFile();

            var sealing = rewrittenReader.Database.Families.Single().SpouseSealing;
            Assert.True(sealing.Sources.Any());
        }

        private GedcomRecordReader RewrittenFile()
        {
            var sourceFile = ".\\Data\\Spouse-sealing.ged";
            var originalReader = GedcomRecordReader.CreateReader(sourceFile);
            var rewrittenPath = sourceFile + ".rewritten";
            GedcomRecordWriter.OutputGedcom(originalReader.Database, rewrittenPath);

            return GedcomRecordReader.CreateReader(rewrittenPath);
        }
    }
}
