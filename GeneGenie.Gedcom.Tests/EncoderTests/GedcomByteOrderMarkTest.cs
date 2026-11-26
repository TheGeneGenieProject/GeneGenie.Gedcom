// <copyright file="GedcomByteOrderMarkTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.EncoderTests;

/// <summary>
/// The reader looks at the first bytes of a file for a byte order mark, and has to use only the bytes the file has.
/// </summary>
public sealed class GedcomByteOrderMarkTest : IDisposable
{
    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// A file too short to hold a byte order mark isn't taken to be UTF-16.
    /// </summary>
    [Fact]
    public void Empty_file_is_not_read_as_unicode()
    {
        var path = files.NewPath();

        var reader = GedcomRecordReader.CreateReader(path);

        Assert.Equal(GedcomCharset.Unknown, reader.Parser.Charset);
    }
}
