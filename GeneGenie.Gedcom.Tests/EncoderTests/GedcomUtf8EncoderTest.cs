// <copyright file="GedcomUtf8EncoderTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.EncoderTests;

/// <summary>
/// Tests that UTF-8 files are recognised from the CHAR tag and that their characters survive loading (issue #117).
/// </summary>

public sealed class GedcomUtf8EncoderTest : IDisposable
{
    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// GEDCOM 5.5.1 spells the character set "UTF-8"; some programs write "UTF8".
    /// Both must be recognised, whether or not the file starts with a byte order mark.
    /// </summary>
    /// <param name="charValue">The value of the CHAR tag in the header.</param>
    /// <param name="withBom">Whether the file starts with a UTF-8 byte order mark.</param>
    [Theory]
    [InlineData("UTF-8", false)]
    [InlineData("UTF8", false)]
    [InlineData("UTF-8", true)]
    [InlineData("UTF8", true)]
    public void Utf8_charset_is_recognised(string charValue, bool withBom)
    {
        var path = files.Write(PersonWithDiacritics(charValue), withBom);

        var reader = GedcomRecordReader.CreateReader(path);

        Assert.Equal(GedcomCharset.UTF16LE, reader.Parser.Charset);
    }

    /// <summary>
    /// A given name with diacritics must be read correctly from a UTF-8 file.
    /// </summary>
    /// <param name="charValue">The value of the CHAR tag in the header.</param>
    /// <param name="withBom">Whether the file starts with a UTF-8 byte order mark.</param>
    [Theory]
    [InlineData("UTF-8", false)]
    [InlineData("UTF8", false)]
    [InlineData("UTF-8", true)]
    public void Given_name_with_diacritics_is_read_correctly(string charValue, bool withBom)
    {
        var path = files.Write(PersonWithDiacritics(charValue), withBom);

        var reader = GedcomRecordReader.CreateReader(path);

        Assert.Equal("Zoë", reader.Database.Individuals.Single().Names.Single().Given);
    }

    /// <summary>
    /// A surname with diacritics must be read correctly from a UTF-8 file.
    /// </summary>
    /// <param name="charValue">The value of the CHAR tag in the header.</param>
    /// <param name="withBom">Whether the file starts with a UTF-8 byte order mark.</param>
    [Theory]
    [InlineData("UTF-8", false)]
    [InlineData("UTF8", false)]
    [InlineData("UTF-8", true)]
    public void Surname_with_diacritics_is_read_correctly(string charValue, bool withBom)
    {
        var path = files.Write(PersonWithDiacritics(charValue), withBom);

        var reader = GedcomRecordReader.CreateReader(path);

        Assert.Equal("Brontë-Ångström", reader.Database.Individuals.Single().Names.Single().Surname);
    }

    private static IEnumerable<string> PersonWithDiacritics(string charValue)
    {
        return TempGedcomFiles.Gedcom(charValue, "0 @I1@ INDI", "1 NAME Zoë /Brontë-Ångström/");
    }
}
