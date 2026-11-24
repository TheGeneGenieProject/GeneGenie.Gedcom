// <copyright file="TempGedcomFiles.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.DataHelperExtensions;

/// <summary>
/// Writes short GEDCOM snippets to temporary files so a test can read them with the real reader,
/// and deletes every file it made when disposed. xUnit creates a new test class instance per test,
/// so a test class that owns one of these and disposes it cleans up after each test.
/// </summary>

public sealed class TempGedcomFiles : IDisposable
{
    private readonly List<string> paths = [];

    /// <summary>
    /// Builds a minimal GEDCOM 5.5.1 file around the given lines.
    /// </summary>
    /// <param name="charValue">The value of the header's CHAR tag.</param>
    /// <param name="recordLines">The lines to place between the header and the trailer.</param>
    /// <returns>The lines of the whole file.</returns>
    public static IEnumerable<string> Gedcom(string charValue, params string[] recordLines)
    {
        string[] header = ["0 HEAD", "1 GEDC", "2 VERS 5.5.1", "2 FORM LINEAGE-LINKED", $"1 CHAR {charValue}"];
        string[] trailer = ["0 TRLR"];

        return [.. header, .. recordLines, .. trailer];
    }

    /// <summary>
    /// Writes the lines to a new temporary file as UTF-8, each ending with a line feed.
    /// </summary>
    /// <param name="lines">The GEDCOM lines to write.</param>
    /// <param name="withBom">Whether to start the file with a UTF-8 byte order mark.</param>
    /// <returns>The path of the file.</returns>
    public string Write(IEnumerable<string> lines, bool withBom = false)
    {
        var path = NewPath();
        File.WriteAllText(path, string.Join("\n", lines) + "\n", new UTF8Encoding(withBom));
        return path;
    }

    /// <summary>
    /// Reserves a new temporary file path, for example to write output to.
    /// </summary>
    /// <returns>The path of an empty temporary file.</returns>
    public string NewPath()
    {
        var path = Path.GetTempFileName();
        paths.Add(path);
        return path;
    }

    /// <summary>
    /// Deletes every file this instance created.
    /// </summary>
    public void Dispose()
    {
        foreach (var path in paths)
        {
            File.Delete(path);
        }
    }
}
