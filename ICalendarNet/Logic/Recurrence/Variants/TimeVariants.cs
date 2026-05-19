using ICalendarNet.Models.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Logic.Recurrence
{
    public static partial class RecurrenceRuleEvaluator
    {
        /// <summary>
        /// Applies BYHOUR rules to the specified date list.
        /// </summary>
        private static IEnumerable<DateTimeOffset> GetHourVariants(
            IEnumerable<DateTimeOffset> dates, CalendarRecurrenceRule pattern, bool? expand)
        {
            if (expand == null || pattern.ByHour.Count == 0)
                return dates;

            if (expand.Value)
                return dates.SelectMany(date =>
                    pattern.ByHour.Select(hour => date.AddHours(-date.Hour + hour)));

            return dates.Where(date => pattern.ByHour.Contains(date.Hour));
        }

        /// <summary>
        /// Applies BYMINUTE rules to the specified date list.
        /// </summary>
        private static IEnumerable<DateTimeOffset> GetMinuteVariants(
            IEnumerable<DateTimeOffset> dates, CalendarRecurrenceRule pattern, bool? expand)
        {
            if (expand == null || pattern.ByMinute.Count == 0)
                return dates;

            if (expand.Value)
                return dates.SelectMany(date =>
                    pattern.ByMinute.Select(minute => date.AddMinutes(-date.Minute + minute)));

            return dates.Where(date => pattern.ByMinute.Contains(date.Minute));
        }

        /// <summary>
        /// Applies BYSECOND rules to the specified date list.
        /// </summary>
        private static IEnumerable<DateTimeOffset> GetSecondVariants(
            IEnumerable<DateTimeOffset> dates, CalendarRecurrenceRule pattern, bool? expand)
        {
            if (expand == null || pattern.BySecond.Count == 0)
                return dates;

            if (expand.Value)
                return dates.SelectMany(date =>
                    pattern.BySecond.Select(second => date.AddSeconds(-date.Second + second)));

            return dates.Where(date => pattern.BySecond.Contains(date.Second));
        }
    }
}