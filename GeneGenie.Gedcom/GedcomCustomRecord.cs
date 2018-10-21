// <copyright file="GedcomCustomRecord.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
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
