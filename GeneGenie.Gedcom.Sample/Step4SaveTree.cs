// <copyright file="Step4SaveTree.cs" company="GeneGenie.com">
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
    /// Tiny sample class on how to save a GEDCOM file.
    /// </summary>
    public class Step4SaveTree
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