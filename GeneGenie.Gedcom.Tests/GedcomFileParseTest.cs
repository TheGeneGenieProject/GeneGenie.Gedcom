// <copyright file="GedcomFileParseTest.cs" company="GeneGenie.com">
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
    using System;
    using System.IO;
    using System.Text;
    using Xunit;

    /// <summary>
    /// General file parsing test. TODO: Could do with merging with the Heiner Eichmann all tags test and the below cleaning up.
    /// </summary>
    public class GedcomFileParseTest
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

        [Theory]
        [InlineData("presidents.ged")] // TODO: Too vague, maybe put in an overall ImportTest class.
        private void Check_file_contents_can_be_parsed(string fileName)
        {
            Parse(fileName);
        }
    }
}
