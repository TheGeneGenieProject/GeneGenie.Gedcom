// <copyright file="GedcomTortureTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using GeneGenie.Gedcom.Parser;
    using Xunit;

    /// <summary>
    /// Loads the torture test files to test every tag can be read at least without falling over.
    /// </summary>
    public class GedcomTortureTest
    {
        [Theory]
        [InlineData(".\\Data\\TortureTests\\TGC551.ged")]
        private void Files_can_be_loaded_without_exceptions(string sourceFile)
        {
            GedcomRecordReader.CreateReader(sourceFile);
        }
    }
}
