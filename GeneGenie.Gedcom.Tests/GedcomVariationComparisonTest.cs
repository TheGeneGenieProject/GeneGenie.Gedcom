// <copyright file="GedcomVariationComparisonTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
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
