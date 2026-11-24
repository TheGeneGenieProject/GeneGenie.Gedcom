// <copyright file="GedcomLocationRecord.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom;

/// <summary>
/// A stand-alone place, written as a top-level _LOC record by some programs and by the GEDCOM-L
/// convention. Events point at it from their PLAC with a _LOC line (see <see cref="GedcomPlace.Location"/>),
/// and it usually holds the coordinates for the place (issue #168).
/// </summary>
/// <remarks>
/// Every line of the record is kept as read, in order, so it is written back whole, including tags the
/// library doesn't model. <see cref="Name"/>, <see cref="Latitude"/> and <see cref="Longitude"/> are read
/// from the NAME line and the MAP's LATI and LONG lines; changing them in code updates those lines when the
/// record is written.
/// </remarks>
public class GedcomLocationRecord : GedcomRecord, IEquatable<GedcomLocationRecord>
{
    private readonly List<GedcomLocationLine> lines = [];

    /// <summary>
    /// Gets the type of the record.
    /// </summary>
    public override GedcomRecordType RecordType
    {
        get { return GedcomRecordType.Location; }
    }

    /// <summary>
    /// Gets the GEDCOM tag for a location record.
    /// </summary>
    public override string GedcomTag
    {
        get { return "_LOC"; }
    }

    /// <summary>
    /// Gets every line of the record as it was read, in order, for reading tags the library doesn't model.
    /// </summary>
    public IReadOnlyList<GedcomLocationLine> Lines
    {
        get { return lines; }
    }

    /// <summary>
    /// Gets or sets the place's name, from the record's NAME line.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the place's latitude, from the LATI line under the record's MAP, such as N50.12345678.
    /// </summary>
    public string Latitude { get; set; }

    /// <summary>
    /// Gets or sets the place's longitude, from the LONG line under the record's MAP, such as E7.12345678.
    /// </summary>
    public string Longitude { get; set; }

    /// <summary>
    /// Adds a line read from the file, picking out the name and coordinates.
    /// </summary>
    /// <param name="line">The line, with its level below the record.</param>
    internal void AddLine(GedcomLocationLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        if (line.Level == 1 && line.Tag == "NAME" && Name == null)
        {
            Name = line.Value;
        }
        else if (line.Level == 2 && IsUnderMap(lines.Count))
        {
            if (line.Tag == "LATI")
            {
                Latitude = line.Value;
            }
            else if (line.Tag == "LONG")
            {
                Longitude = line.Value;
            }
        }

        lines.Add(line);
    }

    /// <summary>
    /// Outputs the record as GEDCOM, writing each line as read but with the current name and coordinates.
    /// </summary>
    /// <param name="sw">The writer to output to.</param>
    public override void Output(TextWriter sw)
    {
        sw.Write(Environment.NewLine);
        sw.Write(Level.ToString());
        sw.Write(" @");
        sw.Write(XrefId);
        sw.Write("@ _LOC");

        var nameWritten = false;
        var mapWritten = false;

        if (!lines.Any(line => line.Level == 1 && line.Tag == "NAME") && !string.IsNullOrEmpty(Name))
        {
            WriteLine(sw, 1, "NAME", Name, false);
            nameWritten = true;
        }

        for (var index = 0; index < lines.Count; index++)
        {
            var line = lines[index];

            if (line.Level == 1 && line.Tag == "NAME" && !nameWritten)
            {
                WriteLine(sw, 1, "NAME", Name, false);
                nameWritten = true;
            }
            else if (line.Level == 2 && line.Tag == "LATI" && IsUnderMap(index))
            {
                WriteLine(sw, 2, "LATI", Latitude, false);
            }
            else if (line.Level == 2 && line.Tag == "LONG" && IsUnderMap(index))
            {
                WriteLine(sw, 2, "LONG", Longitude, false);
            }
            else if (line.Level == 1 && line.Tag == "MAP" && !mapWritten)
            {
                WriteLine(sw, 1, "MAP", string.Empty, false);
                WriteMissingCoordinates(sw, index);
                mapWritten = true;
            }
            else
            {
                WriteLine(sw, line.Level, line.Tag, line.Value, line.IsPointer);
            }
        }

        if (!mapWritten && (!string.IsNullOrEmpty(Latitude) || !string.IsNullOrEmpty(Longitude)))
        {
            WriteLine(sw, 1, "MAP", string.Empty, false);
            WriteMissingCoordinates(sw, lines.Count);
        }
    }

    /// <summary>
    /// Checks if the passed location is equal in terms of user content to the current instance.
    /// Pointers are left out, because the reader renumbers records.
    /// </summary>
    /// <param name="obj">The object to compare against this instance.</param>
    /// <returns>True if the locations match in user content, otherwise false.</returns>
    public override bool IsEquivalentTo(object obj)
    {
        if (obj is not GedcomLocationRecord location)
        {
            return false;
        }

        return Name == location.Name
            && Latitude == location.Latitude
            && Longitude == location.Longitude
            && lines.Where(line => !line.IsPointer).SequenceEqual(location.lines.Where(line => !line.IsPointer));
    }

    /// <summary>
    /// Checks if the passed location is equal in terms of user content to the current instance.
    /// </summary>
    /// <param name="other">The location to compare against this instance.</param>
    /// <returns>True if the locations match in user content, otherwise false.</returns>
    public bool Equals(GedcomLocationRecord other)
    {
        return IsEquivalentTo(other);
    }

    /// <summary>
    /// Checks if the passed object is equal in terms of user content to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare against this instance.</param>
    /// <returns>True if they match in user content, otherwise false.</returns>
    public override bool Equals(object obj)
    {
        return IsEquivalentTo(obj);
    }

    /// <summary>
    /// Gets a hash code based on the user content.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Latitude, Longitude);
    }

    private static void WriteLine(TextWriter sw, int level, string tag, string value, bool isPointer)
    {
        sw.Write(Environment.NewLine);
        sw.Write(level.ToString());
        sw.Write(" ");
        sw.Write(tag);

        if (isPointer)
        {
            sw.Write(" @");
            sw.Write(value);
            sw.Write("@");
        }
        else if (!string.IsNullOrEmpty(value))
        {
            sw.Write(" ");
            sw.Write(value.Replace("@", "@@"));
        }
    }

    // Writes LATI or LONG lines that have a value but weren't in the file, after the MAP at mapIndex.
    private void WriteMissingCoordinates(TextWriter sw, int mapIndex)
    {
        var children = lines.Skip(mapIndex + 1).TakeWhile(line => line.Level > 1).ToList();

        if (!string.IsNullOrEmpty(Latitude) && !children.Any(line => line.Level == 2 && line.Tag == "LATI"))
        {
            WriteLine(sw, 2, "LATI", Latitude, false);
        }

        if (!string.IsNullOrEmpty(Longitude) && !children.Any(line => line.Level == 2 && line.Tag == "LONG"))
        {
            WriteLine(sw, 2, "LONG", Longitude, false);
        }
    }

    // Whether the line at index (or the next line to be added, when index is the count) is under a MAP line.
    private bool IsUnderMap(int index)
    {
        for (var previous = index - 1; previous >= 0; previous--)
        {
            if (lines[previous].Level == 1)
            {
                return lines[previous].Tag == "MAP";
            }
        }

        return false;
    }
}
