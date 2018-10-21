// <copyright file="GedcomAnsiEncoderTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2018 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Tests.EncoderTests
{
    using System.Linq;
    using GeneGenie.Gedcom.Parser;
    using GeneGenie.Gedcom.Parser.Enums;
    using Xunit;

    /// <summary>
    /// Rigorous tests to ensure that ANSI files can be can be properly loaded and converted to Unicode.
    /// </summary>
    public class GedcomAnsiEncoderTest
    {
        /// <summary>
        /// Checks that the ANSI encoding is recognised.
        /// </summary>
        [Fact]
        public void Ansi_charset_is_recognised()
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\Ansi.ged");

            Assert.Equal(GedcomCharset.Ansi, reader.Parser.Charset);
        }

        /// <summary>
        /// Test to catch the addition of child records so that we can add tests for them.
        /// If this fails, we need to test the characters in the new entries.
        /// </summary>
        [Fact]
        public void Record_count_is_125_individuals()
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\Ansi.ged");

            Assert.Equal(125, reader.Database.Individuals.Count);
        }

        /// <summary>
        /// Uses the Ansi test file to test every character in ANSI can be decoded into Unicode.
        /// </summary>
        /// <param name="childName">The name of the child test record in the ansi.ged file.</param>
        /// <param name="rawAnsiValue">The expected integer value to find in the birth place field, before any translation.</param>
        /// <param name="expectedUnicode">The expected text to find in the birth place field, after translation to Unicode.</param>
        [Theory]
        [InlineData("/Euro Sign/", 128, "€")]
        [InlineData("/Single Low-9 Quotation Mark/", 130, "‚")]
        [InlineData("/Latin Small Letter F With Hook/", 131, "ƒ")]
        [InlineData("/Double Low-9 Quotation Mark/", 132, "„")]
        [InlineData("/Horizontal Ellipsis/", 133, "…")]
        [InlineData("/Dagger/", 134, "†")]
        [InlineData("/Double Dagger/", 135, "‡")]
        [InlineData("/Modifier Letter Circumflex Accent/", 136, "ˆ")]
        [InlineData("/Per Mille Sign/", 137, "‰")]
        [InlineData("/Latin Capital Letter S With Caron/", 138, "Š")]
        [InlineData("/Single Left-pointing Angle Quotation Mark/", 139, "‹")]
        [InlineData("/Latin Capital Ligature Oe/", 140, "Œ")]
        [InlineData("/Latin Capital Letter Z With Caron/", 142, "Ž")]
        [InlineData("/Left Single Quotation Mark/", 145, "‘")]
        [InlineData("/Right Single Quotation Mark/", 146, "’")]
        [InlineData("/Left Double Quotation Mark/", 147, "“")]
        [InlineData("/Right Double Quotation Mark/", 148, "”")]
        [InlineData("/Bullet/", 149, "•")]
        [InlineData("/En Dash/", 150, "–")]
        [InlineData("/Em Dash/", 151, "—")]
        [InlineData("/Small Tilde/", 152, "˜")]
        [InlineData("/Trade Mark Sign/", 153, "™")]
        [InlineData("/Latin Small Letter S With Caron/", 154, "š")]
        [InlineData("/Single Right-pointing Angle Quotation Mark/", 155, "›")]
        [InlineData("/Latin Small Ligature Oe/", 156, "œ")]
        [InlineData("/Latin Small Letter Z With Caron/", 158, "ž")]
        [InlineData("/Latin Capital Letter Y With Diaeresis/", 159, "Ÿ")]
        [InlineData("/No-break Space/", 160, "\u00a0")]
        [InlineData("/Inverted Exclamation Mark/", 161, "¡")]
        [InlineData("/Cent Sign/", 162, "¢")]
        [InlineData("/Pound Sign/", 163, "£")]
        [InlineData("/Currency Sign/", 164, "¤")]
        [InlineData("/Yen Sign/", 165, "¥")]
        [InlineData("/Broken Bar/", 166, "¦")]
        [InlineData("/Section Sign/", 167, "§")]
        [InlineData("/Diaeresis/", 168, "¨")]
        [InlineData("/Copyright Sign/", 169, "©")]
        [InlineData("/Feminine Ordinal Indicator/", 170, "ª")]
        [InlineData("/Left-pointing Double Angle Quotation Mark/", 171, "«")]
        [InlineData("/Not Sign/", 172, "¬")]
        [InlineData("/Soft Hyphen/", 173, "­")]
        [InlineData("/Registered Sign/", 174, "®")]
        [InlineData("/Macron/", 175, "¯")]
        [InlineData("/Degree Sign/", 176, "°")]
        [InlineData("/Plus-minus Sign/", 177, "±")]
        [InlineData("/Superscript Two/", 178, "²")]
        [InlineData("/Superscript Three/", 179, "³")]
        [InlineData("/Acute Accent/", 180, "´")]
        [InlineData("/Micro Sign/", 181, "µ")]
        [InlineData("/Pilcrow Sign/", 182, "¶")]
        [InlineData("/Middle Dot/", 183, "·")]
        [InlineData("/Cedilla/", 184, "¸")]
        [InlineData("/Superscript One/", 185, "¹")]
        [InlineData("/Masculine Ordinal Indicator/", 186, "º")]
        [InlineData("/Right-pointing Double Angle Quotation Mark/", 187, "»")]
        [InlineData("/Vulgar Fraction One Quarter/", 188, "¼")]
        [InlineData("/Vulgar Fraction One Half/", 189, "½")]
        [InlineData("/Vulgar Fraction Three Quarters/", 190, "¾")]
        [InlineData("/Inverted Question Mark/", 191, "¿")]
        [InlineData("/Latin Capital Letter A With Grave/", 192, "À")]
        [InlineData("/Latin Capital Letter A With Acute/", 193, "Á")]
        [InlineData("/Latin Capital Letter A With Circumflex/", 194, "Â")]
        [InlineData("/Latin Capital Letter A With Tilde/", 195, "Ã")]
        [InlineData("/Latin Capital Letter A With Diaeresis/", 196, "Ä")]
        [InlineData("/Latin Capital Letter A With Ring Above/", 197, "Å")]
        [InlineData("/Latin Capital Ligature Ae/", 198, "Æ")]
        [InlineData("/Latin Capital Letter C With Cedilla/", 199, "Ç")]
        [InlineData("/Latin Capital Letter E With Grave/", 200, "È")]
        [InlineData("/Latin Capital Letter E With Acute/", 201, "É")]
        [InlineData("/Latin Capital Letter E With Circumflex/", 202, "Ê")]
        [InlineData("/Latin Capital Letter E With Diaeresis/", 203, "Ë")]
        [InlineData("/Latin Capital Letter I With Grave/", 204, "Ì")]
        [InlineData("/Latin Capital Letter I With Acute/", 205, "Í")]
        [InlineData("/Latin Capital Letter I With Circumflex/", 206, "Î")]
        [InlineData("/Latin Capital Letter I With Diaeresis/", 207, "Ï")]
        [InlineData("/Latin Capital Letter Eth/", 208, "Ð")]
        [InlineData("/Latin Capital Letter N With Tilde/", 209, "Ñ")]
        [InlineData("/Latin Capital Letter O With Grave/", 210, "Ò")]
        [InlineData("/Latin Capital Letter O With Acute/", 211, "Ó")]
        [InlineData("/Latin Capital Letter O With Circumflex/", 212, "Ô")]
        [InlineData("/Latin Capital Letter O With Tilde/", 213, "Õ")]
        [InlineData("/Latin Capital Letter O With Diaeresis/", 214, "Ö")]
        [InlineData("/Multiplication Sign/", 215, "×")]
        [InlineData("/Latin Capital Letter O With Stroke/", 216, "Ø")]
        [InlineData("/Latin Capital Letter U With Grave/", 217, "Ù")]
        [InlineData("/Latin Capital Letter U With Acute/", 218, "Ú")]
        [InlineData("/Latin Capital Letter U With Circumflex/", 219, "Û")]
        [InlineData("/Latin Capital Letter U With Diaeresis/", 220, "Ü")]
        [InlineData("/Latin Capital Letter Y With Acute/", 221, "Ý")]
        [InlineData("/Latin Capital Letter Thorn/", 222, "Þ")]
        [InlineData("/Latin Small Letter Sharp S/", 223, "ß")]
        [InlineData("/Latin Small Letter A With Grave/", 224, "à")]
        [InlineData("/Latin Small Letter A With Acute/", 225, "á")]
        [InlineData("/Latin Small Letter A With Circumflex/", 226, "â")]
        [InlineData("/Latin Small Letter A With Tilde/", 227, "ã")]
        [InlineData("/Latin Small Letter A With Diaeresis/", 228, "ä")]
        [InlineData("/Latin Small Letter A With Ring Above/", 229, "å")]
        [InlineData("/Latin Small Ligature Ae/", 230, "æ")]
        [InlineData("/Latin Small Letter C With Cedilla/", 231, "ç")]
        [InlineData("/Latin Small Letter E With Grave/", 232, "è")]
        [InlineData("/Latin Small Letter E With Acute/", 233, "é")]
        [InlineData("/Latin Small Letter E With Circumflex/", 234, "ê")]
        [InlineData("/Latin Small Letter E With Diaeresis/", 235, "ë")]
        [InlineData("/Latin Small Letter I With Grave/", 236, "ì")]
        [InlineData("/Latin Small Letter I With Acute/", 237, "í")]
        [InlineData("/Latin Small Letter I With Circumflex/", 238, "î")]
        [InlineData("/Latin Small Letter I With Diaeresis/", 239, "ï")]
        [InlineData("/Latin Small Letter Eth/", 240, "ð")]
        [InlineData("/Latin Small Letter N With Tilde/", 241, "ñ")]
        [InlineData("/Latin Small Letter O With Grave/", 242, "ò")]
        [InlineData("/Latin Small Letter O With Acute/", 243, "ó")]
        [InlineData("/Latin Small Letter O With Circumflex/", 244, "ô")]
        [InlineData("/Latin Small Letter O With Tilde/", 245, "õ")]
        [InlineData("/Latin Small Letter O With Diaeresis/", 246, "ö")]
        [InlineData("/Division Sign/", 247, "÷")]
        [InlineData("/Latin Small Letter O With Stroke/", 248, "ø")]
        [InlineData("/Latin Small Letter U With Grave/", 249, "ù")]
        [InlineData("/Latin Small Letter U With Acute/", 250, "ú")]
        [InlineData("/Latin Small Letter U With Circumflex/", 251, "û")]
        [InlineData("/Latin Small Letter U With Diaeresis/", 252, "ü")]
        [InlineData("/Latin Small Letter Y With Acute/", 253, "ý")]
        [InlineData("/Latin Small Letter Thorn/", 254, "þ")]
        [InlineData("/Latin Small Letter Y With Diaeresis/", 255, "ÿ")]
        public void Characters_can_be_translated_to_unicode(string childName, int rawAnsiValue, string expectedUnicode)
        {
            var reader = GedcomRecordReader.CreateReader(".\\Data\\Ansi.ged");

            var child = reader.Database.Individuals.First(i => i.GetName().Name == childName);

            Assert.True(expectedUnicode == child.Birth.Place.Name, $"Ansi value of {rawAnsiValue} ({childName}) could not be translated");
        }
    }
}
