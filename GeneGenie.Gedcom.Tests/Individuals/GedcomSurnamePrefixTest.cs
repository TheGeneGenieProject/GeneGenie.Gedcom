// <copyright file="GedcomSurnamePrefixTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.Individuals
{
    using System.IO;
    using System.Linq;
    using GeneGenie.Gedcom.Parser;
    using Xunit;

    /// <summary>
    /// Tests for how a surname prefix taken from the NAME line is combined with the SPFX and SURN name parts (issue #103).
    /// </summary>
    public class GedcomSurnamePrefixTest
    {
        /// <summary>
        /// When SURN holds the whole surname, including the prefix NAME was split on, the prefix must not be kept as well.
        /// </summary>
        /// <param name="nameSurname">The surname as written between the slashes on the NAME line.</param>
        [Theory]
        [InlineData("van Veen")]
        [InlineData("van der Berg")]
        public void Surname_part_that_includes_the_prefix_is_not_prefixed_again(string nameSurname)
        {
            var name = ReadName(
                $"1 NAME Hendrik /{nameSurname}/",
                "2 GIVN Hendrik",
                $"2 SURN {nameSurname}");

            Assert.Equal(string.Empty, name.SurnamePrefix);
            Assert.Equal(nameSurname, name.Surname);
            Assert.Equal($"Hendrik /{nameSurname}/", name.Name);
        }

        /// <summary>
        /// The prefix match should not depend on capitalisation.
        /// </summary>
        [Fact]
        public void Prefix_match_ignores_case()
        {
            var name = ReadName(
                "1 NAME Hendrik /Van Veen/",
                "2 SURN van Veen");

            Assert.Equal(string.Empty, name.SurnamePrefix);
            Assert.Equal("van Veen", name.Surname);
        }

        /// <summary>
        /// A file that splits the surname with SPFX and SURN keeps the split.
        /// </summary>
        [Fact]
        public void Explicit_prefix_and_surname_parts_are_kept()
        {
            var name = ReadName(
                "1 NAME Hendrik /van Veen/",
                "2 SPFX van",
                "2 SURN Veen");

            Assert.Equal("van", name.SurnamePrefix);
            Assert.Equal("Veen", name.Surname);
            Assert.Equal("Hendrik /van Veen/", name.Name);
        }

        /// <summary>
        /// Without name parts, NAME is still split into prefix and surname as before.
        /// </summary>
        [Fact]
        public void Name_line_alone_is_still_split_into_prefix_and_surname()
        {
            var name = ReadName("1 NAME Hendrik /van Veen/");

            Assert.Equal("van", name.SurnamePrefix);
            Assert.Equal("Veen", name.Surname);
        }

        /// <summary>
        /// A surname that does not start with the derived prefix replaces the surname but leaves the prefix alone.
        /// </summary>
        [Fact]
        public void Different_surname_part_keeps_the_derived_prefix()
        {
            var name = ReadName(
                "1 NAME Hendrik /van Veen/",
                "2 SURN Veene");

            Assert.Equal("van", name.SurnamePrefix);
            Assert.Equal("Veene", name.Surname);
        }

        /// <summary>
        /// Writing the file out again must not duplicate the prefix or invent an SPFX value.
        /// </summary>
        [Fact]
        public void Surname_kept_in_one_part_round_trips_without_a_prefix()
        {
            string path = Path.GetTempFileName();
            string rewritten = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, Gedcom("1 NAME Hendrik /van Veen/", "2 GIVN Hendrik", "2 SURN van Veen"));
                var reader = GedcomRecordReader.CreateReader(path);
                GedcomRecordWriter.OutputGedcom(reader.Database, rewritten);

                var lines = File.ReadAllLines(rewritten).Select(line => line.TrimEnd()).ToList();
                Assert.Contains("1 NAME Hendrik /van Veen/", lines);
                Assert.Contains("2 SURN van Veen", lines);
                Assert.DoesNotContain(lines, line => line.StartsWith("2 SPFX ") && line.Length > "2 SPFX ".Length);
            }
            finally
            {
                File.Delete(path);
                File.Delete(rewritten);
            }
        }

        private static GedcomName ReadName(params string[] nameLines)
        {
            string path = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, Gedcom(nameLines));
                var reader = GedcomRecordReader.CreateReader(path);
                return reader.Database.Individuals.Single().Names.Single();
            }
            finally
            {
                File.Delete(path);
            }
        }

        private static string Gedcom(params string[] nameLines)
        {
            var lines = new[] { "0 HEAD", "1 GEDC", "2 VERS 5.5.1", "2 FORM LINEAGE-LINKED", "1 CHAR UTF-8", "0 @I1@ INDI" }
                .Concat(nameLines)
                .Concat(new[] { "0 TRLR", string.Empty });
            return string.Join("\n", lines);
        }
    }
}
