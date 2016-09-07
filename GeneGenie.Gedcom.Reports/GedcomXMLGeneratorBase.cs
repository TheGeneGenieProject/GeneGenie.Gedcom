// <copyright file="GedcomXMLGeneratorBase.cs" company="GeneGenie.com">
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
    using System;
    using System.Xml;

    /// <summary>
    /// TODO Doc
    /// </summary>
    public abstract class GedcomXMLGeneratorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomXMLGeneratorBase"/> class.
        /// </summary>
        public GedcomXMLGeneratorBase()
        {
        }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public GedcomDatabase Database { get; set; }

        /// <summary>
        /// Generates the XML.
        /// </summary>
        /// <returns>XML!</returns>
        public abstract XmlDocument GenerateXML();

        /// <summary>
        /// Appends the header.
        /// </summary>
        /// <param name="root">The root.</param>
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

        /// <summary>
        /// Appends the family.
        /// </summary>
        /// <param name="family">The family.</param>
        /// <param name="root">The root.</param>
        protected void AppendFamily(GedcomFamilyRecord family, XmlNode root)
        {
            family.GenerateXML(root);
        }

        /// <summary>
        /// Appends the individual.
        /// </summary>
        /// <param name="indi">The indi.</param>
        /// <param name="root">The root.</param>
        protected void AppendIndividual(GedcomIndividualRecord indi, XmlNode root)
        {
            indi.GenerateXML(root);
        }

        /// <summary>
        /// Appends the event.
        /// </summary>
        /// <param name="ev">The ev.</param>
        /// <param name="root">The root.</param>
        protected void AppendEvent(GedcomEvent ev, XmlNode root)
        {
            ev.GenerateXML(root);
        }
    }
}
