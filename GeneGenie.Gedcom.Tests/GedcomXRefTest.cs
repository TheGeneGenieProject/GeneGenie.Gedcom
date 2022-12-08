// <copyright file="GedcomXRefTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using GeneGenie.Gedcom.Parser;

    using Xunit;

    /// <summary>
    /// Tests that source records are read in for the varying record types.
    /// </summary>
    public class GedcomXRefTest
    {
        [Theory]
        [InlineData(".\\Data\\simple.ged")]
        [InlineData(".\\Data\\presidents.ged")]
        [InlineData(".\\Data\\Spouse-sealing.ged")]
        [InlineData(".\\Data\\superfluous-ident-test.ged")]
        private void Original_record_id(string sourceFile)
        {
            var originalReader = GedcomRecordReader.CreateReader(sourceFile, false);
            var rewrittenPath = $"{sourceFile}-{System.Guid.NewGuid()}.rewritten";
            GedcomRecordWriter.OutputGedcom(originalReader.Database, rewrittenPath);

            var rewrittenReader = GedcomRecordReader.CreateReader(rewrittenPath, false);

            Assert.Equal(originalReader.Database, rewrittenReader.Database);
            Assert.Equal(originalReader.Database.Header, rewrittenReader.Database.Header);
            Assert.Equal(originalReader.Database.Individuals, rewrittenReader.Database.Individuals);
            CompareGedcomRecordXRefLists(originalReader.Database.Individuals, rewrittenReader.Database.Individuals);
            CompareGedcomRecordXRefLists(originalReader.Database.Individuals, rewrittenReader.Database.Individuals);
            CompareGedcomRecordXRefLists(originalReader.Database.Families, rewrittenReader.Database.Families);
            CompareGedcomRecordXRefLists(originalReader.Database.Sources, rewrittenReader.Database.Sources);
            CompareGedcomRecordXRefLists(originalReader.Database.Repositories, rewrittenReader.Database.Repositories);

            // Media, Notes and Submitters becomes referenced entries
            // Assert.True(CompareGedcomRecordXRefLists(originalReader.Database.Media, rewrittenReader.Database.Media));
            // Assert.True(CompareGedcomRecordXRefLists(originalReader.Database.Notes, rewrittenReader.Database.Notes));
            // Assert.True(CompareGedcomRecordXRefLists(originalReader.Database.Submitters, rewrittenReader.Database.Submitters));
        }

        /// <summary>
        /// Compares two lists of records to see if they are equal.
        /// Uses the automated record id from the base class for sorting.
        /// </summary>
        /// <typeparam name="T">A class that inherits from <see cref="GedcomRecord"/> and implements Equals/GetHashCode.</typeparam>
        /// <param name="list1">The first list of records.</param>
        /// <param name="list2">The second list of records.</param>
        private void CompareGedcomRecordXRefLists<T>(ICollection<T> list1, ICollection<T> list2)
            where T : GedcomRecord
        {
            Assert.Equal(list1.Count, list2.Count);

            var sortedList1 = list1.OrderBy(n => n.GetHashCode()).ToList();
            var sortedList2 = list2.OrderBy(n => n.GetHashCode()).ToList();
            for (int i = 0; i < sortedList1.Count; i++)
            {
                Assert.Equal(sortedList1.ElementAt(i).XRefID, sortedList2.ElementAt(i).XRefID);
            }
        }
    }
}
