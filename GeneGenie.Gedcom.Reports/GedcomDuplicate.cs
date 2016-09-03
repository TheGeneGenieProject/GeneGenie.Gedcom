/*
 *  $Id: GedcomDuplicate.cs 183 2008-06-08 15:31:15Z davek $
 * 
 *  Copyright (C) 2007 David A Knight <david@ritter.demon.co.uk>
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

namespace GeneGenie.Gedcom.Reports
{
    using System.Collections.Generic;
    using GeneGenie.Gedcom;

    public class GedcomDuplicate
    {


        private GedcomDuplicate()
        {
        }



        public delegate void DuplicateFoundFunc(GedcomIndividualRecord indi, List<GedcomIndividualRecord> matches);


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
                        //System.Console.WriteLine(indi.Names[0].Name + " matches " + matchIndi.Names[0].Name + " at " + match + "%");
                    }
                }
            }

            return matches;
        }

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
