// <copyright file="Step3AddAPerson.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Sample
{
    using System;

    /// <summary>
    /// Tiny sample class on how to add a person to a database.
    /// </summary>
    public class Step3AddAPerson
    {
        /// <summary>
        /// Adds a sample person (well, a cartoon mouse) to the presidents file. The mouse may do a better job if elected president.
        /// </summary>
        /// <param name="db">The database to add the individual to.</param>
        public static void AddPerson(GedcomDatabase db)
        {
            var individual = new GedcomIndividualRecord(db);

            var name = individual.Names[0];
            name.Given = "Michael";
            name.Surname = "Mouse";
            name.Nick = "Mickey";

            individual.Names.Add(name);

            var birthDate = new GedcomDate(db);
            birthDate.ParseDateString("24 Jan 1933");
            individual.Events.Add(new GedcomIndividualEvent
            {
                Database = db,
                Date = birthDate,
                EventType = Enums.GedcomEventType.Birth,
            });

            Console.WriteLine($"Added record for '{individual.GetName().Name}' with birth date {individual.Birth.Date.Date1}.");
        }
    }
}