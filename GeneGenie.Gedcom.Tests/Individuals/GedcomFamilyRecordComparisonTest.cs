// <copyright file="GedcomFamilyRecordComparisonTest.cs" company="GeneGenie.com">
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
    /// Tests for equality of family records.
    /// </summary>
    public class GedcomFamilyRecordComparisonTest
    {
        private readonly GedcomFamilyRecord famRec1;
        private readonly GedcomFamilyRecord famRec2;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomFamilyRecordComparisonTest"/> class.
        /// Comparison tests for GedcomFamilyRecord.
        /// </summary>
        public GedcomFamilyRecordComparisonTest()
        {
            famRec1 = GenerateFamilyRecord();
            famRec2 = GenerateFamilyRecord();
        }

        [Fact]
        private void Family_record_is_not_equal_to_null_test()
        {
            Assert.False(famRec1.IsEquivalentTo(null));
        }

        [Fact]
        private void Family_with_different_number_of_children_is_not_equal()
        {
            famRec1.NumberOfChildren = 4;
            famRec2.NumberOfChildren = 6;

            Assert.NotEqual(famRec1, famRec2);
        }

        [Fact]
        private void Family_with_different_children_is_not_equal()
        {
            famRec1.Children.Clear();
            famRec2.Children.Clear();

            famRec1.Children.Add("@I3@");
            famRec2.Children.Add("@I4@");

            Assert.NotEqual(famRec1, famRec2);
        }

        [Fact]
        private void Family_with_different_events_is_not_equal()
        {
            famRec2.Events.Clear();

            Assert.NotEqual(famRec1, famRec2);
        }

        [Fact]
        private void Family_with_different_husband_is_not_equal()
        {
            famRec1.Husband = "@I5@";
            famRec2.Husband = "@I6@";

            Assert.NotEqual(famRec1, famRec2);
        }

        [Fact]
        private void Family_with_different_wife_is_not_equal()
        {
            famRec1.Wife = "@I5@";
            famRec2.Wife = "@I6@";

            Assert.NotEqual(famRec1, famRec2);
        }

        [Fact]
        private void Family_with_different_marriage_is_not_equal()
        {
            famRec1.Events.Add(new GedcomFamilyEvent
            {
                EventType = GedcomEventType.MARR,
                Certainty = GedcomCertainty.Primary
            });
            famRec2.Events.Add(new GedcomFamilyEvent
            {
                EventType = GedcomEventType.MARR,
                Certainty = GedcomCertainty.Questionable
            });

            Assert.NotEqual(famRec1, famRec2);
        }

        [Fact]
        private void Family_with_different_start_status_is_not_equal()
        {
            famRec1.StartStatus = MarriageStartStatus.Partners;
            famRec2.StartStatus = MarriageStartStatus.Single;

            Assert.NotEqual(famRec1, famRec2);
        }

        [Fact]
        private void Family_with_different_submitter_records_is_not_equal()
        {
            famRec1.SubmitterRecords.Add("@I7@");
            famRec2.SubmitterRecords.Add("@I8@");

            Assert.NotEqual(famRec1, famRec2);
        }

        [Fact]
        private void Family_with_same_facts_are_equal()
        {
            Assert.Equal(famRec1, famRec2);
        }

        private GedcomFamilyRecord GenerateFamilyRecord()
        {
            var famRecord = new GedcomFamilyRecord
            {
                NumberOfChildren = 2,
                Husband = "@I1@",
                Wife = "@I2@",
                StartStatus = MarriageStartStatus.Partners
            };

            famRecord.Children.Add("@I3@");
            famRecord.Events.Add(new GedcomFamilyEvent { EventType = GedcomEventType.MARR });
            famRecord.SubmitterRecords.Add("@I4@");

            return famRecord;
        }
    }
}
