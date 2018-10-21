// <copyright file="AnselDecoder.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2012 Richard Birkby - ThunderMain Ltd </author>
// <author> Copyright (C) 2017 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Parser
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Decodes ANSEL characters into Unicode decomposed characters
    /// </summary>
    internal class AnselDecoder : Decoder
    {
        // Push diacritic characters onto this stack so we can pull them off
        // after the latin combining character has been written.
        private readonly Stack<char> diacritics = new Stack<char>(2); // ANSEL has a maximum of 2 diacritics added to a latin to form a precomposed character

        private readonly ushort[] charMapping = new ushort[]
        {
            /* Fill any unused entries with the Unicode replacement character.
             * Unicode allows us to substitute this character for any unknown
             * character in another encoding that cannot be mapped to a known
             * Unicode character */

            /* 128-140 */
            0xFFFD, 0xFFFD, 0xFFFD, 0xFFFD, 0xFFFD,
            0xFFFD, 0xFFFD, 0xFFFD, 0xFFFD, 0xFFFD,
            0xFFFD, 0xFFFD, 0xFFFD,    /* ANSEL RESERVED */

            /* 141 */ 8205, /* USMARC 8D Zero width Joiner */
            /* 142 */ 8204, /* USMARC 8E Zero width Non-Joiner */

            /* 143-160 */
            0xFFFD, 0xFFFD, 0xFFFD, 0xFFFD, 0xFFFD,
            0xFFFD, 0xFFFD, 0xFFFD, 0xFFFD, 0xFFFD,
            0xFFFD, 0xFFFD, 0xFFFD, 0xFFFD, 0xFFFD,
            0xFFFD, 0xFFFD, 0xFFFD, /* ANSEL RESERVED */

            // ANSEL Spacing Graphic Characters
            /* [161] */ 321, /* A1 */
            /* [162] */ 216, /* A2 */
            /* [163] */ 208, /* A3 */
            /* [164] */ 222, /* A4 */
            /* [165] */ 198, /* A5 */
            /* [166] */ 338, /* A6 */
            /* [167] */ 697, /* A7 */
            /* [168] */ 183, /* A8 */
            /* [169] */ 9837, /* A9 */
            /* [170] */ 174, /* AA */
            /* [171] */ 177, /* AB */
            /* [172] */ 416, /* AC */
            /* [173] */ 431, /* AD */
            /* [174] */ 702, /* AE */
            /* [175] */ 0xFFFD, /*  ANSEL RESERVED */
            /* [176] */ 703, /* B0 */
            /* [177] */ 322, /* B1 */
            /* [178] */ 248, /* B2 */
            /* [179] */ 273, /* B3 */
            /* [180] */ 254, /* B4 */
            /* [181] */ 230, /* B5 */
            /* [182] */ 339, /* B6 */
            /* [183] */ 698, /* B7 */
            /* [184] */ 305, /* B8 */
            /* [185] */ 163, /* B9 */
            /* [186] */ 240, /* BA */
            /* [187] */ 0xFFFD, /*  ANSEL RESERVED */
            /* [188] */ 417, /* BC */
            /* [189] */ 432, /* BD */
            /* [190] */ 0xFFFD, /*  ANSEL RESERVED */
            /* [191] */ 0xFFFD, /*  ANSEL RESERVED */
            /* [192] */ 176, /* C0 */
            /* [193] */ 8467, /* C1 */
            /* [194] */ 8471, /* C2 */
            /* [195] */ 169, /* C3 */
            /* [196] */ 9839, /* C4 */
            /* [197] */ 191, /* C5 */
            /* [198] */ 161, /* C6 */

            /* 199-206 */
            0xFFFD, 0xFFFD, 0xFFFD, 0xFFFD, 0xFFFD,
            0xFFFD, 0xFFFD, 0xFFFD, /* ANSEL RESERVED */

            // GEDCOM extension to ANSEL
            /* [207] */ 223, /* CF */

            /* 208-223 */
            0xFFFD, 0xFFFD, 0xFFFD, 0xFFFD, 0xFFFD,
            0xFFFD, 0xFFFD, 0xFFFD, 0xFFFD, 0xFFFD,
            0xFFFD, 0xFFFD, 0xFFFD, 0xFFFD, 0xFFFD,
            0xFFFD, /* ANSEL RESERVED */

            // ANSEL Non-spacing Graphic Characters
            /* [224] */ 777, /* E0 */
            /* [225] */ 768, /* E1 */
            /* [226] */ 769, /* E2 */
            /* [227] */ 770, /* E3 */
            /* [228] */ 771, /* E4 */
            /* [229] */ 772, /* E5 */
            /* [230] */ 774, /* E6 */
            /* [231] */ 775, /* E7 */
            /* [232] */ 776, /* E8 */
            /* [233] */ 780, /* E9 */
            /* [234] */ 778, /* EA */
            /* [235] */ 65056, /* EB */
            /* [236] */ 65057, /* EC */
            /* [237] */ 789, /* ED */
            /* [238] */ 779, /* EE */
            /* [239] */ 784, /* EF */
            /* [240] */ 807, /* F0 */
            /* [241] */ 808, /* F1 */
            /* [242] */ 803, /* F2 */
            /* [243] */ 804, /* F3 */
            /* [244] */ 805, /* F4 */
            /* [245] */ 819, /* F5 */
            /* [246] */ 818, /* F6 */
            /* [247] */ 806, /* F7 */
            /* [248] */ 796, /* F8 */
            /* [249] */ 814, /* F9 */
            /* [250] */ 65058, /* FA */
            /* [251] */ 65059, /* FB */
            /* [252] */ 0xFFFD, /*  ANSEL RESERVED */
            /* [253] */ 0xFFFD, /*  ANSEL RESERVED */
            /* [254] */ 787, /* FE */
            /* [255] */ 0xFFFD, /*  ANSEL RESERVED */
        };

        /// <summary>
        /// When overridden in a derived class, calculates the number of characters produced by decoding a sequence of bytes from the specified byte array.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
        /// <param name="index">The index of the first byte to decode.</param>
        /// <param name="count">The number of bytes to decode.</param>
        /// <returns>
        /// The number of characters produced by decoding the specified sequence of bytes and any bytes in the internal buffer.
        /// </returns>
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            // We replace ANSEL combining chars with UNICODE combining chars
            // So there is a 1:1 mapping
            return count;
        }

        /// <summary>
        /// When overridden in a derived class, decodes a sequence of bytes from the specified byte array and any bytes in the internal buffer into the specified character array.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
        /// <param name="byteIndex">The index of the first byte to decode.</param>
        /// <param name="byteCount">The number of bytes to decode.</param>
        /// <param name="chars">The character array to contain the resulting set of characters.</param>
        /// <param name="charIndex">The index at which to start writing the resulting set of characters.</param>
        /// <returns>
        /// The actual number of characters written into <paramref name="chars" />.
        /// </returns>
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            int numCharsDecoded = 0; // Index into chars array

            // For each byte in the incoming stream
            for (int i = byteIndex; i < byteCount + byteIndex; i++)
            {
                // Decode that byte into the Unicode codepoint
                char unicodeChar = DecodeAnselChar(bytes[i]);

                if (bytes[i] < 0xE0)
                {
                    // Standard spacing character
                    chars[numCharsDecoded + charIndex] = unicodeChar;
                    numCharsDecoded++;

                    // Empty the diacritic stack into the char buffer
                    while (diacritics.Count > 0)
                    {
                        // Unbox the char and append it to our result array
                        chars[numCharsDecoded + charIndex] = diacritics.Pop();

                        numCharsDecoded++;
                    }
                }
                else
                {
                    // Non-spacing combining character

                    // Box the char and place onto the stack
                    diacritics.Push(unicodeChar);
                }
            }

            return numCharsDecoded;
        }

        /// <summary>
        /// Decode the ANSEL Code point into the Unicode code point
        /// </summary>
        /// <param name="anselCodePoint">The ansel code point.</param>
        /// <returns>The Unicode character (or diacritic) mapped from the passed Ansel code point.</returns>
        private char DecodeAnselChar(byte anselCodePoint)
        {
            if (anselCodePoint <= 127)
            {
                return (char)anselCodePoint;
            }

            return (char)charMapping[anselCodePoint - 128];
        }
    }
}
