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

        public GedcomAssociationComparisonTest()
        {
            assoc1 = GenerateCompleteAssociation();
            assoc2 = GenerateCompleteAssociation();
        }

        private void Association_not_equal_to_null()
        {
            Assert.NotEqual(assoc1, null);
        }

        private void Associations_with_different_individuals_not_equal()
        {
            assoc2.Individual = "@ I2 @";

            Assert.NotEqual(assoc1, assoc2);
        }

        private void Associations_with_different_descriptions_not_equal()
        {
            assoc2.Description = "Witness";

            Assert.NotEqual(assoc1, assoc2);
        }

        private void Associations_with_same_facts_are_equal()
        {
            Assert.Equal(assoc1, assoc2);
        }

        private GedcomAssociation GenerateCompleteAssociation()
        {
            return new GedcomAssociation
            {
                Individual = "@ I2 @",
                Description = "Godparent"
            };
        }
    }
}
