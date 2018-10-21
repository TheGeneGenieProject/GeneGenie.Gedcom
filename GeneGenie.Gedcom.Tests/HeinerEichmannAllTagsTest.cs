// <copyright file="HeinerEichmannAllTagsTest.cs" company="GeneGenie.com">
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see http:www.gnu.org/licenses/ .
//
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Parser
{
    using GeneGenie.Gedcom.Parser.Enums;
    using Xunit;

    /// <summary>
    /// Ensures that the parser loads a file that contains all known GEDCOM tags.
    /// TODO: Could do with validating that it actually understood every tag in that file.
    /// </summary>
    public class HeinerEichmannAllTagsTest
    {
        /// <summary>
        /// File sourced from http://heiner-eichmann.de/gedcom/allged.htm
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
