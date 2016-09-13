// <copyright file="GedcomToDoTests.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Tests
{
    using System.Text;
    using Xunit;

    /// <summary>
    /// All of the tests that need writing, converting from old comments and old unit tests.
    /// </summary>
    public class GedcomToDoTests
    {
        [Theory(Skip = "Needs rewriting as many smaller tests, file no longer exists.")]
        [InlineData("TODO:find_this_file.ged")]
        private void Underscores_and_tabs_can_be_parsed(string fileName)
        {
            // This test will fail due to tabs in line value content unless
            // we tell the parser to allow them (they are invalid in GEDCOM)
            // will also fail unless - or _ are allowed in tag names
            // due to a custom tag with - in it.
            // Encoding enc = Encoding.BigEndianUnicode;
            // Parse(fileName, enc, true, true);
        }

        /*
         * TODO: Tests required for:
         *  All encodings.
         *  Comments from old tests that need recreating:
         *   'File has 24963 INDI, 1 is in a CONT'
         *   'File has 91 INDI, 1 is  HEAD/_SCHEMA/INDI though'
         * The following needs sifting through to see what should fail the date parser,
         * what should pass and what messages should be logged.
        [Theory]
        [InlineData("97")]
        [InlineData("0")]
        [InlineData("")]
        [InlineData("1 999")] // Maybe not error, pickup on sanity check.
        [InlineData("199 9")]
        [InlineData("1899 99")]
        [InlineData("1899/0")]

            yield return new object[] { "1/1896", new DateTime(1896, 1, 1) };
            yield return new object[] { "01/1896", new DateTime(1896, 1, 1) };
            yield return new object[] { "02/1896", new DateTime(1896, 2, 1) };
            yield return new object[] { "12/1896", new DateTime(1896, 12, 1) };
        yield return new object[] { "1896 1", new DateTime(1896, 1, 1) };
            yield return new object[] { "1896 1", new DateTime(1896, 1, 1) };
            yield return new object[] { "1896/1", new DateTime(1896, 1, 1) };
            yield return new object[] { "1896/01", new DateTime(1896, 1, 1) };
            yield return new object[] { "1896/02", new DateTime(1896, 2, 1) };
            yield return new object[] { "1896/12", new DateTime(1896, 12, 1) };
            yield return new object[] { "1896 12", new DateTime(1896, 12, 1) };
        private void Invalid_date_formats_are_marked_as_errors(string dateText)
        {
            var date = new GedcomDate();

            date.ParseDateString(dateText);

            Assert.True(date.ErrorOccurredOnParsing);
        }
        */
    }
}
