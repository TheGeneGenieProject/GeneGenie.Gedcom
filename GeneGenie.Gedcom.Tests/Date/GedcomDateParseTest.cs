// <copyright file="GedcomDateParseTest.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Date.Tests
{
    using System.Collections;
    using System.IO;
    using Enums;
    using GeneGenie.Gedcom.Parser;
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

            Assert.True(_notParsedDates == 0, "Unparsed Dates");
        }

        [Theory]
        [InlineData("presidents.ged")]
        private void Check_file_date_contents_can_be_parsed(string fileName)
        {
            Read(fileName);
        }
    }
}
