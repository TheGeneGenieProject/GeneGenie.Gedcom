// <copyright file="GedcomDateParseTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Date.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GeneGenie.Gedcom.Enums;
    using Xunit;

    /// <summary>
    /// Unit tests and data for ensuring dates can be parsed from GEDCOM files.
    /// </summary>
    public class GedcomDateParseTest
    {
        public static IEnumerable<object[]> SinglePartDateData()
        {
            yield return new object[] { "1899", new DateTime(1899, 1, 1) };
            yield return new object[] { " 1899", new DateTime(1899, 1, 1) };
            yield return new object[] { "1899 ", new DateTime(1899, 1, 1) };
            yield return new object[] { " 1899 ", new DateTime(1899, 1, 1) };
            yield return new object[] { "  1899  ", new DateTime(1899, 1, 1) };
        }

        public static IEnumerable<object[]> TwoPartDateData()
        {
            yield return new object[] { "Feb 1896", new DateTime(1896, 2, 1) };
            yield return new object[] { "2 1896", new DateTime(1896, 2, 1) };
            yield return new object[] { "02 1896", new DateTime(1896, 2, 1) };
            yield return new object[] { "12 1896", new DateTime(1896, 12, 1) };
            yield return new object[] { "Dec 1896", new DateTime(1896, 12, 1) };
        }

        public static IEnumerable<object[]> WarningDateData()
        {
            yield return new object[] { "29 Feb 2015", new DateTime(2015, 2, 28), ParserMessageIds.DayOfDateAdjusted };
        }

        public static IEnumerable<object[]> PartialDatesForExpandingData()
        {
            yield return new object[] { "2015", new DateTime(2015, 1, 1), new DateTime(2015, 12, 31, 23, 59, 59), ParserMessageIds.InterpretedAsYearRange };
            yield return new object[] { "Feb 2015", new DateTime(2015, 2, 1), new DateTime(2015, 2, 28, 23, 59, 59), ParserMessageIds.InterpretedAsMonthRange };
        }

        public static IEnumerable<object[]> ErrorDateData()
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
    }
}
