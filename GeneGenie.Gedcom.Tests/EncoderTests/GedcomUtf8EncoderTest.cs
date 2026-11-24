// <copyright file="GedcomUtf8EncoderTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.EncoderTests
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using GeneGenie.Gedcom.Enums;
    using GeneGenie.Gedcom.Parser;
    using Xunit;

    /// <summary>
    /// Tests that UTF-8 files are recognised from the CHAR tag and that their characters survive loading.
    /// </summary>
    public class GedcomUtf8EncoderTest
    {
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
            var reader = Load(charValue, withBom);

            Assert.Equal(GedcomCharset.UTF16LE, reader.Parser.Charset);
        }

        /// <summary>
        /// Names with diacritics must be read correctly from a UTF-8 file (issue #117).
        /// </summary>
        /// <param name="charValue">The value of the CHAR tag in the header.</param>
        /// <param name="withBom">Whether the file starts with a UTF-8 byte order mark.</param>
        [Theory]
        [InlineData("UTF-8", false)]
        [InlineData("UTF8", false)]
        [InlineData("UTF-8", true)]
        public void Diacritics_are_read_correctly(string charValue, bool withBom)
        {
            var reader = Load(charValue, withBom);

            var individual = reader.Database.Individuals.Single();
            Assert.Equal("Zoë", individual.Names.Single().Given);
            Assert.Equal("Brontë-Ångström", individual.Names.Single().Surname);
        }

        private static GedcomRecordReader Load(string charValue, bool withBom)
        {
            string gedcom = string.Join(
                "\n",
                "0 HEAD",
                "1 GEDC",
                "2 VERS 5.5.1",
                "2 FORM LINEAGE-LINKED",
                $"1 CHAR {charValue}",
                "0 @I1@ INDI",
                "1 NAME Zoë /Brontë-Ångström/",
                "0 TRLR",
                string.Empty);

            string path = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, gedcom, new UTF8Encoding(withBom));
                return GedcomRecordReader.CreateReader(path);
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
