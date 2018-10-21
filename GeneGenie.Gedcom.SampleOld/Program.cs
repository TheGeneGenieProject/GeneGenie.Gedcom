// <copyright file="Program.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Sample
{
    using System;

    /// <summary>
    /// Sample console app showing how to read, query, change and save a GEDCOM file.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// App entry point.
        /// </summary>
        public static void Main()
        {
            var db = Step1LoadTreeFromFile.LoadPresidentsTree();
            if (db == null)
            {
                return;
            }

            Step2QueryTree.QueryTree(db);

            Console.WriteLine($"Count of people before adding new person - {db.Individuals.Count}.");
            Step3AddAPerson.AddPerson(db);
            Console.WriteLine($"Count of people after adding new person - {db.Individuals.Count}.");

            Step4SaveTree.Save(db);

            Console.WriteLine("Finished, press a key to continue.");
            Console.ReadKey();
        }
    }
}
