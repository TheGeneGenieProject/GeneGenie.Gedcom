// <copyright file="GedcomNoteRecordTest.cs" company="GeneGenie.com">
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
    /// Test suite for equality of GedcomNoteRecord
    /// </summary>
    public class GedcomNoteRecordTest
    {

        private readonly GedcomNoteRecord noteRecord1;
        private readonly GedcomNoteRecord noteRecord2;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomNoteRecordTest"/> class.
        /// </summary>
        public GedcomNoteRecordTest()
        {
            noteRecord1 = GenerateNoteRecord();
            noteRecord2 = GenerateNoteRecord();
        }

        [Fact]
        private void Note_record_is_not_equal_to_null()
        {
            Assert.NotEqual(noteRecord1, null);
        }

        [Fact]
        private void Note_record_with_different_text_is_not_equal()
        {
            noteRecord1.Text = "note one";
            noteRecord2.Text = "note two";

            Assert.NotEqual(noteRecord1, noteRecord2);
        }

        [Fact]
        private void Note_records_with_same_facts_are_equal()
        {
            Assert.Equal(noteRecord1, noteRecord2);
        }

        private GedcomNoteRecord GenerateNoteRecord()
        {
            return new GedcomNoteRecord
            {
                Text = "sample note"
            };
        }
    }
}
