/*
 *  $Id: GedcomXMLGenerator.cs 183 2008-06-08 15:31:15Z davek $
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
    using System.Xml;

    public class GedcomXMLGenerator : GedcomXMLGeneratorBase
    {

        public GedcomXMLGenerator()
        {
        }




        public override XmlDocument GenerateXML()
        {
            GedcomIndividualReport report = new GedcomIndividualReport();
            report.Database = _database;
            report.AncestorGenerations = int.MaxValue;
            report.DecendantGenerations = -int.MaxValue;

            XmlDocument doc = report.GenerateXML();

            XmlNode root = doc.DocumentElement;

            foreach (GedcomSourceRecord source in _database.Sources)
            {
                source.GenerateXML(root);
            }

            foreach (GedcomRepositoryRecord repo in _database.Repositories)
            {
                repo.GenerateXML(root);
            }

            return doc;
        }



    }
}
