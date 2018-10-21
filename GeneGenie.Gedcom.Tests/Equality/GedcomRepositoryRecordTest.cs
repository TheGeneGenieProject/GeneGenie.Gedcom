// <copyright file="GedcomRepositoryRecordTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.Equality
{
    using Xunit;

    /// <summary>
    /// Test suite for equality of GedcomRepositoryRecord
    /// </summary>
    public class GedcomRepositoryRecordTest
    {
        private readonly GedcomRepositoryRecord rec1;
        private readonly GedcomRepositoryRecord rec2;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomRepositoryRecordTest"/> class.
        /// </summary>
        public GedcomRepositoryRecordTest()
        {
            rec1 = GenerateRepositoryRecord();
            rec2 = GenerateRepositoryRecord();
        }

        [Fact]
        private void Repository_record_is_not_equal_to_null()
        {
            Assert.NotNull(rec1);
        }

        [Fact]
        private void Repository_record_with_different_address_is_not_equal()
        {
            rec1.Address = new GedcomAddress { State = "TX" };
            rec2.Address = new GedcomAddress { State = "MA" };

            Assert.NotEqual(rec1, rec2);
        }

        [Fact]
        private void Repository_record_with_different_citation_is_not_equal()
        {
            rec1.Citations.Add(new GedcomRepositoryCitation { CallNumbers = { "one one one" } });
            rec2.Citations.Add(new GedcomRepositoryCitation { CallNumbers = { "two two two" } });

            Assert.NotEqual(rec1, rec2);
        }

        [Fact]
        private void Repository_record_with_different_name_is_not_equal()
        {
            rec1.Name = "name one";
            rec2.Name = "name two";

            Assert.NotEqual(rec1, rec2);
        }

        [Fact]
        private void Repository_records_with_same_facts_are_equal()
        {
            Assert.Equal(rec1, rec2);
        }

        [Theory]
        [InlineData("a.name", "b.name", -1)]
        [InlineData("a.name", "a.name", 0)]
        [InlineData("b.name", "a.name", 1)]
        private void Repository_records_are_sorted_correctly_by_name(string name1, string name2, int expectedSortOrder)
        {
            var repoRec1 = new GedcomRepositoryRecord { Name = name1 };
            var repoRec2 = new GedcomRepositoryRecord { Name = name2 };

            Assert.Equal(expectedSortOrder, repoRec1.CompareTo(repoRec2));
        }

        private GedcomRepositoryRecord GenerateRepositoryRecord()
        {
            return new GedcomRepositoryRecord
            {
                Address = new GedcomAddress { State = "CA" },
                Citations =
                {
                    new GedcomRepositoryCitation { CallNumbers = { "some call number" } },
                    new GedcomRepositoryCitation { CallNumbers = { "another call number" } },
                },
                Name = "sample repository name",
            };
        }
    }
}