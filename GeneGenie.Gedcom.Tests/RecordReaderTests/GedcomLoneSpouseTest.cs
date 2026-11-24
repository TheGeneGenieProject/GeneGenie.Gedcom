// <copyright file="GedcomLoneSpouseTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.RecordReaderTests;

/// <summary>
/// Tests for one-parent families whose only spouse is in the slot that doesn't match their sex
/// (issue #115). MyFamilyTree, for example, writes a father with no partner as WIFE.
/// </summary>

public sealed class GedcomLoneSpouseTest : IDisposable
{
    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// A lone father written as WIFE must be read as the husband, so code that looks for a
    /// child's father finds him.
    /// </summary>
    [Fact]
    public void Lone_father_written_as_wife_is_read_as_the_husband()
    {
        var path = files.Write(LoneParent("M", "WIFE"));

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal(XRefOf(database, "Parent"), database.Families.Single().Husband);
    }

    /// <summary>
    /// A lone father written as WIFE must not also be left in the wife slot.
    /// </summary>
    [Fact]
    public void Lone_father_written_as_wife_is_not_left_as_the_wife()
    {
        var path = files.Write(LoneParent("M", "WIFE"));

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.True(string.IsNullOrEmpty(database.Families.Single().Wife));
    }

    /// <summary>
    /// A lone mother written as HUSB must be read as the wife.
    /// </summary>
    [Fact]
    public void Lone_mother_written_as_husband_is_read_as_the_wife()
    {
        var path = files.Write(LoneParent("F", "HUSB"));

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal(XRefOf(database, "Parent"), database.Families.Single().Wife);
    }

    /// <summary>
    /// A lone spouse whose sex isn't recorded stays where the file put them.
    /// </summary>
    /// <param name="tag">The family tag the parent is written under.</param>
    [Theory]
    [InlineData("HUSB")]
    [InlineData("WIFE")]
    public void Lone_spouse_with_unknown_sex_is_left_where_written(string tag)
    {
        var path = files.Write(LoneParent("U", tag));

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal(XRefOf(database, "Parent"), tag == "HUSB" ? database.Families.Single().Husband : database.Families.Single().Wife);
    }

    /// <summary>
    /// A family with two spouses is left exactly as written, whatever their sex, so couples of
    /// the same sex aren't rearranged.
    /// </summary>
    [Fact]
    public void Family_with_two_spouses_is_left_as_written()
    {
        var path = files.Write(TempGedcomFiles.Gedcom(
            "UTF-8",
            "0 @I1@ INDI",
            "1 NAME First /Spouse/",
            "1 SEX M",
            "1 FAMS @F1@",
            "0 @I2@ INDI",
            "1 NAME Second /Spouse/",
            "1 SEX M",
            "1 FAMS @F1@",
            "0 @F1@ FAM",
            "1 HUSB @I1@",
            "1 WIFE @I2@"));

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal(XRefOf(database, "Second"), database.Families.Single().Wife);
    }

    /// <summary>
    /// Saving a corrected family writes the lone father as HUSB.
    /// </summary>
    [Fact]
    public void Lone_father_written_as_wife_is_saved_as_husb()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(LoneParent("M", "WIFE"))).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains($"1 HUSB @{XRefOf(database, "Parent")}@", File.ReadAllLines(output));
    }

    private static IEnumerable<string> LoneParent(string sex, string tag)
    {
        // The same shape as the reporter's export: the parent has no FAMS line, and the family
        // comes after both individuals.
        return TempGedcomFiles.Gedcom(
            "UTF-8",
            "0 @I1@ INDI",
            "1 NAME Child /Family/",
            "1 SEX M",
            "1 FAMC @F1@",
            "0 @I2@ INDI",
            "1 NAME Parent /Family/",
            $"1 SEX {sex}",
            "0 @F1@ FAM",
            $"1 {tag} @I2@",
            "1 CHIL @I1@");
    }

    private static string XRefOf(GedcomDatabase database, string given)
    {
        return database.Individuals.Single(individual => individual.Names.Single().Given == given).XRefID;
    }
}
