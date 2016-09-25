// <copyright file="GedcomAssociation.cs" company="GeneGenie.com">
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
    using System;
    using System.IO;
    using Enums;

    /// <summary>
    /// How the given individual is associated to another.
    /// Each GedcomIndividal contains a list of these
    /// </summary>
    public class GedcomAssociation : GedcomRecord
    {
        private string description;

        private string individual;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomAssociation"/> class.
        /// </summary>
        public GedcomAssociation()
        {
        }

        /// <summary>
        /// Gets
        /// </summary>
        /// TODO: Docs
        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Association; }
        }

        /// <summary>
        /// Gets
        /// </summary>
        /// TODO: Docs
        public override string GedcomTag
        {
            get { return "ASSO"; }
        }

        /// <summary>
        /// Gets or sets
        /// </summary>
        /// TODO: Docs
        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                if (value != description)
                {
                    description = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets
        /// </summary>
        /// TODO: Docs
        public string Individual
        {
            get
            {
                return individual;
            }

            set
            {
                if (value != individual)
                {
                    individual = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Outputs a GEDCOM format version of this instance.
        /// </summary>
        /// <param name="tw">Where to output the text to.</param>
        public override void Output(TextWriter tw)
        {
            tw.Write(Environment.NewLine);
            tw.Write(Util.IntToString(Level));
            tw.Write(" ASSO ");
            tw.Write("@");
            tw.Write(Individual);
            tw.Write("@");

            string levelPlusOne = Util.IntToString(Level + 1);

            tw.Write(Environment.NewLine);
            tw.Write(levelPlusOne);
            tw.Write(" RELA ");

            string line = Description.Replace("@", "@@");
            if (line.Length > 25)
            {
                Util.SplitText(tw, line, Level + 1, 25, 1, true);
            }
            else
            {
                tw.Write(line);
            }

            OutputStandard(tw);
        }

        public override bool IsSimilar(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
