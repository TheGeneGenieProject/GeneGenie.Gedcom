// <copyright file="GedcomDuplicateXrefTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.RecordReaderTests;

/// <summary>
/// Tests for files in which a top-level record with a custom tag uses the same id as another record.
/// That isn't valid GEDCOM, but MacFamilyTree writes it, and one clash mustn't stop the whole file
/// loading (issue #180).
/// </summary>
public sealed class GedcomDuplicateXrefTest : IDisposable
{
    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// Two custom records that share an id are both kept, so neither is lost.
    /// </summary>
    /// <param name="replaceXRefs">Whether the reader replaces the file's ids.</param>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Custom_records_that_share_an_id_are_both_kept(bool replaceXRefs)
    {
        var path = files.Write(TwoCustomRecords());

        var database = GedcomRecordReader.CreateReader(path, replaceXRefs).Database;

        Assert.Equal(2, database.UserDefinedRecords.Count);
    }

    /// <summary>
    /// Two custom records that shared an id are given different ids, so both can be saved.
    /// </summary>
    /// <param name="replaceXRefs">Whether the reader replaces the file's ids.</param>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Custom_records_that_share_an_id_get_different_ids(bool replaceXRefs)
    {
        var path = files.Write(TwoCustomRecords());

        var database = GedcomRecordReader.CreateReader(path, replaceXRefs).Database;

        Assert.Equal(2, database.UserDefinedRecords.Select(record => record.XRefID).Distinct().Count());
    }

    /// <summary>
    /// Both custom records that shared an id are written when the file is saved.
    /// </summary>
    /// <param name="tag">The tag of one of the records.</param>
    [Theory]
    [InlineData("_PTF")]
    [InlineData("_STF")]
    public void Custom_records_that_share_an_id_are_both_saved(string tag)
    {
        var database = GedcomRecordReader.CreateReader(files.Write(TwoCustomRecords())).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains(File.ReadAllLines(output), line => line.StartsWith("0 @") && line.EndsWith("@ " + tag));
    }

    /// <summary>
    /// A note keeps an id it shares with a custom record, whichever comes first, so a person's link to
    /// the note still finds it.
    /// </summary>
    /// <param name="customFirst">Whether the custom record comes before the note in the file.</param>
    /// <param name="replaceXRefs">Whether the reader replaces the file's ids.</param>
    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void Note_keeps_an_id_it_shares_with_a_custom_record(bool customFirst, bool replaceXRefs)
    {
        var path = files.Write(NoteAndCustomRecord(customFirst));
        var reader = GedcomRecordReader.CreateReader(path, replaceXRefs);

        var note = reader.Database[reader.Database.Individuals.Single().Notes.Single()];

        Assert.IsType<GedcomNoteRecord>(note);
    }

    /// <summary>
    /// A custom record that shares its id with a note is kept, whichever comes first.
    /// </summary>
    /// <param name="customFirst">Whether the custom record comes before the note in the file.</param>
    /// <param name="replaceXRefs">Whether the reader replaces the file's ids.</param>
    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void Custom_record_that_shares_an_id_with_a_note_is_kept(bool customFirst, bool replaceXRefs)
    {
        var path = files.Write(NoteAndCustomRecord(customFirst));

        var database = GedcomRecordReader.CreateReader(path, replaceXRefs).Database;

        Assert.Single(database.UserDefinedRecords);
    }

    /// <summary>
    /// A link whose tag names the kind of record it points to, such as _STE, finds that record even when
    /// another record with the id came first, as MacFamilyTree's files expect. The link comes before the
    /// record in the file.
    /// </summary>
    /// <param name="replaceXRefs">Whether the reader replaces the file's ids.</param>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Link_before_a_renumbered_record_follows_its_tag(bool replaceXRefs)
    {
        var path = files.Write(TemplateRecords());
        var database = GedcomRecordReader.CreateReader(path, replaceXRefs).Database;

        var record = database[database.Sources.Single().CustomTags.Single(tag => tag.Tag == "_STE").Value];

        Assert.Equal("_STE", ((GedcomUserDefinedRecord)record).Tag);
    }

    /// <summary>
    /// A link whose tag names the kind of record it points to finds that record when the link comes after
    /// the renumbered record in the file.
    /// </summary>
    /// <param name="replaceXRefs">Whether the reader replaces the file's ids.</param>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Link_after_a_renumbered_record_follows_its_tag(bool replaceXRefs)
    {
        var path = files.Write(TemplateRecords());
        var database = GedcomRecordReader.CreateReader(path, replaceXRefs).Database;
        var entry = database.UserDefinedRecords.Single(record => record.Tag == "_STE");

        var record = database[entry.Children.Single(child => child.Tag == "_STF").Value];

        Assert.Equal("_STF", ((GedcomUserDefinedRecord)record).Tag);
    }

    /// <summary>
    /// A link to the record that kept the id still finds it.
    /// </summary>
    /// <param name="replaceXRefs">Whether the reader replaces the file's ids.</param>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Link_to_the_record_that_kept_the_id_still_finds_it(bool replaceXRefs)
    {
        var path = files.Write(TemplateRecords());
        var database = GedcomRecordReader.CreateReader(path, replaceXRefs).Database;
        var entry = database.UserDefinedRecords.Single(record => record.Tag == "_PTE");

        var record = database[entry.Children.Single(child => child.Tag == "_PTF").Value];

        Assert.Equal("_PTF", ((GedcomUserDefinedRecord)record).Tag);
    }

    /// <summary>
    /// A link that follows its tag to a renumbered record is saved with the record's new id.
    /// </summary>
    [Fact]
    public void Link_to_a_renumbered_record_is_saved_with_its_new_id()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(TemplateRecords())).Database;
        var field = database.UserDefinedRecords.Single(record => record.Tag == "_STF");
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains($"1 _STF @{field.XRefID}@", File.ReadAllLines(output));
    }

    /// <summary>
    /// Two records of the same kind that share an id are both kept, so neither is lost (issue #181).
    /// </summary>
    /// <param name="replaceXRefs">Whether the reader replaces the file's ids.</param>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Records_of_the_same_kind_that_share_an_id_are_both_kept(bool replaceXRefs)
    {
        var path = files.Write(TwoPeopleWithOneId());

        var database = GedcomRecordReader.CreateReader(path, replaceXRefs).Database;

        Assert.Equal(2, database.Individuals.Count);
    }

    /// <summary>
    /// Two records of the same kind that share an id are both written when the file is saved.
    /// </summary>
    /// <param name="name">The name of one of the people.</param>
    [Theory]
    [InlineData("1 NAME Ann /Smith/")]
    [InlineData("1 NAME Bob /Smith/")]
    public void Records_of_the_same_kind_that_share_an_id_are_both_saved(string name)
    {
        var database = GedcomRecordReader.CreateReader(files.Write(TwoPeopleWithOneId())).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains(name, File.ReadAllLines(output));
    }

    /// <summary>
    /// A link to an id that two records of the same kind share can't tell them apart, so it finds the
    /// first, as the file reads from the top.
    /// </summary>
    /// <param name="replaceXRefs">Whether the reader replaces the file's ids.</param>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Link_to_an_id_two_records_of_one_kind_share_finds_the_first(bool replaceXRefs)
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @I1@ INDI", "1 NAME Ann /Smith/", "1 NOTE @N1@", "0 @N1@ NOTE First", "0 @N1@ NOTE Second"));
        var database = GedcomRecordReader.CreateReader(path, replaceXRefs).Database;

        var note = (GedcomNoteRecord)database[database.Individuals.Single().Notes.Single()];

        Assert.Equal("First", note.Text);
    }

    /// <summary>
    /// Some programs number each kind of record from 1, so a person and a family share an id. A WIFE link
    /// names a person, so it finds the person (issue #181).
    /// </summary>
    /// <param name="replaceXRefs">Whether the reader replaces the file's ids.</param>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Wife_link_finds_the_person_when_a_family_shares_her_id(bool replaceXRefs)
    {
        var path = files.Write(PersonAndFamilyWithOneId());
        var database = GedcomRecordReader.CreateReader(path, replaceXRefs).Database;

        var wife = database[database.Families.Single().Wife];

        Assert.IsType<GedcomIndividualRecord>(wife);
    }

    /// <summary>
    /// A FAMS link names a family, so it finds the family when a person shares the family's id.
    /// </summary>
    /// <param name="replaceXRefs">Whether the reader replaces the file's ids.</param>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Spouse_link_finds_the_family_when_a_person_shares_its_id(bool replaceXRefs)
    {
        var path = files.Write(PersonAndFamilyWithOneId());
        var database = GedcomRecordReader.CreateReader(path, replaceXRefs).Database;

        var family = database[database.Individuals.Single().SpouseIn.Single().Family];

        Assert.IsType<GedcomFamilyRecord>(family);
    }

    /// <summary>
    /// A person and a family that share an id are both written when the file is saved.
    /// </summary>
    /// <param name="tag">The tag of one of the records.</param>
    [Theory]
    [InlineData("INDI")]
    [InlineData("FAM")]
    public void Person_and_family_that_share_an_id_are_both_saved(string tag)
    {
        var database = GedcomRecordReader.CreateReader(files.Write(PersonAndFamilyWithOneId())).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains(File.ReadAllLines(output), line => line.StartsWith("0 @") && line.EndsWith("@ " + tag));
    }

    /// <summary>
    /// The saved family's WIFE link points to the person, not to the family itself.
    /// </summary>
    /// <param name="replaceXRefs">Whether the reader replaces the file's ids.</param>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Saved_wife_link_points_to_the_person(bool replaceXRefs)
    {
        var database = GedcomRecordReader.CreateReader(files.Write(PersonAndFamilyWithOneId()), replaceXRefs).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains($"1 WIFE @{database.Individuals.Single().XRefID}@", File.ReadAllLines(output));
    }

    /// <summary>
    /// ANCI and DESI point to submitters, the people interested in a person's ancestors or descendants,
    /// so they find the submitter when a person shares the submitter's id.
    /// </summary>
    /// <param name="tag">The tag of the link.</param>
    [Theory]
    [InlineData("ANCI")]
    [InlineData("DESI")]
    public void Interest_link_finds_the_submitter_when_a_person_shares_its_id(string tag)
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @1@ SUBM", "1 NAME Ann Smith", "0 @1@ INDI", "1 NAME Bob /Smith/", "0 @2@ INDI", "1 NAME Cy /Smith/", $"1 {tag} @1@"));
        var database = GedcomRecordReader.CreateReader(path, false).Database;
        var person = database.Individuals.Single(individual => individual.XRefID == "2");

        var submitter = database[tag == "ANCI" ? person.Anci.Single() : person.Desi.Single()];

        Assert.IsType<GedcomSubmitterRecord>(submitter);
    }

    private static IEnumerable<string> TwoPeopleWithOneId()
    {
        return TempGedcomFiles.Gedcom("UTF-8", "0 @I1@ INDI", "1 NAME Ann /Smith/", "0 @I1@ INDI", "1 NAME Bob /Smith/");
    }

    private static IEnumerable<string> PersonAndFamilyWithOneId()
    {
        return TempGedcomFiles.Gedcom("UTF-8", "0 @1@ INDI", "1 NAME Ann /Smith/", "1 SEX F", "1 FAMS @1@", "0 @1@ FAM", "1 WIFE @1@");
    }

    // The shape of MacFamilyTree's template records, where each pair of record kinds shares ids.
    private static IEnumerable<string> TemplateRecords()
    {
        return TempGedcomFiles.Gedcom(
            "UTF-8",
            "0 @S1@ SOUR",
            "1 TITL Source",
            "1 _STE @T0@",
            "0 @TF0@ _PTF",
            "1 _TAG PersonField",
            "0 @T0@ _PTE",
            "1 _PTF @TF0@",
            "0 @TF0@ _STF",
            "1 _TAG SourceField",
            "0 @T0@ _STE",
            "1 _STF @TF0@");
    }

    private static IEnumerable<string> TwoCustomRecords()
    {
        return TempGedcomFiles.Gedcom(
            "UTF-8",
            "0 @I1@ INDI",
            "1 NAME John /Doe/",
            "0 @TF0@ _PTF",
            "1 _TAG PersonTemplateField",
            "0 @TF0@ _STF",
            "1 _TAG SourceTemplateField");
    }

    private static IEnumerable<string> NoteAndCustomRecord(bool customFirst)
    {
        string[] person = ["0 @I1@ INDI", "1 NAME John /Doe/", "1 NOTE @N1@"];
        string[] note = ["0 @N1@ NOTE A note about John"];
        string[] custom = ["0 @N1@ _PTF", "1 _TAG PersonTemplateField"];

        return TempGedcomFiles.Gedcom("UTF-8", customFirst ? [.. person, .. custom, .. note] : [.. person, .. note, .. custom]);
    }
}
