using System;
using System.Globalization;

namespace ICalendarNet.Extensions
{
    public static class CalendarExtensions
    {
        /// <summary>
        /// Calculate the week number according to ISO.8601, as required by RFC 5545.
        /// </summary>
        public static int GetIso8601WeekOfYear(DateTimeOffset time, DayOfWeek firstDayOfWeek)
        {
            // The 4th day of the week (the "Thursday-equivalent") is always inside
            // the week-numbering year this week belongs to. All such mid-week days
            // are exactly 7 days apart, and the first one of the year is by
            // definition the mid-week day of week 1 — so integer-dividing its
            // day-of-year by 7 yields the week number directly.
            var midWeek = time.GetStartOfWeek(firstDayOfWeek).AddDays(3);
            return (midWeek.DayOfYear - 1) / 7 + 1;
        }

        /// <summary>
        /// Calculate and return the date that represents the first day of the week the given date is
        /// in, according to the week numbering required by RFC 5545.
        /// </summary>
        public static DateTimeOffset GetStartOfWeek(this DateTimeOffset t, DayOfWeek firstDayOfWeek)
        {
            var t0 = ((int)firstDayOfWeek) % 7;
            var tn = ((int)t.DayOfWeek) % 7;
            return t.AddDays(-((tn + 7 - t0) % 7));
        }

        /// <summary>
        /// Calculate the year, the given date's week belongs to according to ISO 8601, as required by RFC 5545.
        /// </summary>
        /// <remarks>
        /// A date's nominal year may be different from the year, the week belongs to that the date is in.
        /// I.e. the first and last week of the year may belong to a different year than the date's year.
        /// E.g. for `2019-12-31` with first day of the week being Monday, the method will return 2020,
        /// because the week that contains `2019-12-31` is the first week of 2020.
        /// </remarks>
        public static int GetIso8601YearOfWeek(DateTimeOffset time, DayOfWeek firstDayOfWeek)
        {
            // Same trick: the year of the 4th day of the week is, by RFC 5545's
            // "at least 4 days in that calendar year" rule, the week-numbering year.
            var midWeek = time.GetStartOfWeek(firstDayOfWeek).AddDays(3);
            return midWeek.Year;
        }

        /// <summary>
        /// Calculate the number of weeks in the given year according to ISO 8601, as required by RFC 5545.
        /// </summary>
        public static int GetIso8601WeeksInYear(int year)
        {
            return ISOWeek.GetWeeksInYear(year);
        }

        /// <summary>
        /// Calculate the number of days in the month of the given date.
        /// </summary>
        public static int GetDaysInMonth(DateTimeOffset time)
        {
            return GetDaysInMonth(time.Year, time.Month);
        }

        /// <summary>
        /// Calculate the number of days in the given month of the given year.
        /// </summary>
        public static int GetDaysInMonth(int year, int month)
        {
            return DateTime.DaysInMonth(year, month);
        }

        public static bool TryFindTimeZone(string tzId, out TimeZoneInfo? tz)
        {
#if NET8_0_OR_GREATER
            return TimeZoneInfo.TryFindSystemTimeZoneById(tzId, out tz);
#else
            try
            {
                tz = TimeZoneInfo.FindSystemTimeZoneById(tzId);
                return true;
            }
            catch (TimeZoneNotFoundException)
            {
                //No other method for tryfind in netstandard2.1, so we have to catch the exception and return null if not found
                tz = null;
                return false;
            }
#endif
        }
    }
}