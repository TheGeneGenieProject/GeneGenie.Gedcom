// <copyright file="GedcomAgeTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.Age;

/// <summary>
/// Ages such as "42y 3m" are read from AGE tags, and each number has to keep its own digits.
/// </summary>
public sealed class GedcomAgeTest : IDisposable
{
    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// The number of years is the number written, whether it has one digit or several.
    /// </summary>
    /// <param name="value">The AGE value.</param>
    /// <param name="expectedYears">The years it says.</param>
    [Theory]
    [InlineData("5y", 5)]
    [InlineData("10y", 10)]
    [InlineData("12y", 12)]
    [InlineData("42y 3m", 42)]
    [InlineData("< 1y", 1)]
    [InlineData("> 105y", 105)]
    public void Years_are_read_as_written(string value, int expectedYears)
    {
        var database = new GedcomDatabase();

        var age = GedcomAge.Parse(value, database);

        Assert.Equal(expectedYears, age.Years);
    }

    /// <summary>
    /// Months that follow the years are read as written, not run together with the years.
    /// </summary>
    [Fact]
    public void Months_after_years_are_read_as_written()
    {
        var database = new GedcomDatabase();

        var age = GedcomAge.Parse("42y 3m", database);

        Assert.Equal(3, age.Months);
    }

    /// <summary>
    /// Days are read as written.
    /// </summary>
    [Fact]
    public void Days_are_read_as_written()
    {
        var database = new GedcomDatabase();

        var age = GedcomAge.Parse("21d", database);

        Assert.Equal(21, age.Days);
    }

    /// <summary>
    /// An age on an event in a file is read with the years the file gives.
    /// </summary>
    [Fact]
    public void Age_at_death_is_read_from_a_file()
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @I1@ INDI", "1 NAME Jane /Doe/", "1 DEAT", "2 AGE 42y"));

        var reader = GedcomRecordReader.CreateReader(path);

        Assert.Equal(42, reader.Database.Individuals.Single().Death.Age.Years);
    }
}
