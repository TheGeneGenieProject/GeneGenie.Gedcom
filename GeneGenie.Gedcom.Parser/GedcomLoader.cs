/*
 *  Copyright  (C) 2007 David A Knight <david@ritter.demon.co.uk>
 *  Amendments (C) 2016 Ryan O'Neill <r@genegenie.com>
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

namespace GeneGenie.Gedcom.Parser
{
    using System.IO;
    using System.Text;

    public class GedcomLoader
    {
        public static GedcomErrorState LoadAndParseOriginal(string file)
        {
            var encoder = new ASCIIEncoding();

            GedcomParser parser = new GedcomParser();

            parser.AllowTabs = false;
            parser.AllowHyphenOrUnderscoreInTag = false;

            var dir = ".\\Data";
            var gedcomFile = Path.Combine(dir, file);

            FileStream stream = null;
            try
            {
                FileInfo fi = new FileInfo(gedcomFile);
                stream = fi.OpenRead();

                int bufferSize = (int)fi.Length;
                byte[] buffer = new byte[bufferSize];
                int read = 0;
                while ((read = stream.Read(buffer, 0, bufferSize)) != 0)
                {
                    string input = encoder.GetString(buffer, 0, read).Trim();
                    var error = parser.GedcomParse(input);
                    if (error != GedcomErrorState.NoError)
                        return error;
                }
                return GedcomErrorState.NoError;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        public static GedcomErrorState LoadAndParseUsing(string file)
        {
            var encoder = new ASCIIEncoding();

            GedcomParser parser = new GedcomParser();

            parser.AllowTabs = false;
            parser.AllowHyphenOrUnderscoreInTag = false;

            var dir = ".\\Data";
            var gedcomFile = Path.Combine(dir, file);
            FileInfo fi = new FileInfo(gedcomFile);

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
                        return error;
                }
                return GedcomErrorState.NoError;
            }
        }

        //public static GedcomErrorState ReadInOneShot(string file) // Awful performance.
        //{
        //    var encoder = new ASCIIEncoding();

        //    GedcomParser parser = new GedcomParser();

        //    parser.AllowTabs = false;
        //    parser.AllowHyphenOrUnderscoreInTag = false;

        //    var dir = ".\\Data";
        //    var gedcomFile = Path.Combine(dir, file);

        //    var input = File.ReadAllText(gedcomFile, encoder);
        //    var error = parser.GedcomParse(input);
        //    if (error != GedcomErrorState.NoError)
        //        return error;

        //    return GedcomErrorState.NoError;
        //}
    }
}
