// <copyright file="GedcomMultimediaRecordTest.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Tests.Equality
{
    using Xunit;

    /// <summary>
    /// Tests for equality of multimedia records.
    /// </summary>
    public class GedcomMultimediaRecordTest
    {
        private GedcomMultimediaRecord multRec1;
        private GedcomMultimediaRecord multRec2;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomMultimediaRecordTest"/> class.
        /// </summary>
        public GedcomMultimediaRecordTest()
        {
            multRec1 = GenerateMultimediaRecord();
            multRec2 = GenerateMultimediaRecord();
        }

        [Fact]
        private void Multimedia_record_is_not_equal_to_null()
        {
            Assert.NotNull(multRec1);
        }

        [Fact]
        private void Multimedia_record_with_different_files_is_not_equal()
        {
            multRec1.Files.Add(new GedcomMultimediaFile { Filename = "some_file" });
            multRec2.Files.Add(new GedcomMultimediaFile { Filename = "another_file" });

            Assert.NotEqual(multRec1, multRec2);
        }

        [Fact]
        private void Multimedia_record_with_different_title_is_not_equal()
        {
            multRec1.Title = "first title";
            multRec2.Title = "second title";

            Assert.NotEqual(multRec1, multRec2);
        }

        [Fact]
        private void Multimedia_records_with_same_facts_are_equal()
        {
            Assert.Equal(multRec1, multRec2);
        }

        private GedcomMultimediaRecord GenerateMultimediaRecord()
        {
            return new GedcomMultimediaRecord
            {
                Title = "multimedia title",
                Files =
                {
                    new GedcomMultimediaFile { Filename = "file_name_one" },
                    new GedcomMultimediaFile { Filename = "file_name_two" },
                },
            };
        }
    }
}
