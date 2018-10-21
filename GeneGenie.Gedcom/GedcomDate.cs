// <copyright file="GedcomDate.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom
{
    using System;
    using System.Globalization;
    using System.IO;
    using GeneGenie.Gedcom.Data;
    using GeneGenie.Gedcom.Enums;

    /// <summary>
    /// Defines a date, allowing partial dates, date ranges etc.
    /// </summary>
    public class GedcomDate : GedcomRecord, IComparable, IComparable<GedcomDate>, IEquatable<GedcomDate>
    {
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
        /// Gets or sets the calendar type.
        /// </summary>
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
        /// Gets or sets the date period.
        /// </summary>
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
        /// Gets or sets the time.
        /// </summary>
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
        /// Gets or sets the date, or the first date in a date range.
        /// </summary>
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
        /// Gets the DateTime, or the first DateTime in a date range.
        /// </summary>
        public DateTime? DateTime1
        {
            get { return dateTime1; }
        }

        /// <summary>
        /// Gets or sets the second date in a date range.
        /// </summary>
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
        /// Gets the second DateTime in a date range.
        /// </summary>
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
        /// Gets a text representation of the date period and date.
        /// TODO: Seems wrongly named, returns original value as well as period.
        /// </summary>
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
        /// Compare two dates to see if first is less than the second.
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
            bool anull = Equals(datea, null);
            bool bnull = Equals(dateb, null);

            if (anull && bnull)
            {
                return 0;
            }
            else if (anull)
            {
                return -1;
            }
            else if (bnull)
            {
                return 1;
            }

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
        /// Compare the user entered data against the passed instance for similarity.
        /// </summary>
        /// <param name="obj">The object to compare this instance against.</param>
        /// <returns>
        /// True if instance matches user data, otherwise false.
        /// </returns>
        public override bool IsEquivalentTo(object obj)
        {
            return CompareTo(obj as GedcomDate) == 0;
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

        /// <summary>
        /// Compares the current and passed-in object to see if they are the same.
        /// </summary>
        /// <param name="obj">The object to compare the current instance against.</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the value parameter.</returns>
        public int CompareTo(object obj)
        {
            return CompareTo(obj as GedcomDate);
        }

        /// <summary>
        /// Compares the current and passed-in date to see if they are the same.
        /// </summary>
        /// <param name="otherDate">The date to compare the current instance against.</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the value parameter.</returns>
        public int CompareTo(GedcomDate otherDate)
        {
            return CompareByDate(this, otherDate);
        }

        /// <summary>
        /// Compares the current and passed-in date to see if they are the same.
        /// </summary>
        /// <param name="otherDate">The date to compare the current instance against.</param>
        /// <returns>True if they match, False otherwise.</returns>
        public bool Equals(GedcomDate otherDate)
        {
            return this == otherDate;
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
        /// Returns a percentage based score on how similar the passed record is to the current instance.
        /// </summary>
        /// <param name="date">The date to compare this instance against.</param>
        /// <returns>A score from 0 to 100 representing the percentage match.</returns>
        public decimal CalculateSimilarityScore(GedcomDate date)
        {
            var match = decimal.Zero;

            if (Date1 == date.Date1 && Date2 == date.Date2 && DatePeriod == date.DatePeriod)
            {
                return 100m;
            }

            // compare date components in DateTime if present
            var matches = decimal.Zero;
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

                parts += partsParsed1;
                var date1Match = MatchDateTimes(DateTime1, date.DateTime1);

                // correct for number of date parts parsed
                date1Match *= partsParsed1 / 3m;

                matches += date1Match;

                parts += partsParsed2;
                var date2Match = MatchDateTimes(DateTime2, date.DateTime2);

                // correct for number of date parts parsed
                date2Match *= partsParsed2 / 3m;

                matches += date2Match;

                match = (matches / parts) * 100m;
            }

            return match;
        }

        /// <summary>
        /// Extracts the date period from raw text and attempts to guess if the date period is not present.
        /// </summary>
        /// <param name="dataString">The data string.</param>
        /// <returns>A <see cref="GedcomDatePeriodParseResult"/> object with the period and the text removed from the input string.</returns>
        public GedcomDatePeriodParseResult ExtractDatePeriod(string dataString)
        {
            var result = new GedcomDatePeriodParseResult { DatePeriod = GedcomDatePeriod.Unknown };
            int start = 0, len = dataString.Length;
            var culture = CultureInfo.CurrentCulture;

            foreach (var periodMapping in StaticDateData.PeriodMappings)
            {
                if (periodMapping.TextPosition == GedcomDatePeriodPosition.Prefix)
                {
                    if (dataString.StartsWith(periodMapping.Text, true, culture))
                    {
                        start = periodMapping.Text.Length;
                        len = dataString.Length - start;
                        result.DatePeriod = periodMapping.MapsTo;
                        break;
                    }
                }
                else if (periodMapping.TextPosition == GedcomDatePeriodPosition.Suffix)
                {
                    if (dataString.TrimEnd().EndsWith(periodMapping.Text, true, culture))
                    {
                        start = 0;
                        len = dataString.LastIndexOf(periodMapping.Text);
                        result.DatePeriod = periodMapping.MapsTo;
                        break;
                    }
                }
            }

            result.DataAfterExtraction = dataString.Substring(start, len).TrimStart(new char[] { ' ', '\t' });

            if (result.DatePeriod == GedcomDatePeriod.Unknown || result.DatePeriod == GedcomDatePeriod.Exact)
            {
                // A lot of the time we'll get a partial date that maps to a date range, these should be treated as ranges and not exact dates.
                string[] dateSplit = SplitDateString(result.DataAfterExtraction);
                if (dateSplit.Length == 1 || dateSplit.Length == 2)
                {
                    result.DatePeriod = GedcomDatePeriod.Range;
                }
                else
                {
                    result.DatePeriod = GedcomDatePeriod.Exact;
                }
            }

            return result;
        }

        /// <summary>
        /// Parse passed date into instance properties.
        /// </summary>
        /// <param name="inputDate">The date to parse as a text string.</param>
        public void ParseDateString(string inputDate)
        {
            // clear possible Period cached value;
            this.period = null;

            string dateType = string.Empty;
            var dataString = inputDate; // Preserve inputDate for feedback to the user if it is broken.

            if (dataString.StartsWith("@#"))
            {
                dataString = dataString.Substring(2);
                int i = dataString.IndexOf("@", 2); // TODO: Subtle bug? Should the 2 be there as already trimmed above?
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

            period = dataString;
            var periodResult = ExtractDatePeriod(period);
            dataString = periodResult.DataAfterExtraction;
            DatePeriod = periodResult.DatePeriod;

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

            string[] dateSplit = SplitDateString(dataString);

            dateTime1 = null;
            dateTime2 = null;

            partsParsed1 = 0;
            partsParsed2 = 0;

            if (dateSplit.Length == 1)
            {
                partsParsed1 = 1;
                partsParsed2 = 0;
                dateTime1 = GetDateInfo(dateSplit, 0, 1, calendar, inputDate);
                if (dateTime1 != null)
                {
                    // We only got a year, so we need to interpret that as a range from the start to the end of the year.
                    dateTime2 = dateTime1.Value.AddYears(1).AddSeconds(-1);
                    partsParsed2 = 1;
                    AddParserMessage(ParserMessageIds.InterpretedAsYearRange, inputDate, dateTime1, dateTime2);
                }
            }
            else if (dateSplit.Length == 2)
            {
                partsParsed1 = 2;
                partsParsed2 = 0;
                dateTime1 = GetDateInfo(dateSplit, 0, 2, calendar, inputDate);
                if (dateTime1 != null)
                {
                    // We only got a month and year, so we need to interpret that as a range from the start to the end of the month.
                    dateTime2 = dateTime1.Value.AddMonths(1).AddSeconds(-1);
                    partsParsed2 = 2;
                    AddParserMessage(ParserMessageIds.InterpretedAsMonthRange, inputDate, dateTime1, dateTime2);
                }
            }
            else if (dateSplit.Length == 3)
            {
                // day month year  or year (AND/TO) year
                if (string.Compare(dateSplit[1], "AND", true) != 0 &&
                       string.Compare(dateSplit[1], "TO", true) != 0)
                {
                    partsParsed1 = 1;
                    partsParsed2 = 0;
                    dateTime1 = GetDateInfo(dateSplit, 0, 3, calendar, inputDate);
                }
                else
                {
                    partsParsed1 = 1;
                    partsParsed2 = 1;
                    dateTime1 = GetDateInfo(dateSplit, 0, 1, calendar, inputDate);

                    dateTime2 = GetDateInfo(dateSplit, 2, 1, calendar, inputDate);
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
                        dateTime1 = GetDateInfo(dateSplit, 0, 1, calendar, inputDate);
                        dateTime2 = GetDateInfo(dateSplit, 2, 3, calendar, inputDate);
                    }
                    else if (string.Compare(dateSplit[2], "AND", true) == 0 ||
                             string.Compare(dateSplit[2], "TO", true) == 0)
                    {
                        partsParsed1 = 2;
                        partsParsed2 = 3;
                        dateTime1 = GetDateInfo(dateSplit, 0, 2, calendar, inputDate);
                        dateTime2 = GetDateInfo(dateSplit, 3, 3, calendar, inputDate);
                    }
                    else
                    {
                        // lets assume dateSplit[3] is AND / TO
                        partsParsed1 = 3;
                        partsParsed2 = 3;
                        dateTime1 = GetDateInfo(dateSplit, 0, 3, calendar, inputDate);
                        dateTime2 = GetDateInfo(dateSplit, 4, 3, calendar, inputDate);
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
            tw.Write(Level.ToString());
            tw.Write(" DATE ");

            // only output type if it isn't the default (Gregorian)
            if (dateType != GedcomDateType.Gregorian)
            {
                tw.Write("@#D{0}@ ", dateType.ToString());
            }

            if (!string.IsNullOrWhiteSpace(Period))
            {
                var period = Period.Replace("@", "@@");
                tw.Write(period);
            }

            if (!string.IsNullOrEmpty(Time))
            {
                var line = Time.Replace("@", "@@");

                tw.Write(Environment.NewLine);
                tw.Write("{0} TIME {1}", (Level + 1).ToString(), line);
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

        /// <summary>
        /// Fuzzy matching of dates. TODO: Unit test coverage, no idea how well this works.
        /// </summary>
        /// <param name="dateTimeA">First date to compare.</param>
        /// <param name="dateTimeB">Second date to compare.</param>
        /// <returns>Returns a float rather than an int to allow for some fuzziness
        /// e.g.  10-11-2000 is 10 NOV 2000 or 11 OCT 2000
        /// </returns>
        private decimal MatchDateTimes(DateTime? dateTimeA, DateTime? dateTimeB)
        {
            var matches = decimal.Zero;

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
                        matches += 0.25m;
                    }
                    else if (b.Year >= a.Year - delta && b.Year <= a.Year + delta)
                    {
                        matches += 0.25m;
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
                    matches += 0.75m;
                }
            }

            return matches;
        }

        /// <summary>
        /// Split a date string into parts.
        /// </summary>
        /// <param name="dataString">The date to split.</param>
        /// <returns>Array of strings representing parts of a date.</returns>
        private string[] SplitDateString(string dataString)
        {
            // TODO: the split here accounts for large(ish) amounts of memory allocation
            // Need to do this better, ideally without any splitting.
            return dataString.Split(StaticDateData.GedcomDateParseDelimiters, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Get DateTime information from an array representing parts of a date.
        /// </summary>
        /// <param name="dateSplit">The parts of a valid date.</param>
        /// <param name="start">The first element in the array that contains a valid date part.</param>
        /// <param name="num">The number of elements in the array that contain valid date parts.</param>
        /// <param name="calendar">The calendar for the given date.</param>
        /// <param name="inputDate">The original date string, used for feedback to the user if the date cannot be parsed.</param>
        /// <returns>Return a DateTime if the passed-in values could be parsed into a valid DateTime; otherwise null.</returns>
        private DateTime? GetDateInfo(string[] dateSplit, int start, int num, Calendar calendar, string inputDate)
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
                    foreach (string[] names in StaticDateData.MonthNames)
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
                            AddParserMessage(ParserMessageIds.DayOfDateAdjusted, inputDate, d, daysInMonth);
                            d = daysInMonth;
                        }
                    }

                    try
                    {
                        ret = new DateTime(y, m, d, calendar);
                    }
                    catch
                    {
                        AddParserMessage(ParserMessageIds.DateIsNotValid, inputDate, y, m, d, calendar);
                    }
                }
            }

            return ret;
        }
    }
}
