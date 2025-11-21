// <copyright file="GedcomSubmissionRecordTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.Equality
{
    using Xunit;

    /// <summary>
    /// Test suite for equality of GedcomSubmissionRecord.
    /// </summary>
    public class GedcomSubmissionRecordTest
    {
        private readonly GedcomSubmissionRecord subRec1;
        private readonly GedcomSubmissionRecord subRec2;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomSubmissionRecordTest"/> class.
        /// </summary>
        public GedcomSubmissionRecordTest()
        {
            subRec1 = GenerateSubmissionRecord();
            subRec2 = GenerateSubmissionRecord();
        }

        [Fact]
        private void Submission_record_is_not_equal_to_null()
        {
            Assert.NotNull(subRec1);
        }

        [Fact]
        private void Submission_record_with_different_family_file_is_not_equal()
        {
            subRec1.FamilyFile = "some file";
            subRec2.FamilyFile = "another file";

            Assert.NotEqual(subRec1, subRec2);
        }

        [Fact]
        private void Submission_record_with_different_generations_of_ancestors_is_not_equal()
        {
            subRec1.GenerationsOfAncestors = 5;
            subRec2.GenerationsOfAncestors = 55;

            Assert.NotEqual(subRec1, subRec2);
        }

        [Fact]
        private void Submission_record_with_different_generations_of_descendants_is_not_equal()
        {
            subRec1.GenerationsOfDecendants = 5;
            subRec2.GenerationsOfDecendants = 55;

            Assert.NotEqual(subRec1, subRec2);
        }

        [Fact]
        private void Submission_record_with_different_ordinance_process_flag_is_not_equal()
        {
            subRec1.OrdinanceProcessFlag = true;
            subRec2.OrdinanceProcessFlag = false;

            Assert.NotEqual(subRec1, subRec2);
        }

        [Fact]
        private void Submission_record_with_different_submitter_is_not_equal()
        {
            subRec1.Submitter = "submitter one";
            subRec2.Submitter = "submitter two";

            Assert.NotEqual(subRec1, subRec2);
        }

        [Fact]
        private void Submission_record_with_different_temple_code_is_not_equal()
        {
            subRec1.TempleCode = "code one";
            subRec2.TempleCode = "code two";

            Assert.NotEqual(subRec1, subRec2);
        }

        [Fact]
        private void Submission_records_with_same_facts_are_equal()
        {
            Assert.Equal(subRec1, subRec2);
        }

        private GedcomSubmissionRecord GenerateSubmissionRecord()
        {
            return new GedcomSubmissionRecord
            {
                FamilyFile = "some file",
                GenerationsOfAncestors = 1,
                GenerationsOfDecendants = 1,
                OrdinanceProcessFlag = true,
                Submitter = "a submitter",
                TempleCode = "temple code",
            };
        }
    }
}
