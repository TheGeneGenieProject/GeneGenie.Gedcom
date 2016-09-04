// <copyright file="StaticData.cs" company="GeneGenie.com">
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
// <author> Copyright (C) 2007-2008 David A Knight david@ritter.demon.co.uk </author>

namespace GeneGenie.Gedcom.Parser
{
    /// <summary>
    /// Lists of error strings etc that don't change.
    /// </summary>
    public class StaticData
    {
        /// <summary>
        /// Descriptions for each parse error.
        /// </summary>
        public static readonly string[] ParseErrorDescriptions = new string[]
        {
            "No Error",

            "Level expected but not found",
            "Level needs trailing delimeter",
            "Level is invalid",

            "Xref id needs trailing delimeter",
            "Xref too long",

            "Tag expected",
            "Tag needs trailing delimeter or newline",

            "Line value expected",
            "Line value needs trailing newline",
            "Line value invalid",

            "Unknown Error"
        };
    }
}
