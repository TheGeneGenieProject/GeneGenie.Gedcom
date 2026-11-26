// <copyright file="GedcomSurviveMalformedTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2023 Herbert Oppmann gith@memotech.franken.de </author>

namespace GeneGenie.Gedcom;

/// <summary>
/// Tests to ensure that malformed GEDCOM files are survived.
/// </summary>

public class GedcomSurviveMalformedTest
{
    /// <summary>
    /// Heredis 7 writes a source record that breaks the specification, and the file still has to load.
    /// </summary>
    [Fact]
    public void Heredis7SourceRecord()
    {
        var exception = Record.Exception(() => GedcomRecordReader.CreateReader("./Data/Heredis7PC.ged"));

        Assert.Null(exception);
    }

    /// <summary>
    /// Records that point to a submitter id that doesn't exist still give their change dates when asked.
    /// </summary>
    [Fact]
    public void SubmitterReferenceWithWrongId()
    {
        var reader = GedcomRecordReader.CreateReader("./Data/SubmitterReference.ged");

        var exception = Record.Exception(() => FindLatestChangeDate(reader.Database));

        Assert.Null(exception);
    }

    private static GedcomChangeDate FindLatestChangeDate(GedcomDatabase database)
    {
        GedcomChangeDate latestChangeDate = null;

        foreach (var indi in database.Individuals)
        {
            if ((latestChangeDate == null) ||
                ((indi.ChangeDate != null) && (indi.ChangeDate > latestChangeDate)))
            {
                latestChangeDate = indi.ChangeDate;
            }
        }

        foreach (var fam in database.Families)
        {
            if ((latestChangeDate == null) ||
                ((fam.ChangeDate != null) && (fam.ChangeDate > latestChangeDate)))
            {
                latestChangeDate = fam.ChangeDate;
            }
        }

        return latestChangeDate;
    }
}
