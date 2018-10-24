// <copyright file="StaticDateData.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Data
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using GeneGenie.Gedcom.Enums;

    /// <summary>
    /// Month and period data used for parsing dates from GEDCOM files.
    /// </summary>
    public static class StaticDateData
    {
        /// <summary>
        /// All month names that might come up in GEDCOM date formats, abbreviated, non English as well.
        /// </summary>
        public static readonly ImmutableList<string[]> MonthNames;

        /// <summary>
        /// Delimiters that are valid for parsing the date parts of a GEDCOM date record.
        /// </summary>
        public static readonly char[] GedcomDateParseDelimiters = new char[]
        {
            ' ', '-',
        };

        /// <summary>
        /// A list of mappings of text prefixes and suffixes to their GEDCOM types.
        /// Not all of these are valid as many tags come from programmes that do not obey the standards.
        /// Longer strings that match the start of shorter strings should be listed first (ABT. before ABT).
        ///
        /// Of particular note;
        ///  C or CIRCA from BROSKEEP files, C may be due to the date  being set from a baptism / christening, but if that is the
        ///  case estimate is still reasonable to go with.
        ///
        ///  BROSKEEP seems to be stupid and doesn't make proper use of CAL e.g 'BU.9-6-1825' for a death date means it is really
        ///  the burial date that has just been copied to the death date.
        ///  Same with birth / baptism.
        /// </summary>
        internal static readonly List<GedcomDatePeriodParserMapping> PeriodMappings = new List<GedcomDatePeriodParserMapping>
        {
            { new GedcomDatePeriodParserMapping { Text = "ABOUT", MapsTo = GedcomDatePeriod.Estimate, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "ABT.", MapsTo = GedcomDatePeriod.Estimate, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "ABT", MapsTo = GedcomDatePeriod.Estimate, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "AFTER", MapsTo = GedcomDatePeriod.After, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "AFT.", MapsTo = GedcomDatePeriod.After, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "AFT", MapsTo = GedcomDatePeriod.After, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "BAP", MapsTo = GedcomDatePeriod.Calculated, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "BEF.", MapsTo = GedcomDatePeriod.Before, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "BEF", MapsTo = GedcomDatePeriod.Before, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "BEFORE", MapsTo = GedcomDatePeriod.Before, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "BET.", MapsTo = GedcomDatePeriod.Between, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "BET", MapsTo = GedcomDatePeriod.Between, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "BU.", MapsTo = GedcomDatePeriod.Calculated, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "C.", MapsTo = GedcomDatePeriod.Estimate, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "CAL.", MapsTo = GedcomDatePeriod.Calculated, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "CAL", MapsTo = GedcomDatePeriod.Calculated, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "CIRCA", MapsTo = GedcomDatePeriod.Estimate, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "EST.", MapsTo = GedcomDatePeriod.Estimate, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "EST", MapsTo = GedcomDatePeriod.Estimate, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "FROM.", MapsTo = GedcomDatePeriod.Range, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "FROM", MapsTo = GedcomDatePeriod.Range, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "INT.", MapsTo = GedcomDatePeriod.Interpretation, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "INT", MapsTo = GedcomDatePeriod.Interpretation, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "NOT AFTER", MapsTo = GedcomDatePeriod.Before, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "NOT AFT.", MapsTo = GedcomDatePeriod.Before, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "NOT AFT", MapsTo = GedcomDatePeriod.Before, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "NOT BEFORE", MapsTo = GedcomDatePeriod.After, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "NOT BEF.", MapsTo = GedcomDatePeriod.After, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "NOT BEF", MapsTo = GedcomDatePeriod.After, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "TO.", MapsTo = GedcomDatePeriod.Range, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "TO", MapsTo = GedcomDatePeriod.Range, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "?", MapsTo = GedcomDatePeriod.Estimate, TextPosition = GedcomDatePeriodPosition.Prefix } },
            { new GedcomDatePeriodParserMapping { Text = "?", MapsTo = GedcomDatePeriod.Estimate, TextPosition = GedcomDatePeriodPosition.Suffix } },
        };

        private static readonly string[] ShortMonths = new string[]
            {
            "JAN", "FEB", "MAR", "APR", "MAY",
            "JUN", "JUL", "AUG", "SEP", "OCT",
            "NOV", "DEC",
            };

        private static readonly string[] ShortMonthsPunc = new string[] // non standard
        {
            "JAN.", "FEB.", "MAR.", "APR.", "MAY.",
            "JUN.", "JUL.", "AUG.", "SEP.", "OCT.",
            "NOV.", "DEC.",
        };

        // non standard
        private static readonly string[] ShortMonthsExt = new string[]
        {
            "JAN", "FEB", "MAR", "APR", "MAY",
            "JUN", "JUL", "AUG", "SEPT", "OCT",
            "NOV", "DEC",
        };

        // non standard
        private static readonly string[] ShortMonthsExtPunc = new string[]
        {
            "JAN.", "FEB.", "MAR.", "APR.", "MAY.",
            "JUN.", "JUL.", "AUG.", "SEPT.", "OCT.",
            "NOV.", "DEC.",
        };

        private static readonly string[] LongMonths = new string[]
        {
            "JANUARY", "FEBRUARY", "MARCH", "APRIL", "MAY",
            "JUNE", "JULY", "AUGUST", "SEPTEMBER", "OCTOBER",
            "NOVEMBER", "DECEMBER",
        };

        private static readonly string[] ShortFrenMonths = new string[]
        {
            "VEND", "BRUM", "FRIM", "NIVO", "PLUB",
            "VENT", "GERM", "FLOR", "PRAI", "MESS",
            "THER", "FRUC", "COMP",
        };

        // non standard
        private static readonly string[] ShortFrenMonthsPunc = new string[]
        {
            "VEND.", "BRUM.", "FRIM.", "NIVO.", "PLUB.",
            "VENT.", "GERM.", "FLOR.", "PRAI.", "MESS.",
            "THER.", "FRUC.", "COMP.",
        };

        private static readonly string[] LongFrenMonths = new string[]
        {
            "VENDEMIAIRE", "BRUMAIRE", "FRIMAIRE", "NIVOSE", "PLUVIOSE",
            "VENTOSE", "GERMINAL", "FLOREAL", "PRAIRIAL", "MESSIDOR",
            "THERMIDOR", "FRUCTIDOR", "JOUR_COMPLEMENTAIRS",
        };

        private static readonly string[] ShortHebrMonths = new string[]
        {
            "TSH", "CSH", "KSL", "TVT", "SHV",
            "ADR", "ADS", "NSN", "IYR", "SVN",
            "TMZ", "AAV", "ELL",
        };

        // non standard
        private static readonly string[] ShortHebrMonthsPunc = new string[]
        {
            "TSH.", "CSH.", "KSL.", "TVT.", "SHV.",
            "ADR.", "ADS.", "NSN.", "IYR.", "SVN.",
            "TMZ.", "AAV.", "ELL.",
        };

        private static readonly string[] LongHebrMonths = new string[]
        {
            "TISHRI", "CHESHCAN", "KISLEV", "TEVAT", "SHEVAT",
            "ADAR", "ADAR SHENI", "NISAN", "IYAR", "SIVAN",
            "TAMMUZ", "AV", "ELUL",
        };

        static StaticDateData()
        {
            MonthNames = ImmutableList.Create(
                ShortMonths,
                ShortMonthsPunc,
                ShortMonthsExt,
                ShortMonthsExtPunc,
                LongMonths,
                ShortFrenMonths,
                ShortFrenMonthsPunc,
                LongFrenMonths,
                ShortHebrMonths,
                ShortHebrMonthsPunc,
                LongHebrMonths);
        }
    }
}
