// <copyright file="AnselEncoder.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2012 Richard Birkby - ThunderMain Ltd </author>
// <author> Copyright (C) 2017 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Parser
{
    using System;
    using System.Text;

    /// <summary>
    /// An encoder/decoder for the the ANSEL/US-MARC character set
    /// </summary>
    public class AnselEncoder : Encoding
    {
        private readonly Decoder decoder = new AnselDecoder();

        /// <summary>
        /// Gets the code page of ANSEL
        /// </summary>
        /// <returns>The code page identifier of the current <see cref="T:System.Text.Encoding" />.</returns>
        /// <remarks>
        /// Windows Latin-1 (close enough!)
        /// </remarks>
        public override int CodePage
        {
            get { return 1252; }
        }

        /// <summary>
        /// Gets the human-readable name of the character set
        /// </summary>
        /// <returns>The human-readable description of the current <see cref="T:System.Text.Encoding" />.</returns>
        public override string EncodingName
        {
            get { return "ANSEL"; }
        }

        /// <summary>
        /// Returns a decoder for the ANSEL character set
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Text.Decoder" /> that converts an encoded sequence of bytes into a sequence of characters.
        /// </returns>
        public override Decoder GetDecoder()
        {
            return decoder;
        }

        /// <summary>
        /// When overridden in a derived class, calculates the maximum number of characters produced by decoding the specified number of bytes.
        /// </summary>
        /// <param name="byteCount">The number of bytes to decode.</param>
        /// <returns>
        /// The maximum number of characters produced by decoding the specified number of bytes.
        /// </returns>
        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount;
        }

        /// <summary>
        /// When overridden in a derived class, calculates the number of characters produced by decoding a sequence of bytes from the specified byte array.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
        /// <param name="index">The index of the first byte to decode.</param>
        /// <param name="count">The number of bytes to decode.</param>
        /// <returns>
        /// The number of characters produced by decoding the specified sequence of bytes.
        /// </returns>
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return count;
        }

        /// <summary>
        /// When overridden in a derived class, decodes a sequence of bytes from the specified byte array into the specified character array.
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
            // Use a new Decoder each time because we shouldn't maintain state
            return new AnselDecoder().GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        }

        /********************** UNSUPPORTED FEATURES **********************/

        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="chars">The character array containing the set of characters to encode.</param>
        /// <param name="index">The index of the first character to encode.</param>
        /// <param name="count">The number of characters to encode.</param>
        /// <returns>
        /// The number of bytes produced by encoding the specified characters.
        /// </returns>
        /// <exception cref="NotImplementedException">This function is not needed and should not be called.</exception>
        public override int GetByteCount(char[] chars, int index, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, calculates the maximum number of bytes produced by encoding the specified number of characters.
        /// </summary>
        /// <param name="charCount">The number of characters to encode.</param>
        /// <returns>
        /// The maximum number of bytes produced by encoding the specified number of characters.
        /// </returns>
        public override int GetMaxByteCount(int charCount)
        {
            // StreamReader asks for this whether or not it is encoding a sequence of characters
            return charCount;
        }

        /// <summary>
        /// When overridden in a derived class, encodes a set of characters from the specified character array into the specified byte array.
        /// </summary>
        /// <param name="chars">The character array containing the set of characters to encode.</param>
        /// <param name="charIndex">The index of the first character to encode.</param>
        /// <param name="charCount">The number of characters to encode.</param>
        /// <param name="bytes">The byte array to contain the resulting sequence of bytes.</param>
        /// <param name="byteIndex">The index at which to start writing the resulting sequence of bytes.</param>
        /// <returns>
        /// The actual number of bytes written into <paramref name="bytes" />.
        /// </returns>
        /// <exception cref="NotImplementedException">This function is not needed and should not be called.</exception>
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            throw new NotImplementedException();
        }
    }
}