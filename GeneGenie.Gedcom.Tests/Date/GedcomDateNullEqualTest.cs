// <copyright file="GedcomDateNullEqualTest.cs" company="GeneGenie.com">
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
    using Xunit;

    /// <summary>
    /// Null comparison tests for dates, ensuring a real date never matches a null.
    /// </summary>
    public class GedcomDateNullEqualTest
    {
        [Fact]
        private void Two_null_dates_are_equal_using_type_comparison()
        {
            Assert.True(GedcomDate.Equals(null, null));
        }

        [Fact]
        private void Compare_first_date_against_null_is_not_equal_using_type_comparison()
        {
            Assert.False(GedcomDate.Equals(new GedcomDate(), null));
        }

        [Fact]
        private void Compare_second_date_against_null_is_not_equal_using_type_comparison()
        {
            Assert.False(GedcomDate.Equals(null, new GedcomDate()));
        }

        [Fact]
        private void Compare_empty_date_against_null_is_not_equal_using_instance_comparison()
        {
            var dateA = new GedcomDate();
            Assert.False(dateA.Equals(null));
        }
    }
}
