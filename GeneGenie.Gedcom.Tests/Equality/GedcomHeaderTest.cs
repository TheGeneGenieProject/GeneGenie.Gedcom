// <copyright file="GedcomHeaderTest.cs" company="GeneGenie.com">
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
    public class GedcomHeaderTest
    {
        private GedcomHeader header1;
        private GedcomHeader header2;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomHeaderTest"/> class.
        /// </summary>
        public GedcomHeaderTest()
        {
            header1 = GenerateHeader();
            header2 = GenerateHeader();
        }

        [Fact]
        private void Header_is_not_equal_to_null()
        {
            Assert.NotNull(header1);
        }

        [Fact]
        private void Header_with_different_application_name_is_not_equal()
        {
            header1.ApplicationName = "name one";
            header2.ApplicationName = "name two";

            Assert.NotEqual(header1, header2);
        }

        [Fact]
        private void Header_with_different_application_system_id_is_not_equal()
        {
            header1.ApplicationSystemId = "id one";
            header2.ApplicationSystemId = "id two";

            Assert.NotEqual(header1, header2);
        }

        [Fact]
        private void Header_with_different_content_description_is_not_equal()
        {
            header1.ContentDescription = new GedcomNoteRecord { Text = "description one" };
            header2.ContentDescription = new GedcomNoteRecord { Text = "description two" };

            Assert.NotEqual(header1, header2);
        }

        [Fact]
        private void Header_with_different_copyright_is_not_equal()
        {
            header1.Copyright = "copyright one";
            header2.Copyright = "copyright two";

            Assert.NotEqual(header1, header2);
        }

        [Fact]
        private void Header_with_different_corporation_is_not_equal()
        {
            header1.Corporation = "corp one";
            header2.Corporation = "corp two";

            Assert.NotEqual(header1, header2);
        }

        [Fact]
        private void Header_with_different_corporation_address_is_not_equal()
        {
            header1.CorporationAddress = new GedcomAddress { Country = "US" };
            header2.CorporationAddress = new GedcomAddress { Country = "CA" };

            Assert.NotEqual(header1, header2);
        }

        [Fact]
        private void Header_with_different_filename_is_not_equal()
        {
            header1.Filename = "filename one";
            header2.Filename = "filename two";

            Assert.NotEqual(header1, header2);
        }

        [Fact]
        private void Header_with_different_language_is_not_equal()
        {
            header1.Language = "EN";
            header2.Language = "DE";

            Assert.NotEqual(header1, header2);
        }

        [Fact]
        private void Header_with_different_source_copyright_is_not_equal()
        {
            header1.SourceCopyright = "copyright one";
            header2.SourceCopyright = "copyright two";

            Assert.NotEqual(header1, header2);
        }

        [Fact]
        private void Header_with_different_source_date_is_not_equal()
        {
            header1.SourceDate = new GedcomDate { Date1 = "01/01/1901" };
            header2.SourceDate = new GedcomDate { Date1 = "12/12/2001" };

            Assert.NotEqual(header1, header2);
        }

        [Fact]
        private void Header_with_different_source_name_is_not_equal()
        {
            header1.SourceName = "name one";
            header2.SourceName = "name two";

            Assert.NotEqual(header1, header2);
        }

        [Fact]
        private void Header_with_different_transmission_date_is_not_equal()
        {
            header1.TransmissionDate = new GedcomDate { Date1 = "01/01/1901" };
            header2.TransmissionDate = new GedcomDate { Date1 = "12/12/2001" };

            Assert.NotEqual(header1, header2);
        }

        [Fact]
        private void Headers_with_same_facts_are_equal()
        {
            Assert.Equal(header1, header2);
        }

        private GedcomHeader GenerateHeader()
        {
            return new GedcomHeader
            {
                ApplicationName = "test application name",
                ApplicationSystemId = "test system id",
                ApplicationVersion = "test version",
                ContentDescription = new GedcomNoteRecord { Text = "sample description" },
                Copyright = "test copyright",
                Corporation = "test corporation",
                CorporationAddress = new GedcomAddress { City = "some city" },
                Filename = "test_filename",
                Language = "test language",
                SourceCopyright = "test source copyright",
                SourceDate = new GedcomDate { Date1 = "01/01/1981" },
                SourceName = "test source name",
                TransmissionDate = new GedcomDate { Date1 = "01/01/2001" },
            };
        }
    }
}