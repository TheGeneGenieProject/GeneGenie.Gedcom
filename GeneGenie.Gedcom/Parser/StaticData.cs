// <copyright file="StaticData.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
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
        // TODO: These should be in the same place as the codes they map to otherwise they'll get out of sync. Change to Dictionary and merge.
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

            "Unknown Error",
        };
    }
}
