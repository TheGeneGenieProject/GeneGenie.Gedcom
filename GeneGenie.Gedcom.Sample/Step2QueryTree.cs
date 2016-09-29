// <copyright file="Step2QueryTree.cs" company="GeneGenie.com">
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
    using System.Linq;

    /// <summary>
    /// Tiny sample class on how to query a GEDCOM file.
    /// </summary>
    public class Step2QueryTree
    {
        /// <summary>
        /// Queries the tree for any individual with a name, just to show how to query.
        /// </summary>
        /// <param name="db">The database to query.</param>
        public static void QueryTree(GedcomDatabase db)
        {
            Console.WriteLine($"Found {db.Families.Count} families and {db.Individuals.Count} individuals.");
            var individual = db
                .Individuals
                .FirstOrDefault(f => f.Names.Any());

            if (individual == null)
            {
                Console.WriteLine($"Couldn't find any individuals in the GEDCOM file with a name, which is odd!");
                return;
            }

            Console.WriteLine($"Individual found with a preferred name of '{individual.GetName().Name}'.");
        }
    }
}