// <copyright file="GedcomDateCompareTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Date.Tests
{
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Checks that the date comparision works, which is complex as GEDCOM dates can have a lot of prefixes.
    /// </summary>
    public class GedcomDateCompareTest
    {
        public static IEnumerable<object[]> GetMatchedDates()
        {
            yield return new object[] { string.Empty, string.Empty };
            yield return new object[] { "19 APR 1996", "19 APR 1996" };
            yield return new object[] { "Jan", "Jan" };
            yield return new object[] { "Feb", "FEB" };
        }

        public static IEnumerable<object[]> GetEarlierDates()
        {
            yield return new object[] { "01 Jan 1900", "1 Jan 1900" };
        }

        public static IEnumerable<object[]> GetUnmatchedDates()
        {
            yield return new object[] { "Jan 1900", string.Empty };
            yield return new object[] { "Jan 1900", "1900" };
        }

        public static IEnumerable<object[]> GetDistinctDateRangesAndExpectedSortValue()
        {
            yield return new object[] { "1 Jan 1900", "1 Jan 2000", -1 };
            yield return new object[] { "1 Jan 1900", "1 Jan 1900", 0 };
            yield return new object[] { "1 Jan 2000", "1 Jan 1900", 1 };
        }

        public static GedcomDate CreateDate(string dateText)
        {
            var date = new GedcomDate();
            date.ParseDateString(dateText);
            return date;
        }

        [Theory]
        [MemberData(nameof(GetUnmatchedDates))]
        private void Dates_should_not_match(string dateAText, string dateBText)
        {
            var dateA = CreateDate(dateAText);
            var dateB = CreateDate(dateBText);

            var matched = dateA == dateB;

            Assert.False(matched);
        }

        [Theory]
        [MemberData(nameof(GetMatchedDates))]
        private void Dates_should_match(string dateAText, string dateBText)
        {
            var dateA = CreateDate(dateAText);
            var dateB = CreateDate(dateBText);

            var matched = dateA == dateB;

            Assert.True(matched);
        }

        [Theory]
        [MemberData(nameof(GetEarlierDates))]
        private void First_date_should_be_earlier_due_to_text_sorting(string dateAText, string dateBText)
        {
            var dateA = CreateDate(dateAText);
            var dateB = CreateDate(dateBText);

            var lessThan = dateA < dateB;

            Assert.True(lessThan);
        }

        [Theory]
        [MemberData(nameof(GetDistinctDateRangesAndExpectedSortValue))]
        private void Compare_by_date_sorts_two_dates_correctly(string dateAText, string dateBText, int expectedSortValue)
        {
            var dateA = CreateDate(dateAText);
            var dateB = CreateDate(dateBText);

            var actualSortValue = GedcomDate.CompareByDate(dateA, dateB);

            Assert.Equal(expectedSortValue, actualSortValue);
        }

        [Fact]
        private void Compare_by_date_returns_zero_when_internal_date_values_are_both_null()
        {
            var dateA = new GedcomDate();
            var dateB = new GedcomDate();

            var result = GedcomDate.CompareByDate(dateA, dateB);

            Assert.True(result == 0);
        }

        [Fact]
        private void Compare_by_date_returns_less_than_zero_when_only_first_internal_date_is_null()
        {
            var dateA = new GedcomDate();
            var dateB = CreateDate("1 Jan 1900");

            var result = GedcomDate.CompareByDate(dateA, dateB);

            Assert.True(result == -1);
        }

        [Fact]
        private void Compare_by_date_returns_greater_than_zero_when_only_second_internal_date_is_null()
        {
            var dateA = CreateDate("1 Jan 1900");
            var dateB = new GedcomDate();

            var result = GedcomDate.CompareByDate(dateA, dateB);

            Assert.True(result == 1);
        }

        [Fact]
        private void Compare_by_date_returns_zero_when_dates_are_both_null()
        {
            GedcomDate dateA = null;
            GedcomDate dateB = null;

            var result = GedcomDate.CompareByDate(dateA, dateB);

            Assert.True(result == 0);
        }

        [Fact]
        private void Compare_by_date_returns_less_than_zero_when_only_first_date_is_null()
        {
            GedcomDate dateA = null;
            GedcomDate dateB = new GedcomDate();

            var result = GedcomDate.CompareByDate(dateA, dateB);

            Assert.True(result == -1);
        }

        [Fact]
        private void Compare_by_date_returns_greater_than_zero_when_only_second_date_is_null()
        {
            GedcomDate dateA = new GedcomDate();
            GedcomDate dateB = null;

            var result = GedcomDate.CompareByDate(dateA, dateB);

            Assert.True(result == 1);
        }

        [Theory]
        [MemberData(nameof(GetDistinctDateRangesAndExpectedSortValue))]
        private void CompareTo_sorts_two_dates_correctly(string dateAText, string dateBText, int expectedSortValue)
        {
            var dateA = CreateDate(dateAText);
            var dateB = CreateDate(dateBText);

            var actualSortValue = dateA.CompareTo(dateB);

            Assert.Equal(expectedSortValue, actualSortValue);
        }
    }
}
