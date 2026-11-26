// <copyright file="GedcomTortureTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom;

/// <summary>
/// Loads the torture test files to test every tag can be read at least without falling over.
/// </summary>

public class GedcomTortureTest
{
    /// <summary>
    /// Each torture test file uses every tag the specification defines, so loading each one shows the reader can read every tag without failing.
    /// </summary>
    /// <param name="sourceFile">The torture test file to load.</param>
    [Theory]
    [InlineData("./Data/TortureTests/TGC551.ged")]
    [InlineData("./Data/TortureTests/TGC551LF.ged")]
    [InlineData("./Data/TortureTests/TGC55C.ged")]
    [InlineData("./Data/TortureTests/TGC55CLF.ged")]
    public void Files_can_be_loaded_without_exceptions(string sourceFile)
    {
        var exception = Record.Exception(() => GedcomRecordReader.CreateReader(sourceFile));

        Assert.Null(exception);
    }
}
