/*
 *  $Id: GedcomGraphTest.cs 183 2008-06-08 15:31:15Z davek $
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

namespace GeneGenie.Gedcom.Reports
{
    using System.IO;
    using Xunit;
    using GeneGenie.Gedcom.Parser;

    public class GedcomGraphTest
    {
        private GedcomRecordReader _reader;

        private void Read(string file)
        {
            string dir = ".\\Data";
            string gedcomFile = Path.Combine(dir, file);

            _reader = new GedcomRecordReader();
            _reader.ReadGedcom(gedcomFile);
        }

        [Fact]
        public void Test1()
        {
            Read("test1.ged");

            GedcomGraph graph = new GedcomGraph();
            graph.Database = _reader.Database;

            System.Console.WriteLine("Check 1");

            string id = _reader.Parser.XrefCollection["I0115"];

            graph.Record = graph.Database[id];
            GedcomIndividualRecord relative = graph.Database[id] as GedcomIndividualRecord;

            Assert.True(graph.IsRelated(relative), "Expected Relationship not found");

            System.Console.WriteLine("Check 2");
            id = _reader.Parser.XrefCollection["I0684"];
            graph.Record = graph.Database[id];

            Assert.True(!graph.IsRelated(relative), "Unexpected Relationship found");

            System.Console.WriteLine("Check 3");
            id = _reader.Parser.XrefCollection["I0668"];
            graph.Record = graph.Database[id];

            Assert.True(graph.IsRelated(relative), "Expected Relationship not found");
        }
    }
}
