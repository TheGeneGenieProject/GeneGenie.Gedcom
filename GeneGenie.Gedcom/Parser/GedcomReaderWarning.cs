// <copyright file="GedcomReaderWarning.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Parser;

/// <summary>
/// Something the reader changed, dropped or couldn't read while loading a file, usually because the file
/// breaks the GEDCOM specification, sometimes because the library doesn't read that part of it yet. The
/// reader is forgiving, so most such files still load. Read <see cref="GedcomRecordReader.Warnings"/> to
/// find out what it did, for example to show the user (issue #182).
/// </summary>
/// <param name="lineNumber">The number of the line in the file, counting the first as 1.</param>
/// <param name="code">The kind of problem.</param>
/// <param name="message">What was wrong and what the reader did, in plain English.</param>
/// <param name="xrefId">The id of the record the warning concerns, if there is one.</param>
public class GedcomReaderWarning(int lineNumber, GedcomReaderWarningCode code, string message, string xrefId)
{
    /// <summary>
    /// Gets the kind of problem, for code that handles some kinds differently.
    /// </summary>
    public GedcomReaderWarningCode Code { get; } = code;

    /// <summary>
    /// Gets the number of the line in the file the warning is about, counting the first line as 1. For a
    /// problem with a whole record, such as an empty note, it is the line the record starts on.
    /// </summary>
    public int LineNumber { get; } = lineNumber;

    /// <summary>
    /// Gets what was wrong and what the reader did, in plain English, for showing to a person.
    /// </summary>
    public string Message { get; } = message;

    /// <summary>
    /// Gets the id of the record the warning concerns, as the reader gave it to the record, or an empty
    /// string when the warning isn't about one record.
    /// </summary>
    public string XRefId { get; } = xrefId ?? string.Empty;

    /// <summary>
    /// Returns the line number and message, such as "Line 12: ...".
    /// </summary>
    /// <returns>The warning as text.</returns>
    public override string ToString()
    {
        return $"Line {LineNumber}: {Message}";
    }
}
