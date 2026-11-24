// <copyright file="GedcomCustomTag.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom;

/// <summary>
/// A custom tag, one starting with an underscore that a program added and the library doesn't model, kept
/// with every line under it so it can be read and is written back when the file is saved (issue #166).
/// Find them in <see cref="GedcomRecord.CustomTags"/> and, for top-level records, in
/// <see cref="GedcomDatabase.UserDefinedRecords"/>.
/// </summary>
/// <remarks>
/// The lines under a custom tag are kept as read, whether or not their own tags are custom. A long value
/// may be split over CONC and CONT lines, which stay in <see cref="Children"/>; <see cref="Text"/> joins
/// them back together.
/// </remarks>
public class GedcomCustomTag
{
    /// <summary>
    /// Gets the lines under this one, in the order read, each with the lines under it.
    /// </summary>
    public List<GedcomCustomTag> Children { get; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="Value"/> is a pointer to another record, written
    /// as @XREF@ in the file. The reader updates pointers to match the records' new XRefIDs.
    /// </summary>
    public bool IsPointer { get; set; }

    /// <summary>
    /// Gets or sets the tag, such as _APID.
    /// </summary>
    public string Tag { get; set; }

    /// <summary>
    /// Gets the value with the CONC and CONT lines directly under it joined on, CONT starting a new line.
    /// </summary>
    public string Text
    {
        get
        {
            var text = new StringBuilder(Value);

            foreach (var child in Children)
            {
                if (child.Tag == "CONC")
                {
                    text.Append(child.Value);
                }
                else if (child.Tag == "CONT")
                {
                    text.Append(Environment.NewLine);
                    text.Append(child.Value);
                }
            }

            return text.ToString();
        }
    }

    /// <summary>
    /// Gets or sets the value on the tag's own line, or the XRefID it points to when
    /// <see cref="IsPointer"/> is set. Empty when the line has no value.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Writes this line and the lines under it.
    /// </summary>
    /// <param name="sw">The writer to output to.</param>
    /// <param name="level">The level to write this line at.</param>
    internal void Output(TextWriter sw, int level)
    {
        sw.Write(Environment.NewLine);
        sw.Write(level.ToString());
        sw.Write(" ");
        sw.Write(Tag);

        if (IsPointer)
        {
            sw.Write(" @");
            sw.Write(Value);
            sw.Write("@");
        }
        else if (!string.IsNullOrEmpty(Value))
        {
            sw.Write(" ");
            sw.Write(Value.Replace("@", "@@"));
        }

        foreach (var child in Children)
        {
            child.Output(sw, level + 1);
        }
    }

    /// <summary>
    /// Checks whether another custom tag holds the same tag, value and lines. Pointers are left out,
    /// because the reader renumbers records.
    /// </summary>
    /// <param name="other">The custom tag to compare with.</param>
    /// <returns>True if they hold the same content, otherwise false.</returns>
    internal bool HasSameContent(GedcomCustomTag other)
    {
        if (other == null || Tag != other.Tag || IsPointer != other.IsPointer)
        {
            return false;
        }

        if (!IsPointer && Value != other.Value)
        {
            return false;
        }

        if (Children.Count != other.Children.Count)
        {
            return false;
        }

        return Children.Zip(other.Children).All(pair => pair.First.HasSameContent(pair.Second));
    }
}
