// <copyright file="IndexedKeyCollectionTest.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Tests
{
    using GeneGenie.Gedcom.Helpers;
    using Xunit;

    /// <summary>
    /// TODO: More tests and simplify / split the tests that are there.
    /// </summary>
    public class IndexedKeyCollectionTest
    {
        [Fact]
        private void Test1()
        {
            var col = new IndexedKeyCollection();

            string tmp;

            tmp = col["I01", 0, 3];
            tmp = col["F1", 0, 2];
            tmp = col["F2", 0, 2];
            tmp = col["I02", 0, 3];
            tmp = col["I03", 0, 3];
            tmp = col["I04", 0, 3];
            tmp = col["NOTE1", 0, 5];
            tmp = col["I012", 0, 4];

            System.Console.WriteLine(col.ToString());

            int pos = -1;
            bool found = col.Find("I012", 0, 4, out pos);

            Assert.True(found, "NOT FOUND!");

            string msg = "expected 3\tgot " + pos;
            Assert.True(pos == 3, msg);
        }

        [Fact]
        private void Test2()
        {
            var col = new IndexedKeyCollection();

            string tmp;

            tmp = col["HEAD", 0, 4];
            tmp = col["SOUR", 0, 4];
            tmp = col["VERS", 0, 4];
            tmp = col["NAME", 0, 4];
            tmp = col["CORP", 0, 4];
            tmp = col["ADDR", 0, 4];
            tmp = col["CONT", 0, 4];
            tmp = col["PHON", 0, 4];
            tmp = col["DEST", 0, 4];
            tmp = col["DATE", 0, 4];
            tmp = col["CHAR", 0, 4];
            tmp = col["SUBM", 0, 4];
            tmp = col["FILE", 0, 4];
            tmp = col["GEDC", 0, 4];
            tmp = col["FORM", 0, 4];
            tmp = col["INDI", 0, 4];

            System.Console.WriteLine(col.ToString());

            int pos = -1;
            bool found = col.Find("NAME", 0, 4, out pos);

            Assert.True(found, "NOT FOUND!");

            string msg = "expected 11\tgot " + pos;
            Assert.True(pos == 11, msg);
        }

        [Fact]
        private void Test3()
        {
            IndexedKeyCollection col = new IndexedKeyCollection();

            string[] names = new string[]
            {
                "Abiathar", "Alice Blanch", "Almedia J", "Anna M", "Annie", "Anthony Desmond", "Anthony James", "Barber", "Barton William", "Board", "Bone", "Burchell", "Caswell", "Collins", "Corrinna", "Cullister Anne", "Day", "Deborah", "Dennis", "Duckett", "Edward", "Edwin", "Elizabeth", "Eric Gwyn", "Farrington", "Fiona M", "Forrester", "Frances C", "Geoffrey", "George", "George F", "German", "Gillian", "Grinning", "Groves", "Hannah", "Harry", "Hatfield", "Helen Elizabeth", "Henry James", "Horton", "Horwood", "Hughes", "Hyde", "Ira Walter", "Irene Winifred", "Iris Mary", "Ivor Harding", "Jane Rosemary", "Jean M A", "Jean Maud", "Jennifer Nancy", "Jenny", "Jeremy D J", "Joanne", "John", "John Werrett", "Kate A", "Kathleen Rose Lucy", "Keziah", "Knight", "Locke", "Margaret Mary", "Mary Jane", "Mavis Jean", "Michael Bruce", "Neville", "Paul V", "Pete", "Peter Bryan", "Phillipa", "Prewett", "Raymond Holloway", "Richard G", "Sarah F", "Slim", "Susan", "Suzanne Alison", "Sybil Beatrice", "Sydney Mary", "Tewkesbury", "Thomas", "Timothy", "Timothy J", "Toby", "Tomlinson", "Tracy Jane", "Valerie C", "Vince", "Walker", "Wallace", "Wayne Oakleigh", "Wendy", "Werrett", "Whereatt", "Wherrett", "Wherritt", "White", "Wilbur", "William", "William John", "Yendell", "Young",
            };

            foreach (string s in names)
            {
                string tmp = col[s, 0, s.Length];
            }

            int pos = -1;
            bool found = col.Find("John", 0, 4, out pos);

            Assert.True(found, "NOT FOUND!");

            string msg = "expected 55\tgot " + pos;
            Assert.True(pos == 55, msg);
        }

        [Fact]
        private void Test4()
        {
            IndexedKeyCollection col = new IndexedKeyCollection();

            string[] names = new string[]
            {
                "Grinning", "Sydney Mary ", "Day", "Susan ", "Vince", "Wayne Oakleigh ", "Bone", "Anna M ", "Annie",
            };

            foreach (string s in names)
            {
                string tmp = col[s, 0, s.Length];
            }

            System.Console.WriteLine(col.ToString());

            int pos = -1;
            bool found = col.Find("Grinning", 0, 8, out pos);

            Assert.True(found, "NOT FOUND!");

            string msg = "expected 4\tgot " + pos;
            Assert.True(pos == 4, msg);
        }
    }
}
