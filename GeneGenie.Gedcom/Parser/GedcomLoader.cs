// <copyright file="GedcomLoader.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Parser
{
    using System.IO;
    using System.Text;
    using GeneGenie.Gedcom.Enums;

    /// <summary>
    /// Used by unit tests and benchmarks to load and parse GEDCOM files.
    /// </summary>
    public class GedcomLoader
    {
        /// <summary>
        /// Loads the GEDCOM file and parses it.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>The last error during parsing.</returns>
        public GedcomParser LoadAndParse(string file)
        {
            var encoder = new ASCIIEncoding();

            var parser = new GedcomParser();

            parser.AllowTabs = false;
            parser.AllowHyphenOrUnderscoreInTag = false;

            var dir = ".\\Data";
            var gedcomFile = Path.Combine(dir, file);
            var fi = new FileInfo(gedcomFile);

            using (var stream = new FileStream(gedcomFile, FileMode.Open, FileAccess.Read, FileShare.Read, (int)fi.Length))
            {
                int bufferSize = (int)fi.Length;
                byte[] buffer = new byte[bufferSize];
                int read = 0;
                while ((read = stream.Read(buffer, 0, bufferSize)) != 0)
                {
                    string input = encoder.GetString(buffer, 0, read).Trim();
                    var error = parser.GedcomParse(input);
                    if (error != GedcomErrorState.NoError)
                    {
                        return parser;
                    }
                }

                return parser;
            }
        }
    }
}
