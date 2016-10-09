// <copyright file="GedcomXMLGenerator.cs" company="GeneGenie.com">
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

#if(!XML_NODE_UNDEFINED)
    using System.Xml;
#endif

    /// <summary>
    /// TODO Doc
    /// </summary>
    /// <seealso cref="GeneGenie.Gedcom.Reports.GedcomXMLGeneratorBase" />
    public class GedcomXMLGenerator : GedcomXMLGeneratorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomXMLGenerator"/> class.
        /// </summary>
        public GedcomXMLGenerator()
        {
        }

#if(!XML_NODE_UNDEFINED)

        /// <summary>
        /// Generates the XML.
        /// </summary>
        /// <returns>
        /// XML!
        /// </returns>
        public override XmlDocument GenerateXML()
        {
            GedcomIndividualReport report = new GedcomIndividualReport();
            report.Database = Database;
            report.AncestorGenerations = int.MaxValue;
            report.DecendantGenerations = -int.MaxValue;

            XmlDocument doc = report.GenerateXML();

            XmlNode root = doc.DocumentElement;

            foreach (GedcomSourceRecord source in Database.Sources)
            {
                source.GenerateXML(root);
            }

            foreach (GedcomRepositoryRecord repo in Database.Repositories)
            {
                repo.GenerateXML(root);
            }

            return doc;
        }
#endif

    }
}
