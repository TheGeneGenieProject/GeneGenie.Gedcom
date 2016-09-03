/*
 *  $Id: GedcomXMLGeneratorBase.cs 183 2008-06-08 15:31:15Z davek $
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
    using System;
    using System.Xml;

    public abstract class GedcomXMLGeneratorBase
    {
        protected GedcomDatabase _database;

        protected string _prefix = string.Empty;




        public GedcomXMLGeneratorBase()
        {
        }




        public GedcomDatabase Database
        {
            get { return _database; }
            set { _database = value; }
        }




        public abstract XmlDocument GenerateXML();

        protected void AppendHeader(XmlNode root)
        {
            XmlDocument doc = root.OwnerDocument;

            XmlNode header = doc.CreateElement("HeaderRec");

            XmlNode node;
            XmlAttribute attr;

            node = doc.CreateElement("FileCreation");
            attr = doc.CreateAttribute("Date");
            attr.Value = DateTime.Today.ToString("dd MMM yyyy");
            node.Attributes.Append(attr);
            attr = doc.CreateAttribute("Time");
            attr.Value = DateTime.Now.ToString("HH:mm:ss");
            node.Attributes.Append(attr);
            header.AppendChild(node);

            XmlNode productNode = doc.CreateElement("Product");
            node.AppendChild(productNode);

            node = doc.CreateElement("ProductID");
            node.AppendChild(doc.CreateTextNode("GEDCOM.NET"));
            productNode.AppendChild(node);

            node = doc.CreateElement("Version");
            node.AppendChild(doc.CreateTextNode("0.1"));
            productNode.AppendChild(node);

            node = doc.CreateElement("Name");
            node.AppendChild(doc.CreateTextNode("GEDCOM.NET"));
            productNode.AppendChild(node);

            node = doc.CreateElement("Supplier");
            node.AppendChild(doc.CreateTextNode("david@ritter.demon.co.uk"));
            productNode.AppendChild(node);

            node = doc.CreateElement("Note");
            node.AppendChild(doc.CreateTextNode("GEDCOM.NET Gedcom 6 output Test"));
            header.AppendChild(node);

            root.AppendChild(header);
        }

        protected void AppendFamily(GedcomFamilyRecord family, XmlNode root)
        {
            family.GenerateXML(root);
        }

        protected void AppendIndividual(GedcomIndividualRecord indi, XmlNode root)
        {
            indi.GenerateXML(root);
        }

        protected void AppendEvent(GedcomEvent ev, XmlNode root)
        {
            ev.GenerateXML(root);
        }



    }
}
