using ICalendarNet.Extensions;
using ICalendarNet.Models.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Logic.Recurrence
{
    public static partial class RecurrenceRuleEvaluator
    {
        /// <summary>
        /// Applies BYMONTHDAY rules to the specified date list. If no BYMONTHDAY rules
        /// are specified, the date list is returned unmodified.
        /// </summary>
        private static IEnumerable<DateTimeOffset> GetMonthDayVariants(
            IEnumerable<DateTimeOffset> dates,
            CalendarRecurrenceRule pattern,
            bool? expand,
            ref ExpandContext expandContext)
        {
            if (expand == null || pattern.ByMonthDay.Count == 0)
                return dates;

            if (expand.Value && !expandContext.IsCandidateSetFullyExpanded)
            {
                expandContext.IsCandidateSetFullyExpanded = true;
                return GetMonthDayVariantsExpanded(dates, pattern);
            }

            return GetMonthDayVariantsLimited(dates, pattern);
        }

        private static IEnumerable<DateTimeOffset> GetMonthDayVariantsLimited(
            IEnumerable<DateTimeOffset> dates,
            CalendarRecurrenceRule pattern)
        {
            foreach (var date in dates)
            {
                // If BYMONTH is specified and this date's month is not included, skip it.
                if (pattern.ByMonth.Count > 0 && !pattern.ByMonth.Contains(date.Month))
                    continue;

                if (MatchesAnyMonthDay(date, pattern.ByMonthDay))
                    yield return date;
            }

            // Local helper: checks whether the candidate matches any BYMONTHDAY entry,
            // taking negative values into account (relative to the month's length).
            static bool MatchesAnyMonthDay(DateTimeOffset candidate, IEnumerable<int> monthDays)
            {
                var daysInMonth = CalendarExtensions.GetDaysInMonth(candidate.Year, candidate.Month);
                foreach (var monthDay in monthDays)
                {
                    var byMonthDay = monthDay > 0 ? monthDay : (daysInMonth + monthDay + 1);
                    if (candidate.Day == byMonthDay)
                        return true;
                }
                return false;
            }
        }

        private static IEnumerable<DateTimeOffset> GetMonthDayVariantsExpanded(
            IEnumerable<DateTimeOffset> dates,
            CalendarRecurrenceRule pattern)
        {
            foreach (var date in dates)
            {
                var monthDayDates = new SortedSet<DateTimeOffset>(
                    from monthDay in pattern.ByMonthDay
                    let daysInMonth = CalendarExtensions.GetDaysInMonth(date.Year, date.Month)
                    let monthDayAbs = monthDay > 0 ? monthDay : (daysInMonth + monthDay + 1)
                    where monthDayAbs > 0 && monthDayAbs <= daysInMonth
                    select date.AddDays(-date.Day + monthDayAbs));

                foreach (var d in monthDayDates)
                    yield return d;
            }
        }
    }
}