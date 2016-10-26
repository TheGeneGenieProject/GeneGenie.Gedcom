// <copyright file="GedcomMultimediaFileTest.cs" company="GeneGenie.com">
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
    /// Tests for equality of multimedia files.
    /// </summary>
    public class GedcomMultimediaFileTest
    {
        private GedcomMultimediaFile file1;
        private GedcomMultimediaFile file2;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomMultimediaFileTest"/> class.
        /// </summary>
        public GedcomMultimediaFileTest()
        {
            file1 = GenerateMultimediaFile();
            file2 = GenerateMultimediaFile();
        }

        [Fact]
        private void Multimedia_file_is_not_equal_to_null()
        {
            Assert.NotEqual(file1, null);
        }

        [Fact]
        private void Multimedia_file_with_different_filename_is_not_equal()
        {
            file1.Filename = "file_name_one";
            file2.Filename = "file_name_two";

            Assert.NotEqual(file1, file2);
        }

        [Fact]
        private void Multimedia_file_with_different_format_is_not_equal()
        {
            file1.Format = "format_one";
            file2.Format = "format_two";

            Assert.NotEqual(file1, file2);
        }

        [Fact]
        private void Multimedia_file_with_different_source_media_type_is_not_equal()
        {
            file1.SourceMediaType = "media_type_one";
            file2.SourceMediaType = "media_type_two";

            Assert.NotEqual(file1, file2);
        }

        [Fact]
        private void Multimedia_files_with_same_facts_are_equal()
        {
            Assert.Equal(file1, file2);
        }

        private GedcomMultimediaFile GenerateMultimediaFile()
        {
            return new GedcomMultimediaFile
            {
                Filename = "some_file",
                Format = "some_format",
                SourceMediaType = "some_source_media_type"
            };
        }
    }
}