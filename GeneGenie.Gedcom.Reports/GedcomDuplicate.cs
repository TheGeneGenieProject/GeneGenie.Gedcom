// <copyright file="GedcomDuplicate.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Reports
{
    using System.Collections.Generic;
    using GeneGenie.Gedcom;

    /// <summary>
    /// Find and report duplicates in GEDCOM database.
    /// </summary>
    public class GedcomDuplicate
    {
        private GedcomDuplicate()
        {
        }

        /// <summary>
        /// TODO Doc
        /// </summary>
        /// <param name="indi">The indi.</param>
        /// <param name="matches">The matches.</param>
        public delegate void DuplicateFoundFunc(GedcomIndividualRecord indi, List<GedcomIndividualRecord> matches);

        /// <summary>
        /// Finds the duplicates.
        /// </summary>
        /// <param name="indi">The indi.</param>
        /// <param name="databaseB">The database b.</param>
        /// <param name="matchThreshold">The match threshold.</param>
        /// <returns>A list of duplicate records.</returns>
        public static List<GedcomIndividualRecord> FindDuplicates(GedcomIndividualRecord indi, GedcomDatabase databaseB, float matchThreshold)
        {
            List<GedcomIndividualRecord> matches = new List<GedcomIndividualRecord>();

            foreach (GedcomIndividualRecord matchIndi in databaseB.Individuals)
            {
                // can't match self, databaseB could be the same database as indi.Database
                // so we can check this
                if (matchIndi != indi)
                {
                    float match = indi.IsMatch(matchIndi);

                    if (match > matchThreshold)
                    {
                        matches.Add(matchIndi);

                        // System.Console.WriteLine(indi.Names[0].Name + " matches " + matchIndi.Names[0].Name + " at " + match + "%");
                    }
                }
            }

            return matches;
        }

        /// <summary>
        /// Finds the duplicates.
        /// </summary>
        /// <param name="databaseA">The database a.</param>
        /// <param name="databaseB">The database b.</param>
        /// <param name="matchThreshold">The match threshold.</param>
        /// <param name="foundFunc">The found function.</param>
        public static void FindDuplicates(GedcomDatabase databaseA, GedcomDatabase databaseB, float matchThreshold, DuplicateFoundFunc foundFunc)
        {
            int total = databaseA.Individuals.Count;
            int potential = 0;

            foreach (GedcomIndividualRecord indi in databaseA.Individuals)
            {
                List<GedcomIndividualRecord> matches = FindDuplicates(indi, databaseB, matchThreshold);

                if (matches.Count > 0)
                {
                    foundFunc(indi, matches);
                    potential++;
                }
            }
        }
    }
}
