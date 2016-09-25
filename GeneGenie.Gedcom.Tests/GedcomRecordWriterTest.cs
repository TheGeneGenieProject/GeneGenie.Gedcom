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
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Tests for ensuring that GEDCOM files can be loaded, saved and reopened without data loss.
    /// </summary>
    public class GedcomRecordWriterTest
    {
        [Theory]
        [InlineData(".\\Data\\presidents.ged")]
        [InlineData(".\\Data\\superfluous-ident-test.ged")]
        private void Gedcom_databases_are_equal_after_rewriting(string sourceFile)
        {
            var originalReader = GedcomRecordReader.CreateReader(sourceFile);
            var rewrittenPath = sourceFile + ".rewritten";
            GedcomRecordWriter.OutputGedcom(originalReader.Database, rewrittenPath);

            var rewrittenReader = GedcomRecordReader.CreateReader(rewrittenPath);

            Assert.Equal(originalReader.Database, rewrittenReader.Database);
        }

        [Theory]
        [InlineData(".\\Data\\presidents.ged")]
        [InlineData(".\\Data\\superfluous-ident-test.ged")]
        private void Gedcom_headers_are_equal_after_rewriting(string sourceFile)
        {
            var originalReader = GedcomRecordReader.CreateReader(sourceFile);
            var rewrittenPath = sourceFile + ".rewritten";
            GedcomRecordWriter.OutputGedcom(originalReader.Database, rewrittenPath);

            var rewrittenReader = GedcomRecordReader.CreateReader(rewrittenPath);

            Assert.Equal(originalReader.Database.Header, rewrittenReader.Database.Header);
        }

        [Theory]
        [InlineData(".\\Data\\presidents.ged")]
        [InlineData(".\\Data\\superfluous-ident-test.ged")]
        private void Individuals_are_equal_after_rewriting(string sourceFile)
        {
            var originalReader = GedcomRecordReader.CreateReader(sourceFile);
            var rewrittenPath = sourceFile + ".rewritten";
            GedcomRecordWriter.OutputGedcom(originalReader.Database, rewrittenPath);

            var rewrittenReader = GedcomRecordReader.CreateReader(rewrittenPath);

            Assert.Equal(originalReader.Database.Individuals.OrderBy(i => i.AutomatedRecordId), rewrittenReader.Database.Individuals.OrderBy(i => i.AutomatedRecordId));
        }
    }
}
