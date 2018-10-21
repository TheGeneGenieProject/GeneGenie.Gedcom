// <copyright file="GedcomIndividualRecordFactory.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Tests.DataHelperExtensions
{
    /// <summary>
    /// Helper functions to create people with names for unit testing.
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
            personName.PreferredName = true;

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

                personName.PreferredName = true;
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
