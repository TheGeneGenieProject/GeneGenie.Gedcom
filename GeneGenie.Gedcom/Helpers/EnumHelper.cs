// <copyright file="EnumHelper.cs" company="GeneGenie.com">
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
    using System;

    /// <summary>
    /// Enum helper class for parsing an enum.
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>Parses the specified value.</summary>
        /// <typeparam name="T">The enum type to parse.</typeparam>
        /// <param name="val">The text value to parse.</param>
        /// <returns>The enum equivalent of the passed text.</returns>
        public static T Parse<T>(string val)
        {
            return Parse<T>(val, false);
        }

        /// <summary>Parses the specified value.</summary>
        /// <typeparam name="T">The enum type to parse.</typeparam>
        /// <param name="val">The text value to parse.</param>
        /// <param name="ignoreCase">if set to <c>true</c> the parsing will not be case sensitive.</param>
        /// <returns>The enum equivalent of the passed text.</returns>
        public static T Parse<T>(string val, bool ignoreCase)
        {
            return (T)Enum.Parse(typeof(T), val, ignoreCase);
        }

        /// <summary>Parses the specified value.</summary>
        /// <typeparam name="T">The enum type to parse.</typeparam>
        /// <param name="val">The text value to parse.</param>
        /// <param name="ignoreCase">if set to <c>true</c> the parsing will not be case sensitive.</param>
        /// <param name="defaultValue">The default value if all else fails.</param>
        /// <returns>The enum equivalent of the passed text.</returns>
        public static T Parse<T>(string val, bool ignoreCase, T defaultValue)
        {
            T ret = defaultValue;
            try
            {
                ret = (T)Enum.Parse(typeof(T), val, ignoreCase);
            }
            catch
            {
            }

            return ret;
        }
    }
}
