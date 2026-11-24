// <copyright file="GedcomAncestryTagTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.RecordReaderTests;

/// <summary>
/// Tests for the tags Ancestry.com writes that aren't in GEDCOM 5.5.1: WWW under a citation's DATA, _APID on
/// a citation, and _WLNK web links on a person (issues #153 and #166).
/// </summary>
public sealed class GedcomAncestryTagTest : IDisposable
{
    private const string FirstAddressStart = "http://example.com/show?id=Rep_90c87557&page=8&cHash=a72f89b8fe81a4b36ad64cb";
    private const string FirstAddressEnd = "4505eafce";
    private const string TreeAddress = "https://www.example.com/family-tree/tree/177962383/family";

    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// A web address under a citation's DATA is read, with the CONC line Ancestry splits long addresses
    /// with joined back on.
    /// </summary>
    [Fact]
    public void Web_address_in_citation_data_is_read()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal(FirstAddressStart + FirstAddressEnd, BirthCitation(database).WebAddresses.Single());
    }

    /// <summary>
    /// The CONC under a web address belongs to the address, so it isn't added to the citation's text.
    /// </summary>
    [Fact]
    public void Web_address_does_not_change_the_citation_text()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal("27 June marriage of Jan Roe", BirthCitation(database).Text);
    }

    /// <summary>
    /// A web address is read from a citation directly under the person, whose DATA holds nothing else.
    /// </summary>
    [Fact]
    public void Web_address_on_a_citation_under_the_person_is_read()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal(TreeAddress, PersonCitation(database).WebAddresses.Single());
    }

    /// <summary>
    /// A web address is saved, even when the citation's DATA has no date or text.
    /// </summary>
    [Fact]
    public void Web_address_is_saved()
    {
        var reloaded = SaveAndReload(ReportersExample());

        Assert.Equal(TreeAddress, PersonCitation(reloaded).WebAddresses.Single());
    }

    /// <summary>
    /// A web address too long for one GEDCOM line is split when saved and joined again when read back.
    /// </summary>
    [Fact]
    public void Long_web_address_is_saved_whole()
    {
        var address = "https://example.com/" + new string('a', 400);
        var database = GedcomRecordReader.CreateReader(files.Write(ReportersExample())).Database;
        PersonCitation(database).WebAddresses[0] = address;
        var output = files.NewPath();
        GedcomRecordWriter.OutputGedcom(database, output);

        var reloaded = GedcomRecordReader.CreateReader(output).Database;

        Assert.Equal(address, PersonCitation(reloaded).WebAddresses.Single());
    }

    /// <summary>
    /// Two citations that differ only by web address aren't treated as the same citation.
    /// </summary>
    [Fact]
    public void Citations_with_different_web_addresses_are_not_equal()
    {
        var first = new GedcomSourceCitation { Page = "Page 1" };
        first.WebAddresses.Add("https://example.com/1");
        var second = new GedcomSourceCitation { Page = "Page 1" };
        second.WebAddresses.Add("https://example.com/2");

        var equal = first.IsEquivalentTo(second);

        Assert.False(equal);
    }

    /// <summary>
    /// Ancestry's record id on a citation under an event is read.
    /// </summary>
    [Fact]
    public void Ancestry_record_id_on_an_event_citation_is_read()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal("1,62308::204854828", BirthCitation(database).AncestryRecordIds.Single());
    }

    /// <summary>
    /// Every Ancestry record id on a citation is read, in order.
    /// </summary>
    [Fact]
    public void Every_Ancestry_record_id_on_a_citation_is_read()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal(["46585902219:1030:29943244", "352316457678:1030:177962383"], PersonCitation(database).AncestryRecordIds);
    }

    /// <summary>
    /// Ancestry record ids are saved under their citation.
    /// </summary>
    [Fact]
    public void Ancestry_record_id_is_saved()
    {
        var reloaded = SaveAndReload(ReportersExample());

        Assert.Equal("1,62308::204854828", BirthCitation(reloaded).AncestryRecordIds.Single());
    }

    /// <summary>
    /// A web link's title is read.
    /// </summary>
    [Fact]
    public void Web_link_title_is_read()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal("Find a Grave", database.Individuals.Single().WebLinks.Single().Title);
    }

    /// <summary>
    /// A web link's address is read from the NOTE under it, where Ancestry puts it.
    /// </summary>
    [Fact]
    public void Web_link_url_is_read()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal("https://www.example.com/memorial/1", database.Individuals.Single().WebLinks.Single().Url);
    }

    /// <summary>
    /// A web link whose NOTE points to a note record takes its address from that record.
    /// </summary>
    [Fact]
    public void Web_link_url_is_read_from_a_note_record()
    {
        var path = files.Write(TempGedcomFiles.Gedcom(
            "UTF-8",
            "0 @I1@ INDI",
            "1 NAME Jan /Roe/",
            "1 _WLNK",
            "2 TITL Find a Grave",
            "2 NOTE @N1@",
            "0 @N1@ NOTE https://www.example.com/memorial/2"));

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal("https://www.example.com/memorial/2", database.Individuals.Single().WebLinks.Single().Url);
    }

    /// <summary>
    /// A web link is saved, so the address is still there when the file is read back.
    /// </summary>
    [Fact]
    public void Web_link_is_saved()
    {
        var reloaded = SaveAndReload(ReportersExample());

        Assert.Equal("https://www.example.com/memorial/1", reloaded.Individuals.Single().WebLinks.Single().Url);
    }

    /// <summary>
    /// The web link's note is written inside the link, so saving the file again and again doesn't add a
    /// stray note record each time.
    /// </summary>
    [Fact]
    public void Saving_a_web_link_again_does_not_add_note_records()
    {
        var original = GedcomRecordReader.CreateReader(files.Write(ReportersExample())).Database;
        var once = SaveAndReload(original);

        var twice = SaveAndReload(once);

        Assert.Equal(original.Notes.Count, twice.Notes.Count);
    }

    /// <summary>
    /// Code that found the web link's address through the person's custom records still finds it there.
    /// </summary>
    [Fact]
    public void Web_link_note_is_still_on_the_custom_record()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Single(database.Individuals.Single().Custom.Single(custom => custom.Tag == "_WLNK").Notes);
    }

    private static GedcomSourceCitation BirthCitation(GedcomDatabase database)
    {
        return database.Individuals.Single().Events.Single(fact => fact.EventType == GedcomEventType.Birth).Sources.Single();
    }

    private static GedcomSourceCitation PersonCitation(GedcomDatabase database)
    {
        return database.Individuals.Single().Sources.Single();
    }

    // Shaped like the examples in issues #153 and #166, with the names and addresses replaced.
    private static IEnumerable<string> ReportersExample()
    {
        return TempGedcomFiles.Gedcom(
            "UTF-8",
            "0 @S1@ SOUR",
            "1 TITL Parish register",
            "0 @I1@ INDI",
            "1 NAME Jan /Roe/",
            "1 BIRT",
            "2 DATE 1613",
            "2 PLAC Some Town",
            "2 SOUR @S1@",
            "3 PAGE S.5",
            "3 _APID 1,62308::204854828",
            "3 DATA",
            "4 TEXT 27 June marriage of Jan Roe",
            "4 WWW " + FirstAddressStart,
            "5 CONC " + FirstAddressEnd,
            "1 SOUR @S1@",
            "2 PAGE Ancestry Family Tree",
            "2 DATA",
            "3 WWW " + TreeAddress,
            "2 _APID 46585902219:1030:29943244",
            "2 _APID 352316457678:1030:177962383",
            "1 _WLNK",
            "2 TITL Find a Grave",
            "2 NOTE https://www.example.com/memorial/1");
    }

    private GedcomDatabase SaveAndReload(IEnumerable<string> lines)
    {
        return SaveAndReload(GedcomRecordReader.CreateReader(files.Write(lines)).Database);
    }

    private GedcomDatabase SaveAndReload(GedcomDatabase database)
    {
        var output = files.NewPath();
        GedcomRecordWriter.OutputGedcom(database, output);
        return GedcomRecordReader.CreateReader(output).Database;
    }
}
