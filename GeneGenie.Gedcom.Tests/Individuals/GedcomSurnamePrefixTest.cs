// <copyright file="GedcomSurnamePrefixTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.Individuals
{
    using System;
    using System.IO;
    using System.Linq;
    using GeneGenie.Gedcom.Parser;
    using GeneGenie.Gedcom.Tests.DataHelperExtensions;
    using Xunit;

    /// <summary>
    /// Tests for how a surname prefix taken from the NAME line is combined with the SPFX and SURN name parts (issue #103).
    /// </summary>
    public sealed class GedcomSurnamePrefixTest : IDisposable
    {
        private readonly TempGedcomFiles files = new();

        /// <inheritdoc/>
        public void Dispose()
        {
            files.Dispose();
        }

        /// <summary>
        /// When SURN holds the whole surname, including the prefix NAME was split on, the prefix must not be kept as well.
        /// </summary>
        /// <param name="surname">The surname as written between the slashes on the NAME line and in SURN.</param>
        [Theory]
        [InlineData("van Veen")]
        [InlineData("van der Berg")]
        public void Surname_part_that_includes_the_prefix_leaves_no_separate_prefix(string surname)
        {
            var path = files.Write(Person($"1 NAME Hendrik /{surname}/", "2 GIVN Hendrik", $"2 SURN {surname}"));

            var reader = GedcomRecordReader.CreateReader(path);

            Assert.Equal(string.Empty, SingleName(reader).SurnamePrefix);
        }

        /// <summary>
        /// A surname kept in one part by SURN is stored as given.
        /// </summary>
        /// <param name="surname">The surname as written between the slashes on the NAME line and in SURN.</param>
        [Theory]
        [InlineData("van Veen")]
        [InlineData("van der Berg")]
        public void Surname_part_that_includes_the_prefix_is_kept_whole(string surname)
        {
            var path = files.Write(Person($"1 NAME Hendrik /{surname}/", "2 GIVN Hendrik", $"2 SURN {surname}"));

            var reader = GedcomRecordReader.CreateReader(path);

            Assert.Equal(surname, SingleName(reader).Surname);
        }

        /// <summary>
        /// The full name must show the prefix once, as the reporter saw "Hendrik /van van Veen/" before the fix.
        /// </summary>
        /// <param name="surname">The surname as written between the slashes on the NAME line and in SURN.</param>
        [Theory]
        [InlineData("van Veen")]
        [InlineData("van der Berg")]
        public void Surname_part_that_includes_the_prefix_gives_the_prefix_once_in_the_full_name(string surname)
        {
            var path = files.Write(Person($"1 NAME Hendrik /{surname}/", "2 GIVN Hendrik", $"2 SURN {surname}"));

            var reader = GedcomRecordReader.CreateReader(path);

            Assert.Equal($"Hendrik /{surname}/", SingleName(reader).Name);
        }

        /// <summary>
        /// The prefix match should not depend on capitalisation.
        /// </summary>
        [Fact]
        public void Prefix_match_ignores_case()
        {
            var path = files.Write(Person("1 NAME Hendrik /Van Veen/", "2 SURN van Veen"));

            var reader = GedcomRecordReader.CreateReader(path);

            Assert.Equal(string.Empty, SingleName(reader).SurnamePrefix);
        }

        /// <summary>
        /// A file that splits the surname with SPFX and SURN keeps its prefix part.
        /// </summary>
        [Fact]
        public void Explicit_prefix_part_is_kept()
        {
            var path = files.Write(Person("1 NAME Hendrik /van Veen/", "2 SPFX van", "2 SURN Veen"));

            var reader = GedcomRecordReader.CreateReader(path);

            Assert.Equal("van", SingleName(reader).SurnamePrefix);
        }

        /// <summary>
        /// A file that splits the surname with SPFX and SURN keeps its surname part.
        /// </summary>
        [Fact]
        public void Explicit_surname_part_is_kept()
        {
            var path = files.Write(Person("1 NAME Hendrik /van Veen/", "2 SPFX van", "2 SURN Veen"));

            var reader = GedcomRecordReader.CreateReader(path);

            Assert.Equal("Veen", SingleName(reader).Surname);
        }

        /// <summary>
        /// Explicit SPFX and SURN parts give the prefix once in the full name.
        /// </summary>
        [Fact]
        public void Explicit_parts_give_the_prefix_once_in_the_full_name()
        {
            var path = files.Write(Person("1 NAME Hendrik /van Veen/", "2 SPFX van", "2 SURN Veen"));

            var reader = GedcomRecordReader.CreateReader(path);

            Assert.Equal("Hendrik /van Veen/", SingleName(reader).Name);
        }

        /// <summary>
        /// Without name parts, the NAME line is still split into a prefix, as before the fix.
        /// </summary>
        [Fact]
        public void Name_line_alone_is_split_into_a_prefix()
        {
            var path = files.Write(Person("1 NAME Hendrik /van Veen/"));

            var reader = GedcomRecordReader.CreateReader(path);

            Assert.Equal("van", SingleName(reader).SurnamePrefix);
        }

        /// <summary>
        /// Without name parts, the NAME line is still split into a surname, as before the fix.
        /// </summary>
        [Fact]
        public void Name_line_alone_is_split_into_a_surname()
        {
            var path = files.Write(Person("1 NAME Hendrik /van Veen/"));

            var reader = GedcomRecordReader.CreateReader(path);

            Assert.Equal("Veen", SingleName(reader).Surname);
        }

        /// <summary>
        /// A SURN that does not start with the derived prefix leaves that prefix alone.
        /// </summary>
        [Fact]
        public void Different_surname_part_keeps_the_derived_prefix()
        {
            var path = files.Write(Person("1 NAME Hendrik /van Veen/", "2 SURN Veene"));

            var reader = GedcomRecordReader.CreateReader(path);

            Assert.Equal("van", SingleName(reader).SurnamePrefix);
        }

        /// <summary>
        /// A SURN that does not start with the derived prefix still replaces the surname.
        /// </summary>
        [Fact]
        public void Different_surname_part_replaces_the_surname()
        {
            var path = files.Write(Person("1 NAME Hendrik /van Veen/", "2 SURN Veene"));

            var reader = GedcomRecordReader.CreateReader(path);

            Assert.Equal("Veene", SingleName(reader).Surname);
        }

        /// <summary>
        /// Saving a surname kept in one part writes the NAME line without a repeated prefix.
        /// </summary>
        [Fact]
        public void Surname_kept_in_one_part_is_written_with_the_prefix_once_on_the_name_line()
        {
            var reader = ReadSurnameKeptInOnePart();
            var output = files.NewPath();

            GedcomRecordWriter.OutputGedcom(reader.Database, output);

            Assert.Contains("1 NAME Hendrik /van Veen/", File.ReadAllLines(output));
        }

        /// <summary>
        /// Saving a surname kept in one part writes SURN with the whole surname.
        /// </summary>
        [Fact]
        public void Surname_kept_in_one_part_is_written_whole_in_the_surname_part()
        {
            var reader = ReadSurnameKeptInOnePart();
            var output = files.NewPath();

            GedcomRecordWriter.OutputGedcom(reader.Database, output);

            Assert.Contains("2 SURN van Veen", File.ReadAllLines(output));
        }

        /// <summary>
        /// Saving a surname kept in one part must not invent an SPFX value.
        /// </summary>
        [Fact]
        public void Surname_kept_in_one_part_is_written_without_a_prefix_value()
        {
            var reader = ReadSurnameKeptInOnePart();
            var output = files.NewPath();

            GedcomRecordWriter.OutputGedcom(reader.Database, output);

            Assert.DoesNotContain(File.ReadAllLines(output), line => line.StartsWith("2 SPFX", StringComparison.Ordinal) && line.Trim() != "2 SPFX");
        }

        private static GedcomName SingleName(GedcomRecordReader reader)
        {
            return reader.Database.Individuals.Single().Names.Single();
        }

        private static string[] Person(params string[] nameLines)
        {
            return [.. TempGedcomFiles.Gedcom("UTF-8", ["0 @I1@ INDI", .. nameLines])];
        }

        private GedcomRecordReader ReadSurnameKeptInOnePart()
        {
            return GedcomRecordReader.CreateReader(files.Write(Person("1 NAME Hendrik /van Veen/", "2 GIVN Hendrik", "2 SURN van Veen")));
        }
    }
}
