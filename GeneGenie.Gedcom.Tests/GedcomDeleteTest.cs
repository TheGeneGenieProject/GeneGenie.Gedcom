/*
 *  $Id: GedcomDeleteTest.cs 199 2008-11-15 15:20:44Z davek $
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
    using System;
    using System.Collections;
    using System.IO;
    using Xunit;
    using GeneGenie.Gedcom.Parser;
    using Enums;

    public class GedcomDeleteTest
    {
        private GedcomRecordReader _reader;
        private int _individuals;
        private int _families;

        private void Read(string file)
        {
            string dir = ".\\Data";
            string gedcomFile = Path.Combine(dir, file);

            long start = DateTime.Now.Ticks;
            _reader = new GedcomRecordReader();
            bool success = _reader.ReadGedcom(gedcomFile);
            long end = DateTime.Now.Ticks;

            Assert.True(success, "Failed to read " + gedcomFile);

            _individuals = 0;
            _families = 0;

            Assert.True(_reader.Database.Count > 0, "No records read");

            foreach (DictionaryEntry entry in _reader.Database)
            {
                GedcomRecord record = entry.Value as GedcomRecord;

                if (record.RecordType == GedcomRecordType.Individual)
                {
                    _individuals++;
                }
                else if (record.RecordType == GedcomRecordType.Family)
                {
                    _families++;
                }
            }
        }


        [Fact]
        public void Test1()
        {
            Read("test1.ged");

            Assert.True(90 == _individuals, "Not read all individuals");
            Assert.True(15 == _families, "Not read all families");

            string id = _reader.Parser.XrefCollection["I0145"];
            string sourceID = _reader.Parser.XrefCollection["S01668"];
            string sourceID2 = _reader.Parser.XrefCollection["S10021"];

            GedcomIndividualRecord indi = (GedcomIndividualRecord)_reader.Database[id];

            Assert.True(_reader.Database[sourceID] != null, "Unable to find expected source");
            Assert.True(_reader.Database[sourceID2] != null, "Unable to find expected source 2");

            indi.Delete();

            Assert.True(89 == _reader.Database.Individuals.Count, "Failed to delete individual");
            Assert.True(15 == _reader.Database.Families.Count, "Incorrectly erased family");

            Assert.True(_reader.Database[sourceID2] != null, "Source incorrectly deleted when deleting individual");

            // source should still have a count of 1, the initial ref, we don't want to delete just because all citations
            // have gone, leave the source in the database.
            Assert.True(_reader.Database[sourceID] != null, "Source incorrectly deleted when only used by the deleted individual");
        }

        [Fact]
        public void Test2()
        {
            Read("test2.ged");

            Assert.True(4 == _individuals, "Not read all individuals");
            Assert.True(2 == _families, "Not read all families");

            string id = _reader.Parser.XrefCollection["I04"];

            GedcomIndividualRecord indi = (GedcomIndividualRecord)_reader.Database[id];

            indi.Delete();


            Assert.True(3 == _reader.Database.Individuals.Count, "Failed to delete individual");
            Assert.True(2 == _reader.Database.Families.Count, "Incorrectly erased family");

            id = _reader.Parser.XrefCollection["I01"];

            indi = (GedcomIndividualRecord)_reader.Database[id];

            indi.Delete();

            Assert.True(2 == _reader.Database.Individuals.Count, "Failed to delete individual");
            Assert.True(1 == _reader.Database.Families.Count, "Incorrectly erased family");

            id = _reader.Parser.XrefCollection["I02"];

            indi = (GedcomIndividualRecord)_reader.Database[id];

            indi.Delete();

            Assert.True(1 == _reader.Database.Individuals.Count, "Failed to delete individual");
            Assert.True(1 == _reader.Database.Families.Count, "Incorrectly erased family");

            GedcomFamilyRecord famRec = _reader.Database.Families[0];
            string noteID = famRec.Notes[0];

            Assert.True(_reader.Database[noteID] != null, "Couldn't find expected note on family");

            id = _reader.Parser.XrefCollection["I03"];

            indi = (GedcomIndividualRecord)_reader.Database[id];

            indi.Delete();

            Assert.True(0 == _reader.Database.Individuals.Count, "Failed to delete individual");
            Assert.True(0 == _reader.Database.Families.Count, "Incorrectly erased family");

            Assert.True(null != _reader.Database[noteID], "Incorrectly erased note from family");
        }

        [Fact]
        public void Test3()
        {
            Read("test3.ged");

            Assert.True(4 == _individuals, "Not read all individuals");
            Assert.True(2 == _families, "Not read all families");

            string id = _reader.Parser.XrefCollection["I04"];

            GedcomIndividualRecord indi = (GedcomIndividualRecord)_reader.Database[id];

            indi.Delete();


            Assert.True(3 == _reader.Database.Individuals.Count, "Failed to delete individual");
            Assert.True(2 == _reader.Database.Families.Count, "Incorrectly erased family");

            id = _reader.Parser.XrefCollection["I01"];

            indi = (GedcomIndividualRecord)_reader.Database[id];

            indi.Delete();

            Assert.True(2 == _reader.Database.Individuals.Count, "Failed to delete individual");
            Assert.True(1 == _reader.Database.Families.Count, "Incorrectly erased family");

            id = _reader.Parser.XrefCollection["I02"];

            indi = (GedcomIndividualRecord)_reader.Database[id];

            indi.Delete();

            Assert.True(1 == _reader.Database.Individuals.Count, "Failed to delete individual");
            Assert.True(1 == _reader.Database.Families.Count, "Incorrectly erased family");

            GedcomFamilyRecord famRec = _reader.Database.Families[0];
            string noteID = famRec.Notes[0];

            Assert.True(_reader.Database[noteID] != null, "Couldn't find expected note on family");

            id = _reader.Parser.XrefCollection["I03"];

            indi = (GedcomIndividualRecord)_reader.Database[id];

            indi.Delete();

            Assert.True(0 == _reader.Database.Individuals.Count, "Failed to delete individual");
            Assert.True(0 == _reader.Database.Families.Count, "Incorrectly erased family");

            Assert.True(null != _reader.Database[noteID], "Incorrectly erased note linked from family");
        }
    }
}
