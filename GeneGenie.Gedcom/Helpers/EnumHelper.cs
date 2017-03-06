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
    using System.ComponentModel;

    /// <summary>
    /// Enum helper class for parsing an enum.
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>Parses the specified value.</summary>
        /// <typeparam name="T">The enum type to parse.</typeparam>
        /// <param name="value">The text value to parse.</param>
        /// <returns>The enum equivalent of the passed text.</returns>
        public static T Parse<T>(string value)
        {
            return Parse<T>(value, false);
        }

        /// <summary>Parses the specified value.</summary>
        /// <typeparam name="T">The enum type to parse.</typeparam>
        /// <param name="value">The text value to parse.</param>
        /// <param name="ignoreCase">if set to <c>true</c> the parsing will not be case sensitive.</param>
        /// <returns>The enum equivalent of the passed text.</returns>
        public static T Parse<T>(string value, bool ignoreCase)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value, ignoreCase);
            }
            catch
            {
                return ParseByDescription<T>(value, ignoreCase);
            }
        }

        /// <summary>Parses the specified value.</summary>
        /// <typeparam name="T">The enum type to parse.</typeparam>
        /// <param name="value">The text value to parse.</param>
        /// <param name="ignoreCase">if set to <c>true</c> the parsing will not be case sensitive.</param>
        /// <param name="defaultValue">The default value if all else fails.</param>
        /// <returns>The enum equivalent of the passed text.</returns>
        public static T Parse<T>(string value, bool ignoreCase, T defaultValue)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value, ignoreCase);
            }
            catch
            {
                return ParseByDescription(value, ignoreCase, defaultValue);
            }
        }

        /// <summary>Parses a string into an enum by comparing against the description attribute of the enum.</summary>
        /// <typeparam name="T">The type of the enum to parse to, normally inferred by the compiler.</typeparam>
        /// <param name="value">The text value to parse.</param>
        /// <param name="ignoreCase">if set to <c>true</c> the parsing will not be case sensitive.</param>
        /// <returns>The enum equivalent of the passed text or a default value if parsing does not succeed.</returns>
        public static T ParseByDescription<T>(string value, bool ignoreCase)
        {
            Type enumType = typeof(T);
            foreach (var val in Enum.GetValues(enumType))
            {
                var fi = enumType.GetField(val.ToString());
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0)
                {
                    var attr = attributes[0];
                    var compareType = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                    if (attr.Description.IndexOf(value, compareType) == 0)
                    {
                        return (T)val;
                    }
                }
            }

            throw new ArgumentOutOfRangeException(nameof(value), value, $"Could not parse in to type {enumType}");
        }

        /// <summary>Parses a string into an enum by comparing against the description attribute of the enum.</summary>
        /// <typeparam name="T">The type of the enum to parse to, normally inferred by the compiler.</typeparam>
        /// <param name="value">The text value to parse.</param>
        /// <param name="ignoreCase">if set to <c>true</c> the parsing will not be case sensitive.</param>
        /// <param name="defaultValue">The default value if all else fails.</param>
        /// <returns>The enum equivalent of the passed text or a default value if parsing does not succeed.</returns>
        public static T ParseByDescription<T>(string value, bool ignoreCase, T defaultValue)
        {
            Type enumType = typeof(T);
            foreach (var val in Enum.GetValues(enumType))
            {
                var fi = enumType.GetField(val.ToString());
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0)
                {
                    var attr = attributes[0];
                    var compareType = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                    if (attr.Description.IndexOf(value, compareType) == 0)
                    {
                        return (T)val;
                    }
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Outputs a string version of an enum by using the <see cref="DescriptionAttribute"/>  attribute.
        /// Fails over to the enum name if the <see cref="DescriptionAttribute"/> does not exist.
        /// </summary>
        /// <param name="e">The enum to output.</param>
        /// <returns>A string representation of the enum.</returns>
        public static string ToDescription(this Enum e)
        {
            var attributes = (DescriptionAttribute[])e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : e.ToString();
        }
    }
}
