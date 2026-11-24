// <copyright file="GedcomMultimediaLinkTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.RecordReaderTests;

/// <summary>
/// Tests for what sits under a link to a multimedia record, such as the GEDCOM 7 CROP that MyFamilyTree
/// writes to mark a face in a photo (issue #112).
/// </summary>
public sealed class GedcomMultimediaLinkTest : IDisposable
{
    private readonly TempGedcomFiles files = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// A link to a multimedia record is listed with the record it points to.
    /// </summary>
    [Fact]
    public void Link_points_at_its_multimedia_record()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal(database.Media.Single().XRefID, database.Individuals.Single().MultimediaLinks.Single().MultimediaId);
    }

    /// <summary>
    /// The link is still in the Multimedia list of ids, so code that reads that keeps working.
    /// </summary>
    [Fact]
    public void Link_is_still_in_the_multimedia_list()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal(database.Media.Single().XRefID, database.Individuals.Single().Multimedia.Single());
    }

    /// <summary>
    /// Each part of the crop is read.
    /// </summary>
    /// <param name="part">The crop's part.</param>
    /// <param name="expected">The value in the file.</param>
    [Theory]
    [InlineData("LEFT", 110)]
    [InlineData("TOP", 29)]
    [InlineData("WIDTH", 156)]
    [InlineData("HEIGHT", 153)]
    public void Crop_part_is_read(string part, int expected)
    {
        var path = files.Write(ReportersExample());

        var crop = GedcomRecordReader.CreateReader(path).Database.Individuals.Single().MultimediaLinks.Single().Crop;

        Assert.Equal(expected, CropPart(crop, part));
    }

    /// <summary>
    /// A link without a crop has none, rather than one of zeros.
    /// </summary>
    [Fact]
    public void Link_without_a_crop_has_none()
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @I1@ INDI", "1 NAME Jane /Roe/", "1 OBJE @M1@", "0 @M1@ OBJE", "1 FILE photo.jpg", "2 FORM image/jpeg"));

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Null(database.Individuals.Single().MultimediaLinks.Single().Crop);
    }

    /// <summary>
    /// A title on the link is read.
    /// </summary>
    [Fact]
    public void Link_title_is_read()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal("Wedding day", database.Individuals.Single().MultimediaLinks.Single().Title);
    }

    /// <summary>
    /// A custom tag under the link, such as MyFamilyTree's mark for the person's main photo, is kept.
    /// </summary>
    [Fact]
    public void Custom_tag_under_a_link_is_kept()
    {
        var path = files.Write(ReportersExample());

        var database = GedcomRecordReader.CreateReader(path).Database;

        Assert.Equal("_PRIMARY_IMAGE", database.Individuals.Single().MultimediaLinks.Single().CustomTags.Single(tag => tag.Tag == "_MULTIMEDIA_USES").Value);
    }

    /// <summary>
    /// The lines under the link are saved as they were read.
    /// </summary>
    /// <param name="line">A line under the link, with its level.</param>
    [Theory]
    [InlineData("2 TITL Wedding day")]
    [InlineData("2 CROP")]
    [InlineData("3 LEFT 110")]
    [InlineData("3 TOP 29")]
    [InlineData("3 WIDTH 156")]
    [InlineData("3 HEIGHT 153")]
    [InlineData("2 _MULTIMEDIA_USES _PRIMARY_IMAGE")]
    public void Link_line_is_saved_as_read(string line)
    {
        var database = GedcomRecordReader.CreateReader(files.Write(ReportersExample())).Database;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains(line, File.ReadAllLines(output));
    }

    /// <summary>
    /// The crop is still there when the saved file is read back.
    /// </summary>
    [Fact]
    public void Crop_is_kept_after_saving()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(ReportersExample())).Database;
        var output = files.NewPath();
        GedcomRecordWriter.OutputGedcom(database, output);

        var reloaded = GedcomRecordReader.CreateReader(output).Database;

        Assert.Equal(110, reloaded.Individuals.Single().MultimediaLinks.Single().Crop.Left);
    }

    /// <summary>
    /// A crop changed in code is saved with its new value.
    /// </summary>
    [Fact]
    public void Crop_changed_in_code_is_saved()
    {
        var database = GedcomRecordReader.CreateReader(files.Write(ReportersExample())).Database;
        database.Individuals.Single().MultimediaLinks.Single().Crop.Width = 200;
        var output = files.NewPath();

        GedcomRecordWriter.OutputGedcom(database, output);

        Assert.Contains("3 WIDTH 200", File.ReadAllLines(output));
    }

    private static int? CropPart(GedcomCrop crop, string part)
    {
        return part switch
        {
            "LEFT" => crop.Left,
            "TOP" => crop.Top,
            "WIDTH" => crop.Width,
            _ => crop.Height,
        };
    }

    // Shaped like the example in issue #112, with the person and photo replaced.
    private static IEnumerable<string> ReportersExample()
    {
        return TempGedcomFiles.Gedcom(
            "UTF-8",
            "0 @I1@ INDI",
            "1 NAME Jane /Roe/",
            "1 OBJE @M1@",
            "2 TITL Wedding day",
            "2 _MULTIMEDIA_USES _PRIMARY_IMAGE",
            "2 CROP",
            "3 LEFT 110",
            "3 TOP 29",
            "3 WIDTH 156",
            "3 HEIGHT 153",
            "0 @M1@ OBJE",
            "1 FILE photo.jpg",
            "2 FORM image/jpeg");
    }
}
