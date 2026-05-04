using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.DataTypes.Recurrence
{
    internal static class RecurrenceUtil
    {

        internal static IEnumerable<CalendarPeriod>? GetRecurrenceDates(
            CalendarRecurrenceRule rrule
            , DateTimeOffset dtstart
            , int amount = 1
            , DateTimeOffset? periodStart = null
            , bool addStartDate = true
            , DateTimeOffset? maxDate = null
            , IEnumerable<DateTimeOffset>? exceptionDates = null)
        {
            List<DateTimeOffset> exdates = exceptionDates?.ToList() ?? new List<DateTimeOffset>();
            var evaluator = new RecurrenceRuleEvaluator(rrule);
            return evaluator.Evaluate(dtstart, periodStart, new()
            {
                MaxOccurrencesLimit = amount,
                AddStartDate = addStartDate,
                MaxDateTime = maxDate
            }).Where(t => !exdates.Contains(t.DateStart));
        }
        public static bool?[] GetExpandBehaviorList(CalendarRecurrenceRule p)
        {
            // See the table in RFC 5545 Section 3.3.10 (Page 43).
            // Index mapping (must match RecurrencePatternEvaluator.GetCandidates order!):
            // 0 = BYMONTH, 1 = BYWEEKNO, 2 = BYYEARDAY, 3 = BYMONTHDAY, 4 = BYDAY,
            // 5 = BYHOUR,  6 = BYMINUTE,  7 = BYSECOND, 8 = BYSETPOS (sentinel)
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

                        // RFC 5545 Notes 1 & 2:
                        // BYDAY should act as a limiter when BYMONTHDAY or BYYEARDAY are present.
                        if (p.ByMonthDay.Count > 0 || p.ByYearDay.Count > 0)
                        {
                            row[4] = false;
                        }

                        return row;
                    }
                case FrequencyType.Yearly:
                    {
                        bool?[] row = [true, true, true, true, true, true, true, true, false];

                        // RFC 5545 Notes 1 & 2:
                        // BYDAY should act as a limiter when BYMONTHDAY or BYYEARDAY are present.
                        if (p.ByYearDay.Count > 0 || p.ByMonthDay.Count > 0)
                        {
                            row[4] = false;
                        }

                        return row;
                    }
                default:
                    return [false, null, false, false, false, false, false, false, false];
            }
        }
    }
}
