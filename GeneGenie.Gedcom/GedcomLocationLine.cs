// <copyright file="GedcomLocationLine.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom;

/// <summary>
/// One line of a <see cref="GedcomLocationRecord"/>, kept as it was read so the whole record can be
/// written back, including tags the library doesn't model.
/// </summary>
/// <param name="Level">The line's level below the location record, so 1 for a line directly under it.</param>
/// <param name="Tag">The line's tag, such as NAME, MAP or _CTRY.</param>
/// <param name="Value">The line's value, or an empty string if it has none. For a pointer this is the
/// record's XRefID, without the @ signs.</param>
/// <param name="IsPointer">Whether the value points at another record, such as a larger place.</param>
public sealed record GedcomLocationLine(int Level, string Tag, string Value, bool IsPointer);
