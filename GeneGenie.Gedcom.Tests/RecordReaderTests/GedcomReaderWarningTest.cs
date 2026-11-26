// <copyright file="GedcomReaderWarningTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.RecordReaderTests;

/// <summary>
/// Tests for the warnings the reader gives when it changes, drops or can't read part of a file, so that
/// callers can find out what happened to a file that breaks the specification (issue #182).
/// </summary>
public sealed class GedcomReaderWarningTest : IDisposable
{
    // TempGedcomFiles.Gedcom writes five header lines, so the first record starts on line 6.
    private const int FirstRecordLine = 6;

    // Files that each have one kind of problem, with the code of the warning and the line it is on.
    private static readonly (GedcomReaderWarningCode Code, string[] RecordLines, int Line)[] Problems =
    [
        (GedcomReaderWarningCode.DuplicateXref, ["0 @I1@ INDI", "1 NAME Ann /Smith/", "0 @I1@ INDI", "1 NAME Bob /Smith/"], FirstRecordLine + 2),
        (GedcomReaderWarningCode.DuplicateXref, ["0 @1@ INDI", "1 NAME Ann /Smith/", "0 @1@ FAM", "1 WIFE @1@"], FirstRecordLine + 2),
        (GedcomReaderWarningCode.RecordWithoutXref, ["0 _PLAC Somewhere", "1 _PTE @T0@"], FirstRecordLine),
        (GedcomReaderWarningCode.UnknownTag, ["0 @I1@ INDI", "1 NAME Ann /Smith/", "1 FOOBAR Something"], FirstRecordLine + 2),
        (GedcomReaderWarningCode.UnknownTag, ["0 @I1@ INDI", "1 NAME Ann /Smith/", "0 @F1@ FAM", "1 WIVE @I1@"], FirstRecordLine + 3),
        (GedcomReaderWarningCode.UnknownTag, ["0 SPAM Something"], FirstRecordLine),
        (GedcomReaderWarningCode.InvalidValue, ["0 @F1@ FAM", "1 NCHI lots"], FirstRecordLine + 1),
        (GedcomReaderWarningCode.MissingRecord, ["0 @I1@ INDI", "1 NAME Ann /Smith/", "1 SOUR @S9@"], FirstRecordLine + 2),
        (GedcomReaderWarningCode.WrongKindOfRecord, ["0 @S1@ SOUR", "1 TITL A source", "0 @F1@ FAM", "1 HUSB @S1@"], FirstRecordLine + 3),
        (GedcomReaderWarningCode.EmptyNoteDropped, ["0 @N1@ NOTE"], FirstRecordLine),
        (GedcomReaderWarningCode.EmptyNoteDropped, ["0 @I1@ INDI", "1 NAME Ann /Smith/", "1 NOTE", "1 SEX F"], FirstRecordLine + 2),
        (GedcomReaderWarningCode.AddressReadAsResidence, ["0 @I1@ INDI", "1 NAME Ann /Smith/", "1 ADDR 1 High Street"], FirstRecordLine),
        (GedcomReaderWarningCode.ParseError, ["0 @I12345678901234567890123@ INDI"], FirstRecordLine),
    ];

    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// A valid file gives no warnings, so a caller that rejects files with warnings accepts it.
    /// </summary>
    [Fact]
    public void Valid_file_gives_no_warnings()
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @I1@ INDI", "1 NAME Ann /Smith/", "1 SEX F", "1 FAMS @F1@", "0 @F1@ FAM", "1 WIFE @I1@"));

        var reader = GedcomRecordReader.CreateReader(path);

        Assert.Empty(reader.Warnings);
    }

    /// <summary>
    /// A custom tag is kept and saved, so it isn't reported as dropped.
    /// </summary>
    [Fact]
    public void Custom_tag_gives_no_warning()
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @I1@ INDI", "1 NAME Ann /Smith/", "1 _SEEN Seen in 1900"));

        var reader = GedcomRecordReader.CreateReader(path);

        Assert.Empty(reader.Warnings);
    }

    /// <summary>
    /// A link kept under a custom tag may point to any kind of record, so it isn't reported as pointing
    /// to the wrong kind.
    /// </summary>
    [Fact]
    public void Link_under_a_custom_tag_gives_no_warning()
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @S1@ SOUR", "1 TITL A source", "0 @I1@ INDI", "1 NAME Ann /Smith/", "1 _SEEN Seen in 1900", "2 REF @S1@"));

        var reader = GedcomRecordReader.CreateReader(path);

        Assert.Empty(reader.Warnings);
    }

    /// <summary>
    /// The lines under a tag the reader drops go with it, so they aren't reported again.
    /// </summary>
    [Fact]
    public void Lines_under_a_dropped_tag_are_not_reported_again()
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @I1@ INDI", "1 NAME Ann /Smith/", "1 FOOBAR Something", "2 BAZ More", "3 QUX Even more"));

        var reader = GedcomRecordReader.CreateReader(path);

        Assert.Single(reader.Warnings);
    }

    /// <summary>
    /// Each kind of problem is reported with its own code, so callers can tell them apart.
    /// </summary>
    /// <param name="code">The code expected.</param>
    /// <param name="recordLines">The file's records, which have that problem.</param>
    [Theory]
    [MemberData(nameof(ProblemCodes))]
    public void Problem_is_reported_with_its_code(GedcomReaderWarningCode code, string[] recordLines)
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", recordLines));

        var reader = GedcomRecordReader.CreateReader(path, false);

        Assert.Contains(reader.Warnings, warning => warning.Code == code);
    }

    /// <summary>
    /// Each problem is reported against the line it is on, or the line its record starts on, so the
    /// user can find it in the file.
    /// </summary>
    /// <param name="code">The code of the warning.</param>
    /// <param name="recordLines">The file's records, which have that problem.</param>
    /// <param name="line">The line expected.</param>
    [Theory]
    [MemberData(nameof(ProblemLines))]
    public void Problem_is_reported_against_its_line(GedcomReaderWarningCode code, string[] recordLines, int line)
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", recordLines));
        var reader = GedcomRecordReader.CreateReader(path, false);

        var warning = reader.Warnings.First(warning => warning.Code == code);

        Assert.Equal(line, warning.LineNumber);
    }

    /// <summary>
    /// A warning about one record gives the record's id, so a caller can find the record.
    /// </summary>
    [Fact]
    public void Warning_about_a_record_gives_its_id()
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @N1@ NOTE"));
        var reader = GedcomRecordReader.CreateReader(path, false);

        var warning = reader.Warnings.Single(warning => warning.Code == GedcomReaderWarningCode.EmptyNoteDropped);

        Assert.Equal("N1", warning.XRefId);
    }

    /// <summary>
    /// The files that have each kind of problem, with the warning's code.
    /// </summary>
    /// <returns>The code and the records.</returns>
    public static TheoryData<GedcomReaderWarningCode, string[]> ProblemCodes()
    {
        var data = new TheoryData<GedcomReaderWarningCode, string[]>();

        foreach (var (code, recordLines, _) in Problems)
        {
            data.Add(code, recordLines);
        }

        return data;
    }

    /// <summary>
    /// The files that have each kind of problem, with the warning's code and the line it is on.
    /// </summary>
    /// <returns>The code, the records and the line.</returns>
    public static TheoryData<GedcomReaderWarningCode, string[], int> ProblemLines()
    {
        var data = new TheoryData<GedcomReaderWarningCode, string[], int>();

        foreach (var (code, recordLines, line) in Problems)
        {
            data.Add(code, recordLines, line);
        }

        return data;
    }
}
