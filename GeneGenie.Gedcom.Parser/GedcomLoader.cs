// <copyright file="GedcomLoader.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Parser
{
    using System.IO;
    using System.Text;
    using Enums;

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
        public static GedcomErrorState LoadAndParse(string file)
        {
            var encoder = new ASCIIEncoding();

            GedcomParser parser = new GedcomParser();

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
                        return error;
                    }
                }

                return GedcomErrorState.NoError;
            }
        }
    }
}
