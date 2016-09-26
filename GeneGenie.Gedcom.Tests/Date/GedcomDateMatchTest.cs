// <copyright file="GedcomDateMatchTest.cs" company="GeneGenie.com">
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
    /// Checks the mechanism for testing how similar dates are.
    /// </summary>
    public class GedcomDateMatchTest
    {
        private static IEnumerable<object> DatesToMatch()
        {
            yield return new object[] { string.Empty, string.Empty, 100m };
            yield return new object[] { "19 APR 1996", "19 APR 1996", 100m };
            yield return new object[] { "Jan 1990", "Jan 1990", 100m };
            yield return new object[] { "Feb 2000", "FEB 2000", 100m };
            yield return new object[] { "Jan 1 1990", "Jan 2 1990", 83.3m };
        }

        private static GedcomDate CreateDate(string dateText)
        {
            var date = new GedcomDate();
            date.ParseDateString(dateText);
            return date;
        }

        [Theory]
        [MemberData(nameof(DatesToMatch))]
        private void Dates_should_match(string dateAText, string dateBText, decimal expectedMatch)
        {
            var dateA = CreateDate(dateAText);
            var dateB = CreateDate(dateBText);

            var matched = dateA.CalculateSimilarityScore(dateB);

            Assert.Equal(expectedMatch, matched, 1);
        }
    }
}
