// <copyright file="GedcomGraphTest.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Reports
{
    using System.IO;
    using GeneGenie.Gedcom.Parser;
    using Xunit;

    /// <summary>
    /// TODO: Graph node tests that need rewriting as we don't have the original files and the tests are too large in scope.
    /// </summary>
    public class GedcomGraphTest
    {
        private GedcomRecordReader reader;

        private void Read(string file)
        {
            string dir = ".\\Data";
            string gedcomFile = Path.Combine(dir, file);

            reader = new GedcomRecordReader();
            reader.ReadGedcom(gedcomFile);
        }

        [Fact(Skip = "Needs rewriting as many smaller tests, file no longer exists.")]
        private void Test1()
        {
            Read("test1.ged");

            GedcomGraph graph = new GedcomGraph();
            graph.Database = reader.Database;

            System.Console.WriteLine("Check 1");

            string id = reader.Parser.XrefCollection["I0115"];

            graph.Record = graph.Database[id];
            GedcomIndividualRecord relative = graph.Database[id] as GedcomIndividualRecord;

            Assert.True(graph.IsRelated(relative), "Expected Relationship not found");

            System.Console.WriteLine("Check 2");
            id = reader.Parser.XrefCollection["I0684"];
            graph.Record = graph.Database[id];

            Assert.True(!graph.IsRelated(relative), "Unexpected Relationship found");

            System.Console.WriteLine("Check 3");
            id = reader.Parser.XrefCollection["I0668"];
            graph.Record = graph.Database[id];

            Assert.True(graph.IsRelated(relative), "Expected Relationship not found");
        }
    }
}
