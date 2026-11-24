// <copyright file="GedcomNewDatabaseTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests;

/// <summary>
/// Tests for building a database in code, rather than reading one, and writing it out (issues #121 and #136).
/// The family example is the one shown on the wiki page about creating a GEDCOM file.
/// </summary>
public sealed class GedcomNewDatabaseTest : IDisposable
{
    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// A database created with <c>new</c> has no header, writing it used to throw a NullReferenceException.
    /// </summary>
    [Fact]
    public void Empty_new_database_can_be_written()
    {
        var database = new GedcomDatabase();
        var output = files.NewPath();

        var exception = Record.Exception(() => GedcomRecordWriter.OutputGedcom(database, output));

        Assert.Null(exception);
    }

    /// <summary>
    /// A new database is written with a header, so the file is valid GEDCOM.
    /// </summary>
    [Fact]
    public void New_database_is_written_starting_with_a_header()
    {
        var database = new GedcomDatabase();
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Equal("0 HEAD", File.ReadLines(output).First());
    }

    /// <summary>
    /// Writing a database without a header must not add one to it, so the caller's database is unchanged.
    /// </summary>
    [Fact]
    public void Writing_a_new_database_leaves_it_without_a_header()
    {
        var database = new GedcomDatabase();
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Null(database.Header);
    }

    /// <summary>
    /// A header set by the caller is written instead of the default one.
    /// </summary>
    [Fact]
    public void Header_set_on_a_new_database_is_written()
    {
        var database = new GedcomDatabase();
        _ = new GedcomHeader { ApplicationSystemId = "MyApp", Database = database };
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains("1 SOUR MyApp", File.ReadAllLines(output));
    }

    /// <summary>
    /// A small family built in code can be written and read back with all its people.
    /// </summary>
    [Fact]
    public void Family_built_in_code_reads_back_with_every_individual()
    {
        var output = WriteSmallFamily();

        var reader = GedcomRecordReader.CreateReader(output);

        Assert.Equal(3, reader.Database.Individuals.Count);
    }

    /// <summary>
    /// The family built in code reads back with the father as the husband.
    /// </summary>
    [Fact]
    public void Family_built_in_code_reads_back_with_the_husband()
    {
        var output = WriteSmallFamily();

        var reader = GedcomRecordReader.CreateReader(output);

        Assert.Equal("John", GivenName(reader.Database, reader.Database.Families.Single().Husband));
    }

    /// <summary>
    /// The family built in code reads back with the mother as the wife.
    /// </summary>
    [Fact]
    public void Family_built_in_code_reads_back_with_the_wife()
    {
        var output = WriteSmallFamily();

        var reader = GedcomRecordReader.CreateReader(output);

        Assert.Equal("Mary", GivenName(reader.Database, reader.Database.Families.Single().Wife));
    }

    /// <summary>
    /// The family built in code reads back with its child.
    /// </summary>
    [Fact]
    public void Family_built_in_code_reads_back_with_the_child()
    {
        var output = WriteSmallFamily();

        var reader = GedcomRecordReader.CreateReader(output);

        Assert.Equal("Ann", GivenName(reader.Database, reader.Database.Families.Single().Children.Single()));
    }

    /// <summary>
    /// Keeping one family by deleting everyone else, as the wiki shows, leaves only that family's people.
    /// </summary>
    [Fact]
    public void Keeping_one_family_leaves_only_its_people()
    {
        var output = KeepJohnsFamily();

        var reader = GedcomRecordReader.CreateReader(output);

        Assert.Equal(["Ann", "John", "Mary"], reader.Database.Individuals.Select(i => i.Names.First().Given).Order());
    }

    /// <summary>
    /// Deleting the other people also removes records only they used, so the saved file has no
    /// pointers to records that aren't in it.
    /// </summary>
    [Fact]
    public void Keeping_one_family_leaves_no_dangling_references()
    {
        var output = KeepJohnsFamily();

        var lines = File.ReadAllLines(output);

        Assert.All(Pointers(lines), pointer => Assert.Contains(pointer, Definitions(lines)));
    }

    // The xrefs that records are defined with, from lines such as "0 @XREF1@ INDI".
    private static HashSet<string> Definitions(IEnumerable<string> lines)
    {
        return lines
            .Where(line => line.StartsWith("0 @", StringComparison.Ordinal))
            .Select(line => line.Split(' ')[1])
            .ToHashSet();
    }

    // The xrefs that other lines point to, from lines such as "1 FAMS @XREF2@".
    private static IEnumerable<string> Pointers(IEnumerable<string> lines)
    {
        return lines
            .Where(line => !line.StartsWith("0 ", StringComparison.Ordinal))
            .SelectMany(line => line.Split(' ').Skip(2))
            .Where(word => word.Length > 2 && word.StartsWith('@') && word.EndsWith('@'))
            .Distinct();
    }

    private static string GivenName(GedcomDatabase database, string xref)
    {
        return ((GedcomIndividualRecord)database[xref]).Names.First().Given;
    }

    private string KeepJohnsFamily()
    {
        var source = files.Write(TempGedcomFiles.Gedcom(
            "UTF-8",
            "0 @I1@ INDI",
            "1 NAME John /Smith/",
            "1 SEX M",
            "1 FAMS @F1@",
            "0 @I2@ INDI",
            "1 NAME Mary /Smith/",
            "1 SEX F",
            "1 FAMS @F1@",
            "0 @I3@ INDI",
            "1 NAME Ann /Smith/",
            "1 FAMC @F1@",
            "0 @I4@ INDI",
            "1 NAME Peter /Jones/",
            "1 NOTE @N1@",
            "1 FAMS @F2@",
            "0 @I5@ INDI",
            "1 NAME Susan /Jones/",
            "1 FAMS @F2@",
            "0 @F1@ FAM",
            "1 HUSB @I1@",
            "1 WIFE @I2@",
            "1 CHIL @I3@",
            "0 @F2@ FAM",
            "1 HUSB @I4@",
            "1 WIFE @I5@",
            "0 @N1@ NOTE A note about Peter"));

        // This is the example on the wiki page, line for line.
        var reader = GedcomRecordReader.CreateReader(source);
        var database = reader.Database;

        var john = database.Individuals.First(i => i.Names.First().Given == "John");
        var family = (GedcomFamilyRecord)database[john.SpouseIn.First().Family];
        var keep = new HashSet<string>(family.Children) { family.Husband, family.Wife };

        foreach (var individual in database.Individuals.Where(i => !keep.Contains(i.XRefID)).ToList())
        {
            individual.Delete();
        }

        var output = files.NewPath();
        GedcomRecordWriter.OutputGedcom(database, output);
        return output;
    }

    private string WriteSmallFamily()
    {
        // This is the example on the wiki page, line for line.
        var database = new GedcomDatabase();

        var john = new GedcomIndividualRecord(database, "Smith");
        john.Names[0].Given = "John";
        john.Sex = GedcomSex.Male;

        var mary = new GedcomIndividualRecord(database, "Smith");
        mary.Names[0].Given = "Mary";
        mary.Sex = GedcomSex.Female;

        var family = new GedcomFamilyRecord(database, john, mary);

        var ann = family.AddNewChild();
        ann.Names[0].Given = "Ann";
        ann.Sex = GedcomSex.Female;

        var output = files.NewPath();
        GedcomRecordWriter.OutputGedcom(database, output);
        return output;
    }
}
