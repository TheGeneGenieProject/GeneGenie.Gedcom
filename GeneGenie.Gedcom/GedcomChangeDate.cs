// <copyright file="GedcomChangeDate.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2008 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    /// <summary>
    /// The date on which a GEDCOM record was changed.
    /// </summary>
    public class GedcomChangeDate : GedcomDate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomChangeDate"/> class.
        /// </summary>
        /// <param name="database">The GEDCOM database to associate this date with.</param>
        public GedcomChangeDate(GedcomDatabase database)
            : base(database)
        {
        }

        /// <inheritdoc/>
        protected override void Changed()
        {
        }
    }
}
