// <copyright file="GedcomUserDefinedRecord.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom;

/// <summary>
/// A top-level record with a custom tag the library doesn't model, such as the _MTTAG records Ancestry
/// writes for its tags. It is kept with every line under it, so that it is written back when the file is
/// saved and custom tags that point to it still find it (issue #166). Listed in
/// <see cref="GedcomDatabase.UserDefinedRecords"/>.
/// </summary>
/// <remarks>
/// GEDCOM 5.5.1 calls tags that start with an underscore user-defined tags. This is a whole record of
/// one, unlike <see cref="GedcomCustomTag"/>, which sits under another record. It is unrelated to
/// <see cref="GedcomCustomRecord"/>, which the library has long used for custom tags on people and events.
/// </remarks>
public class GedcomUserDefinedRecord : GedcomRecord
{
    /// <summary>
    /// Gets the lines under the record, in the order read, each with the lines under it.
    /// </summary>
    public List<GedcomCustomTag> Children { get; } = [];

    /// <summary>
    /// Gets the record's tag, the same as <see cref="Tag"/>.
    /// </summary>
    public override string GedcomTag
    {
        get { return Tag; }
    }

    /// <summary>
    /// Gets the type of the record.
    /// </summary>
    public override GedcomRecordType RecordType
    {
        get { return GedcomRecordType.UserDefinedRecord; }
    }

    /// <summary>
    /// Gets or sets the record's tag, such as _MTTAG.
    /// </summary>
    public string Tag { get; set; }

    /// <summary>
    /// Gets or sets the value on the record's own line, if it has one.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Outputs the record and every line under it as GEDCOM.
    /// </summary>
    /// <param name="sw">The writer to output to.</param>
    public override void Output(TextWriter sw)
    {
        sw.Write(Environment.NewLine);
        sw.Write(Level.ToString());
        sw.Write(" @");
        sw.Write(XrefId);
        sw.Write("@ ");
        sw.Write(Tag);

        if (!string.IsNullOrEmpty(Value))
        {
            sw.Write(" ");
            sw.Write(Value.Replace("@", "@@"));
        }

        foreach (var child in Children)
        {
            child.Output(sw, Level + 1);
        }
    }

    /// <summary>
    /// Checks if the passed record holds the same tag, value and lines as this one. Pointers are left out,
    /// because the reader renumbers records.
    /// </summary>
    /// <param name="obj">The object to compare against this instance.</param>
    /// <returns>True if the records match in user content, otherwise false.</returns>
    public override bool IsEquivalentTo(object obj)
    {
        if (obj is not GedcomUserDefinedRecord record)
        {
            return false;
        }

        return Tag == record.Tag
            && Value == record.Value
            && Children.Count == record.Children.Count
            && Children.Zip(record.Children).All(pair => pair.First.HasSameContent(pair.Second));
    }
}
