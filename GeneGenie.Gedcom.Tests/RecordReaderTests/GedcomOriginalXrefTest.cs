// <copyright file="GedcomOriginalXrefTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.RecordReaderTests;

/// <summary>
/// Tests for keeping the record ids from the file, such as @I123@, by reading with replaceXRefs set to
/// false, so they can be used as permanent identifiers (issue #75).
/// </summary>
public sealed class GedcomOriginalXrefTest : IDisposable
{
    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// By default the reader gives records new ids, so files can be combined without their ids clashing.
    /// </summary>
    [Fact]
    public void Record_gets_a_new_id_by_default()
    {
        var path = files.Write(Example());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.DoesNotContain(database.Individuals, individual => individual.XRefID == "I123");
    }

    /// <summary>
    /// A record keeps the id it has in the file when xrefs aren't replaced.
    /// </summary>
    [Fact]
    public void Record_keeps_its_id_from_the_file()
    {
        var path = files.Write(Example());

        var database = GedcomRecordReader.CreateReader(path, replaceXRefs: false).Database;

        Assert.Contains(database.Individuals, individual => individual.XRefID == "I123");
    }

    /// <summary>
    /// A pointer keeps the id from the file too, so it still finds the record.
    /// </summary>
    [Fact]
    public void Pointer_keeps_the_id_from_the_file()
    {
        var path = files.Write(Example());

        var database = GedcomRecordReader.CreateReader(path, replaceXRefs: false).Database;

        Assert.Equal("I123", database.Families.Single().Husband);
    }

    /// <summary>
    /// The ids from the file are written back when the file is saved.
    /// </summary>
    [Fact]
    public void Ids_from_the_file_are_saved()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(Example()), replaceXRefs: false).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains("0 @R321@ REPO", File.ReadAllLines(output));
    }

    /// <summary>
    /// The id the reader makes up for an inline note doesn't clash with a note id used later in the file,
    /// which stopped the file loading.
    /// </summary>
    [Fact]
    public void Inline_note_id_does_not_clash_with_a_note_id_in_the_file()
    {
        var path = files.Write(Example());

        var database = GedcomRecordReader.CreateReader(path, replaceXRefs: false).Database;

        Assert.Equal("Existing note", ((GedcomNoteRecord)database["NOTE1"]).Text);
    }

    /// <summary>
    /// A CHAR line that names another encoding makes the reader start the file again, and the ids the file
    /// uses must still be kept from inline notes then (issue #184).
    /// </summary>
    /// <param name="charset">A character set that makes the reader start again.</param>
    [Theory]
    [InlineData("ANSI")]
    [InlineData("ASCII")]
    [InlineData("IBMPC")]
    public void Inline_note_id_does_not_clash_when_the_character_set_starts_the_file_again(string charset)
    {
        var path = files.Write(Example(charset));

        var database = GedcomRecordReader.CreateReader(path, replaceXRefs: false).Database;

        Assert.Equal("Existing note", ((GedcomNoteRecord)database["NOTE1"]).Text);
    }

    /// <summary>
    /// A person added in code gets an id the file doesn't already use.
    /// </summary>
    [Fact]
    public void Person_added_in_code_gets_an_unused_id()
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @I1@ INDI", "1 NAME Jan /Roe/"));
        var database = GedcomRecordReader.CreateReader(path, replaceXRefs: false).Database;

        var exception = Record.Exception(() => new GedcomIndividualRecord(database));

        Assert.Null(exception);
    }

    /// <summary>
    /// A new record's id skips one already in the database, such as a record added in code with an id
    /// chosen by the caller.
    /// </summary>
    [Fact]
    public void New_id_skips_one_already_in_the_database()
    {
        var database = new GedcomDatabase();
        database.Add("I1", new GedcomIndividualRecord { XRefID = "I1" });

        var exception = Record.Exception(() => new GedcomIndividualRecord(database));

        Assert.Null(exception);
    }

    private static IEnumerable<string> Example(string charset = "UTF-8")
    {
        return TempGedcomFiles.Gedcom(
            charset,
            "0 @I123@ INDI",
            "1 NAME Jan /Roe/",
            "1 FAMS @F7@",
            "1 NOTE An inline note",
            "1 SOUR @S9@",
            "0 @I2@ INDI",
            "1 NAME Ann /Roe/",
            "1 FAMS @F7@",
            "0 @F7@ FAM",
            "1 HUSB @I123@",
            "1 WIFE @I2@",
            "0 @S9@ SOUR",
            "1 TITL Census",
            "1 REPO @R321@",
            "0 @R321@ REPO",
            "1 NAME Archive",
            "0 @NOTE1@ NOTE Existing note");
    }
}
