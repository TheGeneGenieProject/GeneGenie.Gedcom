// <copyright file="GedcomGedcom7DateTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.Date;

/// <summary>
/// Tests for reading dates written the GEDCOM 7 way, with the date's wording in a PHRASE line and the
/// calendar named without an escape, into the GEDCOM 5.5.1 dates the library writes (issue #118).
/// </summary>
public sealed class GedcomGedcom7DateTest : IDisposable
{
    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// A phrase with no date, as MyFamilyTree writes for "either 2020 or 2021", is kept as a GEDCOM 5.5.1
    /// date phrase.
    /// </summary>
    [Fact]
    public void Phrase_with_no_date_is_kept()
    {
        var path = files.Write(Birth("2 DATE", "3 PHRASE EITHER 2020 OR 2021"));

        var date = BirthDate(path);

        Assert.Equal("(EITHER 2020 OR 2021)", date.Period);
    }

    /// <summary>
    /// A phrase with no date that reads as a date, as MyFamilyTree writes for "after 2000", is used as the
    /// date.
    /// </summary>
    [Fact]
    public void Phrase_with_no_date_that_reads_as_a_date_is_the_date()
    {
        var path = files.Write(Birth("2 DATE", "3 PHRASE AFT 2000"));

        var date = BirthDate(path);

        Assert.Equal(GedcomDatePeriod.After, date.DatePeriod);
    }

    /// <summary>
    /// Wording that the forgiving date parser could misread, such as "around 1900", stays a phrase.
    /// </summary>
    /// <param name="phrase">The phrase, with no date.</param>
    [Theory]
    [InlineData("around 1900")]
    [InlineData("EITHER 2020 OR 2021")]
    public void Phrase_with_other_words_stays_a_phrase(string phrase)
    {
        var path = files.Write(Birth("2 DATE", "3 PHRASE " + phrase));

        var date = BirthDate(path);

        Assert.Equal($"({phrase})", date.Period);
    }

    /// <summary>
    /// Trying a phrase as a date doesn't leave a warning on the date when it isn't a valid one.
    /// </summary>
    [Fact]
    public void Phrase_that_is_not_a_valid_date_leaves_no_warning()
    {
        var path = files.Write(Birth("2 DATE", "3 PHRASE 13 13 2000"));

        var date = BirthDate(path);

        Assert.Empty(date.ParserMessages);
    }

    /// <summary>
    /// A phrase with no date is saved as a GEDCOM 5.5.1 date phrase.
    /// </summary>
    [Fact]
    public void Phrase_with_no_date_is_saved()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(Birth("2 DATE", "3 PHRASE EITHER 2020 OR 2021"))).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains("2 DATE (EITHER 2020 OR 2021)", File.ReadAllLines(output));
    }

    /// <summary>
    /// A phrase with a date is kept as a GEDCOM 5.5.1 interpreted date, the date followed by the phrase.
    /// </summary>
    [Fact]
    public void Phrase_with_a_date_is_kept_as_an_interpreted_date()
    {
        var path = files.Write(Birth("2 DATE 1 JAN 2000", "3 PHRASE New year's day"));

        var date = BirthDate(path);

        Assert.Equal("INT 1 JAN 2000 (New year's day)", date.Period);
    }

    /// <summary>
    /// A date with a phrase is still read as a date.
    /// </summary>
    [Fact]
    public void Date_with_a_phrase_is_still_parsed()
    {
        var path = files.Write(Birth("2 DATE 1 JAN 2000", "3 PHRASE New year's day"));

        var date = BirthDate(path);

        Assert.Equal(new DateTime(2000, 1, 1), date.DateTime1);
    }

    /// <summary>
    /// A GEDCOM 5.5.1 interpreted date is parsed without the phrase that follows it.
    /// </summary>
    [Fact]
    public void Interpreted_date_is_parsed_without_its_phrase()
    {
        var path = files.Write(Birth("2 DATE INT 1 JAN 2000 (New year's day)"));

        var date = BirthDate(path);

        Assert.Equal(new DateTime(2000, 1, 1), date.DateTime1);
    }

    /// <summary>
    /// GEDCOM 5.5.1 can't hold a phrase with a date like BEF 1900, so the date is kept as it was.
    /// </summary>
    [Fact]
    public void Phrase_with_a_qualified_date_keeps_the_date()
    {
        var path = files.Write(Birth("2 DATE BEF 1900", "3 PHRASE Before the war"));

        var date = BirthDate(path);

        Assert.Equal("BEF 1900", date.Period);
    }

    /// <summary>
    /// An interpreted date is saved with its phrase.
    /// </summary>
    [Fact]
    public void Phrase_with_a_date_is_saved()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(Birth("2 DATE 1 JAN 2000", "3 PHRASE New year's day"))).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains("2 DATE INT 1 JAN 2000 (New year's day)", File.ReadAllLines(output));
    }

    /// <summary>
    /// A phrase under a date the library doesn't read, such as GEDCOM 7's creation date or one inside a
    /// custom tag, doesn't change an earlier date.
    /// </summary>
    /// <param name="lines">The lines after the birth date, separated by bars.</param>
    [Theory]
    [InlineData("1 CREA|2 DATE|3 PHRASE About then")]
    [InlineData("2 _FOO|3 DATE|4 PHRASE About then")]
    public void Phrase_under_another_date_does_not_change_the_earlier_date(string lines)
    {
        var path = files.Write(Birth(["2 DATE 1 JAN 2000", .. lines.Split('|')]));

        var date = BirthDate(path);

        Assert.Equal("1 JAN 2000", date.Period);
    }

    /// <summary>
    /// A calendar named the GEDCOM 7 way, without the @#D...@ escape, is read.
    /// </summary>
    [Fact]
    public void Gedcom7_calendar_name_is_read()
    {
        var path = files.Write(Birth("2 DATE JULIAN 1 JAN 1700"));

        var date = BirthDate(path);

        Assert.Equal(GedcomDateType.Julian, date.DateType);
    }

    /// <summary>
    /// A date with a GEDCOM 7 calendar name is parsed.
    /// </summary>
    [Fact]
    public void Date_with_a_gedcom7_calendar_name_is_parsed()
    {
        var path = files.Write(Birth("2 DATE JULIAN 1 JAN 1700"));

        var date = BirthDate(path);

        Assert.NotNull(date.DateTime1);
    }

    /// <summary>
    /// A calendar is saved with the GEDCOM 5.5.1 escape spelled as the specification spells it, however
    /// the file named it.
    /// </summary>
    /// <param name="dateLine">The date as read.</param>
    [Theory]
    [InlineData("2 DATE @#DJULIAN@ 1 JAN 1700")]
    [InlineData("2 DATE JULIAN 1 JAN 1700")]
    public void Calendar_is_saved_with_its_escape(string dateLine)
    {
        var database = GedcomRecordReader.CreateReader(files.Write(Birth(dateLine))).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains("2 DATE @#DJULIAN@ 1 JAN 1700", File.ReadAllLines(output));
    }

    private static IEnumerable<string> Birth(params string[] dateLines)
    {
        return TempGedcomFiles.Gedcom("UTF-8", ["0 @I1@ INDI", "1 NAME Jan /Roe/", "1 BIRT", .. dateLines]);
    }

    private static GedcomDate BirthDate(string path)
    {
        return GedcomRecordReader.CreateReader(path).Database.Individuals.Single().Events.Single().Date;
    }
}
