using ICalendarNet.Extensions;
using ICalendarNet.Models.DataTypes;
using ICalendarNet.Models.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ICalendarNet.Logic.Recurrence
{
    public static partial class RecurrenceRuleEvaluator
    {
        private static DateTimeOffset GetMaxYear(DateTimeOffset periodStart)
        {
            if (periodStart > new DateTimeOffset(9800, 1, 1, 0, 0, 0, TimeSpan.Zero))
                return DateTimeOffset.MaxValue;

            return periodStart > DateTimeOffset.UtcNow
                ? periodStart.AddYears(100)
                : DateTimeOffset.UtcNow.AddYears(100);
        }

        private static DateTimeOffset GetFirstDayOfWeekDate(DateTimeOffset date, DayOfWeek firstDayOfWeek)
            => date.AddDays(-((int)date.DayOfWeek + 7 - (int)firstDayOfWeek) % 7);

        /// <summary>
        /// Returns the days since the start of the week, 0 if the date is on the first
        /// day of the week.
        /// </summary>
        private static int GetWeekDayOffset(DateTimeOffset date, DayOfWeek startOfWeek)
            => date.DayOfWeek + ((date.DayOfWeek < startOfWeek) ? 7 : 0) - startOfWeek;

        /// <summary>
        /// Returns a single-element sublist containing the element of <paramref name="dates"/>
        /// at <paramref name="offset"/>. Valid offsets are from 1 to the size of the list.
        /// If an invalid offset is supplied, all elements from <paramref name="dates"/>
        /// are returned (offset == 0 case).
        /// </summary>
        private static IEnumerable<DateTimeOffset> GetOffsetDates(
            IEnumerable<DateTimeOffset> dates, int offset)
        {
            switch (offset)
            {
                case 0:
                    return dates;

                case < 0:
                    {
                        var list = dates as IList<DateTimeOffset> ?? [.. dates];
                        var index = list.Count + offset;
                        return index >= 0 && index < list.Count ? [list[index]] : [];
                    }

                default:
                    return dates.Skip(offset - 1).Take(1);
            }
        }

        private static void IncrementDate(ref DateTimeOffset dt, CalendarRecurrenceRule pattern, int interval, string? timeZone)
        {
            if (interval == 0)
                return;

            try
            {
                var old = dt;
                switch (pattern.Frequency)
                {
                    case FrequencyType.Secondly: dt = old.AddSeconds(interval); break;
                    case FrequencyType.Minutely: dt = old.AddMinutes(interval); break;
                    case FrequencyType.Hourly: dt = old.AddHours(interval); break;
                    case FrequencyType.Daily: dt = old.AddDays(interval); break;
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
                        // RecurrencePatternEvaluator relies on the assumption that after
                        // incrementing, the new refDate is usually at the first day of an
                        // interval.
                        if (old.Day == 29 && old.Month == 2 && DateTime.IsLeapYear(old.Year))
                            dt = old.AddYears(4); // leap year skip to next year
                        else
                            dt = old.AddDays(-old.Day + 1).AddYears(interval);
                        break;

                    default:
                        // Frequency should always be valid at this stage.
                        Debug.Fail($"'{pattern.Frequency}' as RecurrencePattern.Frequency is not implemented.");
                        break;
                }
                if (timeZone != null)
                {
                    var newTz = pattern.Metadata.GetTimeZone(timeZone)?.GetOffsetInMinutes(dt);
                    if (newTz != null && newTz.Value != Convert.ToInt32(dt.Offset.TotalMinutes))
                    {
                        dt = new DateTimeOffset(dt.DateTime, TimeSpan.FromMinutes(newTz.Value));
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // intentionally don't include the outer exception
                throw new ArgumentException(
                    "Evaluation aborted: The maximum supported date-time was exceeded while " +
                    "enumerating a recurrence rule. This commonly happens when trying to " +
                    "enumerate an unbounded RRULE to its end. Consider applying the " +
                    ".TakeWhile() operator.");
            }
        }
    }
}