// <copyright file="GedcomIdentTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Parser
{
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Checks that the IDENT tag does not muck up the import of a file as shown by
    /// http://www.tamurajones.net/GEDCOMIdentifiersCONCAndCONT.xhtml .
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
            var reader = Read("./Data/superfluous-ident-test.ged");

            var noteXref = reader.Database.Individuals.Single().Notes.First();
            var note = reader.Database.Notes.Single(n => n.XrefId == noteXref);

            Assert.Equal("First line of a note.\nSecond line of a note.\nThird line of a note.", note.Text);
        }

        [Fact]
        private void Multiline_note_is_parsed_as_one_note()
        {
            var reader = Read("./Data/superfluous-ident-test.ged");

            var noteXref = reader.Database.Individuals.Single().Notes.First();

            Assert.Single(reader.Database.Notes);
        }
    }
}
