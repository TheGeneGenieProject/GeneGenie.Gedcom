// <copyright file="GedcomDateMatchTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Date.Tests
{
    using Xunit;

    /// <summary>
    /// Checks the mechanism for testing how similar dates are.
    /// </summary>
    public class GedcomDateMatchTest
    {
        [Theory]
        [InlineData("", "", 100)]
        [InlineData("19 APR 1996", "19 APR 1996", 100)]
        [InlineData("Jan 1990", "Jan 1990", 100)]
        [InlineData("Feb 2000", "FEB 2000", 100)]
        [InlineData("Jan 1 1990", "Jan 2 1990", 83.3)]
        private void Dates_should_match(string dateAText, string dateBText, decimal expectedMatch)
        {
            var dateA = CreateDate(dateAText);
            var dateB = CreateDate(dateBText);

            var matched = dateA.CalculateSimilarityScore(dateB);

            Assert.Equal(expectedMatch, matched, 1);
        }

        private static GedcomDate CreateDate(string dateText)
        {
            var date = new GedcomDate();
            date.ParseDateString(dateText);
            return date;
        }
    }
}
