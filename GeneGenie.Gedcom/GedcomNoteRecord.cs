/*
 *  $Id: GedcomNoteRecord.cs 200 2008-11-30 14:34:07Z davek $
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

namespace GeneGenie.Gedcom
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;

    public class GedcomNoteRecord : GedcomRecord
    {
        private string _Text;

        // hack
        public StringBuilder ParsedText;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomNoteRecord"/> class.
        /// </summary>
        public GedcomNoteRecord()
        {
            ParsedText = new StringBuilder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomNoteRecord"/> class.
        /// </summary>
        /// <param name="database"></param>
        public GedcomNoteRecord(GedcomDatabase database)
            : this()
        {
            Level = 0;
            Database = database;
            XRefID = database.GenerateXref("NOTE");
            Text = string.Empty;

            database.Add(XRefID, this);
        }

        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Note; }
        }

        public override string GedcomTag
        {
            get { return "NOTE"; }
        }

        public string Text
        {
            get
            {
                return _Text;
            }

            set
            {
                if (value != _Text)
                {
                    _Text = value;
                    Changed();
                }
            }
        }

        public override void GenerateXML(XmlNode root)
        {
            XmlDocument doc = root.OwnerDocument;

            XmlNode node = doc.CreateElement("Note");

            XmlCDataSection data = doc.CreateCDataSection(Text);
            node.AppendChild(data);

            root.AppendChild(node);
        }

        public override void Output(TextWriter sw)
        {
            sw.Write(Environment.NewLine);
            sw.Write(Util.IntToString(Level));
            sw.Write(" ");

            if (!string.IsNullOrEmpty(_XrefID))
            {
                sw.Write("@");
                sw.Write(_XrefID);
                sw.Write("@ ");
            }

            sw.Write("NOTE ");

            if (!string.IsNullOrEmpty(Text))
            {
                Util.SplitLineText(sw, Text, Level, 248);
            }

            OutputStandard(sw);
        }
    }
}
