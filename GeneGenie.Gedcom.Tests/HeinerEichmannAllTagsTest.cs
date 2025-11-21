// <copyright file="HeinerEichmannAllTagsTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Parser
{
    using GeneGenie.Gedcom.Enums;
    using Xunit;

    /// <summary>
    /// Ensures that the parser loads a file that contains all known GEDCOM tags.
    /// TODO: Could do with validating that it actually understood every tag in that file.
    /// </summary>
    public class HeinerEichmannAllTagsTest
    {
        /// <summary>
        /// File sourced from http://heiner-eichmann.de/gedcom/allged.htm .
        /// </summary>
        [Fact]
        private void Heiner_Eichmanns_test_file_with_nearly_all_tags_loads_and_parses()
        {
            var loader = new GedcomLoader();

            var result = loader.LoadAndParse("allged.ged");

            Assert.Equal(GedcomErrorState.NoError, result.ErrorState);
        }
    }
}
