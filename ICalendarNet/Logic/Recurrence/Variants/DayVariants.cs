using ICalendarNet.Extensions;
using ICalendarNet.Models.DataTypes;
using ICalendarNet.Models.DataTypes.Recurrence;
using ICalendarNet.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Logic.Recurrence
{
    public static partial class RecurrenceRuleEvaluator
    {
        /// <summary>
        /// Applies BYDAY rules to the specified date list. If no BYDAY rules are
        /// specified, the date list is returned unmodified.
        /// </summary>
        private static IEnumerable<DateTimeOffset> GetDayVariants(
            IEnumerable<DateTimeOffset> dates,
            CalendarRecurrenceRule pattern,
            bool? expand,
            ref ExpandContext expandContext)
        {
            if (expand == null || pattern.ByDay.Count == 0)
                return dates;

            if (expand.Value && !expandContext.IsCandidateSetFullyExpanded)
            {
                expandContext.IsCandidateSetFullyExpanded = true;
                return GetDayVariantsExpanded(dates, pattern);
            }

            return GetDayVariantsLimited(dates, pattern);
        }

        private static IEnumerable<DateTimeOffset> GetDayVariantsLimited(
            IEnumerable<DateTimeOffset> dates,
            CalendarRecurrenceRule pattern) =>
            // If no offset is specified, simply test the day of week.
            // When an offset is present, use GetAbsWeekDays to compute the concrete
            // weekday dates according to the frequency and check containment.
            dates.Where(date => pattern.ByDay.Any(weekDay =>
            {
                if (weekDay.Offset == 0)
                    return weekDay.DayOfWeek.Equals(date.DayOfWeek);

                // When limiting with an offset (e.g. "22MO" or "1MO"), compute the
                // absolute dates for that WeekDay in the appropriate scope and check
                // if the candidate matches one of them.
                return GetAbsWeekDays(date, weekDay, pattern).Any(d => d.Equals(date));
            }));

        private static IEnumerable<DateTimeOffset> GetDayVariantsExpanded(
            IEnumerable<DateTimeOffset> dates,
            CalendarRecurrenceRule pattern)
        {
            foreach (var date in dates)
            {
                var weekDayDates = new SortedSet<DateTimeOffset>();
                foreach (var day in pattern.ByDay)
                    foreach (var d in GetAbsWeekDays(date, day, pattern))
                        weekDayDates.Add(d);

                foreach (var d in weekDayDates)
                    yield return d;
            }
        }

        /// <summary>
        /// Returns a list of applicable dates corresponding to the specified week day
        /// in accordance with the frequency specified by this recurrence rule.
        /// </summary>
        private static IEnumerable<DateTimeOffset> GetAbsWeekDays(
            DateTimeOffset date,
            WeekDay weekDay,
            CalendarRecurrenceRule pattern)
        {
            var dates = pattern switch
            {
                { Frequency: FrequencyType.Daily } => GetAbsWeekDaysDaily(date, weekDay),
                { Frequency: FrequencyType.Weekly } or { ByWeekNo.Count: > 0 }
                    => GetAbsWeekDaysWeekly(date, pattern, weekDay),
                { Frequency: FrequencyType.Monthly } or { ByMonth.Count: > 0 }
                    => GetAbsWeekDaysMonthly(date, pattern, weekDay),
                { Frequency: FrequencyType.Yearly } => GetAbsWeekDaysYearly(date, weekDay),
                _ => [],
            };

            return GetOffsetDates(dates, weekDay.Offset);
        }

        private static IEnumerable<DateTimeOffset> GetAbsWeekDaysDaily(DateTimeOffset date, WeekDay weekDay)
            => date.DayOfWeek == weekDay.DayOfWeek ? [date] : [];

        private static IEnumerable<DateTimeOffset> GetAbsWeekDaysYearly(DateTimeOffset date, WeekDay weekDay)
        {
            var year = date.Year;
            var daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;

            // Go to Jan 1 and find first occurrence of target weekday.
            date = date.AddDays(-date.DayOfYear + 1);
            var offset = ((int)weekDay.DayOfWeek - (int)date.DayOfWeek + 7) % 7;
            date = date.AddDays(offset);

            // Yield all occurrences (52 or 53 per year).
            var occurrenceCount = (daysInYear - offset + 6) / 7;
            for (var i = 0; i < occurrenceCount; i++)
            {
                yield return date;
                date = date.AddDays(7);
            }
        }

        private static IEnumerable<DateTimeOffset> GetAbsWeekDaysMonthly(
            DateTimeOffset date, CalendarRecurrenceRule pattern, WeekDay weekDay)
        {
            var month = date.Month;
            var year = date.Year;
            var daysInMonth = CalendarExtensions.GetDaysInMonth(year, month);

            // Go to first day of month, then find first occurrence of target weekday.
            date = date.AddDays(-date.Day + 1);
            var offset = ((int)weekDay.DayOfWeek - (int)date.DayOfWeek + 7) % 7;
            date = date.AddDays(offset);

            // Pre-calculate occurrence count (4 or 5 occurrences per month).
            var occurrenceCount = (daysInMonth - offset + 6) / 7;

            var byWeekNoNormalized = pattern.ByWeekNo.Count > 0
                ? GetByWeekNoForYearNormalized(
                    pattern,
                    CalendarExtensions.GetIso8601YearOfWeek(date, pattern.FirstDayOfWeek))
                : null;

            for (var i = 0; i < occurrenceCount; i++)
            {
                var matchesWeekNo = byWeekNoNormalized == null
                    || byWeekNoNormalized.Contains(
                        CalendarExtensions.GetIso8601WeekOfYear(date, pattern.FirstDayOfWeek));

                var matchesMonth = pattern.ByMonth.Count == 0
                    || pattern.ByMonth.Contains(date.Month);

                if (matchesWeekNo && matchesMonth)
                    yield return date;

                date = date.AddDays(7);
            }
        }

        private static IEnumerable<DateTimeOffset> GetAbsWeekDaysWeekly(
            DateTimeOffset date, CalendarRecurrenceRule pattern, WeekDay weekDay)
        {
            var weekNo = CalendarExtensions.GetIso8601WeekOfYear(date, pattern.FirstDayOfWeek);

            // Go to the first day of the week.
            var weekDayOffset = GetWeekDayOffset(date, pattern.FirstDayOfWeek);
            date = date.AddDays(-weekDayOffset);

            // Find first occurrence of target weekday.
            var offset = ((int)weekDay.DayOfWeek - (int)date.DayOfWeek + 7) % 7;
            date = date.AddDays(offset);

            var currentWeekNo = CalendarExtensions.GetIso8601WeekOfYear(date, pattern.FirstDayOfWeek);
            var nextWeekNo = currentWeekNo;

            var byWeekNoNormalized = pattern.ByWeekNo.Count > 0
                ? GetByWeekNoForYearNormalized(
                    pattern,
                    CalendarExtensions.GetIso8601YearOfWeek(date, pattern.FirstDayOfWeek))
                : null;

            // Boundary case for weekly recurring patterns:
            // Dec 31 may be week 53 while surrounding dates are in week 1 of next year.
            while (currentWeekNo == weekNo
                   || (nextWeekNo < weekNo
                       && currentWeekNo == nextWeekNo
                       && pattern.Frequency == FrequencyType.Weekly))
            {
                var matchesWeekNo = byWeekNoNormalized == null
                    || byWeekNoNormalized.Contains(currentWeekNo);

                var matchesMonth = pattern.ByMonth.Count == 0
                    || pattern.ByMonth.Contains(date.Month);

                if (matchesWeekNo && matchesMonth)
                    yield return date;

                date = date.AddDays(7);
                currentWeekNo = CalendarExtensions.GetIso8601WeekOfYear(date, pattern.FirstDayOfWeek);
            }
        }
    }
}