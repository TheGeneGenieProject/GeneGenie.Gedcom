// <copyright file="GedcomIndividualMatchTest.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom
{
    using System.IO;
    using GeneGenie.Gedcom.Parser;
    using Xunit;

    /// <summary>
    /// TODO: Rewrite into smaller focussed tests against smaller files, strings if possible.
    /// </summary>
    public class GedcomIndividualMatchTest
    {
        private GedcomRecordReader _reader;
        private GedcomDatabase _database;

        private void Read(string file)
        {
            string dir = ".\\Data";
            string gedcomFile = Path.Combine(dir, file);

            _reader = new GedcomRecordReader();
            _reader.ReadGedcom(gedcomFile);
            _database = _reader.Database;

            Assert.True(_reader.Database.Count > 0, "No records read");
        }

        [Fact(Skip = "Needs rewriting as many smaller tests, file no longer exists.")]
        private void Test1()
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
