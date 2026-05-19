using ICalendarNet.Models.DataTypes;
using ICalendarNet.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Logic.Recurrence
{
    public static partial class RecurrenceRuleEvaluator
    {
        /// <summary>
        /// Tracks expansion state across the BY-* pipeline. Once a part performs an
        /// expansion, subsequent parts must only limit/filter to avoid producing a
        /// combinatorial explosion across orthogonal expansions.
        /// </summary>
        /// <example>
        /// BYWEEKNO can expand the candidate set across month and year boundaries.
        /// When BYWEEKNO expands, it sets <see cref="IsCandidateSetFullyExpanded"/>
        /// to <c>true</c> so later BY-parts limit instead.
        /// </example>
        private struct ExpandContext
        {
            public bool IsCandidateSetFullyExpanded { get; set; }
        }

        /// <summary>
        /// Returns a list of possible dates generated from the applicable BY-* rules,
        /// using the specified date as a seed.
        /// </summary>
        private static IEnumerable<DateTimeOffset> GetCandidates(
            DateTimeOffset seedDate,
            CalendarRecurrenceRule pattern,
            bool?[] expandBehaviors)
        {
            var expandContext = new ExpandContext { IsCandidateSetFullyExpanded = false };

            IEnumerable<DateTimeOffset> dates = [seedDate];
            dates = GetMonthVariants(dates, pattern, expandBehaviors[0]);
            dates = GetWeekNoVariants(dates, pattern, expandBehaviors[1], ref expandContext);
            dates = GetYearDayVariants(dates, pattern, expandBehaviors[2], ref expandContext);
            dates = GetMonthDayVariants(dates, pattern, expandBehaviors[3], ref expandContext);
            dates = GetDayVariants(dates, pattern, expandBehaviors[4], ref expandContext);
            dates = GetHourVariants(dates, pattern, expandBehaviors[5]);
            dates = GetMinuteVariants(dates, pattern, expandBehaviors[6]);
            dates = GetSecondVariants(dates, pattern, expandBehaviors[7]);
            dates = ApplySetPosRules(dates, pattern);

            return dates;
        }

        /// <summary>
        /// Applies BYSETPOS rules to <paramref name="dates"/>. Valid positions are
        /// from 1 to the size of the date list. Invalid positions are ignored.
        /// </summary>
        private static IEnumerable<DateTimeOffset> ApplySetPosRules(
            IEnumerable<DateTimeOffset> dates,
            CalendarRecurrenceRule pattern)
        {
            if (pattern.BySetPosition.Count == 0)
                return dates;

            HashSet<int> bySetPos;
            if (pattern.BySetPosition.Any(p => p < 0))
            {
                var materialized = dates.ToList();
                var count = materialized.Count;
                dates = materialized;
                bySetPos = [.. pattern.BySetPosition.Select(p => p < 0 ? count + p + 1 : p)];
            }
            else
            {
                bySetPos = [.. pattern.BySetPosition];
            }

            return dates.Where((_, i) => bySetPos.Contains(i + 1));
        }

        /// <summary>
        /// Builds the expand/limit/skip table for each BY-part as defined in
        /// RFC 5545 §3.3.10 (Page 43).
        /// Index mapping must match <see cref="GetCandidates"/>:
        /// 0=BYMONTH, 1=BYWEEKNO, 2=BYYEARDAY, 3=BYMONTHDAY, 4=BYDAY,
        /// 5=BYHOUR,  6=BYMINUTE, 7=BYSECOND, 8=BYSETPOS (sentinel)
        /// </summary>
        private static bool?[] GetExpandBehaviorList(CalendarRecurrenceRule p)
        {
            switch (p.Frequency)
            {
                case FrequencyType.Minutely:
                    return [false, null, false, false, false, false, false, true, false];

                case FrequencyType.Hourly:
                    return [false, null, false, false, false, false, true, true, false];

                case FrequencyType.Daily:
                    return [false, null, null, false, false, true, true, true, false];

                case FrequencyType.Weekly:
                    return [false, null, null, null, true, true, true, true, false];

                case FrequencyType.Monthly:
                    {
                        bool?[] row = [false, null, null, true, true, true, true, true, false];

                        // RFC 5545 Notes 1 & 2: BYDAY should act as a limiter when
                        // BYMONTHDAY or BYYEARDAY are present.
                        if (p.ByMonthDay.Count > 0 || p.ByYearDay.Count > 0)
                            row[4] = false;

                        return row;
                    }

                case FrequencyType.Yearly:
                    {
                        bool?[] row = [true, true, true, true, true, true, true, true, false];

                        // RFC 5545 Notes 1 & 2: BYDAY limits when BYMONTHDAY/BYYEARDAY are present.
                        if (p.ByYearDay.Count > 0 || p.ByMonthDay.Count > 0)
                            row[4] = false;

                        return row;
                    }

                default:
                    return [false, null, false, false, false, false, false, false, false];
            }
        }
    }
}