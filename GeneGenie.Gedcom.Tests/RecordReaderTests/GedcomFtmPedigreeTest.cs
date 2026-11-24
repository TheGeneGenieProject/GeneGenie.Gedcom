// <copyright file="GedcomFtmPedigreeTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.RecordReaderTests;

/// <summary>
/// Tests for Family Tree Maker's _FREL and _MREL tags, which give a child's relationship to each parent
/// on the CHIL line of the family instead of the standard PEDI tag (issue #43).
/// </summary>
public sealed class GedcomFtmPedigreeTest : IDisposable
{
    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// A child that is natural to both parents has a birth pedigree, the same as PEDI birth would give.
    /// </summary>
    [Fact]
    public void Natural_to_both_parents_is_read_as_a_birth_pedigree()
    {
        var path = files.Write(Family("Natural", "Natural"));

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal(PedigreeLinkageType.Birth, ChildLink(database).Pedigree);
    }

    /// <summary>
    /// A birth pedigree from _FREL and _MREL is saved as the standard PEDI tag, so it isn't lost.
    /// </summary>
    [Fact]
    public void Natural_to_both_parents_is_saved_as_a_standard_birth_pedigree()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(Family("Natural", "Natural"))).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains("2 PEDI birth", File.ReadAllLines(output));
    }

    /// <summary>
    /// A child adopted by the father but natural to the mother is read as adopted by the father.
    /// </summary>
    [Fact]
    public void Adopted_by_the_father_only_is_read_as_father_adopted()
    {
        var path = files.Write(Family("Adopted", "Natural"));

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal(PedigreeLinkageType.FatherAdopted, ChildLink(database).Pedigree);
    }

    /// <summary>
    /// Working out the overall pedigree must not lose the mother's own value.
    /// </summary>
    [Fact]
    public void Adopted_by_the_father_only_keeps_the_mother_natural()
    {
        var path = files.Write(Family("Adopted", "Natural"));

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal(PedigreeLinkageType.Birth, ChildLink(database).MotherPedigree);
    }

    /// <summary>
    /// A child adopted by the mother but natural to the father is read as adopted by the mother.
    /// </summary>
    [Fact]
    public void Adopted_by_the_mother_only_is_read_as_mother_adopted()
    {
        var path = files.Write(Family("Natural", "Adopted"));

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal(PedigreeLinkageType.MotherAdopted, ChildLink(database).Pedigree);
    }

    /// <summary>
    /// A mixed pedigree is saved with both _FREL and _MREL, so the natural parent survives saving and
    /// reading back.
    /// </summary>
    [Fact]
    public void Adopted_by_the_father_only_keeps_the_mother_natural_after_saving()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(Family("Adopted", "Natural"))).Database;
        var output = files.NewPath();
        GedcomRecordWriter.OutputGedcom(database, output);

        var reloaded = GedcomRecordReader.CreateReader(output).Database;

        Assert.Equal(PedigreeLinkageType.Birth, ChildLink(reloaded).MotherPedigree);
    }

    /// <summary>
    /// A family without _FREL or _MREL is saved without them, so other programs' files don't gain
    /// Family Tree Maker's tags.
    /// </summary>
    [Fact]
    public void Family_without_ftm_tags_is_saved_without_them()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(Family(null, null))).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.DoesNotContain(File.ReadAllLines(output), line => line.Contains("_FREL") || line.Contains("_MREL"));
    }

    private static GedcomFamilyLink ChildLink(GedcomDatabase database)
    {
        return database.Individuals.Single(individual => individual.Names[0].Given == "Child").ChildIn.Single();
    }

    private static IEnumerable<string> Family(string fatherRelationship, string motherRelationship)
    {
        string[] relationships = fatherRelationship == null
            ? []
            : [$"2 _FREL {fatherRelationship}", $"2 _MREL {motherRelationship}"];

        return TempGedcomFiles.Gedcom(
            "UTF-8",
            [
                "0 @I1@ INDI",
                "1 NAME Child /Roe/",
                "1 FAMC @F1@",
                "0 @I2@ INDI",
                "1 NAME Father /Roe/",
                "1 SEX M",
                "1 FAMS @F1@",
                "0 @I3@ INDI",
                "1 NAME Mother /Roe/",
                "1 SEX F",
                "1 FAMS @F1@",
                "0 @F1@ FAM",
                "1 HUSB @I2@",
                "1 WIFE @I3@",
                "1 CHIL @I1@",
                .. relationships,
            ]);
    }
}
