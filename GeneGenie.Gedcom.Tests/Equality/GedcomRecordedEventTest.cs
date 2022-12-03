// <copyright file="GedcomRecordedEventTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.Equality
{
    using GeneGenie.Gedcom.Enums;
    using Xunit;

    /// <summary>
    /// Test suite for equality of GedcomRecordedEvent.
    /// </summary>
    public class GedcomRecordedEventTest
    {
        private readonly GedcomRecordedEvent rec1;
        private readonly GedcomRecordedEvent rec2;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomRecordedEventTest"/> class.
        /// </summary>
        public GedcomRecordedEventTest()
        {
            rec1 = GenerateRecordedEvent();
            rec2 = GenerateRecordedEvent();
        }

        [Fact]
        private void Recorded_event_is_not_equal_to_null()
        {
            Assert.NotNull(rec1);
        }

        [Fact]
        private void Recorded_event_is_greater_than_null()
        {
            Assert.Equal(1, rec1.CompareTo(null));
        }

        [Fact]
        private void Recorded_event_with_different_type_is_not_equal()
        {
            rec1.Types.Add(GedcomEventType.EMIG);
            rec2.Types.Add(GedcomEventType.IMMI);

            Assert.NotEqual(rec1, rec2);
        }

        [Fact]
        private void Recorded_event_with_different_date_is_not_equal()
        {
            rec1.Date = new GedcomDate { Date1 = "01/01/1980" };
            rec2.Date = new GedcomDate { Date1 = "12/12/2001" };

            Assert.NotEqual(rec1, rec2);
        }

        [Fact]
        private void Recorded_event_with_different_place_is_not_equal()
        {
            rec1.Place = new GedcomPlace { Name = "place one" };
            rec2.Place = new GedcomPlace { Name = "place two" };

            Assert.NotEqual(rec1, rec2);
        }

        [Fact]
        private void Recorded_events_with_same_facts_are_equal()
        {
            Assert.Equal(rec1, rec2);
        }

        [Fact]
        private void Recorded_events_with_same_facts_are_sorted_equally()
        {
            Assert.Equal(0, rec1.CompareTo(rec2));
        }

        [Fact]
        private void Recorded_event_change_date_is_not_null_when_set()
        {
            var recordedEvent = new GedcomRecordedEvent { ChangeDate = new GedcomChangeDate(null) { Date1 = "1/1/2001" } };

            Assert.NotNull(recordedEvent.ChangeDate);
        }

        [Fact]
        private void Recorded_event_change_date_yields_correct_value_when_set()
        {
            var recordedEvent = new GedcomRecordedEvent { ChangeDate = new GedcomChangeDate(null) { Date1 = "1/1/2001" } };

            Assert.Equal("1/1/2001", recordedEvent.ChangeDate.Date1);
        }

        private GedcomRecordedEvent GenerateRecordedEvent()
        {
            return new GedcomRecordedEvent
            {
                Types = new GedcomRecordList<GedcomEventType>
                {
                    GedcomEventType.CENS,
                    GedcomEventType.CENS_FAM,
                },
                Date = new GedcomDate { Date1 = "01/01/1980" },
                Place = new GedcomPlace { Name = "some place" },
            };
        }
    }
}