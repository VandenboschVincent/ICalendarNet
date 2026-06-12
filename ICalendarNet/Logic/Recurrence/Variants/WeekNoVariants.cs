using ICalendarNet.Extensions;
using ICalendarNet.Models.DataTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ICalendarNet.Logic.Recurrence
{
    public static partial class RecurrenceRuleEvaluator
    {
        /// <summary>
        /// Applies BYWEEKNO rules to the specified date list. If no BYWEEKNO rules are
        /// specified, the date list is returned unmodified.
        /// </summary>
        private static IEnumerable<DateTimeOffset> GetWeekNoVariants(
            IEnumerable<DateTimeOffset> dates,
            CalendarRecurrenceRule pattern,
            bool? expand,
            ref ExpandContext expandContext)
        {
            if (expand == null || pattern.ByWeekNo.Count == 0)
                return dates;

            Debug.Assert(expand.Value);

            // Expand behavior
            var weekNoDates = GetWeekNoVariantsExpanded(dates, pattern).ToList();

            // subsequent parts should only limit, not expand
            expandContext.IsCandidateSetFullyExpanded = true;

            // Apply BYMONTH limit behavior, as we might have expanded over month/year
            // boundaries here, and BYMONTH was already applied earlier.
            return GetMonthVariants(weekNoDates, pattern, expand: false);
        }

        private static IEnumerable<DateTimeOffset> GetWeekNoVariantsExpanded(
            IEnumerable<DateTimeOffset> dates,
            CalendarRecurrenceRule pattern)
        {
            foreach (var (t, weekNo) in dates.SelectMany(
                         t => GetByWeekNoForYearNormalized(pattern, t.Year),
                         (t, weekNo) => (t, weekNo)))
            {
                var date = t;

                // Make sure we start from a reference date that is in a week that belongs
                // to the current year. The actual week is irrelevant — what matters is
                // that the week belongs to the current year and that the week day is
                // preserved.
                if (date.Month == 1)
                    date = date.AddDays(7);
                else if (date.Month >= 12)
                    date = date.AddDays(-7);

                // Determine our current week number, then move to the target week.
                var currWeekNo = CalendarExtensions.GetIso8601WeekOfYear(date, pattern.FirstDayOfWeek);
                date = date.AddDays((weekNo - currWeekNo) * 7);

                // Ignore the week if it doesn't belong to the current year.
                if (CalendarExtensions.GetIso8601YearOfWeek(date, pattern.FirstDayOfWeek) != t.Year)
                    continue;

                // Step back to the first day of the week.
                date = GetFirstDayOfWeekDate(date, pattern.FirstDayOfWeek);

                foreach (var d in Enumerable.Range(0, 7).Select(i => date.AddDays(i)))
                    yield return d;
            }
        }

        /// <summary>
        /// Normalize the BYWEEKNO values to be positive integers (resolving negative
        /// "from end of year" entries to absolute week numbers).
        /// </summary>
        private static List<int> GetByWeekNoForYearNormalized(CalendarRecurrenceRule pattern, int year)
        {
            var weeksInYear = new Lazy<int>(
                () => CalendarExtensions.GetIso8601WeeksInYear(year));

            return [.. pattern.ByWeekNo
                .Select(weekNo => weekNo >= 0 ? weekNo : weeksInYear.Value + weekNo + 1)
                .OrderBy(x => x)];
        }
    }
}