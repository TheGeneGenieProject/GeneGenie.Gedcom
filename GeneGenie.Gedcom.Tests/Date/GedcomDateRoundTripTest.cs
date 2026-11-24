// <copyright file="GedcomDateRoundTripTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.Date;

/// <summary>
/// Tests that dates are written back as they were read. Reading interprets dates for comparison, for
/// example a year on its own as a range covering that year, and that interpretation must not change the
/// date text when the file is saved (issue #81).
/// </summary>
public sealed class GedcomDateRoundTripTest : IDisposable
{
    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// A date that hasn't been changed in code is saved exactly as it was read.
    /// </summary>
    /// <param name="date">The date as written in the file.</param>
    [Theory]
    [InlineData("2021")]
    [InlineData("MAR 2021")]
    [InlineData("3 MAR 2021")]
    [InlineData("9.4.1721")]
    [InlineData("ABT 2021")]
    [InlineData("CIRCA 1908")]
    [InlineData("EST 1908")]
    [InlineData("BEF 1900")]
    [InlineData("BET 1900 AND 1910")]
    [InlineData("FROM 1900 TO 1910")]
    [InlineData("FROM 1900")]
    public void Date_is_saved_as_it_was_read(string date)
    {
        var database = Read(date);
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains($"2 DATE {date}", File.ReadAllLines(output));
    }

    /// <summary>
    /// DateString gives the date as it was read, rather than with a period added by the interpretation.
    /// </summary>
    /// <param name="date">The date as written in the file.</param>
    [Theory]
    [InlineData("2021")]
    [InlineData("MAR 2021")]
    [InlineData("ABT 2021")]
    public void Date_string_is_the_date_as_it_was_read(string date)
    {
        var database = Read(date);

        var dateString = Birth(database).DateString;

        Assert.Equal(date, dateString);
    }

    /// <summary>
    /// Keeping the text must not change how the date is interpreted: a year on its own is still a range
    /// covering the whole year, for comparisons.
    /// </summary>
    [Fact]
    public void Year_on_its_own_is_still_interpreted_as_a_range()
    {
        var database = Read("2021");

        var period = Birth(database).DatePeriod;

        Assert.Equal(GedcomDatePeriod.Range, period);
    }

    /// <summary>
    /// A date changed in code is written from its new value, not from the text that was read.
    /// </summary>
    [Fact]
    public void Date_changed_in_code_is_saved_with_its_new_value()
    {
        var database = Read("3 MAR 2021");
        Birth(database).ParseDateString("5 APR 1999");
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains("2 DATE 5 APR 1999", File.ReadAllLines(output));
    }

    /// <summary>
    /// A partial date changed in code doesn't gain FROM: the range was the reader's interpretation, not
    /// something the date said.
    /// </summary>
    [Fact]
    public void Partial_date_changed_in_code_is_saved_without_from()
    {
        var database = Read("2021");
        Birth(database).Date1 = "1999";
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains("2 DATE 1999", File.ReadAllLines(output));
    }

    /// <summary>
    /// A date that really is a FROM range keeps FROM when its date is changed in code.
    /// </summary>
    [Fact]
    public void From_range_changed_in_code_keeps_from()
    {
        var database = Read("FROM 2021");
        Birth(database).Date1 = "1999";
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains("2 DATE FROM 1999", File.ReadAllLines(output));
    }

    private static GedcomDate Birth(GedcomDatabase database)
    {
        return database.Individuals.Single().Birth.Date;
    }

    private GedcomDatabase Read(string date)
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @I1@ INDI", "1 NAME Jane /Roe/", "1 BIRT", $"2 DATE {date}"));
        return GedcomRecordReader.CreateReader(path).Database;
    }
}
