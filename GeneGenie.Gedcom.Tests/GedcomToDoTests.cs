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

        2) Tags missed from rewrite tests.
        3) Proper tests for missing / corrupted tags on all ged files, can we use a memory stream to avoid writing to the file system?
        4) Plan for iComparable etc on all records, have as a virtual.
        5) Tabbed files, test above.
        6) Ansel Encoding - restore, unit tests.
        7) Name parsing tests according to spec.

        Find all console.writeline, debug.print, debug.* trace.*, diagnostics etc. and ensure we capture them in the parse info logs.
         For example, 'Unknown state / tag parsing'
        Notes are hard work, we need to do lookups by xref instead of just having a list of GedcomNote records attached. Need to fix this as a pain to test and use as well.

        Notes;
        We should implement icomparable<t>/icomparable on objects and replace duff icomparables.
Notes should be attached to individual when not xrefs.
Torture tests http://www.tamurajones.net/ThreeTortureTests.xhtml
Notes have extra lines added
Missing VERS from SOUR as well.
Compare our line count with their line count, should be same ideally.

Individual records need to compare all name elements (Name/surname/given name, notes) plus notes, sources etc for equality.

         * TODO: Tests required for:
         *  All encodings.
         *  Comments from old tests that need recreating:
         *   'File has 24963 INDI, 1 is in a CONT'
         *   'File has 91 INDI, 1 is  HEAD/_SCHEMA/INDI though'
         * The following needs sifting through to see what should fail the date parser,
         * what should pass and what messages should be logged.
         * Some date text has just the month or day, which we don't handle. Find an example of this and figure out how to cope with it.
         * Tests for mixed case months, for example, JuL
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
