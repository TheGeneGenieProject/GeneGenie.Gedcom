// <copyright file="GedcomLocationRecordTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.RecordReaderTests;

/// <summary>
/// Tests for _LOC location records, which some programs write as stand-alone places that events point to
/// from their PLAC, holding the coordinates there rather than on the event (issue #168).
/// </summary>
public sealed class GedcomLocationRecordTest : IDisposable
{
    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// Location records are kept and listed, rather than dropped as unknown records.
    /// </summary>
    [Fact]
    public void Location_record_is_listed_in_the_database()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Single(database.Locations);
    }

    /// <summary>
    /// A location record's name is read.
    /// </summary>
    [Fact]
    public void Location_name_is_read()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal("Some Town", database.Locations.Single().Name);
    }

    /// <summary>
    /// A location record's latitude is read from its MAP.
    /// </summary>
    [Fact]
    public void Location_latitude_is_read()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal("N50.12345678", database.Locations.Single().Latitude);
    }

    /// <summary>
    /// A location record's longitude is read from its MAP.
    /// </summary>
    [Fact]
    public void Location_longitude_is_read()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal("E7.12345678", database.Locations.Single().Longitude);
    }

    /// <summary>
    /// An event's place points at the location record its _LOC names.
    /// </summary>
    [Fact]
    public void Place_points_at_its_location_record()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Same(database.Locations.Single(), ResidencePlace(database).Location);
    }

    /// <summary>
    /// The reporter's use: an event's coordinates can be found through its place's location record.
    /// </summary>
    [Fact]
    public void Place_coordinates_can_be_found_through_its_location()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal("N50.12345678", ResidencePlace(database).Location.Latitude);
    }

    /// <summary>
    /// Lines the library doesn't model are kept and can be read.
    /// </summary>
    [Fact]
    public void Other_location_lines_are_kept()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Contains(database.Locations.Single().Lines, line => line.Tag == "_CTRY" && line.Value == "Deutschland");
    }

    /// <summary>
    /// Every line of a location record is saved as it was read.
    /// </summary>
    /// <param name="line">A line of the location record, with its level.</param>
    [Theory]
    [InlineData("1 NAME Some Town")]
    [InlineData("1 _STAE Some State")]
    [InlineData("1 _CTRY Deutschland")]
    [InlineData("1 MAP")]
    [InlineData("2 LATI N50.12345678")]
    [InlineData("2 LONG E7.12345678")]
    public void Location_line_is_saved_as_read(string line)
    {
        var database = GedcomRecordReader.CreateReader(files.Write(ReportersExample())).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains(line, File.ReadAllLines(output));
    }

    /// <summary>
    /// The place's pointer to its location is saved, so the link survives.
    /// </summary>
    [Fact]
    public void Place_pointer_to_its_location_is_saved()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(ReportersExample())).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains($"3 _LOC @{database.Locations.Single().XRefID}@", File.ReadAllLines(output));
    }

    /// <summary>
    /// After saving and reading back, the place still finds its location's coordinates.
    /// </summary>
    [Fact]
    public void Place_finds_its_location_after_saving()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(ReportersExample())).Database;
        var output = files.NewPath();
        GedcomRecordWriter.OutputGedcom(database, output);

        var reloaded = GedcomRecordReader.CreateReader(output).Database;

        Assert.Equal("E7.12345678", ResidencePlace(reloaded).Location.Longitude);
    }

    /// <summary>
    /// A line nested under another, such as a DATE under NAME, stays under it when saved.
    /// </summary>
    [Fact]
    public void Nested_location_line_stays_under_its_parent()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(Location("1 NAME Old Town", "2 DATE BEF 1900", "1 MAP", "2 LATI N1", "2 LONG E1"))).Database;
        var output = files.NewPath();
        GedcomRecordWriter.OutputGedcom(database, output);

        var lines = File.ReadAllLines(output).ToList();

        Assert.Equal(lines.IndexOf("1 NAME Old Town") + 1, lines.IndexOf("2 DATE BEF 1900"));
    }

    /// <summary>
    /// A pointer inside a location record, such as GEDCOM-L's link to a larger place, still points at the
    /// right record after the reader has renumbered the records.
    /// </summary>
    [Fact]
    public void Pointer_inside_a_location_record_is_saved()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(TempGedcomFiles.Gedcom(
            "UTF-8",
            "0 @L1@ _LOC",
            "1 NAME Some Town",
            "1 _LOC @L2@",
            "0 @L2@ _LOC",
            "1 NAME Some State"))).Database;
        var state = database.Locations.Single(location => location.Name == "Some State");
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains($"1 _LOC @{state.XRefID}@", File.ReadAllLines(output));
    }

    /// <summary>
    /// A coordinate changed in code is saved with its new value.
    /// </summary>
    [Fact]
    public void Latitude_changed_in_code_is_saved()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(ReportersExample())).Database;
        database.Locations.Single().Latitude = "N51.5";
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains("2 LATI N51.5", File.ReadAllLines(output));
    }

    /// <summary>
    /// A place without a _LOC has no location, rather than throwing.
    /// </summary>
    [Fact]
    public void Place_without_a_location_has_none()
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @I1@ INDI", "1 NAME Jane /Roe/", "1 RESI", "2 PLAC Some Town"));

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Null(ResidencePlace(database).Location);
    }

    private static GedcomPlace ResidencePlace(GedcomDatabase database)
    {
        // RESI is a GEDCOM attribute, so the library files it under Attributes rather than Events.
        var individual = database.Individuals.Single();
        return individual.Events.Concat(individual.Attributes).Single(fact => fact.Place != null).Place;
    }

    private static IEnumerable<string> Location(params string[] lines)
    {
        return TempGedcomFiles.Gedcom("UTF-8", ["0 @L1@ _LOC", .. lines]);
    }

    // The anonymised example from issue #168.
    private static IEnumerable<string> ReportersExample()
    {
        return TempGedcomFiles.Gedcom(
            "UTF-8",
            "0 @I1@ INDI",
            "1 NAME Jane /Roe/",
            "1 RESI",
            "2 DATE FROM 1900",
            "2 PLAC Some Town",
            "3 _LOC @XXXXXXXXXL123@",
            "2 ADDR Some Street 1",
            "3 CITY Some Town",
            "3 POST 12345",
            "3 STAE Some State",
            "3 CTRY D",
            "0 @XXXXXXXXXL123@ _LOC",
            "1 NAME Some Town",
            "1 _STAE Some State",
            "1 _CTRY Deutschland",
            "1 MAP",
            "2 LATI N50.12345678",
            "2 LONG E7.12345678");
    }
}
