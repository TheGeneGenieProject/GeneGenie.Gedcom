// <copyright file="GedcomCustomRecord.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom
{
    using System.IO;
    using GeneGenie.Gedcom.Enums;

    /// <summary>
    /// GEDCOM allows for custom tags to be added by applications.
    /// This is essentially a dummy object.
    /// </summary>
    public class GedcomCustomRecord : GedcomEvent
    {
        private const string DefaultTagName = "_CUST";

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomCustomRecord"/> class.
        /// </summary>
        public GedcomCustomRecord()
        {
            EventType = GedcomEventType.Custom;
        }

        /// <inheritdoc/>
        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.CustomRecord; }
        }

        /// <inheritdoc/>
        public override string GedcomTag
        {
            get { return Tag; }
        }

        /// <summary>
        /// Gets or sets the tag associated with this custom record.
        /// </summary>
        public string Tag { get; set; } = DefaultTagName;

        /// <summary>
        /// Placeholder for GEDCOM output code, does not actually output any data.
        /// </summary>
        /// <param name="tw">The writer to output to.</param>
        public override void Output(TextWriter tw)
        {
        }
    }
}
