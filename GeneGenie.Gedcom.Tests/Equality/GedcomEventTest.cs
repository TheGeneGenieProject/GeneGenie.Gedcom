﻿// <copyright file="GedcomEventTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.Equality
{
    using GeneGenie.Gedcom.Enums;
    using Xunit;

    /// <summary>
    /// Tests for equality of GedcomEvent.
    /// </summary>
    public class GedcomEventTest
    {
        private readonly GedcomEvent event1;
        private readonly GedcomEvent event2;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomEventTest"/> class.
        /// Comparison tests for event.
        /// </summary>
        public GedcomEventTest()
        {
            event1 = GenerateEvent();
            event2 = GenerateEvent();
        }

        [Fact]
        private void Event_is_not_equal_to_null_test()
        {
            Assert.NotNull(event1);
        }

        [Fact]
        private void Event_with_different_address_is_not_equal()
        {
            event1.Address = new GedcomAddress { City = "Detroit", State = "MI" };
            event2.Address = new GedcomAddress { City = "San Antonio", State = "TX" };

            Assert.NotEqual(event1, event2);
        }

        [Fact]
        private void Event_with_different_cause_is_not_equal()
        {
            event1.Cause = "cause 1";
            event2.Cause = "cause 2";

            Assert.NotEqual(event1, event2);
        }

        [Fact]
        private void Event_with_different_certainty_is_not_equal()
        {
            event1.Certainty = GedcomCertainty.Primary;
            event2.Certainty = GedcomCertainty.Questionable;

            Assert.NotEqual(event1, event2);
        }

        [Fact]
        private void Event_with_different_classification_is_not_equal()
        {
            event1.Classification = "abc";
            event2.Classification = "def";

            Assert.NotEqual(event1, event2);
        }

        [Fact]
        private void Event_with_different_date_is_not_equal()
        {
            event1.Date = new GedcomDate { Date1 = "01/01/1901" };
            event2.Date = new GedcomDate { Date1 = "12/12/2001" };

            Assert.NotEqual(event1, event2);
        }

        [Fact]
        private void Event_with_different_name_is_not_equal()
        {
            event1.EventName = "an event";
            event2.EventName = "some other event";

            Assert.NotEqual(event1, event2);
        }

        [Fact]
        private void Event_with_different_type_is_not_equal()
        {
            event1.EventType = GedcomEventType.ADOP;
            event2.EventType = GedcomEventType.CENS;

            Assert.NotEqual(event1, event2);
        }

        [Fact]
        private void Event_with_different_place_is_not_equal()
        {
            event1.Place = new GedcomPlace { Name = "place one" };
            event2.Place = new GedcomPlace { Name = "place two" };

            Assert.NotEqual(event1, event2);
        }

        [Fact]
        private void Event_with_different_religious_affiliation_is_not_equal()
        {
            event1.ReligiousAffiliation = "affiliation one";
            event2.ReligiousAffiliation = "affiliation two";

            Assert.NotEqual(event1, event2);
        }

        [Fact]
        private void Event_with_different_responsible_agency_is_not_equal()
        {
            event1.ResponsibleAgency = "agency one";
            event2.ResponsibleAgency = "agency two";

            Assert.NotEqual(event1, event2);
        }

        [Fact]
        private void Events_with_same_facts_are_equal()
        {
            Assert.Equal(event1, event2);
        }

        private GedcomEvent GenerateEvent()
        {
            return new GedcomEvent
            {
                Address = new GedcomAddress { City = "Detroit", State = "MI" },
                Cause = "some cause",
                Certainty = GedcomCertainty.Unknown,
                Classification = "some classification",
                Date = new GedcomDate { Date1 = "01/01/1980" },
                EventName = "some event name",
                EventType = GedcomEventType.BAPM,
                Place = new GedcomPlace { Name = "sample place name" },
                ReligiousAffiliation = "sample affiliation",
                ResponsibleAgency = "sample agency",
            };
        }
    }
}