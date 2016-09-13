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

    /// <summary>
    /// TODO: Tidy and refactor.
    /// </summary>
    public class GedcomRecordWriterTest
    {
        private GedcomRecordWriter writer;

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

            writer = new GedcomRecordWriter();
            writer.Test = true;
            writer.Database = reader.Database;
            writer.GedcomFile = Path.Combine(outputDir, file);

            writer.ApplicationName = "Gedcom.NET";
            writer.ApplicationSystemId = "Gedcom.NET";
            writer.ApplicationVersion = "Test Suite";
            writer.Corporation = "David A Knight";

            writer.WriteGedcom();

            string expectedOutput = Path.Combine(expectedDir, file);
            if (!File.Exists(expectedOutput))
            {
                File.Copy(writer.GedcomFile, expectedOutput);
            }

            string written = File.ReadAllText(writer.GedcomFile);
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
