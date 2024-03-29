﻿// <copyright file="LineTerminatorTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.RecordReaderTests
{
    using GeneGenie.Gedcom.Parser;
    using System;
    using System.IO;
    using System.Text;
    using Xunit;

    /// <summary>
    /// Tests for verifying GEDCOM line terminators can be found.
    /// </summary>
    public class LineTerminatorTests
    {
        /// <summary>
        /// Tests to ensure we can detect if the source GEDCOM file has CR, CRLF or LF terminated lines.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="expectedTerminator"></param>
        [Theory]
        [InlineData("First line. \n Second line.", "\n")]
        [InlineData("First line. \r Second line.", "\r")]
        public void Line_delimiter_can_be_parsed(string text, string expectedTerminator)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                var sr = new StreamReader(ms);

                var newlineTerminator = GedcomRecordReader.DetectNewline(sr);

                Assert.Equal(expectedTerminator, newlineTerminator);
            }
        }

        /// <summary>
        /// Tests that an if a line terminator is not found in the text, the default system newline is used.
        /// </summary>
        [Fact]
        public void System_default_line_delimiter_is_used()
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes("")))
            {
                var sr = new StreamReader(ms);

                var newlineTerminator = GedcomRecordReader.DetectNewline(sr);

                Assert.Equal(Environment.NewLine, newlineTerminator);
            }
        }
    }
}
