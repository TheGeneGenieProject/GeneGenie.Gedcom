// <copyright file="GedcomSurviveMalformedTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2023 Herbert Oppmann gith@memotech.franken.de </author>


namespace GeneGenie.Gedcom
{
    using System;
    using GeneGenie.Gedcom.Parser;
    using Xunit;

    /// <summary>
    /// Tests to ensure that malformed GEDCOM files are survived.
    /// </summary>
    public class GedcomSurviveMalformedTest
    {
        [Fact]
        private void SubmitterReferenceWithWrongId()
        {
            var reader = GedcomRecordReader.CreateReader("./Data/SubmitterReference.ged");
            GedcomChangeDate LatestChangeDate = null;
            foreach (GedcomIndividualRecord indi in reader.Database.Individuals)
            {
                if ((LatestChangeDate == null) ||
                    ((indi.ChangeDate != null) && (indi.ChangeDate > LatestChangeDate)))
                {
                    LatestChangeDate = indi.ChangeDate;
                }
            }
            foreach (GedcomFamilyRecord fam in reader.Database.Families)
            {
                if ((LatestChangeDate == null) ||
                    ((fam.ChangeDate != null) && (fam.ChangeDate > LatestChangeDate)))
                {
                    LatestChangeDate = fam.ChangeDate;
                }
            }
        }
    }
}
