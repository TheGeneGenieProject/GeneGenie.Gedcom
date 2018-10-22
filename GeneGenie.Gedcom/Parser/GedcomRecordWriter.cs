// <copyright file="GedcomRecordWriter.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
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
        /// Helper method to output a standard GEDCOM file without needing to create a writer.
        /// </summary>
        /// <param name="database">The database to output.</param>
        /// <param name="file">The file path to output to.</param>
        public static void OutputGedcom(GedcomDatabase database, string file)
        {
            var writer = new GedcomRecordWriter();
            writer.WriteGedcom(database, file);
        }

        /// <summary>
        /// Helper method to output a standard GEDCOM file without needing to create a writer.
        /// </summary>
        /// <param name="database">The database to output.</param>
        /// <param name="stream">The stream to write to.</param>
        public static void OutputGedcom(GedcomDatabase database, Stream stream)
        {
            var writer = new GedcomRecordWriter();
            writer.WriteGedcom(database, stream);
        }

        /// <summary>
        /// Outputs the currently set GedcomDatabase to the currently set file
        /// </summary>
        public void WriteGedcom()
        {
            WriteGedcom(Database, GedcomFile);
        }

        /// <summary>
        /// Outputs a GedcomDatabase to the given file.
        /// </summary>
        /// <param name="database">The GedcomDatabase to write.</param>
        /// <param name="file">The filename to write to.</param>
        public void WriteGedcom(GedcomDatabase database, string file)
        {
            Encoding enc = new UTF8Encoding();
            using (var writer = new GedcomStreamWriter(file, false, enc))
            {
                Write(database, writer);
            }
        }

        /// <summary>
        /// Outputs a GedcomDatabase to the passed stream.
        /// </summary>
        /// <param name="database">The GedcomDatabase to write.</param>
        /// <param name="stream">The stream to write to.</param>
        public void WriteGedcom(GedcomDatabase database, Stream stream)
        {
            Encoding enc = new UTF8Encoding();
            using (var writer = new GedcomStreamWriter(stream, enc, 1024, true))
            {
                Write(database, writer);
            }
        }

        /// <summary>
        /// Writes the specified database to the passed writer.
        /// Not for use outside this class as the writer must be
        /// responsibly disposed in a using block by the caller.
        /// </summary>
        /// <param name="database">The database to write.</param>
        /// <param name="writer">The writer to use for outputting the database.</param>
        private void Write(GedcomDatabase database, GedcomStreamWriter writer)
        {
            writer.AllowInformationSeparatorOnSave = AllowInformationSeparatorOnSave;
            writer.AllowLineTabsSave = AllowLineTabsSave;
            writer.AllowTabsSave = AllowTabsSave;

            database.Header.Output(writer);

            // write records
            foreach (DictionaryEntry entry in database)
            {
                var record = entry.Value as GedcomRecord;

                record.Output(writer);
                writer.Write(Environment.NewLine);
            }

            writer.Write(Environment.NewLine);
            writer.WriteLine("0 TRLR");
            writer.Write(Environment.NewLine);
        }
    }
}
