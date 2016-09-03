/*
 *  $Id: GedcomFileParseTest.cs 198 2008-11-15 15:18:04Z davek $
 * 
 *  Copyright (C) 2007 David A Knight <david@ritter.demon.co.uk>
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 *
 */

namespace GeneGenie.Gedcom.Parser
{
    using System;
    using System.IO;
    using System.Text;
    using Xunit;

    public class GedcomFileParse
    {
        private void GedcomParser_ParseError(object sender, EventArgs e)
        {
            GedcomParser parser = sender as GedcomParser;
            string errstr = GedcomParser.GedcomErrorString(parser.ErrorState);

            Assert.Empty("Parser error: " + errstr);
        }

        private void GedcomParser_TagFound(object sender, EventArgs e)
        {

        }

        private void Parse(string file)
        {
            Parse(file, false, false);
        }

        private void Parse(string file, Encoding encoder)
        {
            Parse(file, encoder, false, false);
        }

        private void Parse(string file, bool allowTabs, bool allowHyphenOrUnderscoreInTag)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            Parse(file, encoder, allowTabs, allowHyphenOrUnderscoreInTag);
        }

        private void Parse(string file, Encoding encoder, bool allowTabs, bool allowHyphenOrUnderscoreInTag)
        {
            string dir = ".\\Data";

            GedcomParser parser = new GedcomParser();

            parser.AllowTabs = allowTabs;
            parser.AllowHyphenOrUnderscoreInTag = allowHyphenOrUnderscoreInTag;

            parser.ParserError += GedcomParser_ParseError;
            parser.TagFound += GedcomParser_TagFound;


            string gedcomFile = Path.Combine(dir, file);

            FileStream stream = null;
            try
            {
                FileInfo fi = new FileInfo(gedcomFile);
                stream = fi.OpenRead();

                int bufferSize = (int)fi.Length;
                byte[] buffer = new byte[bufferSize];
                int read = 0;
                while ((read = stream.Read(buffer, 0, bufferSize)) != 0)
                {
                    System.Console.WriteLine("Read into buffer");
                    string input = encoder.GetString(buffer, 0, read).Trim();
                    System.Console.WriteLine("Got string");
                    System.Console.WriteLine("Begin Parse: " + gedcomFile);
                    parser.GedcomParse(input);
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        [Fact]
        public void Test1()
        {
            Parse("test1.ged");
        }

        [Fact]
        public void Test2()
        {
            Parse("test2.ged");
        }

        [Fact]
        public void Test3()
        {
            Parse("test3.ged");
        }

        [Fact]
        public void Presidents()
        {
            Parse("presidents.ged");
        }

        [Fact]
        public void Werrett()
        {
            Parse("werrett.ged");
        }

        [Fact]
        public void Whereat()
        {
            Parse("whereat.ged");
        }

        [Fact]
        public void Database1()
        {
            Parse("Database1.ged");
        }

        [Fact]
        public void Durand1()
        {
            Parse("FAM_DD_4_2noms.ged");
        }

        [Fact]
        public void Durand2()
        {
            // This test will fail due to tabs in line value content unless
            // we tell the parser to allow them (they are invalid in GEDCOM)
            // will also fail unless - or _ are allowed in tag names
            // due to a custom tag with - in it.
            Encoding enc = Encoding.BigEndianUnicode;
            Parse("TOUT200801_unicode.ged", enc, true, true);
        }

        [Fact]
        public void Durand3()
        {
            // This test will fail due to tabs in line value content unless
            // we tell the parser to allow them (they are invalid in GEDCOM)
            // will also fail unless - or _ are allowed in tag names
            // due to a custom tag with - in it.
            Parse("test_gedcom-net.ged", true, true);
        }

        [Fact]
        public void Kollmann()
        {
            Parse("Kollmann.ged");
        }
    }
}
