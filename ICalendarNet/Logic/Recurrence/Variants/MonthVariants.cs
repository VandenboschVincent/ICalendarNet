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
        /// Applies BYMONTH rules to the specified date list. If no BYMONTH rules are
        /// specified, the date list is returned unmodified.
        /// </summary>
        private static IEnumerable<DateTimeOffset> GetMonthVariants(
            IEnumerable<DateTimeOffset> dates,
            CalendarRecurrenceRule pattern,
            bool? expand)
        {
            if (expand == null || pattern.ByMonth.Count == 0)
                return dates;

            if (expand.Value)
            {
                // Expand behavior
                return dates.SelectMany(d =>
                    pattern.ByMonth.Select(month => d.AddMonths(month - d.Month)));
            }

            // Limit behavior
            if (pattern.Frequency == FrequencyType.Weekly)
            {
                // The dates here represent weeks, with each date being the start of a week
                // except for the initial reference date. Return weeks that have any day
                // within BYMONTH.
                return dates.Where(date =>
                    pattern.ByMonth.Contains(date.Month)
                    || pattern.ByMonth.Contains(date.AddDays(6).Month));
            }

            return dates.Where(date => pattern.ByMonth.Contains(date.Month));
        }
    }
}