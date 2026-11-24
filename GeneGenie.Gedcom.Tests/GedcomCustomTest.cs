// <copyright file="GedcomCustomTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2020 Ryan O'Neill https://github.com/RyanONeill1970 </author>

namespace GeneGenie.Gedcom.Tests;

/// <summary>
/// Tests for ensuring the custom.ged file can be parsed and custom fields found.
/// </summary>

public class GedcomCustomTest
{
    private GedcomRecordReader GetReader(string file)
    {
        var reader = new GedcomRecordReader();
        reader.ReadGedcom(file);
        return reader;
    }

    [Fact]
    private void Custom_marriage_name_tag_can_be_read()
    {
        var reader = GetReader("./Data/custom.ged");

        var mother = reader.Database.Individuals.SingleOrDefault(x => x.GetName().Name == "/Mother/");

        // Checks the obsolete Custom list still works for code that reads it.
#pragma warning disable CS0618
        Assert.Contains(mother.Custom, c => c.Tag == "_MARNM");
#pragma warning restore CS0618
    }

    [Fact]
    private void Custom_marriage_name_value_can_be_read()
    {
        var reader = GetReader("./Data/custom.ged");

        var mother = reader.Database.Individuals.SingleOrDefault(x => x.GetName().Name == "/Mother/");

        // Checks the obsolete Custom list still works for code that reads it.
#pragma warning disable CS0618
        Assert.Contains(mother.Custom, c => c.Classification == "/Married name/");
#pragma warning restore CS0618
    }
}
