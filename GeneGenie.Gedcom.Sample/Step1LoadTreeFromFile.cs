// <copyright file="Step1LoadTreeFromFile.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Sample
{
    using System;
    using GeneGenie.Gedcom.Parser;

    /// <summary>
    /// Tiny sample class on how to load a GEDCOM file.
    /// </summary>
    public class Step1LoadTreeFromFile
    {
        /// <summary>
        /// Loads the presidents tree.
        /// </summary>
        /// <returns>A database reader that can be used to access the parsed database.</returns>
        public static GedcomDatabase LoadPresidentsTree()
        {
            var db = LoadGedcomFromFile();
            if (db == null)
            {
                return null;
            }

            Console.WriteLine("Loaded presidents test file.");
            return db;
        }

        private static GedcomDatabase LoadGedcomFromFile()
        {
            var gedcomReader = GedcomRecordReader.CreateReader("Data\\presidents.ged");
            if (gedcomReader.Parser.ErrorState != Parser.Enums.GedcomErrorState.NoError)
            {
                Console.WriteLine($"Could not read file, encountered error {gedcomReader.Parser.ErrorState} press a key to continue.");
                Console.ReadKey();
                return null;
            }

            return gedcomReader.Database;
        }
    }
}