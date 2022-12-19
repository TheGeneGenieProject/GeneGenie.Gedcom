// <copyright file="GedcomDateMatchTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Date.Tests
{
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Checks the mechanism for testing how similar dates are.
    /// </summary>
    public class GedcomDateMatchTest
    {
        public static IEnumerable<object[]> DatesToMatch()
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
