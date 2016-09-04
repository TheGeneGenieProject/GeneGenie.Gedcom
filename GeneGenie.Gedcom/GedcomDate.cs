// <copyright file="GedcomDate.cs" company="GeneGenie.com">
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
    using System.Globalization;
    using System.IO;
    using Enums;

    /// <summary>
    /// Defines a date, allowing partial dates, date ranges etc.
    /// </summary>
    public class GedcomDate : GedcomRecord
    {
        private static readonly string[] ShortMonths = new string[]
        {
            "JAN", "FEB", "MAR", "APR", "MAY",
            "JUN", "JUL", "AUG", "SEP", "OCT",
            "NOV", "DEC"
        };

        private static readonly string[] ShortMonthsPunc = new string[] // non standard
        {
            "JAN.", "FEB.", "MAR.", "APR.", "MAY.",
            "JUN.", "JUL.", "AUG.", "SEP.", "OCT.",
            "NOV.", "DEC."
        };

        // non standard
        private static readonly string[] ShortMonthsExt = new string[]
        {
            "JAN", "FEB", "MAR", "APR", "MAY",
            "JUN", "JUL", "AUG", "SEPT", "OCT",
            "NOV", "DEC"
        };

        // non standard
        private static readonly string[] ShortMonthsExtPunc = new string[]
        {
            "JAN.", "FEB.", "MAR.", "APR.", "MAY.",
            "JUN.", "JUL.", "AUG.", "SEPT.", "OCT.",
            "NOV.", "DEC."
        };

        private static readonly string[] LongMonths = new string[]
        {
            "JANUARY", "FEBRUARY", "MARCH", "APRIL", "MAY",
            "JUNE", "JULY", "AUGUST", "SEPTEMBER", "OCTOBER",
            "NOVEMBER", "DECEMBER"
        };

        private static readonly string[] ShortFrenMonths = new string[]
        {
            "VEND", "BRUM", "FRIM", "NIVO", "PLUB",
            "VENT", "GERM", "FLOR", "PRAI", "MESS",
            "THER", "FRUC", "COMP"
        };

        // non standard
        private static readonly string[] ShortFrenMonthsPunc = new string[]
        {
            "VEND.", "BRUM.", "FRIM.", "NIVO.", "PLUB.",
            "VENT.", "GERM.", "FLOR.", "PRAI.", "MESS.",
            "THER.", "FRUC.", "COMP."
        };

        private static readonly string[] LongFrenMonths = new string[]
        {
            "VENDEMIAIRE", "BRUMAIRE", "FRIMAIRE", "NIVOSE", "PLUVIOSE",
            "VENTOSE", "GERMINAL", "FLOREAL", "PRAIRIAL", "MESSIDOR",
            "THERMIDOR", "FRUCTIDOR", "JOUR_COMPLEMENTAIRS"
        };

        private static readonly string[] ShortHebrMonths = new string[]
        {
            "TSH", "CSH", "KSL", "TVT", "SHV",
            "ADR", "ADS", "NSN", "IYR", "SVN",
            "TMZ", "AAV", "ELL"
        };

        // non standard
        private static readonly string[] ShortHebrMonthsPunc = new string[]
        {
            "TSH.", "CSH.", "KSL.", "TVT.", "SHV.",
            "ADR.", "ADS.", "NSN.", "IYR.", "SVN.",
            "TMZ.", "AAV.", "ELL."
        };

        private static readonly string[] LongHebrMonths = new string[]
        {
            "TISHRI", "CHESHCAN", "KISLEV", "TEVAT", "SHEVAT",
            "ADAR", "ADAR SHENI", "NISAN", "IYAR", "SIVAN",
            "TAMMUZ", "AV", "ELUL"
        };

        private static readonly string[][] MonthNames = new string[][]
        {
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
            LongHebrMonths
        };

        private GedcomDateType dateType;
        private GedcomDatePeriod datePeriod;

        private string period = null;
        private string time;
        private string date1;
        private int partsParsed1;
        private string date2;
        private int partsParsed2;

        private DateTime? dateTime1;
        private DateTime? dateTime2;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomDate"/> class.
        /// </summary>
        public GedcomDate()
        {
            date1 = string.Empty;
            date2 = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomDate"/> class.
        /// </summary>
        /// <param name="database">The database to associate the date with.</param>
        public GedcomDate(GedcomDatabase database)
            : this()
        {
            this.Database = database;
        }

        /// <inheritdoc/>
        public override GedcomRecordType RecordType
        {
            get { return GedcomRecordType.Date; }
        }

        /// <inheritdoc/>
        public override string GedcomTag
        {
            get { return "DATE"; }
        }

        /// <summary>
        /// Gets or sets
        /// </summary>
        /// TODO: Doc
        public GedcomDateType DateType
        {
            get
            {
                return dateType;
            }

            set
            {
                if (value != dateType)
                {
                    dateType = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets
        /// </summary>
        /// TODO: Doc
        public GedcomDatePeriod DatePeriod
        {
            get
            {
                return datePeriod;
            }

            set
            {
                if (value != datePeriod)
                {
                    datePeriod = value;
                    period = null;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets
        /// </summary>
        /// TODO: Doc
        public string Time
        {
            get
            {
                return time;
            }

            set
            {
                if (value != time)
                {
                    time = value;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets or sets
        /// </summary>
        /// TODO: Doc
        public string Date1
        {
            get
            {
                return date1;
            }

            set
            {
                if (value != date1)
                {
                    date1 = value;
                    period = null;
                    Changed();
                }
            }
        }

        /// <summary>
        ///
        /// Gets </summary>
        /// TODO: Doc
        public DateTime? DateTime1
        {
            get { return dateTime1; }
        }

        /// <summary>
        ///
        /// Gets or sets </summary>
        /// TODO: Doc
        public string Date2
        {
            get
            {
                return date2;
            }

            set
            {
                if (value != date2)
                {
                    date2 = value;
                    period = null;
                    Changed();
                }
            }
        }

        /// <summary>
        /// Gets
        /// </summary>
        /// TODO: Doc
        public DateTime? DateTime2
        {
            get { return dateTime2; }
        }

        // Util properties to get the date as a string

        /// <summary>
        /// Gets
        /// </summary>
        /// TODO: Doc
        /// TODO: cache this value, clear cache when _DatePeriod / _Date1 / _Date2 / _Time change
        public string DateString
        {
            get
            {
                string ret;

                if (!string.IsNullOrEmpty(Time))
                {
                    ret = Period + " " + Time;
                }
                else
                {
                    ret = Period;
                }

                return ret;
            }
        }

        /// <summary>
        ///
        /// Gets </summary>
        public string Period
        {
            get
            {
                if (period == null)
                {
                    switch (datePeriod)
                    {
                        case GedcomDatePeriod.Exact:
                            period = date1;
                            break;
                        case GedcomDatePeriod.After:
                            period = string.Format("AFT {0}", date1);
                            break;
                        case GedcomDatePeriod.Before:
                            period = string.Format("BEF {0}", date1);
                            break;
                        case GedcomDatePeriod.Between:
                            // TODO: this is a hack as we don't parse _Date2 in
                            // properly yet and just end up with it all in _Date1
                            if (string.IsNullOrEmpty(date2))
                            {
                                period = string.Format("BET {0}", date1);
                            }
                            else
                            {
                                period = string.Format("BET {0} AND {1}", date1, date2);
                            }

                            break;
                        case GedcomDatePeriod.About:
                            period = string.Format("ABT {0}", date1);
                            break;
                        case GedcomDatePeriod.Calculated:
                            period = string.Format("CAL {0}", date1);
                            break;
                        case GedcomDatePeriod.Estimate:
                            period = string.Format("EST {0}", date1);
                            break;
                        case GedcomDatePeriod.Interpretation:
                            period = string.Format("INT {0}", date1);
                            break;
                        case GedcomDatePeriod.Range:
                            // TODO: this is a hack as we don't parse _Date2 in
                            // properly yet and just end up with it all in _Date1
                            if (string.IsNullOrEmpty(date2))
                            {
                                period = string.Format("FROM {0}", date1);
                            }
                            else
                            {
                                period = string.Format("FROM {0} TO {1}", date1, date2);
                            }

                            break;
                    }
                }

                return period;
            }
        }

        /// <summary>
        /// Compare two dates to see if first is greater than the second.
        /// </summary>
        /// <param name="a">First date to compare.</param>
        /// <param name="b">Second date to compare.</param>
        /// <returns>bool</returns>
        public static bool operator <(GedcomDate a, GedcomDate b)
        {
            int ret = CompareByDate(a, b);

            return ret < 0;
        }

        /// <summary>
        /// Compare two dates to see if first is kess than the second.
        /// </summary>
        /// <param name="a">First date to compare.</param>
        /// <param name="b">Second date to compare.</param>
        /// <returns>bool</returns>
        public static bool operator >(GedcomDate a, GedcomDate b)
        {
            int ret = CompareByDate(a, b);

            return ret > 0;
        }

        /// <summary>
        /// Compare two dates to see if they are not equal.
        /// </summary>
        /// <param name="a">First date to compare.</param>
        /// <param name="b">Second date to compare.</param>
        /// <returns>bool</returns>
        public static bool operator !=(GedcomDate a, GedcomDate b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Compare two dates to see if they are equal.
        /// </summary>
        /// <param name="a">First date to compare.</param>
        /// <param name="b">Second date to compare.</param>
        /// <returns>bool</returns>
        public static bool operator ==(GedcomDate a, GedcomDate b)
        {
            bool ret = false;

            bool anull = Equals(a, null);
            bool bnull = Equals(b, null);

            if (!anull && !bnull)
            {
                ret = CompareByDate(a, b) == 0;
            }
            else if (anull && bnull)
            {
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// Compare two GEDCOM format dates.
        /// </summary>
        /// <param name="datea">First date to compare.</param>
        /// <param name="dateb">Second date to compare.</param>
        /// <returns>0 if equal, -1 if datea less than dateb, else 1.</returns>
        public static int CompareByDate(GedcomDate datea, GedcomDate dateb)
        {
            int ret = CompareNullableDateTime(datea.DateTime1, dateb.DateTime1);

            if (ret == 0)
            {
                ret = CompareNullableDateTime(datea.DateTime2, dateb.DateTime2);

                if (ret == 0)
                {
                    ret = string.Compare(datea.Date1, dateb.Date1, true);
                    if (ret == 0)
                    {
                        ret = string.Compare(datea.Date2, dateb.Date2, true);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this == (GedcomDate)obj;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            // Overflow is fine, just wrap.
            unchecked
            {
                int hash = 17;

                hash *= 23 + DateTime1.GetHashCode();
                hash *= 23 + DateTime2.GetHashCode();
                hash *= 23 + Date1.GetHashCode();
                hash *= 23 + Date2.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// TODO: Doc
        /// </summary>
        /// <param name="date">TODO: Doc 2</param>
        /// <returns>TODO: Doc 3</returns>
        public float IsMatch(GedcomDate date)
        {
            float match = 0F;

            if (Date1 == date.Date1 && Date2 == date.Date2 && DatePeriod == date.DatePeriod)
            {
                match = 100.0F;
            }
            else
            {
                // compare date components in DateTime if present
                float matches = 0;
                int parts = 0;

                DateTime? dateDate1 = date.DateTime1;
                DateTime? dateDate2 = date.DateTime2;

                // same type, nice and simple,
                // range is the same as between as far as we are concerned
                // for instance an Occupation could have been FROM a TO B
                // or BETWEEN a AND b
                // logic doesn't hold for one off events such a birth, but
                // then a Range doesn't make sense for those anyway so if
                // we have one should be safe to assume it is Between
                if (DateType == date.DateType &&
                    (DatePeriod == date.DatePeriod ||
                    (DatePeriod == GedcomDatePeriod.Range && date.DatePeriod == GedcomDatePeriod.Between) ||
                    (DatePeriod == GedcomDatePeriod.Between && date.DatePeriod == GedcomDatePeriod.Range)))
                {
                    matches++;

                    // checked 1 value
                    parts++;

                    parts += 3;
                    float date1Match = Utility.DateHelper.MatchDateTimes(DateTime1, dateDate1);

                    // correct for number of date parts parsed
                    date1Match *= partsParsed1 / 3.0F;

                    matches += date1Match;

                    parts += 3;
                    float date2Match = Utility.DateHelper.MatchDateTimes(DateTime2, dateDate2);

                    // correct for number of date parts parsed
                    date2Match *= partsParsed2 / 3.0F;

                    matches += date2Match;

                    match = (matches / parts) * 100.0F;
                }
            }

            return match;
        }

        /// <summary>
        /// Parse passed date into instance properties.
        /// </summary>
        /// <param name="dataString">The date to parse as a text string.</param>
        public void ParseDateString(string dataString)
        {
            // clear possible Period cached value;
            this.period = null;

            string dateType = string.Empty;

            if (dataString.StartsWith("@#"))
            {
                dataString = dataString.Substring(2);
                int i = dataString.IndexOf("@", 2);
                if (i != -1)
                {
                    dateType = dataString.Substring(0, i).ToUpper();
                    dataString = dataString.Substring(i + 1);
                }
            }

            switch (dateType)
            {
                case "@#DGREGORIAN@":
                    DateType = GedcomDateType.Gregorian;
                    break;
                case "@#DJULIAN@":
                    DateType = GedcomDateType.Julian;
                    break;
                case "@#DHEBREW@":
                    DateType = GedcomDateType.Hebrew;
                    break;
                case "@#DROMAN@":
                    DateType = GedcomDateType.Roman;
                    break;
                case "@#DUNKNOWN@":
                    DateType = GedcomDateType.Unknown;
                    break;
                default:
                    DateType = GedcomDateType.Gregorian;
                    break;
            }

            // try to determine date period, let the fun begin
            string period = dataString;
            int len = 0;

            CultureInfo culture = System.Globalization.CultureInfo.CurrentCulture;

            if (period.StartsWith("BEF ", true, culture))
            {
                len = "BEF ".Length;
                DatePeriod = GedcomDatePeriod.Before;
            }
            else if (period.StartsWith("AFT ", true, culture))
            {
                len = "AFT ".Length;
                DatePeriod = GedcomDatePeriod.After;
            }
            else if (period.StartsWith("BET ", true, culture))
            {
                len = "BET ".Length;
                DatePeriod = GedcomDatePeriod.Between;
            }
            else if (period.StartsWith("FROM ", true, culture))
            {
                len = "FROM ".Length;
                DatePeriod = GedcomDatePeriod.Range;
            }
            else if (period.StartsWith("TO ", true, culture))
            {
                len = "TO ".Length;
                DatePeriod = GedcomDatePeriod.Range;
            }
            else if (period.StartsWith("ABT ", true, culture) ||
                        period.StartsWith("EST ", true, culture))
            {
                len = "EST ".Length;
                DatePeriod = GedcomDatePeriod.Estimate;
            }
            else if (period.StartsWith("CAL ", true, culture))
            {
                len = "CAL ".Length;
                DatePeriod = GedcomDatePeriod.Calculated;
            }
            else if (period.StartsWith("INT ", true, culture))
            {
                len = "INT ".Length;
                DatePeriod = GedcomDatePeriod.Interpretation;
            }

            // all the rest here are not valid gedcom, but have been seen
            // in gedcom files
            else if (period.StartsWith("BEF.", true, culture))
            {
                len = "BEF.".Length;
                DatePeriod = GedcomDatePeriod.Before;
            }
            else if (period.StartsWith("AFT.", true, culture))
            {
                len = "AFT.".Length;
                DatePeriod = GedcomDatePeriod.After;
            }
            else if (period.StartsWith("BET.", true, culture))
            {
                len = "BET.".Length;
                DatePeriod = GedcomDatePeriod.Between;
            }
            else if (period.StartsWith("ABT.", true, culture) ||
                        period.StartsWith("EST.", true, culture))
            {
                len = "EST.".Length;
                DatePeriod = GedcomDatePeriod.Estimate;
            }
            else if (period.StartsWith("CAL. ", true, culture))
            {
                len = "CAL.".Length;
                DatePeriod = GedcomDatePeriod.Calculated;
            }
            else if (period.StartsWith("INT. ", true, culture))
            {
                len = "INT.".Length;
                DatePeriod = GedcomDatePeriod.Interpretation;
            }

            // C or CIRCA isn't valid either
            // See BROSKEEP comment below, C may be due to the date
            // being set from a baptism / christening, but if that is the case
            // estimate is still reasonable to go with
            else if (period.StartsWith("C.", true, culture))
            {
                len = "C.".Length;
                DatePeriod = GedcomDatePeriod.Estimate;
            }
            else if (period.StartsWith("CIRCA ", true, culture))
            {
                len = "CIRCA ".Length;
                DatePeriod = GedcomDatePeriod.Estimate;
            }

            // BROSKEEP seems to be stupid and doesn't make proper
            // use of CAL    e.g   BU.9-6-1825  for a death date
            // means it is really the burial date that has just been
            // copied to the death date
            else if (period.StartsWith("BU.", true, culture))
            {
                len = "BU.".Length;
                DatePeriod = GedcomDatePeriod.Calculated;
            }

            // same with birth / baptism, as it does this is C safe being CIRCA ?
            else if (period.StartsWith("BAP.", true, culture))
            {
                len = "BAP.".Length;
                DatePeriod = GedcomDatePeriod.Calculated;
            }

            // Yet another non standard prefix, seen in BROSKEEP
            else if (period.StartsWith("ABOUT ", true, culture))
            {
                len = "ABOUT ".Length;
                DatePeriod = GedcomDatePeriod.Estimate;
            }

            // and another
            else if (period.StartsWith("BEFORE ", true, culture))
            {
                len = "BEFORE ".Length;
                DatePeriod = GedcomDatePeriod.Before;
            }

            // not seen but expected
            else if (period.StartsWith("AFTER ", true, culture))
            {
                len = "AFTER ".Length;
                DatePeriod = GedcomDatePeriod.After;
            }

            // we may also have NOT BEFORE, NOT BEF. NOT BEF
            // NOT AFTER, NOT AFT. NOT AFT  etc.
            else if (period.StartsWith("NOT BEF ", true, culture))
            {
                len = "NOT BEF ".Length;
                DatePeriod = GedcomDatePeriod.After;
            }
            else if (period.StartsWith("NOT BEF.", true, culture))
            {
                len = "NOT BEF.".Length;
                DatePeriod = GedcomDatePeriod.After;
            }
            else if (period.StartsWith("NOT BEFORE ", true, culture))
            {
                len = "NOT BEFORE ".Length;
                DatePeriod = GedcomDatePeriod.After;
            }
            else if (period.StartsWith("NOT AFT ", true, culture))
            {
                len = "NOT AFT ".Length;
                DatePeriod = GedcomDatePeriod.Before;
            }
            else if (period.StartsWith("NOT AFT.", true, culture))
            {
                len = "NOT AFT.".Length;
                DatePeriod = GedcomDatePeriod.Before;
            }
            else if (period.StartsWith("NOT AFTER ", true, culture))
            {
                len = "NOT AFTER ".Length;
                DatePeriod = GedcomDatePeriod.Before;
            }

            dataString = dataString.Substring(len).TrimStart(new char[] { ' ', '\t' });

            Calendar calendar = null;

            switch (DateType)
            {
                case GedcomDateType.French:
                    // TODO: no FrenCalendar!
                    Date1 = dataString;
                    throw new NotImplementedException();
                    break;
                case GedcomDateType.Gregorian:
                    calendar = new GregorianCalendar();
                    Date1 = dataString;
                    break;
                case GedcomDateType.Hebrew:
                    calendar = new HebrewCalendar();
                    Date1 = dataString;
                    break;
                case GedcomDateType.Julian:
                    calendar = new JulianCalendar();
                    Date1 = dataString;
                    break;
                case GedcomDateType.Roman:
                    // TODO: no RomanCalendar!
                    Date1 = dataString;
                    throw new NotImplementedException();
                    break;
            }

            // TODO: the split here accounts for large(ish) amounts of memory allocation
            // Need to do this better, ideally without any splitting.
            string[] dateSplit = dataString.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);

            dateTime1 = null;
            dateTime2 = null;

            partsParsed1 = 0;
            partsParsed2 = 0;

            if (dateSplit.Length == 1)
            {
                partsParsed1 = 1;
                partsParsed2 = 0;
                dateTime1 = GetDateInfo(dateSplit, 0, 1, calendar);
            }
            else if (dateSplit.Length == 2)
            {
                partsParsed1 = 2;
                partsParsed2 = 0;
                dateTime1 = GetDateInfo(dateSplit, 0, 2, calendar);
            }
            else if (dateSplit.Length == 3)
            {
                // day month year  or year (AND/TO) year
                if (string.Compare(dateSplit[1], "AND", true) != 0 &&
                       string.Compare(dateSplit[1], "TO", true) != 0)
                {
                    partsParsed1 = 1;
                    partsParsed2 = 0;
                    dateTime1 = GetDateInfo(dateSplit, 0, 3, calendar);
                }
                else
                {
                    partsParsed1 = 1;
                    partsParsed2 = 1;
                    dateTime1 = GetDateInfo(dateSplit, 0, 1, calendar);

                    dateTime2 = GetDateInfo(dateSplit, 2, 1, calendar);
                }
            }
            else if (dateSplit.Length > 4)
            {
                // AND ?  TO ?
                if (DatePeriod == GedcomDatePeriod.Between ||
                       DatePeriod == GedcomDatePeriod.Range)
                {
                    // where is the AND / TO ?
                    if (string.Compare(dateSplit[1], "AND", true) == 0 ||
                           string.Compare(dateSplit[1], "TO", true) == 0)
                    {
                        partsParsed1 = 1;
                        partsParsed2 = 3;
                        dateTime1 = GetDateInfo(dateSplit, 0, 1, calendar);
                        dateTime2 = GetDateInfo(dateSplit, 2, 3, calendar);
                    }
                    else if (string.Compare(dateSplit[2], "AND", true) == 0 ||
                             string.Compare(dateSplit[2], "TO", true) == 0)
                    {
                        partsParsed1 = 2;
                        partsParsed2 = 3;
                        dateTime1 = GetDateInfo(dateSplit, 0, 2, calendar);
                        dateTime2 = GetDateInfo(dateSplit, 3, 3, calendar);
                    }
                    else
                    {
                        // lets assume dateSplit[3] is AND / TO
                        partsParsed1 = 3;
                        partsParsed2 = 3;
                        dateTime1 = GetDateInfo(dateSplit, 0, 3, calendar);
                        dateTime2 = GetDateInfo(dateSplit, 4, 3, calendar);
                    }
                }
                else
                {
                    // assume date is generic text, can't do much with it
                    // TODO: We should feed back to the user here.
                }
            }

            if ((DatePeriod == GedcomDatePeriod.Exact &&
                dateTime1 == null) || !dateTime1.HasValue)
            {
                // unable to parse, let's try some more methods
                // as these dates are used for analysis it doesn't matter
                // too much if the format is wrong, e.g. we read 12-11-1994
                // as 12 NOV 1994 when it is meant as 11 DEC 1994, we don't
                // throw away the original data at all or use these dates
                // when writing the data back out
                // TODO: format provider instead of null?
                DateTime date;
                if (DateTime.TryParseExact(dataString, new string[] { "d-M-yyyy", "M-d-yyyy", "yyyy-M-d", "d.M.yyyy", "M.d.yyyy", "yyyy.M.d" }, null, DateTimeStyles.None, out date))
                {
                    dateTime1 = date;
                    partsParsed1 = 3;
                    partsParsed2 = 0;
                }

                // other values seen,  UNKNOWN, PRIVATE, DECEASED, DEAD
                // These have probably been entered so the system
                // it was entered on doesn't remove the event due
                // to lack of any date entered.
                // accept as unparsable
            }
        }

        /// <summary>
        /// Output GEDCOM format for this instance.
        /// </summary>
        /// <param name="tw">Where to output the data to.</param>
        public override void Output(TextWriter tw)
        {
            tw.Write(Environment.NewLine);
            tw.Write(Util.IntToString(Level));
            tw.Write(" DATE ");

            // only output type if it isn't the default (Gregorian)
            if (dateType != GedcomDateType.Gregorian)
            {
                tw.Write("@#D{0}@ ", dateType.ToString());
            }

            string line = Period.Replace("@", "@@");
            tw.Write(line);

            if (!string.IsNullOrEmpty(Time))
            {
                line = Time.Replace("@", "@@");

                tw.Write(Environment.NewLine);
                tw.Write("{0} TIME {1}", Util.IntToString(Level + 1), line);
            }

            OutputStandard(tw);
        }

        /// <summary>
        /// Compare two possibly null dates.
        /// </summary>
        /// <param name="dateaDate">First date to compare.</param>
        /// <param name="datebDate">Second date to compare.</param>
        /// <returns>0 if dates match, -1 if dateaDate is less than datebDate, otherwise 1.</returns>
        private static int CompareNullableDateTime(DateTime? dateaDate, DateTime? datebDate)
        {
            if (dateaDate == null && datebDate == null)
            {
                return 0;
            }

            if (dateaDate.HasValue && datebDate.HasValue)
            {
                return DateTime.Compare(dateaDate.Value, datebDate.Value);
            }

            if (!dateaDate.HasValue)
            {
                return -1;
            }

            return 1;
        }

        private static DateTime? GetDateInfo(string[] dateSplit, int start, int num, Calendar calendar)
        {
            string year = string.Empty;
            string month = string.Empty;
            string day = string.Empty;
            //// bool bc = false;

            DateTime? ret = null;

            CultureInfo culture = System.Globalization.CultureInfo.CurrentCulture;

            // only parse if we have the expected number of date parts
            if (!(start != 0 && num == 3 && (dateSplit.Length < start + num)))
            {
                if (num == 1)
                {
                    // year only
                    year = dateSplit[start];
                    //// bc = false;
                    if (year.EndsWith("B.C.", true, culture))
                    {
                        //// bc = true;
                        year = year.Substring(0, year.Length - "B.C.".Length);
                    }
                }
                else if (num == 2)
                {
                    // month
                    month = dateSplit[start];

                    // year
                    year = dateSplit[start + 1];
                    //// bc = false;
                    if (year.EndsWith("B.C.", true, culture))
                    {
                        //// bc = true;
                        year = year.Substring(0, year.Length - "B.C.".Length);
                    }
                }
                else if (num == 3)
                {
                    // day
                    day = dateSplit[start];

                    // month
                    month = dateSplit[start + 1];

                    // year
                    year = dateSplit[start + 2];
                    //// bc = false;
                    if (year.EndsWith("B.C.", true, culture))
                    {
                        // bc = true;
                        year = year.Substring(0, year.Length - "B.C.".Length);
                    }
                }

                int y = 1;
                int m = 1;
                int d = 1;

                if ((!int.TryParse(month, out m)) && month != string.Empty)
                {
                    // month name, find month number
                    foreach (string[] names in MonthNames)
                    {
                        int i = 1;
                        bool match = false;
                        foreach (string monthName in names)
                        {
                            if (string.Compare(monthName, month, true) == 0)
                            {
                                match = true;
                                break;
                            }

                            i++;
                        }

                        if (match)
                        {
                            m = i;
                            break;
                        }
                    }
                }

                int.TryParse(day, out d);

                // year could be of the form 1980/81
                // have 2 datetimes for each date ?
                // only having 1 won't lose the data, could prevent proper merge
                // though as the DateTime will be used for comparison
                if (year.IndexOf('/') != -1)
                {
                    year = year.Substring(0, year.IndexOf('/'));
                }

                // if we have the month as > 12 then must be mm dd yyyy
                // and not dd mm yyyy
                if (m > 12)
                {
                    int tmp = d;
                    d = m;
                    m = tmp;
                }

                if (int.TryParse(year, out y))
                {
                    if (m == 0)
                    {
                        m = 1;
                    }

                    if (d == 0)
                    {
                        d = 1;
                    }

                    if (y == 0)
                    {
                        y = 1;
                    }

                    // ignore era, dates won't be bc, no way to get info back
                    // that far reliably so shouldn't be an issue

                    // try and correct for invalid dates, such as
                    // in presidents.ged with 29 FEB 1634/35
                    int daysInMonth = 0;

                    if (m > 0 && m <= 12 && y > 0 && y < 9999)
                    {
                        daysInMonth = calendar.GetDaysInMonth(y, m);
                        if (d > daysInMonth)
                        {
                            d = daysInMonth;
                        }
                    }

                    try
                    {
                        ret = new DateTime(y, m, d, calendar);
                    }
                    catch
                    {
                        // TODO: Don't swallow exceptions, need some way to feedback to user.
                        // if we fail to parse not much we can do,
                        // just don't provide a datetime
                    }
                }
            }

            return ret;
        }
    }
}
