// <copyright file="ParserMessageIdSeverity.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
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
