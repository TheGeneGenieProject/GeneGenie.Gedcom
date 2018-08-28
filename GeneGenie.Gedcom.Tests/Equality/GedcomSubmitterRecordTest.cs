// <copyright file="GedcomSubmitterRecordTest.cs" company="GeneGenie.com">
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
    /// Test suite for equality of GedcomSubmitterRecord
    /// </summary>
    public class GedcomSubmitterRecordTest
    {
        private readonly GedcomSubmitterRecord subRec1;
        private readonly GedcomSubmitterRecord subRec2;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomSubmitterRecordTest"/> class.
        /// </summary>
        public GedcomSubmitterRecordTest()
        {
            subRec1 = GenerateSubmitterRecord();
            subRec2 = GenerateSubmitterRecord();
        }

        [Fact]
        private void Submitter_record_is_not_equal_to_null()
        {
            Assert.NotNull(subRec1);
        }

        [Fact]
        private void Submitter_record_with_different_address_is_not_equal()
        {
            subRec1.Address = new GedcomAddress { City = "El Paso" };
            subRec2.Address = new GedcomAddress { City = "Pasadena" };

            Assert.NotEqual(subRec1, subRec2);
        }

        [Fact]
        private void Submitter_record_with_different_language_preferences_is_not_equal()
        {
            subRec1.LanguagePreferences.Add("ar-MA");
            subRec2.LanguagePreferences.Add("es-UY");

            Assert.NotEqual(subRec1, subRec2);
        }

        [Fact]
        private void Submitter_record_with_different_name_is_not_equal()
        {
            subRec1.Name = "name one";
            subRec2.Name = "name two";

            Assert.NotEqual(subRec1, subRec2);
        }

        [Fact]
        private void Submitter_record_with_different_registered_rfn_is_not_equal()
        {
            subRec1.RegisteredRFN = "some value";
            subRec2.RegisteredRFN = "another value";

            Assert.NotEqual(subRec1, subRec2);
        }

        [Fact]
        private void Submitter_records_with_same_facts_are_equal()
        {
            Assert.Equal(subRec1, subRec2);
        }

        private GedcomSubmitterRecord GenerateSubmitterRecord()
        {
            return new GedcomSubmitterRecord
            {
                Address = new GedcomAddress { City = "submitter city" },
                LanguagePreferences = { "en-US", "fr-FR" },
                Name = "submitter name",
                RegisteredRFN = "submitter rfn"
            };
        }
    }
}
