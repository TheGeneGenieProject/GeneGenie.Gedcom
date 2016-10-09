// <copyright file="GedcomXMLGeneratorTest.cs" company="GeneGenie.com">
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
#define XML_NODE_UNDEFINED

namespace GeneGenie.Gedcom.Reports
{
    using System.IO;
#if (!XML_NODE_UNDEFINED)
    using System.Xml;
    using GeneGenie.Gedcom.Parser;
    using Xunit;

    /// <summary>
    /// TODO: Understand what XML is being output and who consumes it.
    /// </summary>
    public class GedcomXMLGeneratorTest
    {
        private GedcomRecordReader reader;

        private void DumpXML(string file)
        {
            string dir = ".\\Data";
            string gedcomFile = Path.Combine(dir, file);

            reader = new GedcomRecordReader();
            reader.ReadGedcom(gedcomFile);

            GedcomXMLGenerator gen = new GedcomXMLGenerator();
            gen.Database = reader.Database;

            XmlDocument doc = gen.GenerateXML();

            string xmlOutput = Path.Combine(dir, "XmlOutput");
            if (!Directory.Exists(xmlOutput))
            {
                Directory.CreateDirectory(xmlOutput);
            }

            string xmlFile = Path.Combine(xmlOutput, file + ".xml");

            doc.Save(xmlFile);
        }

        [Theory]
        [InlineData("presidents.ged")]
        private void Test1(string fileName)
        {
            DumpXML(fileName);
        }
    }
    #endif

}
