// <copyright file="GedcomChangeDateReadTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2023 Herbert Oppmann gith@memotech.franken.de </author>

namespace GeneGenie.Gedcom
{
    using System;
    using System.Linq;
    using GeneGenie.Gedcom.Parser;
    using Xunit;

    /// <summary>
    /// Tests that the change dates are read in for the varying record types.
    /// </summary>
    public class GedcomChangeDateReadTest
    {
        [Fact]
        private void Read_sample_and_check_changed_dates()
        {
            var reader = GedcomRecordReader.CreateReader("./Data/changedate.ged");

            // TODO: Submission records are parsed but not stored in the DataBase, so can't check this currently

            GedcomChangeDate Submitter_ChangeDate = reader.Database.Submitters.Single().ChangeDate;
            Assert.Equal("02 JUN 2023 10:11:12", Submitter_ChangeDate?.DateString);

            var father = reader.Database.Individuals.SingleOrDefault(x => x.GetName().Name == "/Father/");
            Assert.Equal("03 JUN 2023 10:11:13", father?.ChangeDate?.DateString);

            GedcomChangeDate Family_ChangeDate = reader.Database.Families.Single().ChangeDate;
            Assert.Equal("04 JUN 2023 10:11:14", Family_ChangeDate?.DateString);

            GedcomChangeDate Source_ChangeDate = reader.Database.Sources.Single().ChangeDate;
            Assert.Equal("05 JUN 2023 10:11:15", Source_ChangeDate?.DateString);

            GedcomChangeDate Repository_ChangeDate = reader.Database.Repositories.Single().ChangeDate;
            Assert.Equal("06 JUN 2023 10:11:16", Repository_ChangeDate?.DateString);

            GedcomChangeDate Note_ChangeDate = reader.Database.Notes.Single().ChangeDate;
            Assert.Equal("07 JUN 2023 10:11:17", Note_ChangeDate?.DateString);

            GedcomChangeDate Media_ChangeDate = reader.Database.Media.Single().ChangeDate;
            Assert.Equal("08 JUN 2023 10:11:18", Media_ChangeDate?.DateString);
        }
    }
}
