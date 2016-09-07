// <copyright file="GedcomRecordReaderTest.cs" company="GeneGenie.com">
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
    using System;
    using System.Collections;
    using System.IO;
    using Gedcom.Enums;
    using Xunit;

    public class GedcomRecordReaderTest
    {
        private GedcomRecordReader Read(string file)
        {
            string dir = ".\\Data";
            string gedcomFile = Path.Combine(dir, file);

            long start = DateTime.Now.Ticks;
            var reader = new GedcomRecordReader();
            bool success = reader.ReadGedcom(gedcomFile);
            long end = DateTime.Now.Ticks;

            System.Console.WriteLine("Read time: " + TimeSpan.FromTicks(end - start).TotalSeconds + " seconds");

            Assert.True(success, "Failed to read " + gedcomFile);

            var individuals = 0;
            var families = 0;

            Assert.True(reader.Database.Count > 0, "No records read");

            foreach (DictionaryEntry entry in reader.Database)
            {
                var record = entry.Value as GedcomRecord;

                if (record.RecordType == GedcomRecordType.Individual)
                {
                    individuals++;
                }
                else if (record.RecordType == GedcomRecordType.Family)
                {
                    families++;
                }
            }

            System.Console.WriteLine(gedcomFile + " contains " + individuals + " individuals");
            return reader;
        }

        [Theory]
        [InlineData("simple.ged", 1)]
        [InlineData("presidents.ged", 1042)]
        private void Correct_family_count_can_be_read(string file, int familyCount)
        {
            var reader = Read(file);

            Assert.Equal(familyCount, reader.Database.Families.Count);
        }

        [Theory]
        [InlineData("simple.ged", 3)]
        [InlineData("presidents.ged", 2145)]
        private void Correct_individual_count_can_be_read(string file, int individualCount)
        {
            var reader = Read(file);

            Assert.Equal(individualCount, reader.Database.Individuals.Count);
        }

        /*
         * TODO: Tests for:
         *  All encodings.
         *  Comments from old tests that need recreating:
         *   'File has 24963 INDI, 1 is in a CONT'
         *   'File has 91 INDI, 1 is  HEAD/_SCHEMA/INDI though'

    [Fact]
        public void TGC551LF()
        {
            var _reader = Read("TGC551LF.ged");

            GedcomHeader header = _reader.Database.Header;

            Assert.False(string.IsNullOrWhiteSpace(header.Copyright), "Missing copyright");

            Assert.True(header.Submitter.Name == "John A. Nairn", "Submitter not correctly read");

            Assert.True(header.ContentDescription != null, "Missing content description");

            Assert.True(header.CorporationAddress != null, "Missing corporation address");

            Assert.True(_individuals == 15, "Not read all individuals");
            Assert.True(_families == 7, "Not read all families");
        }

         * */
    }
}
