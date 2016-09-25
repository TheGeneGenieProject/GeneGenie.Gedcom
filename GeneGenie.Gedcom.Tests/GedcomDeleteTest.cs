// <copyright file="GedcomDeleteTest.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom
{
    using System.Linq;
    using Parser;
    using Xunit;

    /// <summary>
    /// Tests for deleting individuals, ensuring that sources etc are also deleted if no longer referenced.
    /// </summary>
    public class GedcomDeleteTest
    {
        [Theory]
        [InlineData(".\\Data\\presidents.ged")]
        [InlineData(".\\Data\\superfluous-ident-test.ged")]
        private void Individuals_can_be_deleted(string sourceFile)
        {
            var reader = GedcomRecordReader.CreateReader(sourceFile);
            var originalCount = reader.Database.Individuals.Count;

            reader.Database.Individuals.Last().Delete();

            Assert.Equal(originalCount - 1, reader.Database.Individuals.Count);
        }

        [Fact]
        private void Individual_can_be_deleted_and_source_records_for_sub_facts_are_dereferenced()
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\multiple-sources.ged");
            var sourceId = reader.Parser.XrefCollection["SRC2"];
            var originalRefCount = reader.Database[sourceId].RefCount;

            reader.Database.Individuals.First().Delete();

            Assert.Equal(2, originalRefCount);
            Assert.Equal(1, reader.Database[sourceId].RefCount);
        }

        [Fact]
        private void Deleting_all_individuals_with_shared_reference_to_source_deletes_source()
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\multiple-sources.ged");
            var sourceId = reader.Parser.XrefCollection["SRC2"];
            var originalRefCount = reader.Database[sourceId].RefCount;

            reader.Database.Individuals.First().Delete();
            reader.Database.Individuals.Single().Delete();

            Assert.Equal(2, originalRefCount);
            Assert.Null(reader.Database[sourceId]);
        }

        [Fact]
        private void Deleting_individual_with_unique_reference_to_source_deletes_source()
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\multiple-sources.ged");
            var sourceId = reader.Parser.XrefCollection["SRC1"];
            var originalSourceCount = reader.Database.Sources.Count;

            reader.Database.Individuals.First().Delete();

            Assert.Equal(3, originalSourceCount);
            Assert.Equal(2, reader.Database.Sources.Count);
        }

        [Fact]
        private void Individuals_can_be_deleted_and_family_is_removed_if_no_one_left()
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\multiple-sources.ged");

            reader.Database.Individuals.First().Delete();
            reader.Database.Individuals.Single().Delete();

            Assert.Equal(0, reader.Database.Families.Count);
        }

        [Fact]
        private void One_individual_from_family_can_be_deleted_and_family_still_remains()
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\multiple-sources.ged");

            reader.Database.Individuals.First().Delete();

            Assert.Equal(1, reader.Database.Families.Count);
        }

        [Fact]
        private void One_individual_from_family_can_be_deleted_and_family_still_has_note()
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\multiple-sources.ged");

            reader.Database.Individuals.First().Delete();

            Assert.Equal(1, reader.Database.Families.Single().Notes.Count);
        }

        [Fact]
        private void After_family_is_removed_notes_are_removed_internally()
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\multiple-sources.ged");
            var noteId = reader.Database.Families[0].Notes[0];

            reader.Database.Individuals.First().Delete();
            reader.Database.Individuals.Single().Delete();

            Assert.Null(reader.Database[noteId]);
        }
    }
}
