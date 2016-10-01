// <copyright file="StringHelper.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Helpers
{
    /// <summary>
    /// Helper class for determining the properties of a string.
    /// </summary>
    /// <remarks>
    /// Unsure of the rationale behind this class, it could have been put in place as a faster
    /// version of the .Net BCL functions. Before replacing it with the BCL version we should
    /// benchmark it.
    /// </remarks>
    public static class StringHelper
    {
        /// <summary>
        /// Determines whether the string consists solely of whitespace.
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns>
        ///   <c>true</c> if is completely white space; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWhiteSpace(string str)
        {
            bool empty = true;

            foreach (char c in str)
            {
                if (!char.IsWhiteSpace(c))
                {
                    empty = false;
                    break;
                }
            }

            return empty;
        }
    }
}