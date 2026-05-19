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
        /// Returns a list of start dates in the specified period represented by this
        /// recurrence pattern. The base date is used to inject default values so that
        /// returned dates are in the correct format (e.g. respecting BYHOUR rather
        /// than the search start's time-of-day).
        /// </summary>
        private static IEnumerable<DateTimeOffset> GetDates(
            DateTimeOffset seed,
            DateTimeOffset? periodStart,
            CalendarRecurrenceRule pattern,
            EvaluationOptions? options)
        {
            var originalDate = seed;
            var seedCopy = seed;
            var periodStartDt = periodStart?.ToOffset(seed.Offset);

            if (pattern.Frequency == FrequencyType.Yearly && pattern.ByWeekNo.Count != 0)
            {
                // Dates in the first or last week of the year could belong to weeks that
                // belong to the prev/next year, in which case we must adjust that year.
                // This is necessary to get the intervals right.
                IncrementDate(
                    ref seedCopy,
                    pattern,
                    Calendar.GetIso8601YearOfWeek(seedCopy, pattern.FirstDayOfWeek) - seedCopy.Year);
            }

            // Optimize the start time for selecting candidates (only applicable where
            // a COUNT is not specified).
            if (pattern.Count is null)
            {
                var incremented = seedCopy;
                while (incremented < periodStartDt)
                {
                    seedCopy = incremented;
                    IncrementDate(ref incremented, pattern, pattern.Interval);
                }
            }
            else if (pattern.Count < 1)
            {
                throw new ArgumentException("Count must be greater than 0");
            }

            // Do the enumeration in a separate method, as it is a generator method that is
            // only executed after enumeration started. In order to do most validation upfront,
            // do as many steps outside the generator as possible.
            return EnumerateDates(originalDate, seedCopy, pattern, options);
        }

        /// <summary>
        /// Eagerly prepares state for <see cref="EnumerateDatesIterator"/>. Splitting the
        /// setup from the iterator ensures any setup-time exceptions surface immediately
        /// at call time, not lazily on the first <c>MoveNext()</c>.
        /// </summary>
        private static IEnumerable<DateTimeOffset> EnumerateDates(
            DateTimeOffset originalDate,
            DateTimeOffset intervalRefTime,
            CalendarRecurrenceRule pattern,
            EvaluationOptions? options)
        {
            var expandBehavior = GetExpandBehaviorList(pattern);
            var searchEndDate = GetSearchEndDate(pattern);

            return EnumerateDatesIterator(
                originalDate,
                intervalRefTime,
                pattern,
                options,
                expandBehavior,
                searchEndDate);
        }

        private static IEnumerable<DateTimeOffset> EnumerateDatesIterator(
            DateTimeOffset originalDate,
            DateTimeOffset intervalRefTime,
            CalendarRecurrenceRule pattern,
            EvaluationOptions? options,
            bool?[] expandBehavior,
            DateTimeOffset? searchEndDate)
        {
            var noCandidateIncrementCount = 0;
            var dateCount = 0;

            while (true)
            {
                var lowerLimit = GetIntervalLowerLimit(intervalRefTime, pattern, originalDate);

                var candidates = GetCandidates(
                    lowerLimit > intervalRefTime ? lowerLimit : intervalRefTime,
                    pattern,
                    expandBehavior);

                if (!candidates.Any(x => x <= lowerLimit) && searchEndDate < lowerLimit)
                    break;

                foreach (var t in candidates.Where(t => t >= originalDate))
                {
                    noCandidateIncrementCount = 0;

                    // candidates MAY occur before periodStart
                    // (e.g. FREQ=YEARLY;BYWEEKNO=1 could return dates from the previous year).
                    // UNTIL is applied outside this method, after TZ conversion has been applied.
                    yield return t;
                    dateCount++;

                    if (dateCount >= pattern.Count)
                        yield break;
                }

                if (noCandidateIncrementCount > options?.MaxUnmatchedIncrementsLimit)
                    break;

                noCandidateIncrementCount++;
                IncrementDate(ref intervalRefTime, pattern, pattern.Interval);
            }
        }

        /// <summary>
        /// Computes a coarse upper bound used purely as a performance heuristic to stop
        /// incrementing once UNTIL has been clearly passed. Precise UNTIL handling is
        /// done outside this method after TZ conversion.
        /// </summary>
        private static DateTimeOffset? GetSearchEndDate(CalendarRecurrenceRule pattern)
            // Add 1d to UNTIL to cover any time shift and DST changes.
            => pattern.Until?.AddDays(1);

        /// <summary>
        /// Find the lowest possible date/time for a recurrence in the given interval.
        /// </summary>
        /// <remarks>
        /// For most frequencies the interval's lower limit is simply the provided
        /// <paramref name="intervalRefTime"/>. YEARLY rules require special handling:
        /// - If BYMONTH is present and BYWEEKNO is not, an occurrence for the interval
        ///   might fall earlier in the year than the intervalRefTime's month/day. In
        ///   that case we compute the earliest possible date/time that could be
        ///   generated for the interval.
        /// - If neither BYMONTH nor BYWEEKNO is present, we use the original date's month.
        /// - If only BYWEEKNO is present, the interval may contain days from the previous
        ///   or next year (ISO week boundaries). In that case we adjust the interval
        ///   start to the first day of the configured week so we don't miss candidates
        ///   that belong to the week containing Jan 1st.
        /// </remarks>
        private static DateTimeOffset GetIntervalLowerLimit(
            DateTimeOffset intervalRefTime,
            CalendarRecurrenceRule pattern,
            DateTimeOffset originalDate)
        {
            return pattern switch
            {
                { Frequency: FrequencyType.Yearly, ByMonth.Count: 0, ByWeekNo.Count: 0 }
                    => YearlyLimitWithoutMonthOrWeek(intervalRefTime, pattern, originalDate),

                { Frequency: FrequencyType.Yearly, ByMonth.Count: > 0, ByWeekNo.Count: 0 }
                    => YearlyLimitWithMonth(intervalRefTime, pattern, originalDate),

                { Frequency: FrequencyType.Yearly, ByWeekNo.Count: not 0 }
                    => GetFirstDayOfWeekDate(intervalRefTime, pattern.FirstDayOfWeek),

                {
                    Frequency: FrequencyType.Weekly,
                    ByMonth.Count: 0, ByWeekNo.Count: 0, ByDay.Count: 0,
                    ByMonthDay.Count: 0, ByYearDay.Count: 0
                }
                    => GetFirstDayOfWeekDate(intervalRefTime.AddDays(6), originalDate.DayOfWeek),

                _ => intervalRefTime,
            };
        }

        private static DateTimeOffset YearlyLimitWithoutMonthOrWeek(
            DateTimeOffset intervalRefTime,
            CalendarRecurrenceRule pattern,
            DateTimeOffset originalDate)
        {
            // Return intervalRefTime but use the month from the original DTSTART.
            // Else, the earliest candidate for the interval might be too early.
            // We shift intervalRefTime by the difference in months — this relies on
            // AddMonths month-end semantics (Jan 31 -> Feb 28/29) instead of manual clamping.
            var monthDelta = originalDate.Month - intervalRefTime.Month;
            var adjusted = intervalRefTime.AddMonths(monthDelta);
            var day = adjusted.Day;

            if (pattern.ByDay.Count == 0
                && pattern.ByMonthDay.Count == 0
                && pattern.ByYearDay.Count == 0
                && adjusted.Month == originalDate.Month)
            {
                // No other BY* rules can pull the candidate before the original date,
                // so we can clamp the day to the original day in month.
                day = originalDate.Day;
            }

            return new DateTimeOffset(
                adjusted.Year, adjusted.Month, day,
                adjusted.Hour, adjusted.Minute, adjusted.Second,
                intervalRefTime.Offset);
        }

        private static DateTimeOffset YearlyLimitWithMonth(
            DateTimeOffset intervalRefTime,
            CalendarRecurrenceRule pattern,
            DateTimeOffset originalDate)
        {
            // When evaluating a YEARLY rule that restricts months (BYMONTH) but not
            // week numbers, the earliest candidate inside the interval can be in an
            // earlier month/day than intervalRefTime (e.g. BYMONTH=1 with intervalRefTime
            // anchored on a later month). Compute the earliest plausible date/time for
            // this interval and use that as the lower limit.
            //
            // We pick:
            //  - year  = intervalRefTime.Year
            //  - month = smallest BYMONTH
            //  - day   = smallest BYMONTHDAY (clamped) or original DTSTART day
            //  - time  = smallest BYHOUR/BYMINUTE/BYSECOND or original DTSTART time
            var year = intervalRefTime.Year;
            var month = pattern.ByMonth.Min();
            var daysInMonth = Calendar.GetDaysInMonth(year, month);

            int day;
            if (pattern.ByMonthDay.Count > 0)
            {
                // Map BYMONTHDAY entries (positive and negative) to absolute days
                // in the target month, then pick the smallest.
                day = pattern.ByMonthDay
                    .Select(md => md > 0
                        ? Math.Min(md, daysInMonth)
                        : Math.Max(1, daysInMonth + md + 1))
                    .Min();
            }
            else
            {
                day = Math.Min(originalDate.Day, daysInMonth);
            }

            var hour = pattern.ByHour.Count > 0 ? pattern.ByHour.Min() : originalDate.Hour;
            var minute = pattern.ByMinute.Count > 0 ? pattern.ByMinute.Min() : originalDate.Minute;
            var second = pattern.BySecond.Count > 0 ? pattern.BySecond.Min() : originalDate.Second;

            return new DateTimeOffset(year, month, day, hour, minute, second, intervalRefTime.Offset);
        }
    }
}