// <copyright file="GedcomIndividualComparisonTest.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Parser
{
    using System.Collections.Generic;
    using Gedcom.Enums;
    using Tests.DataHelperExtensions;
    using Xunit;

    /// <summary>
    /// Tests for equality of individuals.
    /// </summary>
    public class GedcomIndividualComparisonTest
    {
        private readonly GedcomDatabase gedcomDb;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomIndividualComparisonTest"/> class.
        /// </summary>
        public GedcomIndividualComparisonTest()
        {
            gedcomDb = new GedcomDatabase();
        }

        [Fact]
        private void Individual_are_sorted_alphanumerically()
        {
            var person1 = gedcomDb.NamedPerson("1");
            var person2 = gedcomDb.NamedPerson("2");

            var li = new List<GedcomIndividualRecord> { person1, person2 };
            li.Sort();

            var sortOrder = person1.CompareTo(person2);

            Assert.Equal(-1, sortOrder);
        }

        [Fact]
        private void Individual_are_sorted_alphanumerically_when_reversed()
        {
            var person1 = gedcomDb.NamedPerson("1");
            var person2 = gedcomDb.NamedPerson("2");

            var li = new List<GedcomIndividualRecord> { person2, person1 };
            li.Sort();

            var sortOrder = person2.CompareTo(person1);

            Assert.Equal(1, sortOrder);
        }

        [Fact]
        private void Different_count_of_individuals_is_not_equal()
        {
            var list1 = new List<GedcomIndividualRecord> { new GedcomIndividualRecord() };
            var list2 = new List<GedcomIndividualRecord> { };

            Assert.NotEqual(list1, list2);
        }

        [Fact]
        private void Check_same_count_of_null_individuals_is_equal()
        {
            var list1 = new List<GedcomIndividualRecord> { new GedcomIndividualRecord() };
            var list2 = new List<GedcomIndividualRecord> { new GedcomIndividualRecord() };

            Assert.Equal(list1, list2);
        }

        [Fact]
        private void Two_lists_with_the_same_count_but_different_individuals_are_not_equal()
        {
            var list1 = new List<GedcomIndividualRecord> { gedcomDb.NamedPerson("Ryan") };
            var list2 = new List<GedcomIndividualRecord> { gedcomDb.NamedPerson("David") };

            Assert.NotEqual(list1, list2);
        }

        [Fact]
        private void Individual_is_not_similar_to_null()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");

            Assert.False(person1.IsEquivalentTo(null));
        }

        [Fact]
        private void Individual_is_not_equal_to_null()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");

            Assert.False(Equals(null, person1));
        }

        [Fact]
        private void Individual_is_sorted_after_null()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");

            var sortOrder = person1.CompareTo(null);

            Assert.Equal(1, sortOrder);
        }

        [Theory]
        [InlineData("Mary", "Bob", "1")]
        [InlineData("Bob", "Bob", "0")]
        [InlineData("Bob", "Mary", "-1")]
        private void Individuals_are_sorted_correctly_by_name(string name1, string name2, int expectedRelativePosition)
        {
            var person1 = gedcomDb.NamedPerson(name1);
            var person2 = gedcomDb.NamedPerson(name2);

            Assert.Equal(expectedRelativePosition, person1.CompareTo(person2));
        }

        [Fact]
        private void Individuals_with_different_names_are_not_equal()
        {
            var person1 = gedcomDb.NamedPerson("Sam");
            var person2 = gedcomDb.NamedPerson("Kate");

            Assert.NotEqual(person1, person2);
        }

        [Fact]
        private void Individuals_with_more_names_are_sorted_last()
        {
            var person1 = gedcomDb.NamedPerson("Bob");
            var person2 = gedcomDb.NamedPerson("Bob");

            person1.Names.Add(new GedcomName { Given = "Alice" });

            Assert.Equal(1, person1.CompareTo(person2));
        }

        [Fact]
        private void Individuals_with_different_sex_are_not_similar()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");
            person2.Sex = GedcomSex.Male;

            Assert.False(person1.IsEquivalentTo(person2));
        }

        [Fact]
        private void Individuals_with_different_sex_are_not_equal()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");
            person2.Sex = GedcomSex.Male;

            Assert.NotEqual(person1, person2);
        }

        [Theory]
        [InlineData(GedcomSex.NotSet, GedcomSex.Male, -1)]
        [InlineData(GedcomSex.Male, GedcomSex.Male, 0)]
        [InlineData(GedcomSex.Male, GedcomSex.NotSet, 1)]
        private void Individuals_are_sorted_correctly_by_sex(GedcomSex sex1, GedcomSex sex2, int expectedRelativePosition)
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            person1.Sex = sex1;
            person2.Sex = sex2;

            Assert.Equal(expectedRelativePosition, person1.CompareTo(person2));
        }

        [Fact]
        private void Individuals_with_different_events_are_not_equal()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            person1.Events.Add(new GedcomIndividualEvent { EventName = "Event A" });
            person2.Events.Add(new GedcomIndividualEvent { EventName = "Event B" });

            Assert.NotEqual(person1, person2);
        }

        [Fact]
        private void Individuals_with_different_number_of_events_are_not_equal()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            person1.Events.Add(new GedcomIndividualEvent());
            person1.Events.Add(new GedcomIndividualEvent());
            person2.Events.Add(new GedcomIndividualEvent());

            Assert.NotEqual(person1, person2);
        }

        [Theory]
        [InlineData("Event A", "Event B", -1)]
        [InlineData("Event A", "Event A", 0)]
        [InlineData("Event B", "Event A", 1)]
        private void Individuals_are_sorted_correctly_by_event(string event1, string event2, int expectedRelativePosition)
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            person1.Events.Add(new GedcomIndividualEvent { EventName = event1 });
            person2.Events.Add(new GedcomIndividualEvent { EventName = event2 });

            Assert.Equal(expectedRelativePosition, person1.CompareTo(person2));
        }

        [Fact]
        private void Individuals_with_different_attributes_are_not_equal()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            person1.Attributes.Add(new GedcomIndividualEvent { EventName = "Attributes A" });
            person2.Attributes.Add(new GedcomIndividualEvent { EventName = "Attributes B" });

            Assert.NotEqual(person1, person2);
        }

        [Fact]
        private void Individuals_with_different_number_of_attributes_are_not_equal()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            person1.Attributes.Add(new GedcomIndividualEvent());
            person1.Attributes.Add(new GedcomIndividualEvent());
            person2.Attributes.Add(new GedcomIndividualEvent());

            Assert.NotEqual(person1, person2);
        }

        [Theory]
        [InlineData("Attribute A", "Attribute B", -1)]
        [InlineData("Attribute A", "Attribute A", 0)]
        [InlineData("Attribute B", "Attribute A", 1)]
        private void Individuals_are_sorted_correctly_by_attribute(string attribute1, string attribute2, int expectedRelativePosition)
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            person1.Events.Add(new GedcomIndividualEvent { EventName = attribute1 });
            person2.Events.Add(new GedcomIndividualEvent { EventName = attribute2 });

            Assert.Equal(expectedRelativePosition, person1.CompareTo(person2));
        }

        [Fact]
        private void Individuals_with_different_children_are_not_equal()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            person1.ChildIn.Add(new GedcomFamilyLink { Status = ChildLinkageStatus.Challenged });
            person2.ChildIn.Add(new GedcomFamilyLink { Status = ChildLinkageStatus.Proven });

            Assert.NotEqual(person1, person2);
        }

        [Fact]
        private void Individuals_with_different_number_of_children_are_not_equal()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            person1.ChildIn.Add(new GedcomFamilyLink());
            person1.ChildIn.Add(new GedcomFamilyLink());
            person2.ChildIn.Add(new GedcomFamilyLink());

            Assert.NotEqual(person1, person2);
        }

        [Theory]
        [InlineData(ChildLinkageStatus.Unknown, ChildLinkageStatus.Challenged, -1)]
        [InlineData(ChildLinkageStatus.Challenged, ChildLinkageStatus.Challenged, 0)]
        [InlineData(ChildLinkageStatus.Challenged, ChildLinkageStatus.Unknown, 1)]
        private void Individuals_are_sorted_correctly_by_child(ChildLinkageStatus status1, ChildLinkageStatus status2, int expectedRelativePosition)
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            person1.ChildIn.Add(new GedcomFamilyLink { Status = status1 });
            person2.ChildIn.Add(new GedcomFamilyLink { Status = status2 });

            Assert.Equal(expectedRelativePosition, person1.CompareTo(person2));
        }

        [Fact]
        private void Individuals_with_different_spouses_are_not_equal()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            person1.SpouseIn.Add(new GedcomFamilyLink { Status = ChildLinkageStatus.Challenged });
            person2.SpouseIn.Add(new GedcomFamilyLink { Status = ChildLinkageStatus.Proven });

            Assert.NotEqual(person1, person2);
        }

        [Fact]
        private void Individuals_with_different_number_of_spouses_are_not_equal()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            person1.SpouseIn.Add(new GedcomFamilyLink());
            person1.SpouseIn.Add(new GedcomFamilyLink());
            person2.SpouseIn.Add(new GedcomFamilyLink());

            Assert.NotEqual(person1, person2);
        }

        [Theory]
        [InlineData(false, true, -1)]
        [InlineData(true, true, 0)]
        [InlineData(true, false, 1)]
        private void Individuals_are_sorted_correctly_by_spouse(bool pref1, bool pref2, int expectedRelativePosition)
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            person1.SpouseIn.Add(new GedcomFamilyLink { PreferedSpouse = pref1 });
            person2.SpouseIn.Add(new GedcomFamilyLink { PreferedSpouse = pref2 });

            Assert.Equal(expectedRelativePosition, person1.CompareTo(person2));
        }

        [Fact]
        private void Individuals_with_different_associations_are_not_equal()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            person1.Associations.Add(new GedcomAssociation { Description = "Godparent" });
            person2.Associations.Add(new GedcomAssociation { Description = "Witness" });

            Assert.NotEqual(person1, person2);
        }

        [Fact]
        private void Individuals_with_different_number_of_associations_are_not_equal()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            person1.Associations.Add(new GedcomAssociation());
            person1.Associations.Add(new GedcomAssociation());
            person2.Associations.Add(new GedcomAssociation());

            Assert.NotEqual(person1, person2);
        }

        [Theory]
        [InlineData("Godparent", "Witness", -1)]
        [InlineData("Friend", "Friend", 0)]
        [InlineData("Witness", "Godparent", 1)]
        private void Individuals_are_sorted_correctly_by_association(string assoc1, string assoc2, int expectedRelativePosition)
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            person1.Associations.Add(new GedcomAssociation { Description = assoc1 });
            person2.Associations.Add(new GedcomAssociation { Description = assoc2 });

            Assert.Equal(expectedRelativePosition, person1.CompareTo(person2));
        }

        [Fact]
        private void Individuals_with_different_aliases_are_not_equal()
        {
            var person1 = gedcomDb.NamedPerson("Mary");
            var person2 = gedcomDb.NamedPerson("Mary");

            person1.Alia.Add("Miriam");
            person2.Alia.Add("Maria");

            Assert.NotEqual(person1, person2);
        }

        [Fact]
        private void Individuals_with_different_number_of_aliases_are_not_equal()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            person1.Alia.Add(string.Empty);
            person1.Alia.Add(string.Empty);
            person2.Alia.Add(string.Empty);

            Assert.NotEqual(person1, person2);
        }

        [Theory]
        [InlineData("Maria", "Miriam", -1)]
        [InlineData("Poppy", "Poppy", 0)]
        [InlineData("Miriam", "Maria", 1)]
        private void Individuals_are_sorted_correctly_by_alias(string alias1, string alias2, int expectedRelativePosition)
        {
            var person1 = gedcomDb.NamedPerson("Mary");
            var person2 = gedcomDb.NamedPerson("Mary");

            person1.Alia.Add(alias1);
            person2.Alia.Add(alias2);

            Assert.Equal(expectedRelativePosition, person1.CompareTo(person2));
        }

        [Fact]
        private void Individuals_with_different_addresses_are_not_equal()
        {
            var person1 = gedcomDb.NamedPerson("Mary");
            var person2 = gedcomDb.NamedPerson("Mary");

            person1.Address = new GedcomAddress { Country = "United States" };
            person2.Address = new GedcomAddress { Country = "Canada" };

            Assert.NotEqual(person1, person2);
        }

        [Fact]
        private void Individuals_with_fewer_notes_are_ordered_first()
        {
            var person1 = gedcomDb.NamedPerson("Mary");
            var person2 = gedcomDb.NamedPerson("Mary");

            person1.Notes.Add("Note 1");
            person1.Notes.Add("Note 2");
            person2.Notes.Add("Note 3");

            Assert.Equal(1, person1.CompareTo(person2));
        }

        [Fact]
        private void Individuals_with_same_note_text_are_equal()
        {
            var person1 = CreateIndividualForNoteTest("Same Note");
            var person2 = CreateIndividualForNoteTest("Same Note");

            Assert.Equal(person1, person2);
        }

        [Theory]
        [InlineData("Aaaa", "Zzzz", -1)]
        [InlineData("Note", "Note", 0)]
        [InlineData("Zzzz", "Aaaa", 1)]
        private void Individuals_are_sorted_correctly_by_note(string noteText1, string noteText2, int expectedRelativePosition)
        {
            var person1 = CreateIndividualForNoteTest(noteText1);
            var person2 = CreateIndividualForNoteTest(noteText2);

            Assert.Equal(expectedRelativePosition, person1.CompareTo(person2));
        }

        [Fact]
        private void Individuals_with_same_facts_are_similar()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            Assert.True(person1.IsEquivalentTo(person2));
        }

        [Fact]
        private void Individuals_with_same_facts_are_sorted_equally()
        {
            var person1 = gedcomDb.NamedPerson("Ryan");
            var person2 = gedcomDb.NamedPerson("Ryan");

            var sortOrder = person1.CompareTo(person2);

            Assert.Equal(0, sortOrder);
        }

        private GedcomIndividualRecord CreateIndividualForNoteTest(string noteText)
        {
            var xrefId = gedcomDb.GenerateXref("NOTE");
            var note = new GedcomNoteRecord { Database = gedcomDb, XRefID = xrefId, Text = noteText };
            gedcomDb.Add(xrefId, note);
            var person = gedcomDb.NamedPerson("Mary");
            person.Notes.Add(xrefId);
            return person;
        }
    }
}
