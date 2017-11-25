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
// <author> Copyright (C) 2017 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Parser
{
    using System.IO;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// Tests for ensuring that GEDCOM files can be loaded, saved and reopened without data loss.
    /// </summary>
    public class GedcomRecordWriterTest
    {
        private readonly ITestOutputHelper output;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomRecordWriterTest"/> class.
        /// </summary>
        /// <param name="output">A helper class used to log output to the test runner.</param>
        public GedcomRecordWriterTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData(".\\Data\\presidents.ged")]
        [InlineData(".\\Data\\Spouse-sealing.ged")]
        [InlineData(".\\Data\\superfluous-ident-test.ged")]
        private void Gedcom_databases_are_equal_after_rewriting(string sourceFile)
        {
            var originalReader = GedcomRecordReader.CreateReader(sourceFile);
            var rewrittenPath = $"{sourceFile}-{System.Guid.NewGuid()}.rewritten";
            GedcomRecordWriter.OutputGedcom(originalReader.Database, rewrittenPath);

            var rewrittenReader = GedcomRecordReader.CreateReader(rewrittenPath);

            AttachFileContentsToTest(sourceFile, rewrittenPath);
            Assert.Equal(originalReader.Database, rewrittenReader.Database);
        }

        [Theory]
        [InlineData(".\\Data\\presidents.ged")]
        [InlineData(".\\Data\\Spouse-sealing.ged")]
        [InlineData(".\\Data\\superfluous-ident-test.ged")]
        private void Gedcom_headers_are_equal_after_rewriting(string sourceFile)
        {
            var originalReader = GedcomRecordReader.CreateReader(sourceFile);
            var rewrittenPath = $"{sourceFile}-{System.Guid.NewGuid()}.rewritten";
            GedcomRecordWriter.OutputGedcom(originalReader.Database, rewrittenPath);

            var rewrittenReader = GedcomRecordReader.CreateReader(rewrittenPath);

            AttachFileContentsToTest(sourceFile, rewrittenPath);
            Assert.Equal(originalReader.Database.Header, rewrittenReader.Database.Header);
        }

        [Theory]
        [InlineData(".\\Data\\presidents.ged")]
        [InlineData(".\\Data\\Spouse-sealing.ged")]
        [InlineData(".\\Data\\superfluous-ident-test.ged")]
        private void Individuals_are_equal_after_rewriting(string sourceFile)
        {
            var originalReader = GedcomRecordReader.CreateReader(sourceFile);
            var rewrittenPath = $"{sourceFile}-{System.Guid.NewGuid()}.rewritten";
            GedcomRecordWriter.OutputGedcom(originalReader.Database, rewrittenPath);

            var rewrittenReader = GedcomRecordReader.CreateReader(rewrittenPath);

            AttachFileContentsToTest(sourceFile, rewrittenPath);
            Assert.True(GedcomGenericListComparer.CompareGedcomRecordLists(originalReader.Database.Individuals, rewrittenReader.Database.Individuals));
        }

        /// <summary>
        /// To help diagnose file differences, this function logs the contents of the files to the test runner.
        /// </summary>
        /// <param name="sourceFile">Path to source file to show.</param>
        /// <param name="rewrittenPath">Path to file that has been rewritten.</param>
        private void AttachFileContentsToTest(string sourceFile, string rewrittenPath)
        {
            var sourceContents = File.ReadAllText(sourceFile);
            var rewrittenContents = File.ReadAllText(rewrittenPath);

            output.WriteLine("Comparing files, output was;");
            output.WriteLine("******* Source *************");
            output.WriteLine(sourceContents);
            output.WriteLine("******* Rewritten **********");
            output.WriteLine(rewrittenContents);
            output.WriteLine("****************************");
        }
    }
}
