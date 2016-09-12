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
    using System.Linq;

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

        private static IEnumerable<object> WarningDateData()
        {
            yield return new object[] { "29 Feb 2015", new DateTime(2015, 2, 28), ParserMessageIds.DayOfDateAdjusted };
        }

        private static IEnumerable<object> PartialDatesForExpandingData()
        {
            yield return new object[] { "2015", new DateTime(2015, 1, 1), new DateTime(2015, 12, 31, 23, 59, 59), ParserMessageIds.InterpretedAsYearRange };
            yield return new object[] { "Feb 2015", new DateTime(2015, 2, 1), new DateTime(2015, 2, 28, 23, 59, 59), ParserMessageIds.InterpretedAsMonthRange };
        }

        private static IEnumerable<object> ErrorDateData()
        {
            yield return new object[] { "32 13 2015", ParserMessageIds.DateIsNotValid };
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
        private void Estimate_indicators_cause_years_to_be_imported_as_estimates(string dateText, int expectedYear)
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
         * Three types of alert;
         *  * Info - We did something that was quite safe but they may want to know.
         *  * Warning - We found an ambiguous piece of data but think we corrected it, it needs reviewing.
         *  * Error - We found a mistake that we were unable to rectify and the user needs to edit or skip it.
         * */

        [Theory]
        [MemberData(nameof(WarningDateData))]
        private void Dates_are_corrected_and_marked_with_warning_for_user_review(string dateText, DateTime expectedDate, ParserMessageIds expectedMessage)
        {
            var date = new GedcomDate();

            date.ParseDateString(dateText);

            Assert.Equal(expectedDate, date.DateTime1);
            Assert.Equal(expectedMessage, date.ParserMessages.Single().MessageId);
        }

        [Theory]
        [MemberData(nameof(PartialDatesForExpandingData))]
        private void Partial_dates_are_interpreted_as_ranges_and_noted_as_changed(string dateText, DateTime dateFrom, DateTime dateTo, ParserMessageIds expectedMessage)
        {
            var date = new GedcomDate();

            date.ParseDateString(dateText);

            Assert.Equal(GedcomDatePeriod.Range, date.DatePeriod);
            Assert.Equal(dateFrom, date.DateTime1);
            Assert.Equal(dateTo, date.DateTime2);
            Assert.Equal(expectedMessage, date.ParserMessages.Single().MessageId);
        }

        [Theory]
        [MemberData(nameof(ErrorDateData))]
        private void Dates_are_faulty_and_marked_with_error_for_user_review(string dateText, ParserMessageIds expectedMessage)
        {
            var date = new GedcomDate();

            date.ParseDateString(dateText);

            Assert.Equal(expectedMessage, date.ParserMessages.Single().MessageId);
        }

        /* [Theory]
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
