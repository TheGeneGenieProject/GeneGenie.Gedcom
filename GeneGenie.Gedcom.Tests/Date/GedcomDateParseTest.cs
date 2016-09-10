// <copyright file="GedcomDateParseTest.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Date.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Enums;
    using GeneGenie.Gedcom.Parser;
    using Xunit;

    public class GedcomDateParseTest
    {
        private static IEnumerable<object> SinglePartDateData()
        {
            yield return new object[] { "1899", new DateTime(1899, 1, 1) };
            yield return new object[] { " 1899", new DateTime(1899, 1, 1) };
            yield return new object[] { "1899 ", new DateTime(1899, 1, 1) };
            yield return new object[] { " 1899 ", new DateTime(1899, 1, 1) };
            yield return new object[] { "  1899  ", new DateTime(1899, 1, 1) };
        }

        private static IEnumerable<object> TwoPartDateData()
        {
            yield return new object[] { "Feb 1896", new DateTime(1896, 2, 1) };
            yield return new object[] { "2 1896", new DateTime(1896, 2, 1) };
            yield return new object[] { "02 1896", new DateTime(1896, 2, 1) };
            yield return new object[] { "12 1896", new DateTime(1896, 12, 1) };
            yield return new object[] { "Dec 1896", new DateTime(1896, 12, 1) };
        }

        [Theory]
        [MemberData(nameof(SinglePartDateData))]
        private void Single_part_string_is_treated_as_year(string dateText, DateTime expectedDate1)
        {
            var date = new GedcomDate();

            date.ParseDateString(dateText);

            Assert.Equal(expectedDate1, date.DateTime1);
        }

        [Theory]
        [MemberData(nameof(TwoPartDateData))]
        private void Two_part_string_is_treated_as_year_and_month(string dateText, DateTime expectedDate1)
        {
            var date = new GedcomDate();

            date.ParseDateString(dateText);

            Assert.Equal(expectedDate1, date.DateTime1);
        }

        [Theory]
        [InlineData("ABT 1997", 1997)]
        [InlineData("EST 97", 97)]
        [InlineData("97 ?", 97)]
        [InlineData("1999 ?", 1999)]
        [InlineData("   1999 ?   ", 1999)]
        private void Estimate_indicators_cause_yearsto_be_imported_as_estimates(string dateText, int expectedYear)
        {
            var date = new GedcomDate();

            date.ParseDateString(dateText);

            Assert.Equal(GedcomDatePeriod.Estimate, date.DatePeriod);
            Assert.Equal(expectedYear, date.DateTime1.Value.Year);
        }

        /*
         * Where input dates are;
         *  a) ambiguous, we should correct and flag.
         *  b) are not 3 part, we should store a range internally for indexing and keep the initial precision.
         *  c) invalid should be flagged (month out of range, day out of range, year only 2 digit).
         * We should also record the date format used for each input on full dates so we can see if they switch
         * during the import. If they do, the user needs to be told and asked which date format is OK.
         * */

        /* TODO: Many cases where we potentially guess the date, for example, if 1 or 2 fields of date.
        [Theory]
        [InlineData("1896/97")]
        [InlineData("1899/01")]
        [InlineData("1899/98")]
        [InlineData("1899/99")]
        [InlineData("1899/1")]
        [InlineData("1899/01")]
        private void Four_digits_followed_by_delimiter_and_two_digits_are_marked_as_corrected(string dateText)
        {
            var date = new GedcomDate();

            date.ParseDateString(dateText);

            Assert.True(date.CorrectionAppliedOnParsing);
        }

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
