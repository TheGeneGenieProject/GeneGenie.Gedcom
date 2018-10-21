// <copyright file="GedcomTortureTest.cs" company="GeneGenie.com">
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
