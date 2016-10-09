// <copyright file="GedcomIndividualReport.cs" company="GeneGenie.com">
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
#if (!XML_NODE_UNDEFINED)
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;
#endif
    using Enums;
    using Gedcom.Enums;

    /// <summary>
    /// TODO Doc
    /// </summary>
    /// <seealso cref="GeneGenie.Gedcom.Reports.GedcomXMLGeneratorBase" />
    public class GedcomIndividualReport : GedcomXMLGeneratorBase
    {
        private GedcomRecord record;

        private GedcomIndividualRecord indi;

        private List<string> processed;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomIndividualReport"/> class.
        /// </summary>
        public GedcomIndividualReport()
        {
        }

        /// <summary>
        /// Gets or sets the record.
        /// </summary>
        /// <exception cref="System.Exception">
        /// Database must be set before Record
        /// or
        /// Non individual record set on Individual Report
        /// </exception>
        public GedcomRecord Record
        {
            get
            {
                return record;
            }

            set
            {
                if (Database == null)
                {
                    throw new Exception("Database must be set before Record");
                }

                record = value;
                if (record.RecordType != GedcomRecordType.Individual)
                {
                    throw new Exception("Non individual record set on Individual Report");
                }

                indi = (GedcomIndividualRecord)record;
                if (string.IsNullOrEmpty(XrefId))
                {
                    XrefId = indi.XRefID;
                }
            }
        }

        /// <summary>
        /// Gets or sets the ancestor generations.
        /// </summary>
        public int AncestorGenerations { get; set; } = 1;

        /// <summary>
        /// Gets or sets the decendant generations.
        /// </summary>
        /// <value>
        /// The decendant generations.
        /// </value>
        public int DecendantGenerations { get; set; } = -1;

        /// <summary>
        /// Gets or sets the name of the base XSLT.
        /// </summary>
        public string BaseXsltName { get; set; }

        /// <summary>
        /// Gets or sets the xref identifier.
        /// </summary>
        public string XrefId { get; set; }

#if (!XML_NODE_UNDEFINED)

        /// <summary>
        /// Generates the XML.
        /// </summary>
        /// <returns>
        /// XML!
        /// </returns>
        public override XmlDocument GenerateXML()
        {
            processed = new List<string>();

            XmlDocument doc = new XmlDocument();

            XmlNode root = doc.CreateElement("GEDCOM");

            doc.AppendChild(root);

            AppendHeader(root);

            if (indi != null)
            {
                AppendIndividualDetails(indi, root, 0);
            }
            else
            {
                foreach (GedcomIndividualRecord indi in Database.Individuals)
                {
                    AppendIndividualDetails(indi, root, 0);
                }

                foreach (GedcomSourceRecord source in Database.Sources)
                {
                    if (!processed.Contains(source.XRefID))
                    {
                        processed.Add(source.XRefID);
                        source.GenerateXML(root);
                    }
                }

                foreach (GedcomRepositoryRecord repo in Database.Repositories)
                {
                    repo.GenerateXML(root);
                }
            }

            processed = null;

            return doc;
        }

        /// <summary>
        /// Transforms the specified document.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="format">The format.</param>
        /// <returns>The transformed content.</returns>
        public string Transform(XmlDocument doc, GedcomReportFormat format)
        {
            string xslt = string.Format("Gedcom.Reports.Xslt.{0}-{1}.xslt", BaseXsltName, format.ToString());

            Assembly asm = GetType().Assembly;
            Stream xsltStream = asm.GetManifestResourceStream(xslt);

            string tmpFile = null;

            if (xsltStream != null)
            {
                XmlTextReader reader = new XmlTextReader(xsltStream);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(reader);

                XPathNavigator nav = doc.CreateNavigator();
                XsltArgumentList args = new XsltArgumentList();

                args.AddParam("xrefId", string.Empty, XrefId);

                tmpFile = Path.GetTempFileName();

                using (FileStream file = File.Open(tmpFile, FileMode.Append))
                {
                    transform.Transform(nav, args, file);
                }
            }

            return tmpFile;
        }

        /// <summary>
        /// Creates the report.
        /// </summary>
        /// <returns>Path to the report.</returns>
        public string CreateReport()
        {
            XmlDocument doc = GenerateXML();
            string filename = Transform(doc, GedcomReportFormat.Html);

            return filename;
        }

        private void AppendSources(GedcomRecord record, XmlNode root)
        {
            foreach (GedcomSourceCitation citation in record.Sources)
            {
                string sourceId = citation.Source;
                if (!processed.Contains(sourceId))
                {
                    processed.Add(sourceId);

                    GedcomSourceRecord source = Database[sourceId] as GedcomSourceRecord;
                    if (source != null)
                    {
                        source.GenerateXML(root);
                    }
                    else
                    {
                        throw new Exception("Source citation references non existant source");
                    }
                }
            }
        }

        private void AppendFamilyDetails(GedcomFamilyLink link, XmlNode root, int generation)
        {
            string famID = link.Family;

            if (!processed.Contains(famID))
            {
                processed.Add(famID);

                GedcomFamilyRecord fam = Database[famID] as GedcomFamilyRecord;

                if (fam != null)
                {
                    foreach (GedcomFamilyEvent famEvent in fam.Events)
                    {
                        famEvent.EventXRefID = Database.GenerateXref("EVENT");
                        AppendEvent(famEvent, root);

                        AppendSources(famEvent, root);
                    }

                    AppendFamily(fam, root);

                    if (!string.IsNullOrEmpty(fam.Husband))
                    {
                        GedcomIndividualRecord husb = Database[fam.Husband] as GedcomIndividualRecord;
                        if (husb != null)
                        {
                            AppendIndividualDetails(husb, root, generation);
                        }
                        else
                        {
                            throw new Exception("Husband points to non individual record");
                        }
                    }

                    if (!string.IsNullOrEmpty(fam.Wife))
                    {
                        GedcomIndividualRecord wife = Database[fam.Wife] as GedcomIndividualRecord;
                        if (wife != null)
                        {
                            AppendIndividualDetails(wife, root, generation);
                        }
                        else
                        {
                            throw new Exception("Husband points to non individual record");
                        }
                    }

                    foreach (string childID in fam.Children)
                    {
                        GedcomIndividualRecord child = Database[childID] as GedcomIndividualRecord;
                        if (child != null)
                        {
                            int childGeneration = generation - 1;
                            AppendIndividualDetails(child, root, childGeneration);
                        }
                        else
                        {
                            throw new Exception("Child points to non individual record");
                        }
                    }
                }
                else
                {
                    throw new Exception("Family link points to non family record");
                }
            }
        }
#endif

#if (!XML_NODE_UNDEFINED)

        private void AppendIndividualDetails(GedcomIndividualRecord indi, XmlNode root, int generation)
        {
            if (!processed.Contains(indi.XRefID))
            {
                processed.Add(indi.XRefID);

                foreach (GedcomIndividualEvent indiEvent in indi.Events)
                {
                    indiEvent.EventXRefID = Database.GenerateXref("EVENT");
                    AppendEvent(indiEvent, root);
                    AppendSources(indiEvent, root);
                }

                AppendIndividual(indi, root);

                if (generation < AncestorGenerations)
                {
                    foreach (GedcomFamilyLink link in indi.ChildIn)
                    {
                        AppendFamilyDetails(link, root, generation + 1);
                    }
                }

                if (generation > DecendantGenerations)
                {
                    foreach (GedcomFamilyLink link in indi.SpouseIn)
                    {
                        AppendFamilyDetails(link, root, generation);
                    }
                }
            }
        }
#endif
    }
}
