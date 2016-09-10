// <copyright file="GedcomDateCompareTest.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Date.Tests
{
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Checks that the date comparision works, which is complex as GEDCOM dates can have a lot of prefixes.
    /// </summary>
    public class GedcomDateCompareTest
    {
        private static IEnumerable<object> GetMatchedDates()
        {
            yield return new object[] { string.Empty, string.Empty };
            yield return new object[] { "19 APR 1996", "19 APR 1996" };
            yield return new object[] { "Jan", "Jan" };
            yield return new object[] { "Feb", "FEB" };
        }

        private static IEnumerable<object> GetEarlierDates()
        {
            yield return new object[] { "01 Jan 1900", "1 Jan 1900" };
        }

        private static IEnumerable<object> GetUnmatchedDates()
        {
            yield return new object[] { "Jan 1900", string.Empty };
            yield return new object[] { "Jan 1900", "1900" };
        }

        private static GedcomDate CreateDate(string dateText)
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
    }
}
