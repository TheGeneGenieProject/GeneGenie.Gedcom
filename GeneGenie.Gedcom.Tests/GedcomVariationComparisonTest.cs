// <copyright file="GedcomVariationComparisonTest.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Tests
{
    using Xunit;

    /// <summary>
    /// Tests for equality of addresses.
    /// </summary>
    public class GedcomVariationComparisonTest
    {
        [Fact]
        private void CompareTo_does_not_return_zero_when_compared_to_null()
        {
            GedcomVariation var1 = new GedcomVariation();
            GedcomVariation var2 = null;

            Assert.False(var1.CompareTo(var2) == 0);
        }

        [Fact]
        private void CompareTo_returns_zero_when_all_facts_are_equal()
        {
            GedcomVariation var1 = GenerateComparableVariation();
            GedcomVariation var2 = GenerateComparableVariation();

            Assert.True(var1.CompareTo(var2) == 0);
        }

        [Fact]
        private void Equals_returns_true_when_all_facts_are_equal()
        {
            GedcomVariation var1 = GenerateComparableVariation();
            GedcomVariation var2 = GenerateComparableVariation();

            Assert.True(var1.Equals(var2));
        }

        [Fact]
        private void CompareTo_does_not_return_zero_when_different_values()
        {
            GedcomVariation var1 = GenerateComparableVariation();
            GedcomVariation var2 = GenerateComparableVariation();

            var1.Value = "Value A";
            var2.Value = "Value B";

            Assert.False(var1.CompareTo(var2) == 0);
        }

        [Fact]
        private void CompareTo_does_not_return_zero_when_different_varation_types()
        {
            GedcomVariation var1 = GenerateComparableVariation();
            GedcomVariation var2 = GenerateComparableVariation();

            var1.VariationType = "Varation Type A";
            var2.VariationType = "Variation Type B";

            Assert.False(var1.CompareTo(var2) == 0);
        }

        private GedcomVariation GenerateComparableVariation()
        {
            return new GedcomVariation
            {
                Value = "Some Value",
                VariationType = "Some Variation Type",
            };
        }
    }
}
