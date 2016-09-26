// <copyright file="GedcomIndividualMatchTest.cs" company="GeneGenie.com">
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
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using Enums;
    using Tests.DataHelperExtensions;
    using Xunit;

    /// <summary>
    /// Tests the ability to match individuals on user entered data.
    /// </summary>
    public class GedcomIndividualMatchTest
    {
        private readonly GedcomDatabase gedcomDb;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomIndividualMatchTest"/> class.
        /// </summary>
        public GedcomIndividualMatchTest()
        {
            gedcomDb = new GedcomDatabase();
        }

        [Fact]
        private void Individuals_without_data_do_not_match_at_all()
        {
            var individual1 = new GedcomIndividualRecord();
            var individual2 = new GedcomIndividualRecord();

            var match = individual1.IsMatch(individual2);

            Assert.Equal(decimal.Zero, match);
        }

        [Fact]
        private void Individuals_match_with_one_name()
        {
            var individual1 = gedcomDb.NamedPerson("Ryan");
            var individual2 = gedcomDb.NamedPerson("Ryan");

            var match = individual1.IsMatch(individual2);

            Assert.Equal(100m, match);
        }

        [Fact]
        private void Individuals_match_with_two_names()
        {
            var individual1 = gedcomDb.NamedPerson("Ryan");
            var individual2 = gedcomDb.NamedPerson("Ryan");
            individual1.Names.Add(new GedcomName { Given = "Ryan", Surname = "O'Neill" });
            individual2.Names.Add(new GedcomName { Given = "Ryan", Surname = "O'Neill" });

            var match = individual1.IsMatch(individual2);

            Assert.Equal(100m, match);
        }

        [Fact]
        private void Individuals_match_on_name_and_date_of_birth()
        {
            var individual1 = gedcomDb.NamedPerson("Ryan");
            var individual2 = gedcomDb.NamedPerson("Ryan");
            individual1.Events.Add(CreateEvent(GedcomEventType.BIRT, "Jan 1 1990", "Paris"));
            individual2.Events.Add(CreateEvent(GedcomEventType.BIRT, "Jan 1 1990", "Paris"));

            var match = individual1.IsMatch(individual2);

            Assert.Equal(100m, match);
        }

        [Fact]
        private void Individuals_match_on_name_date_of_birth_death_and_sex()
        {
            var individual1 = gedcomDb.NamedPerson("Ryan");
            var individual2 = gedcomDb.NamedPerson("Ryan");
            individual1.Events.Add(CreateEvent(GedcomEventType.BIRT, "Jan 1 1900", "Paris"));
            individual1.Events.Add(CreateEvent(GedcomEventType.DEAT, "Jan 1 2000", "Paris"));
            individual1.Sex = GedcomSex.Female;
            individual2.Events.Add(CreateEvent(GedcomEventType.BIRT, "Jan 1 1900", "Paris"));
            individual2.Events.Add(CreateEvent(GedcomEventType.DEAT, "Jan 1 2000", "Paris"));
            individual2.Sex = GedcomSex.Female;

            var match = individual1.IsMatch(individual2);

            Assert.Equal(100m, match);
        }

        [Fact]
        private void Individuals_are_rated_as_a_lower_match_when_one_fact_is_missing()
        {
            var individual1 = gedcomDb.NamedPerson("Ryan");
            var individual2 = gedcomDb.NamedPerson("Ryan");
            individual1.Events.Add(CreateEvent(GedcomEventType.BIRT, "Jan 1 1900", "Paris"));
            individual1.Events.Add(CreateEvent(GedcomEventType.DEAT, "Jan 1 2000", "Paris"));
            individual1.Sex = GedcomSex.Female;
            individual2.Events.Add(CreateEvent(GedcomEventType.BIRT, "Jan 1 1900", "Paris"));
            individual2.Sex = GedcomSex.Female;

            var match = individual1.IsMatch(individual2);

            Assert.Equal(75m, match);
        }

        private GedcomIndividualEvent CreateEvent(GedcomEventType eventType, string dateText, string placeName)
        {
            var ev = new GedcomIndividualEvent
            {
                EventType = eventType,
                Date = new GedcomDate(gedcomDb)
            };
            ev.Date.ParseDateString(dateText);
            ev.Place = new GedcomPlace
            {
                Name = placeName
            };

            return ev;
        }
    }
}
