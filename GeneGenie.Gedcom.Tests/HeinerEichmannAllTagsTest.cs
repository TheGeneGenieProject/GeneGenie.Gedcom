/*
 *  Copyright  (C) 2007 David A Knight <david@ritter.demon.co.uk>
 *  Amendments (C) 2016 Ryan O'Neill <r@genegenie.com>
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 *
 */

namespace GeneGenie.Gedcom.Parser
{
    using System.IO;
    using System.Text;
    using Xunit;

    public class HeinerEichmannAllTagsTest
    {
        /// <summary>
        /// File sourced from http://heiner-eichmann.de/gedcom/allged.htm
        /// </summary>
        [Fact]
        public void Heiner_Eichmanns_test_file_with_nearly_all_tags_loads_and_parses()
        {
            var result = GedcomLoader.LoadAndParseOriginal("allged.ged");

            Assert.Equal(GedcomErrorState.NoError, result);
        }

    }
}
