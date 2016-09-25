// <copyright file="GedcomIdentTest.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Parser
{
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Checks that the IDENT tag does not muck up the import of a file as shown by
    /// http://www.tamurajones.net/GEDCOMIdentifiersCONCAndCONT.xhtml
    /// </summary>
    public class GedcomIdentTest
    {
        private GedcomRecordReader Read(string file)
        {
            var reader = new GedcomRecordReader();
            reader.ReadGedcom(file);
            return reader;
        }

        [Fact]
        private void Multiline_note_is_read_when_dodgy_ident_tag_is_used()
        {
            var reader = Read(".\\Data\\superfluous-ident-test.ged");

            var noteXref = reader.Database.Individuals.Single().Notes.First();
            var note = reader.Database.Notes.Single(n => n.XrefId == noteXref);

            Assert.Equal("First line of a note.\r\nSecond line of a note.\r\nThird line of a note.", note.Text);
        }

        [Fact]
        private void Multiline_note_is_parsed_as_one_note()
        {
            var reader = Read(".\\Data\\superfluous-ident-test.ged");

            var noteXref = reader.Database.Individuals.Single().Notes.First();

            Assert.Equal(1, reader.Database.Notes.Count);
        }
    }
}
