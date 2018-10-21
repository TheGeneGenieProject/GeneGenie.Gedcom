// <copyright file="GedcomAssociationComparisonTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
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
