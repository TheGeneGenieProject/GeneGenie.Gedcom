// <copyright file="GedcomFamilyDivorceTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.Individuals;

/// <summary>
/// Tests for reaching a family's divorce event in the same way as its marriage (issue #119).
/// </summary>
public sealed class GedcomFamilyDivorceTest : IDisposable
{
    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// A family's divorce must be readable without searching its events, as the marriage already is.
    /// </summary>
    [Fact]
    public void Divorce_is_read_from_the_family()
    {
        var path = files.Write(Couple("1 MARR", "2 DATE 12 JUN 2020", "1 DIV", "2 DATE 3 SEP 2021"));

        var family = GedcomRecordReader.CreateReader(path).Database.Families.Single();

        Assert.Equal("3 SEP 2021", family.Divorce.Date.DateString);
    }

    /// <summary>
    /// Reading the divorce must not change which event is the marriage.
    /// </summary>
    [Fact]
    public void Marriage_is_still_read_when_there_is_a_divorce()
    {
        var path = files.Write(Couple("1 MARR", "2 DATE 12 JUN 2020", "1 DIV", "2 DATE 3 SEP 2021"));

        var family = GedcomRecordReader.CreateReader(path).Database.Families.Single();

        Assert.Equal("12 JUN 2020", family.Marriage.Date.DateString);
    }

    /// <summary>
    /// A family with no divorce event has no divorce, rather than throwing.
    /// </summary>
    [Fact]
    public void Family_without_a_divorce_has_none()
    {
        var path = files.Write(Couple("1 MARR", "2 DATE 12 JUN 2020"));

        var family = GedcomRecordReader.CreateReader(path).Database.Families.Single();

        Assert.Null(family.Divorce);
    }

    /// <summary>
    /// A divorce added in code is returned too, not only one read from a file.
    /// </summary>
    [Fact]
    public void Divorce_added_in_code_is_returned()
    {
        var database = new GedcomDatabase();
        var family = new GedcomFamilyRecord(database, new GedcomIndividualRecord(database, "Roe"), new GedcomIndividualRecord(database, "Roe"));
        var divorce = family.AddNewEvent(GedcomEventType.DIV);

        var found = family.Divorce;

        Assert.Same(divorce, found);
    }

    private static IEnumerable<string> Couple(params string[] familyEvents)
    {
        return TempGedcomFiles.Gedcom(
            "UTF-8",
            [
                "0 @I1@ INDI",
                "1 NAME Joe /Roe/",
                "1 SEX M",
                "1 FAMS @F1@",
                "0 @I2@ INDI",
                "1 NAME Jane /Roe/",
                "1 SEX F",
                "1 FAMS @F1@",
                "0 @F1@ FAM",
                "1 HUSB @I1@",
                "1 WIFE @I2@",
                .. familyEvents,
            ]);
    }
}
