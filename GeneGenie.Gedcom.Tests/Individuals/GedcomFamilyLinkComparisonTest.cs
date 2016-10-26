// <copyright file="GedcomFamilyLinkComparisonTest.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Tests.Individuals
{
    using Enums;
    using Xunit;

    /// <summary>
    /// Tests for equality of family links.
    /// </summary>
    public class GedcomFamilyLinkComparisonTest
    {
        [Fact]
        private void Family_link_is_not_equal_to_null_test()
        {
            var familyLink = new GedcomFamilyLink();

            Assert.True(familyLink.CompareTo(null) == 1);
        }

        [Fact]
        private void Family_links_with_same_facts_are_equal()
        {
            var familyLink1 = GenerateComparableFamilyLink();
            var familyLink2 = GenerateComparableFamilyLink();

            Assert.True(familyLink1.CompareTo(familyLink2) == 0);
        }

        [Fact]
        private void Family_links_with_same_facts_are_similar()
        {
            var familyLink1 = GenerateComparableFamilyLink();
            var familyLink2 = GenerateComparableFamilyLink();

            Assert.True(familyLink1.IsEquivalentTo(familyLink2));
            Assert.True(familyLink1.Equals(familyLink2));
        }

        [Fact]
        private void Family_links_with_different_father_pedigrees_are_not_equal_test()
        {
            var familyLink1 = GenerateComparableFamilyLink();
            var familyLink2 = GenerateComparableFamilyLink();

            familyLink1.FatherPedigree = PedigreeLinkageType.Adopted;

            Assert.False(familyLink1.CompareTo(familyLink2) == 0);
        }

        [Fact]
        private void Family_links_with_different_mother_pedigrees_are_not_equal_test()
        {
            var familyLink1 = GenerateComparableFamilyLink();
            var familyLink2 = GenerateComparableFamilyLink();

            familyLink1.MotherPedigree = PedigreeLinkageType.Adopted;

            Assert.False(familyLink1.CompareTo(familyLink2) == 0);
        }

        [Fact]
        private void Family_links_with_different_pedigrees_are_not_equal_test()
        {
            var familyLink1 = GenerateComparableFamilyLink();
            var familyLink2 = GenerateComparableFamilyLink();

            familyLink1.Pedigree = PedigreeLinkageType.Adopted;

            Assert.False(familyLink1.CompareTo(familyLink2) == 0);
        }

        [Fact]
        private void Family_links_with_different_preferred_spouse_are_not_equal_test()
        {
            var familyLink1 = GenerateComparableFamilyLink();
            var familyLink2 = GenerateComparableFamilyLink();

            familyLink1.PreferedSpouse = true;

            Assert.False(familyLink1.CompareTo(familyLink2) == 0);
        }

        [Fact]
        private void Family_links_with_different_statuses_are_not_equal_test()
        {
            var familyLink1 = GenerateComparableFamilyLink();
            var familyLink2 = GenerateComparableFamilyLink();

            familyLink1.Status = ChildLinkageStatus.Challenged;

            Assert.False(familyLink1.CompareTo(familyLink2) == 0);
        }

        private GedcomFamilyLink GenerateComparableFamilyLink()
        {
            return new GedcomFamilyLink
            {
                FatherPedigree = PedigreeLinkageType.Unknown,
                MotherPedigree = PedigreeLinkageType.Unknown,
                Pedigree = PedigreeLinkageType.Unknown,
                PreferedSpouse = false,
                Status = ChildLinkageStatus.Unknown
            };
        }
    }
}
