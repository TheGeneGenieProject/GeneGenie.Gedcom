// <copyright file="GedcomMultimediaTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using GeneGenie.Gedcom.Parser;
    using GeneGenie.Gedcom.Tests.DataHelperExtensions;
    using Xunit;

    /// <summary>
    /// Titles and the _PRIM flag are written in more than one place, and both have to survive a save.
    /// </summary>
    public sealed class GedcomMultimediaTests : IDisposable
    {
        private readonly TempGedcomFiles files = new();

        /// <inheritdoc/>
        public void Dispose()
        {
            files.Dispose();
        }

        [Fact]
        public void File_level_title_and_primary_flag_round_trip()
        {
            const string gedcom = """
                0 HEAD
                1 GEDC
                2 VERS 5.5.1
                1 CHAR UTF-8
                0 @I1@ INDI
                1 NAME Jane /Roe/
                1 OBJE
                2 FILE photos/jane.jpg
                3 FORM jpg
                3 TITL Jane at home
                2 _PRIM Y
                0 TRLR
                """;

            string path = Path.GetTempFileName();
            string rewritten = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, gedcom);
                var reader = GedcomRecordReader.CreateReader(path);
                var media = reader.Database.Individuals.Single().Multimedia
                    .Select(xref => (GedcomMultimediaRecord)reader.Database[xref])
                    .Single();
                var file = media.Files.Single();
                Assert.Equal("photos/jane.jpg", file.Filename);
                Assert.Equal("jpg", file.Format);
                Assert.Equal("Jane at home", file.Title);
                Assert.True(file.IsPrimary);

                GedcomRecordWriter.OutputGedcom(reader.Database, rewritten);
                var again = GedcomRecordReader.CreateReader(rewritten);
                var saved = again.Database.Media.SelectMany(record => record.Files).Single(item => item.Filename == "photos/jane.jpg");
                Assert.Equal("Jane at home", saved.Title);
                Assert.True(saved.IsPrimary);
            }
            finally
            {
                File.Delete(path);
                File.Delete(rewritten);
            }
        }

        [Fact]
        public void Record_level_title_is_kept()
        {
            const string gedcom = """
                0 HEAD
                1 GEDC
                2 VERS 5.5.1
                1 CHAR UTF-8
                0 @I1@ INDI
                1 NAME Jane /Roe/
                1 OBJE
                2 TITL Portrait
                2 FILE photos/jane.jpg
                3 FORM jpg
                0 TRLR
                """;

            string path = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, gedcom);
                var reader = GedcomRecordReader.CreateReader(path);
                var media = reader.Database.Individuals.Single().Multimedia
                    .Select(xref => (GedcomMultimediaRecord)reader.Database[xref])
                    .Single();

                Assert.Equal("Portrait", media.Title);
                Assert.Equal("photos/jane.jpg", media.Files.Single().Filename);
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// With no title set, Title falls back to the file names. That fallback must follow later
        /// changes to the files, not stay fixed at the first value read.
        /// </summary>
        [Fact]
        public void Generated_title_lists_files_added_after_it_was_first_read()
        {
            var media = SingleMedia(files.Write(MediaWithOneFile()));
            _ = media.Title;
            media.Files.Add(new GedcomMultimediaFile { Database = media.Database, Filename = "b.jpg", Format = "jpg" });

            var title = media.Title;

            Assert.Equal("a.jpg, b.jpg", title);
        }

        /// <summary>
        /// The file-name fallback is not a real title, so saving must not write it as one, even after
        /// it has been read and the files have changed.
        /// </summary>
        [Fact]
        public void Generated_title_is_not_written_after_files_change()
        {
            var media = SingleMedia(files.Write(MediaWithOneFile()));
            _ = media.Title;
            media.Files.Add(new GedcomMultimediaFile { Database = media.Database, Filename = "b.jpg", Format = "jpg" });
            var output = files.NewPath();

            GedcomRecordWriter.OutputGedcom(media.Database, output);

            Assert.DoesNotContain(File.ReadAllLines(output), line => line.Contains(" TITL "));
        }

        /// <summary>
        /// A record-level title must be written exactly once, not once per place it is held.
        /// </summary>
        [Fact]
        public void Record_level_title_is_written_once()
        {
            var media = SingleMedia(files.Write(MediaWithRecordTitle()));
            var output = files.NewPath();

            GedcomRecordWriter.OutputGedcom(media.Database, output);

            Assert.Single(File.ReadAllLines(output), line => line.Contains(" TITL Portrait"));
        }

        /// <summary>
        /// A record-level title must survive a save and reload unchanged.
        /// </summary>
        [Fact]
        public void Record_level_title_reads_back_after_saving()
        {
            var media = SingleMedia(files.Write(MediaWithRecordTitle()));
            var output = files.NewPath();
            GedcomRecordWriter.OutputGedcom(media.Database, output);

            var reloaded = SingleMedia(output);

            Assert.Equal("Portrait", reloaded.Title);
        }

        private static GedcomMultimediaRecord SingleMedia(string path)
        {
            return GedcomRecordReader.CreateReader(path).Database.Media.Single();
        }

        private static string[] MediaWithOneFile()
        {
            return [.. TempGedcomFiles.Gedcom("UTF-8", "0 @I1@ INDI", "1 NAME Jane /Roe/", "1 OBJE", "2 FILE a.jpg", "3 FORM jpg")];
        }

        private static string[] MediaWithRecordTitle()
        {
            return [.. TempGedcomFiles.Gedcom("UTF-8", "0 @I1@ INDI", "1 NAME Jane /Roe/", "1 OBJE", "2 TITL Portrait", "2 FILE photos/jane.jpg", "3 FORM jpg")];
        }
    }
}
