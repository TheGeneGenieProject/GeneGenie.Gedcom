// <copyright file="GedcomCustomTagTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.RecordReaderTests;

/// <summary>
/// Tests for keeping custom tags, the ones starting with an underscore that programs add and the library
/// doesn't model, so they can be read and are written back when the file is saved (issue #166).
/// </summary>
public sealed class GedcomCustomTagTest : IDisposable
{
    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// A custom tag on a person is kept with its value.
    /// </summary>
    [Fact]
    public void Custom_tag_on_a_person_is_kept()
    {
        var path = files.Write(Example());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal("Seen in 1900", database.Individuals.Single().CustomTags.Single(tag => tag.Tag == "_SEEN").Value);
    }

    /// <summary>
    /// The lines under a custom tag are kept under it.
    /// </summary>
    [Fact]
    public void Line_under_a_custom_tag_is_kept()
    {
        var path = files.Write(Example());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal("Census", database.Individuals.Single().CustomTags.Single(tag => tag.Tag == "_SEEN").Children.Single(child => child.Tag == "TYPE").Value);
    }

    /// <summary>
    /// A custom tag's text includes the CONC and CONT lines it was split over.
    /// </summary>
    [Fact]
    public void Custom_tag_text_joins_its_continuation_lines()
    {
        var path = files.Write(Example());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal("Seen in 1900" + Environment.NewLine + "and 1910", database.Individuals.Single().CustomTags.Single(tag => tag.Tag == "_SEEN").Text);
    }

    /// <summary>
    /// Custom tags, and the lines under them, are saved as they were read, wherever they appear.
    /// </summary>
    /// <param name="line">A line from the example, with its level.</param>
    [Theory]
    [InlineData("1 _SEEN Seen in 1900")]
    [InlineData("2 TYPE Census")]
    [InlineData("2 CONT and 1910")]
    [InlineData("3 _FOUND Yes")]
    [InlineData("3 _PLACE_ID 42")]
    [InlineData("2 _APPROVED Y")]
    [InlineData("1 _COLOR blue")]
    [InlineData("2 _FAVOURITE Y")]
    public void Custom_tag_line_is_saved_as_read(string line)
    {
        var database = GedcomRecordReader.CreateReader(files.Write(Example())).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains(line, File.ReadAllLines(output));
    }

    /// <summary>
    /// A custom tag is saved once, not again each time the file is read and saved.
    /// </summary>
    [Fact]
    public void Custom_tag_is_saved_once()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(Example())).Database;
        var reloaded = GedcomRecordReader.CreateReader(SaveTo(database)).Database;

        var lines = File.ReadAllLines(SaveTo(reloaded));

        Assert.Single(lines, line => line == "1 _SEEN Seen in 1900");
    }

    /// <summary>
    /// A custom tag added in code is saved.
    /// </summary>
    [Fact]
    public void Custom_tag_added_in_code_is_saved()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(Example())).Database;
        database.Individuals.Single().CustomTags.Add(new GedcomCustomTag { Tag = "_ADDED", Value = "In code" });
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains("1 _ADDED In code", File.ReadAllLines(output));
    }

    /// <summary>
    /// A top-level record with a custom tag, such as Ancestry's _MTTAG, is kept.
    /// </summary>
    [Fact]
    public void Custom_record_is_kept()
    {
        var path = files.Write(Example());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal("Brick wall", database.UserDefinedRecords.Single().Children.Single(child => child.Tag == "NAME").Value);
    }

    /// <summary>
    /// A custom record is saved, so pointers to it don't dangle.
    /// </summary>
    [Fact]
    public void Custom_record_is_saved()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(Example())).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains($"0 @{database.UserDefinedRecords.Single().XRefID}@ _MTTAG", File.ReadAllLines(output));
    }

    /// <summary>
    /// A pointer in a custom tag still points at the right record after the reader has renumbered them.
    /// </summary>
    [Fact]
    public void Pointer_in_a_custom_tag_is_saved()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(Example())).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains($"1 _MTTAG @{database.UserDefinedRecords.Single().XRefID}@", File.ReadAllLines(output));
    }

    /// <summary>
    /// Custom tags the library already reads and writes itself, such as _UID, aren't kept a second time.
    /// </summary>
    [Fact]
    public void Custom_tag_the_library_reads_is_not_saved_twice()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(Example())).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Single(File.ReadAllLines(output), line => line.Contains("_UID"));
    }

    /// <summary>
    /// The lines under a custom tag on a citation, however deep, aren't mistaken for the citation's own
    /// text, which would otherwise be written a second time each time the file is saved.
    /// </summary>
    /// <param name="lines">The custom tag and its lines, separated by bars, starting at level 2.</param>
    [Theory]
    [InlineData("2 _NOTE Checked|3 CONC  twice")]
    [InlineData("2 _FOO|3 TEXT abc|4 CONC def")]
    [InlineData("2 _FOO|3 DATA|4 TEXT abc")]
    public void Lines_under_a_custom_tag_do_not_change_the_citation_text(string lines)
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", ["0 @S1@ SOUR", "1 TITL Parish register", "0 @I1@ INDI", "1 NAME Jan /Roe/", "1 SOUR @S1@", .. lines.Split('|')]));

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Null(database.Individuals.Single().Sources.Single().Text);
    }

    /// <summary>
    /// The lines under a custom tag on a name aren't mistaken for parts of the name.
    /// </summary>
    [Fact]
    public void Lines_under_a_custom_tag_do_not_change_the_name()
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @I1@ INDI", "1 NAME Jan /Roe/", "2 _FOO", "3 GIVN Piet", "3 SURN Doe"));

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal("Roe", database.Individuals.Single().Names.Single().Surname);
    }

    /// <summary>
    /// The lines under a custom tag on a note record aren't added to the note's text.
    /// </summary>
    [Fact]
    public void Lines_under_a_custom_tag_do_not_change_the_note_text()
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @N1@ NOTE Hello", "1 _FOO", "2 CONT Not part of the note", "1 CONC  world"));

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal("Hello world", database.Notes.Single().Text);
    }

    private static IEnumerable<string> Example()
    {
        return TempGedcomFiles.Gedcom(
            "UTF-8",
            "0 @I1@ INDI",
            "1 NAME Jan /Roe/",
            "1 _SEEN Seen in 1900",
            "2 TYPE Census",
            "2 CONT and 1910",
            "2 DATE 1900",
            "3 _FOUND Yes",
            "1 _UID 0123456789ABCDEF",
            "1 BIRT",
            "2 PLAC Some Town",
            "3 _PLACE_ID 42",
            "2 _APPROVED Y",
            "1 FAMS @F1@",
            "2 _FAVOURITE Y",
            "1 _MTTAG @T1@",
            "0 @F1@ FAM",
            "1 HUSB @I1@",
            "1 _COLOR blue",
            "0 @T1@ _MTTAG",
            "1 NAME Brick wall");
    }

    private string SaveTo(GedcomDatabase database)
    {
        var output = files.NewPath();
        GedcomRecordWriter.OutputGedcom(database, output);
        return output;
    }
}
