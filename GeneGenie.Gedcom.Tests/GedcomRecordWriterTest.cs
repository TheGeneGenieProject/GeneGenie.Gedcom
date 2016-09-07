// <copyright file="GedcomRecordWriterTest.cs" company="GeneGenie.com">
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
            {
                Directory.CreateDirectory(outputDir);
            }

            if (!Directory.Exists(expectedDir))
            {
                Directory.CreateDirectory(expectedDir);
            }

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

        [Theory]
        [InlineData("presidents.ged")]
        private void Test1(string fileName)
        {
            Write(fileName);
        }
    }
}
