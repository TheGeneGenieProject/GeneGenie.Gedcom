using System.Collections.Generic;
using GeneGenie.Gedcom.Tests.DataHelperExtensions;
using Xunit;

namespace GeneGenie.Gedcom.Tests.Individuals
{
    public class GedcomFamilyLinkComparisonTest
    {
        private readonly GedcomDatabase gedcomDb;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomFamilyLinkComparisonTest"/> class.
        /// </summary>
        /// <remarks>
        /// Test suite for the <see cref="GedcomFamilyLinkComparisonTest"/> class.
        /// </remarks>
        public GedcomFamilyLinkComparisonTest()
        {
            gedcomDb = new GedcomDatabase();
        }

        [Fact]
        private void Family_link_is_not_equal_to_null_test()
        {
            var familyLink = new GedcomFamilyLink();

            Assert.True(familyLink.CompareTo(null) == 1);
        }

        //[Theory]
        //[InlineData("ABT 1997", 1997)]
        //[InlineData("EST 97", 97)]
        //[InlineData("97 ?", 97)]
        //private void Family_link_is_not_equal_to_null_test()
        //{
        //    var familyLink = new GedcomFamilyLink();

        //    Assert.True(familyLink.CompareTo(null) == 1);
        //}
    }
}
