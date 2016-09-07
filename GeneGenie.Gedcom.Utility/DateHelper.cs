// <copyright file="DateHelper.cs" company="GeneGenie.com">
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

namespace GeneGenui.Gedcom.Utility
{
    using System;

    public class DateHelper
    {
        private DateHelper()
        {
        }

        // returns a float rather than an int to allow for some fuzziness
        // e.g.  10-11-2000 is 10 NOV 2000 or 11 OCT 2000
        public static float MatchDateTimes(DateTime? dateTimeA, DateTime? dateTimeB)
        {
            float matches = 0;

            if ((dateTimeA == null && dateTimeB == null) ||
                ((!dateTimeA.HasValue) && (!dateTimeB.HasValue)))
            {
                matches += 3;
            }
            else if (dateTimeA != null && dateTimeA.HasValue && dateTimeB != null && dateTimeB.HasValue)
            {
                DateTime a = dateTimeA.Value;
                DateTime b = dateTimeB.Value;

                if (a.Year == b.Year)
                {
                    matches++;
                }
                else
                {
                    // TODO: arbitrary delta
                    const int delta = 5;

                    if (a.Year >= b.Year - delta && a.Year <= b.Year + delta)
                    {
                        matches += 0.25F;
                    }
                    else if (b.Year >= a.Year - delta && b.Year <= a.Year + delta)
                    {
                        matches += 0.25F;
                    }
                }

                if (a.Month == b.Month)
                {
                    matches++;
                }
                else
                {
                    // TODO: what delta should we check for?
                }

                if (a.Day == b.Day)
                {
                    matches++;
                }
                else
                {
                    // TODO: what delta should we check for?
                }

                // date formats may differ
                if (a.Day == b.Month && a.Month == b.Day && a.Month != b.Month && a.Day != b.Day)
                {
                    // day + month should be +2, however
                    // as this is a fudge to handle diff formats don't
                    // give full weighting
                    matches += 0.75F;
                }
            }

            return matches;
        }
    }
}
