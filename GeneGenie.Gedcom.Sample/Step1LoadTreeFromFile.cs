// <copyright file="Step1LoadTreeFromFile.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Sample
{
    using System;
    using GeneGenie.Gedcom.Parser;

    /// <summary>
    /// Tiny sample class on how to load a GEDCOM file.
    /// </summary>
    public static class Step1LoadTreeFromFile
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
            var gedcomReader = GedcomRecordReader.CreateReader("Data/presidents.ged");
            if (gedcomReader.Parser.ErrorState != Enums.GedcomErrorState.NoError)
            {
                Console.WriteLine($"Could not read file, encountered error {gedcomReader.Parser.ErrorState} press a key to continue.");
                Console.ReadKey();
                return null;
            }

            return gedcomReader.Database;
        }
    }
}