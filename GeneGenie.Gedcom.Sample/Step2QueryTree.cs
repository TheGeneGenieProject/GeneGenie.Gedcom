// <copyright file="Step2QueryTree.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Sample
{
    using System;
    using System.Linq;

    /// <summary>
    /// Tiny sample class on how to query a GEDCOM file.
    /// </summary>
    public static class Step2QueryTree
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