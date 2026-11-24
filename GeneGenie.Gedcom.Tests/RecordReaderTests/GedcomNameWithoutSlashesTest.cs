// <copyright file="GedcomNameWithoutSlashesTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.RecordReaderTests;

/// <summary>
/// Tests for names written without slashes. GEDCOM marks the surname with slashes, so a name without them
/// has no surname, rather than its last word being taken as one (issue #142).
/// </summary>
public sealed class GedcomNameWithoutSlashesTest : IDisposable
{
    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// A name without slashes has no surname.
    /// </summary>
    /// <param name="nameLine">The NAME line.</param>
    [Theory]
    [InlineData("1 NAME Dukak Bey")]
    [InlineData("1 NAME Dukak Bey \"Demir Yaylı\"")]
    public void Name_without_slashes_has_no_surname(string nameLine)
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @I1@ INDI", nameLine));

        var name = ReadName(path);

        Assert.Equal(string.Empty, name.Surname);
    }

    /// <summary>
    /// A name without slashes is all given name.
    /// </summary>
    [Fact]
    public void Name_without_slashes_is_all_given_name()
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @I1@ INDI", "1 NAME Dukak Bey"));

        var name = ReadName(path);

        Assert.Equal("Dukak Bey", name.Given);
    }

    /// <summary>
    /// A name with slashes still has its surname.
    /// </summary>
    [Fact]
    public void Name_with_slashes_still_has_its_surname()
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @I1@ INDI", "1 NAME Jan /Roe/"));

        var name = ReadName(path);

        Assert.Equal("Roe", name.Surname);
    }

    /// <summary>
    /// A SURN line still gives the surname when the NAME line has no slashes.
    /// </summary>
    [Fact]
    public void Surname_line_still_gives_the_surname()
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @I1@ INDI", "1 NAME Jan Roe", "2 SURN Roe"));

        var name = ReadName(path);

        Assert.Equal("Roe", name.Surname);
    }

    private static GedcomName ReadName(string path)
    {
        return GedcomRecordReader.CreateReader(path).Database.Individuals.Single().Names.Single();
    }
}
