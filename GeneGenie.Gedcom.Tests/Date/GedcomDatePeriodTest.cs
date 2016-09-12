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
    using System.Collections.Generic;
    using Enums;
    using Xunit;

    public class GedcomDatePeriodTest
    {
        // TODO: Exercise all of the other date period code paths and options here.
        private static IEnumerable<object> ExpectedPeriodData()
        {
            yield return new object[] { "1899", GedcomDatePeriod.Range };
            yield return new object[] { "Jan 1899", GedcomDatePeriod.Range };
            yield return new object[] { "1 Jan 1899", GedcomDatePeriod.Exact };
            yield return new object[] { "01 Jan 1899", GedcomDatePeriod.Exact };
        }

        [Theory]
        [MemberData(nameof(ExpectedPeriodData))]
        private void Single_part_string_is_treated_as_year(string dateText, GedcomDatePeriod expectedPeriod)
        {
            var date = new GedcomDate();

            date.ParseDateString(dateText);

            Assert.Equal(expectedPeriod, date.DatePeriod);
        }
    }
}
