using ICalendarNet.Logic.Recurrence;
using ICalendarNet.Models.Components;
using ICalendarNet.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Logic.TimeZone
{
    internal static class TimeZoneHelper
    {
        private const int MinTimezoneOffsetLength = 4;
        private const int MaxTimezoneOffsetLength = 5;
        private const int RecurrenceEvaluationCount = 10;

        public static int GetOffsetInMinutes(CalendarTimeZone timeZone, DateTimeOffset? dateTime = null)
        {
            var target = dateTime ?? DateTimeOffset.UtcNow;

            // Assume current offset for getting the correct time zone
            var assumedOffset = target.Offset.TotalMinutes;

            var components = timeZone.SubComponents
                .Where(t => t.ComponentType is ICalComponent.STANDARD or ICalComponent.DAYLIGHT)
                .Cast<CalendarStandard>().ToList();

            var periods = CollectPeriods(components, target, assumedOffset);
            return periods.OrderByDescending(p => p.Date)
                .FirstOrDefault(t => ConvertToOffset(t.Date, assumedOffset) <= target).Offset;
        }

        private static List<DateTimeAndOffset> CollectPeriods(
            List<CalendarStandard> components,
            DateTimeOffset target,
            double assumedOffset)
        {
            var isSingleComponent = components.Count == 1;
            List<DateTimeAndOffset> periods = [];
            foreach (var standard in components)
            {
                if (!TryGetOffsetMinutes(standard.TimezoneOffsetTo, out var offset))
                    continue;

                var start = ConvertToOffset(standard.DateTimeStart, assumedOffset);
                if (start is null || start > target)
                    continue;

                periods.AddRange(AddRecurrenceRulePeriods(standard, start.Value, offset, target, assumedOffset));
                periods.AddRange(AddRecurrenceDatePeriods(standard, start.Value, offset, target, assumedOffset, isSingleComponent));
            }
            return periods;
        }

        private static bool TryGetOffsetMinutes(string? timezoneOffsetTo, out int offsetMinutes)
        {
            offsetMinutes = 0;
            if (timezoneOffsetTo is null ||
                timezoneOffsetTo.Length is < MinTimezoneOffsetLength or > MaxTimezoneOffsetLength)
            {
                return false;
            }

            offsetMinutes = ConvertToOffsetInMinutes(timezoneOffsetTo);
            return true;
        }

        private static IEnumerable<DateTimeAndOffset> AddRecurrenceRulePeriods(
            CalendarStandard standard,
            DateTimeOffset start,
            int offset,
            DateTimeOffset target,
            double assumedOffset)
        {
            var rrule = standard.GetRecurrenceRule();
            if (rrule is null)
                return [];

            var until = ConvertToOffset(rrule.Until, assumedOffset);
            if (until is not null && until < target)
                return [];

            var exDates = standard.ExceptionDateTimes?
                .Select(t => ConvertToOffset(t, assumedOffset))
                .ToList();

            var lookbackStart = target.AddYears(-1);
            var calculationStart = start > lookbackStart ? start : lookbackStart;

            var foundDates = RecurrenceRuleEvaluator.GetRecurrenceDates(
                rrule,
                start,
                RecurrenceEvaluationCount,
                calculationStart,
                false,
                target,
                exDates);

            if (foundDates is null)
                return [];

            return foundDates.Select(d => new DateTimeAndOffset(offset, d));
        }

        private static IEnumerable<DateTimeAndOffset> AddRecurrenceDatePeriods(
            CalendarStandard standard,
            DateTimeOffset start,
            int offset,
            DateTimeOffset target,
            double assumedOffset,
            bool isSingleComponent)
        {
            var validDates = standard.RecurrenceDates?
                .Where(t => t.DateEnd is null || ConvertToOffset(t.DateEnd, assumedOffset) >= target)
                .ToList();

            if (validDates is { Count: > 0 })
            {
                return validDates.Select(t => new DateTimeAndOffset(offset, t.DateStart));
            }
            else if (isSingleComponent)
            {
                // Single component with no recurrence dates is always considered active.
                return [new DateTimeAndOffset(offset, start)];
            }
            return [];
        }

        private static DateTimeOffset? ConvertToOffset(DateTimeOffset? dateTime, double offsetInMinutes)
        {
            if (dateTime is null)
                return null;
            return ConvertToOffset(dateTime.Value, offsetInMinutes);
        }

        private static DateTimeOffset ConvertToOffset(DateTimeOffset dateTime, double offsetInMinutes)
        {
            return new DateTimeOffset(dateTime.DateTime, TimeSpan.FromMinutes(offsetInMinutes));
        }

        private static int ConvertToOffsetInMinutes(ReadOnlySpan<char> offset)
        {
            var sign = 1;
            if (offset[0] is '+' or '-')
            {
                sign = offset[0] == '-' ? -1 : 1;
                offset = offset[1..];
            }
            if (offset.Length < 4) return 0;

            if (int.TryParse(offset[..2], out int hours) &&
                int.TryParse(offset.Slice(2, 2), out int minutes))
            {
                return sign * (hours * 60 + minutes);
            }
            return 0;
        }

        private readonly record struct DateTimeAndOffset(int Offset, DateTimeOffset Date)
        {
            public override string ToString() => Date.ToString("o");
        }
    }
}
