// <copyright file="GedcomIndividualRecordFactory.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Tests.DataHelperExtensions
{
    /// <summary>
    /// Helper functions to create people with names.
    /// </summary>
    public static class GedcomIndividualRecordFactory
    {
        /// <summary>
        /// Create an individual with a specific name.
        /// </summary>
        /// <param name="gedcomDb">The gedcom database to attach the name and individual to.</param>
        /// <param name="name">The name to place directly into the name field.</param>
        /// <returns>The constructed individual.</returns>
        public static GedcomIndividualRecord NamedPerson(this GedcomDatabase gedcomDb, string name)
        {
            var personName = new GedcomName();
            personName.Level = 1;
            personName.Database = gedcomDb;
            personName.Name = name;
            personName.PreferedName = true;

            var person = new GedcomIndividualRecord(gedcomDb);
            person.Names.Clear();
            person.Names.Add(personName);
            return person;
        }

        /// <summary>
        /// Create an individual with a specific name.
        /// </summary>
        /// <param name="gedcomDb">The gedcom database to attach the name and individual to.</param>
        /// <param name="givenName">The given name (first name) to attach to the new individual.</param>
        /// <param name="surname">The surname (last name) to attach to the new individual.</param>
        /// <returns>The constructed individual.</returns>
        public static GedcomIndividualRecord NamedPerson(this GedcomDatabase gedcomDb, string givenName, string surname)
        {
            var person = new GedcomIndividualRecord(gedcomDb);
            person.Names.Clear();
            if (!string.IsNullOrWhiteSpace(givenName) || !string.IsNullOrWhiteSpace(surname))
            {
                var personName = new GedcomName();
                personName.Level = 1;
                personName.Database = gedcomDb;
                if (!string.IsNullOrWhiteSpace(givenName))
                {
                    personName.Given = givenName;
                }

                personName.PreferedName = true;
                if (!string.IsNullOrWhiteSpace(surname))
                {
                    personName.Surname = surname;
                }

                person.Names.Add(personName);
            }

            return person;
        }
    }
}
