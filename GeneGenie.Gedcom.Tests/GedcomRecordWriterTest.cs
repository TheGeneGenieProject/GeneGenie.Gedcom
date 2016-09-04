/*
 *  Copyright  (C) 2007 David A Knight <david@ritter.demon.co.uk>
 *  Amendments (C) 2016 Ryan O'Neill <r@genegenie.com>
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
    using System.IO;
    using Xunit;

    public class GedcomRecordWriterTest
    {
        private GedcomRecordWriter _writer;

        public GedcomRecordWriterTest()
        {
        }

        private void Write(string file)
        {
            string dir = ".\\Data";
            string gedcomFile = Path.Combine(dir, file);

            string outputDir = Path.Combine(dir, "Output");
            string expectedDir = Path.Combine(dir, "Expected");

            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);
            if (!Directory.Exists(expectedDir))
                Directory.CreateDirectory(expectedDir);

            GedcomRecordReader reader = new GedcomRecordReader();
            reader.ReadGedcom(gedcomFile);

            Assert.True(reader.Database.Count > 0, "No records read");

            _writer = new GedcomRecordWriter();
            _writer.Test = true;
            _writer.Database = reader.Database;
            _writer.GedcomFile = Path.Combine(outputDir, file);

            _writer.ApplicationName = "Gedcom.NET";
            _writer.ApplicationSystemId = "Gedcom.NET";
            _writer.ApplicationVersion = "Test Suite";
            _writer.Corporation = "David A Knight";

            _writer.WriteGedcom();

            string expectedOutput = Path.Combine(expectedDir, file);
            if (!File.Exists(expectedOutput))
            {
                File.Copy(_writer.GedcomFile, expectedOutput);
            }

            string written = File.ReadAllText(_writer.GedcomFile);
            string expected = File.ReadAllText(expectedOutput);

            Assert.True(written == expected, "Output differs from expected");
        }

        [Fact]
        public void Test1()
        {
            Write("test1.ged");
        }

        [Fact]
        public void Test2()
        {
            Write("test2.ged");
        }

        [Fact]
        public void Presidents()
        {
            Write("presidents.ged");
        }

        [Fact]
        public void Werrett()
        {
            Write("werrett.ged");
        }

        [Fact]
        public void Whereat()
        {
            Write("whereat.ged");
        }

        [Fact]
        public void Database1()
        {
            Write("Database1.ged");
        }

        [Fact]
        public void Durand1()
        {
            Write("FAM_DD_4_2noms.ged");
        }

        [Fact]
        public void Durand2()
        {
            Write("TOUT200801_unicode.ged");
        }

        [Fact]
        public void Durand3()
        {
            Write("test_gedcom-net.ged");
        }

        [Fact]
        public void Kollmann()
        {
            Write("Kollmann.ged");
        }

        [Fact]
        public void TGC551LF()
        {
            Write("TGC551LF.ged");
        }
    }
}

