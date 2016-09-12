// <copyright file="ParserMessageIdSeverity.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Enums
{
    /// <summary>
    /// Severity of the messages logged during import.
    /// </summary>
    public enum ParserMessageIdSeverity
    {
        /// <summary>So that we don't accidentally default to the first id if not initialised, we use unknown.</summary>
        Unknown = 0,

        /// <summary>The user can view these, but they should not need to.</summary>
        Information = 1,

        /// <summary>The user needs to know something and / or make a decision.</summary>
        Warning = 2,

        /// <summary>We couldn't automatically import something, the user needs to intervene.</summary>
        Error = 3,
    }
}
