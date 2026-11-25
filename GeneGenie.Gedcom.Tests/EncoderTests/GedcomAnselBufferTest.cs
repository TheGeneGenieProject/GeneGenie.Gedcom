// <copyright file="GedcomAnselBufferTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.EncoderTests;

/// <summary>
/// Tests for ANSEL diacritics that fall at the end of a block of bytes the decoder is given, with their
/// letter at the start of the next, which made reading some files fail (issue #183).
/// </summary>
public class GedcomAnselBufferTest
{
    // The size of the blocks StreamReader reads, so a test knows where one block ends. 128 is the smallest
    // StreamReader uses, so it is used as given.
    private const int BlockSize = 128;

    // ANSEL's combining acute, grave and circumflex accents, which come before the letter they go on.
    private const byte Acute = 0xE2;
    private const byte Grave = 0xE1;
    private const byte Circumflex = 0xE3;

    /// <summary>
    /// A diacritic at the end of one block is written after its letter at the start of the next, and the
    /// rest of the text is read as it is.
    /// </summary>
    [Fact]
    public void Diacritic_before_a_letter_in_the_next_block_is_read()
    {
        var bytes = Letters('a', BlockSize - 1).Append(Acute).Concat(Letters('b', BlockSize)).ToArray();
        using var reader = AnselReader(bytes);

        var text = reader.ReadToEnd();

        Assert.Equal(new string('a', BlockSize - 1) + "b́" + new string('b', BlockSize - 1), text);
    }

    /// <summary>
    /// More diacritics in a row than ANSEL allows, at the end of a block, don't stop the text being read.
    /// </summary>
    [Fact]
    public void Too_many_diacritics_at_the_end_of_a_block_do_not_stop_the_read()
    {
        var bytes = Letters('a', BlockSize - 3).Concat([Grave, Acute, Circumflex]).Concat(Letters('b', BlockSize)).ToArray();
        using var reader = AnselReader(bytes);

        var exception = Record.Exception(() => reader.ReadToEnd());

        Assert.Null(exception);
    }

    private static IEnumerable<byte> Letters(char letter, int count)
    {
        return Enumerable.Repeat((byte)letter, count);
    }

    private static StreamReader AnselReader(byte[] bytes)
    {
        return new StreamReader(new MemoryStream(bytes), new AnselEncoder(), false, BlockSize);
    }
}
