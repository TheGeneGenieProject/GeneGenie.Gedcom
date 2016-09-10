// <copyright file="GedcomXMLTransformTest.cs" company="GeneGenie.com">
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
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using Xunit;

    /// <summary>
    /// Tests that a GEDCOM file can be read and then reported on.
    /// Currently missing a .XSL file from the original fork.
    /// </summary>
    public class GedcomXMLTransformTest
    {
        [Theory(Skip = "Recreate the missing XSL file and get this report test working.")]
        [InlineData("presidents.ged")]
        private void Test1(string fileName)
        {
            DumpXML(fileName);
        }

        private void DumpXML(string file)
        {
            string dir = ".\\Data";

            string xmlOutput = Path.Combine(dir, "XmlOutput");
            if (!Directory.Exists(xmlOutput))
            {
                Directory.CreateDirectory(xmlOutput);
            }

            string xmlFile = Path.Combine(xmlOutput, file + ".xml");

            string xslFileDir = ".\\Data\\Xsl";
            if (!Directory.Exists(xslFileDir))
            {
                Directory.CreateDirectory(xslFileDir);
            }

            string xslFile = Path.Combine(xslFileDir, "Surnames.xsl");

            XPathDocument doc = new XPathDocument(xmlFile);

            var transform = new XslCompiledTransform();
            transform.Load(xslFile);

            transform.Transform(doc, null, System.Console.Out);
            System.Console.Out.Flush();
        }
    }
}
