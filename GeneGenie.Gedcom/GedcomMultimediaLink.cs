// <copyright file="GedcomMultimediaLink.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom;

/// <summary>
/// A link from a record, such as a person, to a multimedia record, with what sits under the link: a title,
/// the GEDCOM 7 crop for the part of the image the link uses, and custom tags such as MyFamilyTree's
/// _MULTIMEDIA_USES (issue #112). See <see cref="GedcomRecord.MultimediaLinks"/>.
/// </summary>
public class GedcomMultimediaLink : GedcomRecord
{
    /// <summary>
    /// Gets or sets the part of the image the link uses, from the CROP under the link, or null if it has
    /// none. GEDCOM 5.5.1 has no CROP, but it is written back as read, so programs that read GEDCOM 7 keep it.
    /// </summary>
    public GedcomCrop Crop { get; set; }

    /// <summary>
    /// Gets the GEDCOM tag for a multimedia link.
    /// </summary>
    public override string GedcomTag
    {
        get { return "OBJE"; }
    }

    /// <summary>
    /// Gets or sets the XRefID of the multimedia record the link points to.
    /// </summary>
    public string MultimediaId { get; set; }

    /// <summary>
    /// Gets the type of the record.
    /// </summary>
    public override GedcomRecordType RecordType
    {
        get { return GedcomRecordType.MultimediaLink; }
    }

    /// <summary>
    /// Gets or sets the title for the image as used by this link, from the TITL under the link, or null.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Checks if the passed link points to the same record with the same title and crop.
    /// </summary>
    /// <param name="obj">The object to compare against this instance.</param>
    /// <returns>True if the links match in user content, otherwise false.</returns>
    public override bool IsEquivalentTo(object obj)
    {
        return obj is GedcomMultimediaLink link
            && MultimediaId == link.MultimediaId
            && Title == link.Title
            && Crop?.Left == link.Crop?.Left
            && Crop?.Top == link.Crop?.Top
            && Crop?.Width == link.Crop?.Width
            && Crop?.Height == link.Crop?.Height;
    }

    /// <summary>
    /// Writes the lines under the link, after the OBJE line its owner writes.
    /// </summary>
    /// <param name="sw">The writer to output to.</param>
    /// <param name="level">The level of the lines under the link.</param>
    internal void OutputDetails(TextWriter sw, int level)
    {
        if (!string.IsNullOrEmpty(Title))
        {
            sw.Write(Environment.NewLine);
            sw.Write($"{level} TITL {Title.Replace("@", "@@")}");
        }

        if (Crop != null)
        {
            sw.Write(Environment.NewLine);
            sw.Write($"{level} CROP");

            // In the order the GEDCOM 7 specification lists them.
            WriteCropPart(sw, level + 1, "TOP", Crop.Top);
            WriteCropPart(sw, level + 1, "LEFT", Crop.Left);
            WriteCropPart(sw, level + 1, "HEIGHT", Crop.Height);
            WriteCropPart(sw, level + 1, "WIDTH", Crop.Width);
        }

        OutputCustomTags(sw, level);
    }

    private static void WriteCropPart(TextWriter sw, int level, string tag, int? value)
    {
        if (value == null)
        {
            return;
        }

        sw.Write(Environment.NewLine);
        sw.Write($"{level} {tag} {value}");
    }
}
