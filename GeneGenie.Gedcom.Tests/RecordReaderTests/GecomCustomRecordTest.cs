// <copyright file="GedcomCustomRecordTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2023 Herbert Oppmann gith@memotech.franken.de </author>


namespace GeneGenie.Gedcom.Tests.RecordReaderTests
{
    using GeneGenie.Gedcom.Parser;
    using System;
    using Xunit;

    /// <summary>
    /// Tests to ensure that custom records are correctly read in.
    /// </summary>
    public class GedcomCustomRecordTest
    {
        /// <summary>
        /// Test for custom record '_UID' in individual record.
        /// </summary>
        [Fact]
        public void Record_UID()
        {
            var reader = GedcomRecordReader.CreateReader("./Data/UidAndBurg.ged");
            GedcomIndividualRecord indi = reader.Database.Individuals[0];
            GedcomCustomRecord cr = indi.Custom[0];
            Assert.Equal("_UID", cr.Tag);
            Assert.Equal("A5A812A4C0FE44C9A98F8D4627073B69AB88", cr.Classification);
        }

        /// <summary>
        /// Test for custom record '_BURG' in event record.
        /// </summary>
        [Fact]
        public void Record_BURG()
        {
            var reader = GedcomRecordReader.CreateReader("./Data/UidAndBurg.ged");
            GedcomIndividualRecord indi = reader.Database.Individuals[0];
            GedcomEvent er = indi.Events[0];
            GedcomCustomRecord cr = er.Custom[0];
            Assert.Equal("_BURG", cr.Tag);
            Assert.Equal("unbekannt", cr.Classification);
        }
    }
}
