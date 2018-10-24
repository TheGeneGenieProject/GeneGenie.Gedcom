// <copyright file="Step4SaveTree.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Sample
{
    using System;
    using GeneGenie.Gedcom.Parser;

    /// <summary>
    /// Tiny sample class on how to save a GEDCOM file.
    /// </summary>
    public static class Step4SaveTree
    {
        /// <summary>
        /// Saves the sample database out to a new file.
        /// </summary>
        /// <param name="db">The database to save.</param>
        public static void Save(GedcomDatabase db)
        {
            GedcomRecordWriter.OutputGedcom(db, "Rewritten.ged");
            Console.WriteLine($"Output database to rewritten.ged.");
        }
    }
}