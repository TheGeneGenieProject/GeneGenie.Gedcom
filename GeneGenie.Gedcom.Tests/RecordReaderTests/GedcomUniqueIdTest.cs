// <copyright file="GedcomUniqueIdTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.RecordReaderTests;

/// <summary>
/// Tests for reading an individual's unique identifiers: the GEDCOM 7 UID tag and the _UID tag that many
/// GEDCOM 5.5.1 programs write (issue #143).
/// </summary>
public sealed class GedcomUniqueIdTest : IDisposable
{
    private const string FirstId = "af0b0a68-b97a-4bd5-b390-3717bdf61182";
    private const string SecondId = "A5A812A4C0FE44C9A98F8D4627073B69AB88";

    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// The GEDCOM 7 UID tag must be readable from the individual, rather than being dropped.
    /// </summary>
    /// <param name="tag">The tag the identifier is written with.</param>
    [Theory]
    [InlineData("UID")]
    [InlineData("_UID")]
    public void Unique_id_is_read_onto_the_individual(string tag)
    {
        var path = files.Write(Person($"1 {tag} {FirstId}"));

        var individual = GedcomRecordReader.CreateReader(path).Database.Individuals.Single();

        Assert.Equal([FirstId], individual.UniqueIds);
    }

    /// <summary>
    /// GEDCOM 7 allows more than one UID on a record, and all of them must be kept.
    /// </summary>
    [Fact]
    public void Every_unique_id_is_kept()
    {
        var path = files.Write(Person($"1 UID {FirstId}", $"1 _UID {SecondId}"));

        var individual = GedcomRecordReader.CreateReader(path).Database.Individuals.Single();

        Assert.Equal([FirstId, SecondId], individual.UniqueIds);
    }

    /// <summary>
    /// _UID stays in the custom records too, so code that already reads it from there keeps working.
    /// </summary>
    [Fact]
    public void Underscore_uid_is_still_a_custom_record()
    {
        var path = files.Write(Person($"1 _UID {SecondId}"));

        var individual = GedcomRecordReader.CreateReader(path).Database.Individuals.Single();

        // Checks the obsolete Custom list still works for code that reads it.
#pragma warning disable CS0618
        Assert.Contains(individual.Custom, custom => custom.Tag == "_UID" && custom.Classification == SecondId);
#pragma warning restore CS0618
    }

    /// <summary>
    /// Unique ids survive saving, written as _UID because the library writes GEDCOM 5.5.1.
    /// </summary>
    [Fact]
    public void Unique_id_is_saved_as_underscore_uid()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(Person($"1 UID {FirstId}"))).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains($"1 _UID {FirstId}", File.ReadAllLines(output));
    }

    /// <summary>
    /// An individual without a unique id has an empty list, rather than null.
    /// </summary>
    [Fact]
    public void Individual_without_a_unique_id_has_none()
    {
        var path = files.Write(Person());

        var individual = GedcomRecordReader.CreateReader(path).Database.Individuals.Single();

        Assert.Empty(individual.UniqueIds);
    }

    private static IEnumerable<string> Person(params string[] lines)
    {
        return TempGedcomFiles.Gedcom("UTF-8", ["0 @I1@ INDI", "1 NAME Jane /Roe/", .. lines]);
    }
}
