using ICalendarNet.Models.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Logic.Recurrence
{
    public static partial class RecurrenceRuleEvaluator
    {
        /// <summary>
        /// Applies BYYEARDAY rules to a sequence of candidate dates.
        /// </summary>
        /// <param name="dates">Sequence of candidate dates.</param>
        /// <param name="pattern">Recurrence pattern containing the BYYEARDAY values.</param>
        /// <param name="expand">
        /// <c>true</c>  = expand each input into the concrete dates for that year;
        /// <c>false</c> = limit/filter to BYYEARDAY matches for the input's year;
        /// <c>null</c>  = no-op, return <paramref name="dates"/> unchanged.
        /// </param>
        /// <param name="expandContext">
        /// If <see cref="ExpandContext.IsCandidateSetFullyExpanded"/> is already <c>true</c>,
        /// expansion is suppressed and limit mode is used.
        /// When expansion runs, this method sets the flag to <c>true</c>.
        /// </param>
        /// <remarks>
        /// Expanded dates are constrained to the same calendar year as the input date;
        /// out-of-range BYYEARDAY values (e.g. ±366 in non-leap years) are ignored.
        /// </remarks>
        private static IEnumerable<DateTimeOffset> GetYearDayVariants(
            IEnumerable<DateTimeOffset> dates,
            CalendarRecurrenceRule pattern,
            bool? expand,
            ref ExpandContext expandContext)
        {
            if (expand is null || pattern.ByYearDay.Count == 0)
                return dates;

            if (expand == true && !expandContext.IsCandidateSetFullyExpanded)
            {
                expandContext.IsCandidateSetFullyExpanded = true;
                return GetYearDayVariantsExpanded(dates, pattern);
            }

            // Limit behavior
            return GetYearDayVariantsLimited(dates, pattern);
        }

        private static IEnumerable<DateTimeOffset> GetYearDayVariantsExpanded(
            IEnumerable<DateTimeOffset> dates,
            CalendarRecurrenceRule pattern)
        {
            foreach (var date in dates)
            {
                var anchor = date;
                var yearDayDates = new SortedSet<DateTimeOffset>(
                    pattern.ByYearDay
                        .Select(yearDay => yearDay > 0
                            ? anchor.AddDays(-anchor.DayOfYear + yearDay)
                            : anchor.AddDays(-anchor.DayOfYear + 1).AddYears(1).AddDays(yearDay))
                        // Ignore BY values that don't fit into the current year
                        // (i.e. ±366 in non-leap-years).
                        .Where(d => d.Year == anchor.Year));

                foreach (var d in yearDayDates)
                    yield return d;
            }
        }

        private static IEnumerable<DateTimeOffset> GetYearDayVariantsLimited(
            IEnumerable<DateTimeOffset> dates,
            CalendarRecurrenceRule pattern)
        {
            foreach (var date in dates)
            {
                var candidates =
                    from yearDay in pattern.ByYearDay
                    let newDate = yearDay > 0
                        ? date.AddDays(-date.DayOfYear + yearDay)
                        : date.AddDays(-date.DayOfYear + 1).AddYears(1).AddDays(yearDay)
                    select newDate;

                if (candidates.Contains(date))
                    yield return date;
            }
        }
    }
}