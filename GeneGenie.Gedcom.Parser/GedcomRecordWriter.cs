// <copyright file="GedcomRecordWriter.cs" company="GeneGenie.com">
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
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>
// <author> Copyright (C) 2007-2008 David A Knight david@ritter.demon.co.uk </author>

namespace GeneGenie.Gedcom.Parser
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Used to save a GedcomDatabase to a GEDCOM file.
    /// </summary>
    public class GedcomRecordWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomRecordWriter"/> class.
        /// Create a GEDCOM writer for saving a database to a GEDCOM file.
        /// </summary>
        public GedcomRecordWriter()
        {
        }

        /// <summary>
        /// Gets or sets the name of the GEDCOM file being written.
        /// </summary>
        public string GedcomFile { get; set; }

        /// <summary>
        /// Gets or sets the database for the file being written.
        /// </summary>
        public GedcomDatabase Database { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use test mode.
        /// A flag to indiciate if this is a test output,
        /// used in test code to ensure the output can match
        /// against the expected output by making the date the same
        /// </summary>
        public bool Test { get; set; }

        /// <summary>
        /// Gets or sets the name of the application that created the GEDCOM file.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the application version that created the GEDCOME file.
        /// </summary>
        public string ApplicationVersion { get; set; }

        /// <summary>
        /// Gets or sets the application system identifier.
        /// </summary>
        public string ApplicationSystemId { get; set; }

        /// <summary>
        /// Gets or sets the owner name for the software that created the GEDCOM.
        /// </summary>
        public string Corporation { get; set; }

        /// <summary>
        /// Gets or sets the corporation address.
        /// </summary>
        /// <summary>
        /// The corporation address.
        /// </summary>
        public GedcomAddress CorporationAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use the information separator when saving.
        /// </summary>
        /// <summary>
        /// <c>true</c> if [allow information separator on save]; otherwise, <c>false</c>.
        /// </summary>
        public bool AllowInformationSeparatorOnSave { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow line tabs save].
        /// </summary>
        /// <summary>
        ///   <c>true</c> if [allow line tabs save]; otherwise, <c>false</c>.
        /// </summary>
        public bool AllowLineTabsSave { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow tabs save].
        /// </summary>
        /// <summary>
        ///   <c>true</c> if [allow tabs save]; otherwise, <c>false</c>.
        /// </summary>
        public bool AllowTabsSave { get; set; }

        /// <summary>
        /// Outputs the currently set GedcomDatabase to the currently set file
        /// </summary>
        public void WriteGedcom()
        {
            WriteGedcom(Database, GedcomFile);
        }

        /// <summary>
        /// Outputs a GedcomDatabase to the given file
        /// </summary>
        /// <param name="database">The GedcomDatabase to write</param>
        /// <param name="file">The filename to write to</param>
        public void WriteGedcom(GedcomDatabase database, string file)
        {
            Encoding enc = new UTF8Encoding();
            using (GedcomStreamWriter w = new GedcomStreamWriter(file, false, enc))
            {
                w.AllowInformationSeparatorOnSave = AllowInformationSeparatorOnSave;
                w.AllowLineTabsSave = AllowLineTabsSave;
                w.AllowTabsSave = AllowTabsSave;

                // write header
                GedcomHeader header = Database.Header;
                header.Test = Test;
                header.Filename = file;

                header.ApplicationName = ApplicationName;
                header.ApplicationSystemID = ApplicationSystemId;
                header.ApplicationVersion = ApplicationVersion;
                header.Corporation = Corporation;
                header.CorporationAddress = CorporationAddress;

                header.Output(w);

                // write records
                foreach (DictionaryEntry entry in database)
                {
                    GedcomRecord record = entry.Value as GedcomRecord;

                    record.Output(w);
                    w.Write(Environment.NewLine);
                }

                w.Write(Environment.NewLine);
                w.WriteLine("0 TRLR");
                w.Write(Environment.NewLine);
            }
        }

        private class GedcomStreamWriter : StreamWriter
        {
            private int tabSize = 4;
            private string tab = "    ";

            public GedcomStreamWriter(Stream s)
                : base(s)
            {
            }

            public GedcomStreamWriter(string s)
                : base(s)
            {
            }

            public GedcomStreamWriter(Stream s, Encoding e)
                : base(s, e)
            {
            }

            public GedcomStreamWriter(string s, bool append)
                : base(s, append)
            {
            }

            public GedcomStreamWriter(Stream s, Encoding e, int bufSize)
                : base(s, e, bufSize)
            {
            }

            public GedcomStreamWriter(string s, bool append, Encoding e)
                : base(s, append, e)
            {
            }

            public GedcomStreamWriter(string s, bool append, Encoding e, int bufSize)
                : base(s, append, e, bufSize)
            {
            }

            public bool AllowInformationSeparatorOnSave { get; set; }

            public bool AllowLineTabsSave { get; set; }

            public bool AllowTabsSave { get; set; }

            public int TabSize
            {
                get
                {
                    return tabSize;
                }

                set
                {
                    tabSize = value;
                    tab = string.Empty.PadRight(tabSize);
                }
            }

            public override void Write(char value)
            {
                if (!AllowInformationSeparatorOnSave && value == 0x1f)
                {
                    base.Write(" ");
                }
                else if ((!AllowLineTabsSave && value == 0x0b) || (!AllowTabsSave && value == 0x09))
                {
                    base.Write(tab);
                }
                else
                {
                    base.Write(value);
                }
            }

            public override void Write(char[] buffer, int index, int count)
            {
                base.Write(buffer, index, count);
            }

            public override void Write(object value)
            {
                Write(value.ToString());
            }

            public override void Write(string value)
            {
                string tmp = value;

                if (!AllowInformationSeparatorOnSave)
                {
                    tmp = tmp.Replace("\x001f", " ");
                }

                if (!AllowLineTabsSave)
                {
                    tmp = tmp.Replace("\x000b", tab);
                }

                if (!AllowTabsSave)
                {
                    tmp = tmp.Replace("\x0009", tab);
                }

                base.Write(value);
            }

            public override void Write(string format, object arg0)
            {
                Write(string.Format(format, arg0));
            }

            public override void Write(string format, object arg0, object arg1)
            {
                Write(string.Format(format, arg0, arg1));
            }

            public override void Write(string format, object arg0, object arg1, object arg2)
            {
                Write(string.Format(format, arg0, arg1, arg2));
            }

            public override void WriteLine(char value)
            {
                if (!AllowInformationSeparatorOnSave && value == 0x1f)
                {
                    base.WriteLine(" ");
                }
                else if ((!AllowLineTabsSave && value == 0x0b) || (!AllowTabsSave && value == 0x09))
                {
                    base.WriteLine(tab);
                }
                else
                {
                    base.WriteLine(value);
                }
            }

            public override void WriteLine(char[] buffer, int index, int count)
            {
                base.WriteLine(buffer, index, count);
            }

            public override void WriteLine(object value)
            {
                WriteLine(value.ToString());
            }

            public override void WriteLine(string value)
            {
                string tmp = value;

                if (!AllowInformationSeparatorOnSave)
                {
                    tmp = tmp.Replace("\x001f", " ");
                }

                if (!AllowLineTabsSave)
                {
                    tmp = tmp.Replace("\x000b", tab);
                }

                if (!AllowTabsSave)
                {
                    tmp = tmp.Replace("\x0009", tab);
                }

                base.WriteLine(tmp);
            }

            public override void WriteLine(string format, object arg0)
            {
                WriteLine(string.Format(format, arg0));
            }

            public override void WriteLine(string format, object arg0, object arg1)
            {
                WriteLine(string.Format(format, arg0, arg1));
            }

            public override void WriteLine(string format, object arg0, object arg1, object arg2)
            {
                WriteLine(string.Format(format, arg0, arg1, arg2));
            }
        }
    }
}
