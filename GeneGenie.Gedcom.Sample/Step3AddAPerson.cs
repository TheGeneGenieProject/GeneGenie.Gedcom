// <copyright file="Step3AddAPerson.cs" company="GeneGenie.com">
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