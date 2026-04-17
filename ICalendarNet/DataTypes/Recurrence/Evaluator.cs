using ICalendarNet.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace ICalendarNet.DataTypes.Recurrence
{
    public abstract class Evaluator
    {
        protected static void IncrementDate(ref DateTimeOffset dt, CalendarRecurrenceRule pattern, int interval)
        {
            if (interval == 0)
                return;

            try
            {
                var old = dt;
                switch (pattern.Frequency)
                {
                    case FrequencyType.Secondly:
                        dt = old.AddSeconds(interval);
                        break;
                    case FrequencyType.Minutely:
                        dt = old.AddMinutes(interval);
                        break;
                    case FrequencyType.Hourly:
                        dt = old.AddHours(interval);
                        break;
                    case FrequencyType.Daily:
                        dt = old.AddDays(interval);
                        break;
                    case FrequencyType.Weekly:
                        dt = old.AddWeeks(interval, pattern.FirstDayOfWeek);
                        break;
                    case FrequencyType.Monthly:
                        if (old.Day == 29 && old.Month == 2 && DateTime.IsLeapYear(old.Year))
                            dt = old.AddYears(4); // leap year skip to next valid date
                        else
                            dt = old.AddDays(-old.Day + 1).AddMonths(interval);
                        break;
                    case FrequencyType.Yearly:
                        // RecurrencePatternEvaluator relies on the assumption that after incrementing, the new refDate
                        // is usually at the first day of an interval.
                        if (old.Day == 29 && old.Month == 2 && DateTime.IsLeapYear(old.Year))
                            dt = old.AddYears(4); // leap year skip to next year
                        else
                            dt = old.AddDays(-old.Day + 1).AddYears(interval);
                        break;
                    default:
                        // Frequency should always be valid at this stage.
                        System.Diagnostics.Debug.Fail($"'{pattern.Frequency}' as RecurrencePattern.Frequency is not implemented.");
                        break;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // intentionally don't include the outer exception
                throw new ArgumentException("Evaluation aborted: The maximum supported date-time was exceeded while enumerating a recurrence rule. This commonly happens when trying to enumerate an unbounded RRULE to its end. Consider applying the .TakeWhile() operator.");
            }
        }

        public abstract IEnumerable<CalendarPeriod> Evaluate(DateTimeOffset referenceDate, DateTimeOffset? periodStart, EvaluationOptions? options);
    }
}
