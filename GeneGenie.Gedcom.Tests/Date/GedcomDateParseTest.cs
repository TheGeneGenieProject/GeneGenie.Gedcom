/*
 *  $Id: GedcomDateParseTest.cs 188 2008-09-27 14:42:14Z davek $
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

namespace GeneGenie.Gedcom.Date.Tests
{
    using GeneGenie.Gedcom.Parser;
    using System.Collections;
    using System.IO;
    using Xunit;

    public class GedcomDateParseTest
    {
        private GedcomRecordReader _reader;

        private int _parsedDates = 0;
        private int _notParsedDates = 0;

        private void DateCheck(GedcomDate date)
        {
            if (date != null)
            {
                if (!string.IsNullOrEmpty(date.Date1))
                {
                    if (date.DateTime1 != null && date.DateTime1.HasValue)
                    {
                        _parsedDates++;
                    }
                    else
                    {
                        _notParsedDates++;
                        System.Console.WriteLine("Unparsed: " + date.Date1);
                    }
                }
                if (!string.IsNullOrEmpty(date.Date2))
                {
                    if (date.DateTime2 != null && date.DateTime2.HasValue)
                    {
                        _parsedDates++;
                    }
                    else
                    {
                        _notParsedDates++;
                        System.Console.WriteLine("Unparsed: " + date.Date2);
                    }
                }
            }
        }

        private void Read(string file)
        {
            string dir = ".\\Data";
            string gedcomFile = Path.Combine(dir, file);

            _reader = new GedcomRecordReader();
            _reader.ReadGedcom(gedcomFile);

            Assert.True(_reader.Database.Count > 0, "No records read");

            _parsedDates = 0;
            _notParsedDates = 0;
            foreach (DictionaryEntry entry in _reader.Database)
            {
                GedcomRecord record = entry.Value as GedcomRecord;

                if (record.RecordType == GedcomRecordType.Individual)
                {
                    GedcomIndividualRecord indi = (GedcomIndividualRecord)record;

                    foreach (GedcomIndividualEvent ev in indi.Attributes)
                    {
                        DateCheck(ev.Date);
                    }

                    foreach (GedcomIndividualEvent ev in indi.Events)
                    {
                        DateCheck(ev.Date);
                    }
                }
                else if (record.RecordType == GedcomRecordType.Family)
                {
                    GedcomFamilyRecord fam = (GedcomFamilyRecord)record;

                    foreach (GedcomFamilyEvent ev in fam.Events)
                    {
                        DateCheck(ev.Date);
                    }
                }
            }

            System.Console.WriteLine(gedcomFile + ": parsed " + _parsedDates + "\t unparsed " + _notParsedDates);

            Assert.True(0 == _notParsedDates, "Unparsed Dates");
        }


        [Fact]
        public void Test1()
        {
            Read("test1.ged");
        }

        [Fact]
        public void Test2()
        {
            Read("test2.ged");
        }

        [Fact]
        public void Test3()
        {
            Read("test3.ged");
        }

        [Fact]
        public void Presidents()
        {
            Read("presidents.ged");
        }

        [Fact]
        public void Werrett()
        {
            Read("werrett.ged");
        }

        [Fact]
        public void Whereat()
        {
            Read("whereat.ged");
        }

        [Fact]
        public void Database1()
        {
            Read("Database1.ged");
        }

        [Fact]
        public void TGC551LF()
        {
            Read("TGC551LF.ged");
        }

        [Fact]
        public void Durand1()
        {
            Read("FAM_DD_4_2noms.ged");
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
        }
    }
}
