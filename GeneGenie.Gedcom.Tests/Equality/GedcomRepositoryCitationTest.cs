// <copyright file="GedcomRepositoryCitationTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.Equality
{
    using GeneGenie.Gedcom.Enums;
    using Xunit;

    /// <summary>
    /// Test suite for equality of GedcomRepositoryCitation.
    /// </summary>
    public class GedcomRepositoryCitationTest
    {
        private readonly GedcomRepositoryCitation cit1;
        private readonly GedcomRepositoryCitation cit2;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomRepositoryCitationTest"/> class.
        /// </summary>
        public GedcomRepositoryCitationTest()
        {
            cit1 = GenerateRepositoryCitation();
            cit2 = GenerateRepositoryCitation();
        }

        [Fact]
        private void Citation_is_not_equal_to_null()
        {
            Assert.NotNull(cit1);
        }

        [Fact]
        private void Citation_with_different_call_numbers_is_not_equal()
        {
            cit1.CallNumbers.Add("call nbr one");
            cit2.CallNumbers.Add("call nbr two");

            Assert.NotEqual(cit1, cit2);
        }

        [Fact]
        private void Citation_with_different_media_types_is_not_equal()
        {
            cit1.MediaTypes.Add(SourceMediaType.Magazine);
            cit2.MediaTypes.Add(SourceMediaType.Newspaper);

            Assert.NotEqual(cit1, cit2);
        }

        [Fact]
        private void Citation_with_different_other_media_types_is_not_equal()
        {
            cit1.OtherMediaTypes.Add("etc");
            cit2.OtherMediaTypes.Add("something");

            Assert.NotEqual(cit1, cit2);
        }

        [Fact]
        private void Citation_with_different_repository_is_not_equal()
        {
            cit1.Repository = "repo one";
            cit2.Repository = "repo two";

            Assert.NotEqual(cit1, cit2);
        }

        [Fact]
        private void Citations_with_same_facts_are_equal()
        {
            Assert.Equal(cit1, cit2);
        }

        private GedcomRepositoryCitation GenerateRepositoryCitation()
        {
            return new GedcomRepositoryCitation
            {
                CallNumbers = { "abc123", "xyz890" },
                MediaTypes = { SourceMediaType.Book, SourceMediaType.Electronic },
                OtherMediaTypes = { "one", "two" },
                Repository = "sample repo",
            };
        }
    }
}