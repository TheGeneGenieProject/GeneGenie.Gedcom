// <copyright file="GedcomPlaceTest.cs" company="GeneGenie.com">
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
    using Xunit;

    /// <summary>
    /// Tests for equality of family records.
    /// </summary>
    public class GedcomPlaceTest
    {
        private readonly GedcomPlace place1;
        private readonly GedcomPlace place2;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomPlaceTest"/> class.
        /// Comparison tests for places.
        /// </summary>
        public GedcomPlaceTest()
        {
            place1 = GeneratePlace();
            place2 = GeneratePlace();
        }

        [Fact]
        private void Event_is_not_equal_to_null_test()
        {
            Assert.NotEqual(place1, null);
        }

        [Fact]
        private void Event_with_different_form_is_not_equal_test()
        {
            place1.Form = "some form one";
            place2.Form = "some form two";

            Assert.NotEqual(place1, place2);
        }

        [Fact]
        private void Event_with_different_latitude_is_not_equal_test()
        {
            place1.Latitude = "44.92057";
            place2.Latitude = "33.844847";

            Assert.NotEqual(place1, place2);
        }

        [Fact]
        private void Event_with_different_longitude_is_not_equal_test()
        {
            place1.Latitude = "-116.359998";
            place2.Latitude = "-89.132027";

            Assert.NotEqual(place1, place2);
        }

        [Fact]
        private void Event_with_different_name_is_not_equal_test()
        {
            place1.Name = "name one";
            place2.Name = "name two";

            Assert.NotEqual(place1, place2);
        }

        [Fact]
        private void Event_with_different_phonetic_varations_is_not_equal_test()
        {
            place1.PhoneticVariations.Clear();
            place1.PhoneticVariations.Add(new GedcomVariation
            {
                Value = "variation value",
                VariationType = "variation type"
            });
            place2.PhoneticVariations.Clear();
            place2.PhoneticVariations.Add(new GedcomVariation
            {
                Value = "another variation value",
                VariationType = "another variation type"
            });

            Assert.NotEqual(place1, place2);
        }

        [Fact]
        private void Event_with_different_romanized_varations_is_not_equal_test()
        {
            place1.RomanizedVariations.Clear();
            place1.RomanizedVariations.Add(new GedcomVariation
            {
                Value = "variation value",
                VariationType = "variation type"
            });
            place2.RomanizedVariations.Clear();
            place2.RomanizedVariations.Add(new GedcomVariation
            {
                Value = "another variation value",
                VariationType = "another variation type"
            });

            Assert.NotEqual(place1, place2);
        }

        [Fact]
        private void Places_with_same_facts_are_equal()
        {
            Assert.Equal(place1, place2);
        }

        private GedcomPlace GeneratePlace()
        {
            return new GedcomPlace
            {
                Form = "test form",
                Latitude = "44.33328",
                Longitude = "-93.44786",
                Name = "some place",
                PhoneticVariations =
                {
                    new GedcomVariation { Value = "variation value one", VariationType = "variation type one" },
                    new GedcomVariation { Value = "variation value two", VariationType = "variation type two" }
                },
                RomanizedVariations =
                {
                    new GedcomVariation { Value = "variation value", VariationType = "variation type" }
                }
            };
        }
    }
}