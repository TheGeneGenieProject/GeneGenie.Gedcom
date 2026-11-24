namespace GeneGenie.Gedcom.Tests
{
    using System.IO;
    using System.Linq;
    using GeneGenie.Gedcom.Parser;
    using Xunit;

    /// <summary>
    /// Titles and the _PRIM flag are written in more than one place, and both have to survive a save.
    /// </summary>
    public class GedcomMultimediaTests
    {
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

        [Fact]
        public void Generated_title_is_not_written_after_files_change()
        {
            const string gedcom = """
                0 HEAD
                1 GEDC
                2 VERS 5.5.1
                1 CHAR UTF-8
                0 @I1@ INDI
                1 NAME Jane /Roe/
                1 OBJE
                2 FILE a.jpg
                3 FORM jpg
                0 TRLR
                """;

            string path = Path.GetTempFileName();
            string rewritten = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, gedcom);
                var reader = GedcomRecordReader.CreateReader(path);
                var media = reader.Database.Media.Single();

                // Reading Title builds one from the file names; that must not become a real title.
                Assert.Equal("a.jpg", media.Title);
                media.Files.Add(new GedcomMultimediaFile { Database = reader.Database, Filename = "b.jpg", Format = "jpg" });
                Assert.Equal("a.jpg, b.jpg", media.Title);

                GedcomRecordWriter.OutputGedcom(reader.Database, rewritten);
                Assert.DoesNotContain(File.ReadAllLines(rewritten), line => line.Contains(" TITL "));
            }
            finally
            {
                File.Delete(path);
                File.Delete(rewritten);
            }
        }

        [Fact]
        public void Record_level_title_round_trips_once()
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
            string rewritten = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, gedcom);
                var reader = GedcomRecordReader.CreateReader(path);
                GedcomRecordWriter.OutputGedcom(reader.Database, rewritten);

                Assert.Single(File.ReadAllLines(rewritten), line => line.Contains(" TITL Portrait"));
                var again = GedcomRecordReader.CreateReader(rewritten);
                Assert.Equal("Portrait", again.Database.Media.Single().Title);
            }
            finally
            {
                File.Delete(path);
                File.Delete(rewritten);
            }
        }
    }
}
