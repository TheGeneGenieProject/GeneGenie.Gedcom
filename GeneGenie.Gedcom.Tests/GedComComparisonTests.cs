// <copyright file="GedComComparisonTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Tests
{
    using Xunit;

    /// <summary>
    /// Class GedComComparisonTests.
    /// </summary>
    public class GedComComparisonTests
    {
        /// <summary>
        /// GedCOM comparison gedcom association is equivalent to expect are equal.
        /// </summary>
        [Fact]
        public void GedComComparison_GedcomAssociation_IsEquivalentTo_ExpectAreEqual()
        {
            // Arrange
            var object1 = new GedcomAssociation();
            var object2 = new GedcomAssociation();

            // Act and Assert
            Assert.True(object1.IsEquivalentTo(object2));
            Assert.True(object2.IsEquivalentTo(object1));
        }

        /// <summary>
        /// GedCOM comparison gedcom date is equivalent to expect are equal.
        /// </summary>
        [Fact]
        public void GedComComparison_GedcomDate_IsEquivalentTo_ExpectAreEqual()
        {
            // Arrange
            var object1 = new GedcomDate();
            var object2 = new GedcomDate();

            // Act and Assert
            Assert.True(object1.IsEquivalentTo(object2));
            Assert.True(object2.IsEquivalentTo(object1));
        }

        /// <summary>
        /// GedCOM comparison gedcom event is equivalent to expect are equal.
        /// </summary>
        [Fact]
        public void GedComComparison_GedcomEvent_IsEquivalentTo_ExpectAreEqual()
        {
            // Arrange
            var object1 = new GedcomEvent { Database = new GedcomDatabase() };
            var object2 = new GedcomEvent { Database = new GedcomDatabase() };

            // Act and Assert
            Assert.True(object1.IsEquivalentTo(object2));
            Assert.True(object2.IsEquivalentTo(object1));
        }

        /// <summary>
        /// GedCOM comparison gedcom family link is equivalent to expect are equal.
        /// </summary>
        [Fact]
        public void GedComComparison_GedcomFamilyLink_IsEquivalentTo_ExpectAreEqual()
        {
            // Arrange
            var object1 = new GedcomFamilyLink();
            var object2 = new GedcomFamilyLink();

            // Act and Assert
            Assert.True(object1.IsEquivalentTo(object2));
            Assert.True(object2.IsEquivalentTo(object1));
        }

        /// <summary>
        /// GedCOM comparison gedcom family record is equivalent to expect are equal.
        /// </summary>
        [Fact]
        public void GedComComparison_GedcomFamilyRecord_IsEquivalentTo_ExpectAreEqual()
        {
            // Arrange
            var object1 = new GedcomFamilyRecord { Database = new GedcomDatabase() };
            var object2 = new GedcomFamilyRecord { Database = new GedcomDatabase() };

            // Act and Assert
            Assert.True(object1.IsEquivalentTo(object2));
            Assert.True(object2.IsEquivalentTo(object1));
        }

        /// <summary>
        /// GedCOM comparison gedcom header is equivalent to expect are equal.
        /// </summary>
        [Fact]
        public void GedComComparison_GedcomHeader_IsEquivalentTo_ExpectAreEqual()
        {
            // Arrange
            var object1 = new GedcomHeader();
            var object2 = new GedcomHeader();

            // Act and Assert
            Assert.True(object1.IsEquivalentTo(object2));
            Assert.True(object2.IsEquivalentTo(object1));
        }

        /// <summary>
        /// GedCOM comparison gedcom individual record is equivalent to expect are equal.
        /// </summary>
        [Fact]
        public void GedComComparison_GedcomIndividualRecord_IsEquivalentTo_ExpectAreEqual()
        {
            // Arrange
            var object1 = new GedcomIndividualRecord();
            var object2 = new GedcomIndividualRecord();

            // Act and Assert
            Assert.True(object1.IsEquivalentTo(object2));
            Assert.True(object2.IsEquivalentTo(object1));
        }

        /// <summary>
        /// GedCOM comparison gedcom multimedia record is equivalent to expect are equal.
        /// </summary>
        [Fact]
        public void GedComComparison_GedcomMultimediaRecord_IsEquivalentTo_ExpectAreEqual()
        {
            // Arrange
            var object1 = new GedcomMultimediaRecord { Database = new GedcomDatabase() };
            var object2 = new GedcomMultimediaRecord { Database = new GedcomDatabase() };

            // Act and Assert
            Assert.True(object1.IsEquivalentTo(object2));
            Assert.True(object2.IsEquivalentTo(object1));
        }

        /// <summary>
        /// GedCOM comparison gedcom name is equivalent to expect are equal.
        /// </summary>
        [Fact]
        public void GedComComparison_GedcomName_IsEquivalentTo_ExpectAreEqual()
        {
            // Arrange
            var object1 = new GedcomName();
            var object2 = new GedcomName();

            // Act and Assert
            Assert.True(object1.IsEquivalentTo(object2));
            Assert.True(object2.IsEquivalentTo(object1));
        }

        /// <summary>
        /// GedCOM comparison gedcom note record is equivalent to expect are equal.
        /// </summary>
        [Fact]
        public void GedComComparison_GedcomNoteRecord_IsEquivalentTo_ExpectAreEqual()
        {
            // Arrange
            var object1 = new GedcomNoteRecord();
            var object2 = new GedcomNoteRecord();

            // Act and Assert
            Assert.True(object1.IsEquivalentTo(object2));
            Assert.True(object2.IsEquivalentTo(object1));
        }

        /// <summary>
        /// GedCOM comparison gedcom place is equivalent to expect are equal.
        /// </summary>
        [Fact]
        public void GedComComparison_GedcomPlace_IsEquivalentTo_ExpectAreEqual()
        {
            // Arrange
            var object1 = new GedcomPlace { Database = new GedcomDatabase() };
            var object2 = new GedcomPlace { Database = new GedcomDatabase() };

            // Act and Assert
            Assert.True(object1.IsEquivalentTo(object2));
            Assert.True(object2.IsEquivalentTo(object1));
        }

        /// <summary>
        /// GedCOM comparison gedcom repository citation is equivalent to expect are equal.
        /// </summary>
        [Fact]
        public void GedComComparison_GedcomRepositoryCitation_IsEquivalentTo_ExpectAreEqual()
        {
            // Arrange
            var object1 = new GedcomRepositoryCitation();
            var object2 = new GedcomRepositoryCitation();

            // Act and Assert
            Assert.True(object1.IsEquivalentTo(object2));
            Assert.True(object2.IsEquivalentTo(object1));
        }

        /// <summary>
        /// GedCOM comparison gedcom repository record is equivalent to expect are equal.
        /// </summary>
        [Fact]
        public void GedComComparison_GedcomRepositoryRecord_IsEquivalentTo_ExpectAreEqual()
        {
            // Arrange
            var object1 = new GedcomRepositoryRecord { Database = new GedcomDatabase() };
            var object2 = new GedcomRepositoryRecord { Database = new GedcomDatabase() };

            // Act and Assert
            Assert.True(object1.IsEquivalentTo(object2));
            Assert.True(object2.IsEquivalentTo(object1));
        }

        /// <summary>
        /// GedCOM comparison gedcom source citation is equivalent to expect are equal.
        /// </summary>
        [Fact]
        public void GedComComparison_GedcomSourceCitation_IsEquivalentTo_ExpectAreEqual()
        {
            // Arrange
            var object1 = new GedcomSourceCitation();
            var object2 = new GedcomSourceCitation();

            // Act and Assert
            Assert.True(object1.IsEquivalentTo(object2));
            Assert.True(object2.IsEquivalentTo(object1));
        }

        /// <summary>
        /// GedCOM comparison gedcom source record is equivalent to expect are equal.
        /// </summary>
        [Fact]
        public void GedComComparison_GedcomSourceRecord_IsEquivalentTo_ExpectAreEqual()
        {
            // Arrange
            var object1 = new GedcomSourceRecord { Database = new GedcomDatabase() };
            var object2 = new GedcomSourceRecord { Database = new GedcomDatabase() };

            // Act and Assert
            Assert.True(object1.IsEquivalentTo(object2));
            Assert.True(object2.IsEquivalentTo(object1));
        }

        /// <summary>
        /// GedCOM comparison gedcom submission record is equivalent to expect are equal.
        /// </summary>
        [Fact]
        public void GedComComparison_GedcomSubmissionRecord_IsEquivalentTo_ExpectAreEqual()
        {
            // Arrange
            var object1 = new GedcomSubmissionRecord();
            var object2 = new GedcomSubmissionRecord();

            // Act and Assert
            Assert.True(object1.IsEquivalentTo(object2));
            Assert.True(object2.IsEquivalentTo(object1));
        }

        /// <summary>
        /// GedCOM comparison gedcom submitter record is equivalent to expect are equal.
        /// </summary>
        [Fact]
        public void GedComComparison_GedcomSubmitterRecord_IsEquivalentTo_ExpectAreEqual()
        {
            // Arrange
            var object1 = new GedcomSubmitterRecord { Database = new GedcomDatabase() };
            var object2 = new GedcomSubmitterRecord { Database = new GedcomDatabase() };

            // Act and Assert
            Assert.True(object1.IsEquivalentTo(object2));
            Assert.True(object2.IsEquivalentTo(object1));
        }
    }
}