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
                .Cast<CalendarStandard>()
                .ToList();

            var periods = CollectPeriods(components, target, assumedOffset);

            var currentPeriod = periods
                .Where(p => ConvertToOffset(p.Date, assumedOffset) <= target)
#if NET6_0_OR_GREATER
                .MaxBy(p => p.Date);
#else
                .OrderByDescending(t => t.Date).FirstOrDefault();
#endif

            return currentPeriod?.Offset ?? 0;
        }

        private static List<DateTimeAndOffset> CollectPeriods(
            List<CalendarStandard> components,
            DateTimeOffset target,
            double assumedOffset)
        {
            var periods = new List<DateTimeAndOffset>();
            var isSingleComponent = components.Count == 1;

            foreach (var standard in components)
            {
                if (!TryGetOffsetMinutes(standard.TimezoneOffsetTo, out var offset))
                    continue;

                var start = ConvertToOffset(standard.DateTimeStart, assumedOffset);
                if (start is null || start > target)
                    continue;

                AddRecurrenceRulePeriods(periods, standard, start.Value, offset, target, assumedOffset);
                AddRecurrenceDatePeriods(periods, standard, start.Value, offset, target, assumedOffset, isSingleComponent);
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

        private static void AddRecurrenceRulePeriods(
            List<DateTimeAndOffset> periods,
            CalendarStandard standard,
            DateTimeOffset start,
            int offset,
            DateTimeOffset target,
            double assumedOffset)
        {
            var rrule = standard.GetRecurrenceRule();
            if (rrule is null)
                return;

            var until = ConvertToOffset(rrule.Until, assumedOffset);
            if (until is not null && until < target)
                return;

            var exDates = standard.ExceptionDateTimes?
                .Select(t => ConvertToOffset(t, assumedOffset));

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
                return;

            periods.AddRange(foundDates.Select(d => new DateTimeAndOffset(offset, d.DateStart)));
        }

        private static void AddRecurrenceDatePeriods(
            List<DateTimeAndOffset> periods,
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
                periods.AddRange(validDates.Select(t => new DateTimeAndOffset(offset, t.DateStart)));
            }
            else if (isSingleComponent)
            {
                // Single component with no recurrence dates is always considered active.
                periods.Add(new DateTimeAndOffset(offset, start));
            }
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
            var sign = offset.StartsWith("-") ? -1 : 1;
            var hourParts = offset.TrimStart("+-")[..2];
            var minuteParts = offset.TrimStart("+-").Slice(2, 2);
            if (int.TryParse(hourParts, out int hours) && int.TryParse(minuteParts, out int minutes))
                return sign * (hours * 60 + minutes);
            return 0;
        }

        private sealed class DateTimeAndOffset(int offset, DateTimeOffset date)
        {
            public int Offset { get; set; } = offset;
            public DateTimeOffset Date { get; set; } = date;

            public override string ToString()
            {
                return Date.ToString("o");
            }
        }
    }
}
