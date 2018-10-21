// <copyright file="GedcomAssociationComparisonTest.cs" company="GeneGenie.com">
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
// <author> Copyright (C) 2016 Grant Winney </author>

namespace GeneGenie.Gedcom.Tests.Individuals
{
    using Xunit;

    /// <summary>
    /// Tests for equality of associations.
    /// </summary>
    public class GedcomAssociationComparisonTest
    {
        private GedcomAssociation assoc1;
        private GedcomAssociation assoc2;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomAssociationComparisonTest"/> class.
        /// </summary>
        public GedcomAssociationComparisonTest()
        {
            assoc1 = GenerateCompleteAssociation();
            assoc2 = GenerateCompleteAssociation();
        }

        [Fact]
        private void Association_not_equal_to_null()
        {
            Assert.NotNull(assoc1);
        }

        [Fact]
        private void Associations_with_different_individuals_not_equal()
        {
            assoc1.Individual = "@ I2 @";
            assoc2.Individual = "@ I4 @";

            Assert.NotEqual(assoc1, assoc2);
        }

        [Fact]
        private void Associations_with_different_descriptions_not_equal()
        {
            assoc1.Description = "Godparent";
            assoc2.Description = "Witness";

            Assert.NotEqual(assoc1, assoc2);
        }

        [Fact]
        private void Associations_with_same_facts_are_equal()
        {
            Assert.Equal(assoc1, assoc2);
        }

        private GedcomAssociation GenerateCompleteAssociation()
        {
            return new GedcomAssociation
            {
                Individual = "@ I2 @",
                Description = "Godparent",
            };
        }
    }
}
