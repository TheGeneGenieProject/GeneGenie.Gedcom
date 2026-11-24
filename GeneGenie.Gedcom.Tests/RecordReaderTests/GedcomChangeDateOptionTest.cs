// <copyright file="GedcomChangeDateOptionTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.RecordReaderTests;

/// <summary>
/// Tests for turning off the change dates (CHAN) that the library adds when a record is changed, for
/// programs that repair or convert files and shouldn't mark every record as changed (issue #80).
/// </summary>
public sealed class GedcomChangeDateOptionTest : IDisposable
{
    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// By default, changing a record still gives it a change date, so existing users see no difference.
    /// </summary>
    [Fact]
    public void Changing_a_record_adds_a_change_date_by_default()
    {
        var individual = ReadPerson().Individuals.Single();

        individual.Sex = GedcomSex.Female;

        Assert.NotNull(individual.ChangeDate);
    }

    /// <summary>
    /// With change dates turned off, changing a record doesn't give it one.
    /// </summary>
    [Fact]
    public void Changing_a_record_adds_no_change_date_when_turned_off()
    {
        var database = ReadPerson();
        database.UpdateChangeDates = false;
        var individual = database.Individuals.Single();

        individual.Sex = GedcomSex.Female;

        Assert.Null(individual.ChangeDate);
    }

    /// <summary>
    /// With change dates turned off, a change date read from the file is kept as it was.
    /// </summary>
    [Fact]
    public void Change_date_from_the_file_is_kept_when_turned_off()
    {
        var database = ReadPerson("1 CHAN", "2 DATE 03 JUN 2023", "3 TIME 10:11:13");
        database.UpdateChangeDates = false;
        var individual = database.Individuals.Single();

        individual.Sex = GedcomSex.Female;

        Assert.Equal("03 JUN 2023 10:11:13", individual.ChangeDate.DateString);
    }

    /// <summary>
    /// With change dates turned off, a record created in code has no change date either.
    /// </summary>
    [Fact]
    public void Record_created_in_code_has_no_change_date_when_turned_off()
    {
        var database = new GedcomDatabase { UpdateChangeDates = false };

        var individual = new GedcomIndividualRecord(database, "Roe");

        Assert.Null(individual.ChangeDate);
    }

    /// <summary>
    /// With change dates turned off, a changed file is saved without any CHAN lines.
    /// </summary>
    [Fact]
    public void Changed_file_is_saved_without_change_dates_when_turned_off()
    {
        var database = ReadPerson();
        database.UpdateChangeDates = false;
        database.Individuals.Single().Sex = GedcomSex.Female;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.DoesNotContain(File.ReadAllLines(output), line => line.Contains(" CHAN"));
    }

    private GedcomDatabase ReadPerson(params string[] extraLines)
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", ["0 @I1@ INDI", "1 NAME Jane /Roe/", "1 SEX M", .. extraLines]));
        return GedcomRecordReader.CreateReader(path).Database;
    }
}
