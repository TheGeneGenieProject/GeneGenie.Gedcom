/*
 *  $Id: GedcomRecordReaderTest.cs 192 2008-11-01 21:36:29Z davek $
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

namespace GeneGenie.Gedcom.Parser
{
    using Gedcom.Enums;
    using System;
    using System.Collections;
    using System.IO;
    using Xunit;

    public class GedcomRecordReaderTest
    {
        private int _individuals;
        private int _families;

        private GedcomRecordReader Read(string file)
        {
            string dir = ".\\Data";
            string gedcomFile = Path.Combine(dir, file);

            long start = DateTime.Now.Ticks;
            var _reader = new GedcomRecordReader();
            bool success = _reader.ReadGedcom(gedcomFile);
            long end = DateTime.Now.Ticks;

            System.Console.WriteLine("Read time: " + TimeSpan.FromTicks(end - start).TotalSeconds + " seconds");

            Assert.True(success, "Failed to read " + gedcomFile);

            _individuals = 0;
            _families = 0;

            Assert.True(_reader.Database.Count > 0, "No records read");

            foreach (DictionaryEntry entry in _reader.Database)
            {
                var record = entry.Value as GedcomRecord;

                if (record.RecordType == GedcomRecordType.Individual)
                {
                    _individuals++;
                }
                else if (record.RecordType == GedcomRecordType.Family)
                {
                    _families++;
                }
            }

            System.Console.WriteLine(gedcomFile + " contains " + _individuals + " individuals");
            return _reader;
        }

        [Fact]
        public void Simple_file_with_one_family_can_be_read()
        {
            var reader = Read("simple.ged");

            Assert.Equal(1, reader.Database.Families.Count);
        }

        [Fact]
        public void Simple_file_with_3_people_can_be_read()
        {
            var reader = Read("simple.ged");

            Assert.Equal(3, reader.Database.Individuals.Count);
        }

        //[Fact]
        //public void Fan_file_with_19_generations_can_be_read()
        //{
        //    var reader = Read("ENFAN19.ged");

        //    Assert.Equal(1, reader.Database.Families.Count);
        //}

        [Fact]
        public void Test1()
        {
            Read("test1.ged");

            // file has 91 INDI, 1 is  HEAD/_SCHEMA/INDI though

            Assert.True(90 == _individuals, "Not read all individuals");
            Assert.True(15 == _families, "Not read all families");
        }


        [Fact]
        public void Test2()
        {
            Read("test2.ged");

            Assert.True(4 == _individuals, "Not read all individuals");
            Assert.True(2 == _families, "Not read all families");
        }

        [Fact]
        public void Test3()
        {
            Read("test3.ged");

            Assert.True(4 == _individuals, "Not read all individuals");
            Assert.True(2 == _families, "Not read all families");
        }

        [Fact]
        public void Presidents()
        {
            Read("presidents.ged");

            Assert.True(2145 == _individuals, "Not read all individuals");
            Assert.True(1042 == _families, "Not read all families");
        }

        [Fact]
        public void Werrett()
        {
            Read("werrett.ged");

            Assert.True(12338 == _individuals, "Not read all individuals");
            Assert.True(4206 == _families, "Not read all families");
        }

        [Fact]
        public void Whereat()
        {
            Read("whereat.ged");

            Assert.True(263 == _individuals, "Not read all individuals");
            Assert.True(78 == _families, "Not read all families");
        }

        [Fact]
        public void Database1()
        {
            Read("Database1.ged");

            // file has 24963 INDI, 1 is in a CONT

            Assert.True(24962 == _individuals, "Not read all individuals");
            Assert.True(8217 == _families, "Not read all families");
        }

        [Fact]
        public void TGC551LF()
        {
            var _reader = Read("TGC551LF.ged");

            GedcomHeader header = _reader.Database.Header;

            Assert.False(string.IsNullOrWhiteSpace(header.Copyright), "Missing copyright");

            Assert.True("John A. Nairn" == header.Submitter.Name, "Submitter not correctly read");

            Assert.True(null != header.ContentDescription, "Missing content description");

            Assert.True(null != header.CorporationAddress, "Missing corporation address");

            Assert.True(15 == _individuals, "Not read all individuals");
            Assert.True(7 == _families, "Not read all families");
        }

        [Fact]
        public void Durand1()
        {
            Read("FAM_DD_4_2noms.ged");

            Assert.True(5 == _individuals, "Not read all individuals");
            Assert.True(2 == _families, "Not read all families");
        }

        [Fact]
        public void Durand2()
        {
            Read("TOUT200801_unicode.ged");
        }

        [Fact]
        public void Durand3()
        {
            Read("test_gedcom-net.ged");
        }

        [Fact]
        public void Kollmann()
        {
            Read("Kollmann.ged");

            Assert.True(408 == _individuals, "Not read all individuals");
            Assert.True(156 == _families, "Not read all families");
        }
    }
}

