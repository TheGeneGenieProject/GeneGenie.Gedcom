/*
 *  $Id: GedcomIndividualMatchTest.cs 183 2008-06-08 15:31:15Z davek $
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

namespace GeneGenie.Gedcom
{
    using GeneGenie.Gedcom.Parser;
    using System.IO;
    using Xunit;

    public class GedcomIndividualMatchTest
    {
        private GedcomRecordReader _reader;
        private GedcomDatabase _database;

        public GedcomIndividualMatchTest()
        {
        }

        private void Read(string file)
        {
            string dir = ".\\Data";
            string gedcomFile = Path.Combine(dir, file);

            _reader = new GedcomRecordReader();
            _reader.ReadGedcom(gedcomFile);
            _database = _reader.Database;

            Assert.True(_reader.Database.Count > 0, "No records read");
        }

        [Fact]
        public void Test1()
        {
            Read("test1.ged");

            System.Console.WriteLine("Check 1");

            // Elizabeth Rutherford
            // BIRT 29 Jan 1811

            string id = _reader.Parser.XrefCollection["I0115"];

            System.Console.WriteLine("I0115 maps to " + id);

            GedcomIndividualRecord indi = (GedcomIndividualRecord)_database[id];
            GedcomIndividualRecord indi2 = (GedcomIndividualRecord)_database[id];

            float match = indi.IsMatch(indi2);

            System.Console.WriteLine("Match: " + match.ToString());

            Assert.True(match == 100.0F, "Individual failed to match themself");

            // Freddie Vashti Adams
            // BIRT 25 Aug 1906
            // DEAT 31 Mar 2991

            System.Console.WriteLine("Check 2");

            id = _reader.Parser.XrefCollection["I0684"];

            System.Console.WriteLine("I0684 maps to " + id);

            indi2 = (GedcomIndividualRecord)_database[id];

            match = indi.IsMatch(indi2);

            System.Console.WriteLine("Match: " + match.ToString());

            Assert.True(match != 100.0F, "Individual matched another 100%");

            foreach (GedcomIndividualRecord indiRec in _database.Individuals)
            {
                match = indi.IsMatch(indiRec);
                if (match > 50)
                {
                    System.Console.WriteLine(match + "% match on " + indiRec.XRefID + " " + indiRec.Names[0].Name);
                }
            }
        }

    }
}
