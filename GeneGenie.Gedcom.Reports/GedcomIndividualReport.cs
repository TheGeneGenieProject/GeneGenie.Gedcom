/*
 *  $Id: GedcomIndividualReport.cs 187 2008-08-17 13:28:47Z davek $
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
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Xsl;
    using System.Xml.XPath;
    using System.Reflection;

    public class GedcomIndividualReport : GedcomXMLGeneratorBase
    {
        public enum GedcomReportFormat
        {
            Doc,
            Html,
            Pdf,
            Svg,
            Txt,
            Xls
        };


        protected GedcomRecord _record;

        protected GedcomIndividualRecord _indi;

        protected int _ancestorGenerations = 1;
        protected int _decendantGenerations = -1;

        protected string _baseXsltName;

        protected string _xrefId;

        protected List<string> _processed;



        public GedcomIndividualReport()
        {
        }




        public GedcomRecord Record
        {
            get { return _record; }
            set
            {
                if (_database == null)
                {
                    throw new Exception("Database must be set before Record");
                }

                _record = value;

                if (_record.RecordType != GedcomRecordType.Individual)
                {
                    throw new Exception("Non individual record set on Individual Report");
                }

                _indi = (GedcomIndividualRecord)_record;

                if (string.IsNullOrEmpty(_xrefId))
                {
                    _xrefId = _indi.XRefID;
                }
            }
        }

        public int AncestorGenerations
        {
            get { return _ancestorGenerations; }
            set { _ancestorGenerations = value; }
        }

        public int DecendantGenerations
        {
            get { return _decendantGenerations; }
            set { _decendantGenerations = value; }
        }

        public string BaseXsltName
        {
            get { return _baseXsltName; }
            set { _baseXsltName = value; }
        }

        public string XrefID
        {
            get { return _xrefId; }
            set { _xrefId = value; }
        }


        private void AppendSources(GedcomRecord record, XmlNode root)
        {
            foreach (GedcomSourceCitation citation in record.Sources)
            {
                string sourceId = citation.Source;
                if (!_processed.Contains(sourceId))
                {
                    _processed.Add(sourceId);

                    GedcomSourceRecord source = _database[sourceId] as GedcomSourceRecord;
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

            if (!_processed.Contains(famID))
            {
                _processed.Add(famID);

                GedcomFamilyRecord fam = _database[famID] as GedcomFamilyRecord;

                if (fam != null)
                {
                    foreach (GedcomFamilyEvent famEvent in fam.Events)
                    {
                        famEvent.EventXRefID = _database.GenerateXref("EVENT");
                        AppendEvent(famEvent, root);

                        AppendSources(famEvent, root);
                    }

                    AppendFamily(fam, root);

                    if (!string.IsNullOrEmpty(fam.Husband))
                    {
                        GedcomIndividualRecord husb = _database[fam.Husband] as GedcomIndividualRecord;
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
                        GedcomIndividualRecord wife = _database[fam.Wife] as GedcomIndividualRecord;
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
                        GedcomIndividualRecord child = _database[childID] as GedcomIndividualRecord;
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

        private void AppendIndividualDetails(GedcomIndividualRecord indi, XmlNode root, int generation)
        {
            if (!_processed.Contains(indi.XRefID))
            {
                _processed.Add(indi.XRefID);

                foreach (GedcomIndividualEvent indiEvent in indi.Events)
                {
                    indiEvent.EventXRefID = _database.GenerateXref("EVENT");
                    AppendEvent(indiEvent, root);
                    AppendSources(indiEvent, root);
                }
                AppendIndividual(indi, root);

                if (generation < _ancestorGenerations)
                {
                    foreach (GedcomFamilyLink link in indi.ChildIn)
                    {
                        AppendFamilyDetails(link, root, generation + 1);
                    }
                }

                if (generation > _decendantGenerations)
                {
                    foreach (GedcomFamilyLink link in indi.SpouseIn)
                    {
                        AppendFamilyDetails(link, root, generation);
                    }
                }
            }
        }

        public override XmlDocument GenerateXML()
        {
            _processed = new List<string>();

            XmlDocument doc = new XmlDocument();

            XmlNode root = doc.CreateElement("GEDCOM");

            doc.AppendChild(root);

            AppendHeader(root);

            if (_indi != null)
            {
                AppendIndividualDetails(_indi, root, 0);
            }
            else
            {
                foreach (GedcomIndividualRecord indi in _database.Individuals)
                {
                    AppendIndividualDetails(indi, root, 0);
                }

                foreach (GedcomSourceRecord source in _database.Sources)
                {
                    if (!_processed.Contains(source.XRefID))
                    {
                        _processed.Add(source.XRefID);
                        source.GenerateXML(root);
                    }
                }

                foreach (GedcomRepositoryRecord repo in _database.Repositories)
                {
                    repo.GenerateXML(root);
                }
            }

            _processed = null;

            return doc;
        }

        public string Transform(XmlDocument doc, GedcomReportFormat format)
        {
            string xslt = string.Format("Gedcom.Reports.Xslt.{0}-{1}.xslt", _baseXsltName, format.ToString());

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

                args.AddParam("xrefId", string.Empty, _xrefId);

                tmpFile = Path.GetTempFileName();

                using (FileStream file = File.Open(tmpFile, FileMode.Append))
                {
                    transform.Transform(nav, args, file);
                }

            }

            return tmpFile;
        }

        public string CreateReport()
        {
            XmlDocument doc = GenerateXML();
            string filename = Transform(doc, GedcomReportFormat.Html);

            return filename;
        }




    }
}
