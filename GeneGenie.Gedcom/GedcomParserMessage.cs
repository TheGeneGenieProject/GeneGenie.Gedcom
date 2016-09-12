// <copyright file="GedcomParserMessage.cs" company="GeneGenie.com">
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
    using System.Collections.Generic;
    using GeneGenie.Gedcom.Enums;

    public class GedcomParserMessage
    {
        public GedcomParserMessage(ParserMessageIds messageId, object[] additional)
        {
            MessageId = messageId;
            AdditionalData.AddRange(additional);
        }

        public List<object> AdditionalData { get; } = new List<object>();

        public ParserMessageIds MessageId { get; set; }

        public ParserMessageIdSeverity Severity
        {
            get
            {
                if ((int)MessageId <= 100)
                {
                    return ParserMessageIdSeverity.Information;
                }

                if ((int)MessageId <= 200)
                {
                    return ParserMessageIdSeverity.Warning;
                }

                if ((int)MessageId <= 300)
                {
                    return ParserMessageIdSeverity.Error;
                }

                return ParserMessageIdSeverity.Unknown;
            }
        }
    }
}
