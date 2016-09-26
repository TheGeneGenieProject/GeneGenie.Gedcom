// <copyright file="GedcomRepositoryCitation.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom
{
    using System;
    using System.IO;
    using System.Xml;
    using Enums;

    /// <summary>
    /// TODO: Doc
    /// </summary>
    /// <seealso cref="GedcomRecord" />
    public class GedcomRepositoryCitation : GedcomRecord
    {
        private string repository;

        private GedcomRecordList<string> callNumbers;
        private GedcomRecordList<SourceMediaType> mediaTypes;

        // This is a hack for broken GEDCOM files that misuse MEDI
        private GedcomRecordList<string> otherMediaTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomRepositoryCitation"/> class.
        /// </summary>
        public GedcomRepositoryCitation()
        {
            callNumbers = new GedcomRecordList<string>();
            callNumbers.Changed += ListChanged;

            mediaTypes = new GedcomRecordList<SourceMediaType>();
            mediaTypes.Changed += ListChanged;
        }

        /// <summary>
        /// Gets the type of the record.
        /// </summary>
        /// <value>
        /// The type of the record.
        /// </value>
        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.RepositoryCitation; }
        }

        /// <summary>
        /// Gets the gedcom tag.
        /// </summary>
        /// <value>
        /// The gedcom tag.
        /// </value>
        public override string GedcomTag
        {
            get { return "REPO"; }
        }

        /// <summary>
        /// Gets or sets the repository.
        /// </summary>
        /// <value>
        /// The repository.
        /// </value>
        public string Repository
        {
            get
            {
                return repository;
            }

            set
            {
                if (value != repository)
                {
                    repository = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets the call numbers.
        /// </summary>
        /// <value>
        /// The call numbers.
        /// </value>
        public GedcomRecordList<string> CallNumbers
        {
            get { return callNumbers; }
        }

        /// <summary>
        /// Gets the media types.
        /// </summary>
        /// <value>
        /// The media types.
        /// </value>
        public GedcomRecordList<SourceMediaType> MediaTypes
        {
            get { return mediaTypes; }
        }

        /// <summary>
        /// Gets or sets the other media types.
        /// </summary>
        /// <value>
        /// The other media types.
        /// </value>
        public GedcomRecordList<string> OtherMediaTypes
        {
            get
            {
                if (otherMediaTypes == null)
                {
                    otherMediaTypes = new GedcomRecordList<string>();
                    otherMediaTypes.Changed += ListChanged;
                }

                return otherMediaTypes;
            }

            set
            {
                if (value != OtherMediaTypes)
                {
                    OtherMediaTypes = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public override void Delete()
        {
            base.Delete();

            GedcomRepositoryRecord repo = (GedcomRepositoryRecord)Database[Repository];

            repo.Citations.Remove(this);

            repo.Delete();
        }

        /// <summary>
        /// Generates the XML.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="num">The number.</param>
        public void GenerateXML(XmlNode root, int num)
        {
            XmlDocument doc = root.OwnerDocument;

            XmlNode node = doc.CreateElement("Repository");

            if (!string.IsNullOrEmpty(Repository))
            {
                XmlNode repoLink = doc.CreateElement("Link");

                XmlAttribute attr = doc.CreateAttribute("Target");
                attr.Value = "RepositoryRec";
                repoLink.Attributes.Append(attr);

                attr = doc.CreateAttribute("Ref");
                attr.Value = Repository;
                repoLink.Attributes.Append(attr);

                node.AppendChild(repoLink);
            }

            // GEDCOM 6 doesn't map to GEDCOM 5.5 very well,
            // have to do some work in GedcomSourceRec to output one SourceRec
            // per call number in each citation
            string callNumber = CallNumbers[num];

            XmlNode callNo = doc.CreateElement("CallNbr");
            callNo.AppendChild(doc.CreateTextNode(callNumber));

            node.AppendChild(callNo);

            root.AppendChild(node);
        }

        /// <summary>
        /// Outputs the specified sw.
        /// </summary>
        /// <param name="sw">The sw.</param>
        public override void Output(TextWriter sw)
        {
            sw.Write(Environment.NewLine);
            sw.Write(Util.IntToString(Level));
            sw.Write(" ");
            sw.Write(GedcomTag);

            if (!string.IsNullOrEmpty(Repository))
            {
                sw.Write(" @");
                sw.Write(Repository);
                sw.Write("@ ");
            }

            OutputStandard(sw);

            if (CallNumbers.Count > 0)
            {
                string levelPlusOne = Util.IntToString(Level + 1);
                string levelPlusTwo = null;

                int i = 0;
                foreach (string callNumber in CallNumbers)
                {
                    sw.Write(Environment.NewLine);
                    sw.Write(levelPlusOne);
                    sw.Write(" CALN ");
                    string line = callNumber.Replace("@", "@@");
                    sw.Write(line);

                    SourceMediaType mediaType = MediaTypes[i];
                    int otherIndex = 0;
                    if (mediaType != SourceMediaType.None)
                    {
                        if (levelPlusTwo == null)
                        {
                            levelPlusTwo = Util.IntToString(Level + 2);
                        }

                        sw.Write(Environment.NewLine);
                        sw.Write(levelPlusTwo);
                        sw.Write(" MEDI ");
                        if (mediaType != SourceMediaType.Other)
                        {
                            string type = mediaType.ToString().Replace('_', ' ');
                            sw.Write(type);
                        }
                        else
                        {
                            sw.Write(OtherMediaTypes[otherIndex++]);
                        }
                    }

                    i++;
                }
            }
        }

        /// <summary>
        /// Compare the user entered data against the passed instance for similarity.
        /// </summary>
        /// <param name="obj">The object to compare this instance against.</param>
        /// <returns>
        /// True if instance matches user data, otherwise false.
        /// </returns>
        public override bool IsEquivalentTo(object obj)
        {
            var repository = obj as GedcomRepositoryCitation;

            if (repository == null)
            {
                return false;
            }

            if (!GedcomGenericListComparer.CompareLists(CallNumbers, repository.CallNumbers))
            {
                return false;
            }

            if (!GedcomGenericListComparer.CompareLists(MediaTypes, repository.MediaTypes))
            {
                return false;
            }

            if (!GedcomGenericListComparer.CompareLists(OtherMediaTypes, repository.OtherMediaTypes))
            {
                return false;
            }

            if (!Equals(Repository, repository.Repository))
            {
                return false;
            }

            return true;
        }
    }
}
