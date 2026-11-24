// <copyright file="GedcomCrop.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom;

/// <summary>
/// The part of an image a link uses, such as one person's face in a group photo, from the GEDCOM 7 CROP
/// structure under a link to a multimedia record (issue #112). Values are in pixels from the image's top
/// left corner. See <see cref="GedcomMultimediaLink.Crop"/>.
/// </summary>
/// <remarks>
/// Each part is null when the file leaves it out. GEDCOM 7 then treats LEFT and TOP as 0, and WIDTH and
/// HEIGHT as reaching the image's right and bottom edges.
/// </remarks>
public class GedcomCrop
{
    /// <summary>
    /// Gets or sets the height of the area, from the HEIGHT line.
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// Gets or sets how far the area starts from the image's left edge, from the LEFT line.
    /// </summary>
    public int? Left { get; set; }

    /// <summary>
    /// Gets or sets how far the area starts from the image's top edge, from the TOP line.
    /// </summary>
    public int? Top { get; set; }

    /// <summary>
    /// Gets or sets the width of the area, from the WIDTH line.
    /// </summary>
    public int? Width { get; set; }
}
