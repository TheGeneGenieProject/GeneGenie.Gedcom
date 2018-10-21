// <copyright file="GedcomSubmitterRecordTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
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
                RegisteredRFN = "submitter rfn",
            };
        }
    }
}
