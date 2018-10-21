// <copyright file="GedcomRecordedEventTest.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Tests.Equality
{
    using GeneGenie.Gedcom.Enums;
    using Xunit;

    /// <summary>
    /// Test suite for equality of GedcomRecordedEvent
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