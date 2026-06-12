using ICalendarNet.Extensions;
using ICalendarNet.Models.Components;
using ICalendarNet.Models.DataTypes;
using ICalendarNet.Models.DataTypes.Recurrence;
using ICalendarNet.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Logic.TimeZone
{
    internal static class TimeZoneInfoHelper
    {
        private const int MinTimezoneOffsetLength = 4;
        private const int MaxTimezoneOffsetLength = 5;

        public static TimeZoneInfo? GetTimeZone(CalendarTimeZone timeZone)
        {
            var id = timeZone.TimeZoneId;
            if (string.IsNullOrWhiteSpace(id))
                return TimeZoneInfo.Local;

            if (CalendarExtensions.TryFindTimeZone(id, out TimeZoneInfo? tz))
                return tz;

            var components = timeZone.SubComponents
                .Where(t => t.ComponentType is ICalComponent.STANDARD or ICalComponent.DAYLIGHT)
                .Cast<CalendarStandard>()
                .ToList();
            var standard = components.FirstOrDefault(c => c.ComponentType == ICalComponent.STANDARD);
            var daylight = components.Where(c => c.ComponentType == ICalComponent.DAYLIGHT).ToList();
            var baseOffset = GetOffsetMinutes(standard?.TimezoneOffsetTo);
            var standardTime = TimeSpan.FromMinutes(baseOffset);
            var standardRule = standard?.GetRecurrenceRule();
            tz = TimeZoneInfo.CreateCustomTimeZone(
                id: id,
                baseUtcOffset: standardTime,
                displayName: id,
                standardDisplayName: "EST",
                daylightDisplayName: "EDT",
                adjustmentRules: [.. GetRulesForRrule(daylight, standardTime, standardRule, standard?.DateTimeStart)]);
            return tz;
        }

        private static IEnumerable<TimeZoneInfo.AdjustmentRule> GetRulesForRrule(
            List<CalendarStandard> daylights,
            TimeSpan standardUtcOffset,
            CalendarRecurrenceRule? standardRule,
            DateTimeOffset? standardDateTime)
        {
            if (daylights == null || daylights.Count == 0) yield break;

            // Get Standard Transition Details (End of Daylight Saving Time)
            var standardMonth = standardRule?.ByMonth.FirstOrDefault();
            var standardDay = standardRule?.ByDay.FirstOrDefault() ?? new WeekDay(DayOfWeek.Sunday);
            DateTime standardTimeOfDay = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)
                .Add(standardDateTime?.TimeOfDay ?? TimeSpan.FromHours(2));

            var standardWeek = GetWeekOfMonth(standardDay, standardMonth);
            foreach (var daylight in daylights.OrderBy(t => t.DateTimeStart))
            {
                var daylightOffset = GetOffsetMinutes(daylight.TimezoneOffsetTo);
                var daylightRule = daylight.GetRecurrenceRule();
                var daylightDelta = TimeSpan.FromMinutes(daylightOffset) - standardUtcOffset;

                DateTime daylightTimeOfDay = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)
                    .Add(daylight.DateTimeStart?.TimeOfDay ?? TimeSpan.FromHours(2));

                if (daylightRule != null
                    && standardWeek.HasValue
                    && standardMonth.HasValue
                    && daylightRule.ByMonth.FirstOrDefault() is int dayLightMonth
                    && daylightRule.ByDay.FirstOrDefault() is WeekDay dayLightDay
                    && GetWeekOfMonth(dayLightDay, dayLightMonth) is int dayLightWeek)
                {
                    // --- RRULE IMPLEMENTATION (Floating Rules) ---
                    yield return TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(
                        dateStart: daylight.DateTimeStart?.DateTime.Date ?? DateTime.MinValue.Date,
                        dateEnd: daylightRule.Until?.Date ?? DateTime.MaxValue.Date,
                        daylightDelta: daylightDelta,
                        daylightTransitionStart: TimeZoneInfo.TransitionTime.CreateFloatingDateRule(
                            timeOfDay: daylightTimeOfDay,
                            month: dayLightMonth,
                            week: dayLightWeek,
                            dayOfWeek: dayLightDay.DayOfWeek),
                        daylightTransitionEnd: TimeZoneInfo.TransitionTime.CreateFloatingDateRule(
                            timeOfDay: standardTimeOfDay,
                            month: standardMonth.Value,
                            week: standardWeek.Value,
                            dayOfWeek: standardDay.DayOfWeek));
                }
                else if (daylight.RecurrenceDates != null && daylight.RecurrenceDates.Any())
                {
                    // --- RDATE IMPLEMENTATION (Fixed Date Rules) ---
                    var rdates = daylight.RecurrenceDates.OrderBy(t => t.DateStart).ToList();

                    for (int i = 0; i < rdates.Count; i++)
                    {
                        var rdate = rdates[i];

                        // Determine the end date of this specific rule adjustment block
                        DateTime? nexBlockStart = rdates.ElementAtOrDefault(i + 1)?.DateStart.Date;
                        DateTime ruleEndDate = rdate.DateEnd?.Date
                                               ?? nexBlockStart
                                               ?? daylightRule?.Until?.DateTime.Date
                                               ?? new DateTimeOffset(9999, 1, 1, 0, 0, 0, TimeSpan.Zero).Date;
                        if (nexBlockStart != null && ruleEndDate <= nexBlockStart)
                            ruleEndDate = nexBlockStart.Value.AddDays(-1);

                        yield return TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(
                            dateStart: rdate.DateStart.Date,
                            dateEnd: ruleEndDate,
                            daylightDelta: daylightDelta,
                            daylightTransitionStart: TimeZoneInfo.TransitionTime.CreateFixedDateRule(
                                timeOfDay: daylightTimeOfDay,
                                month: rdate.DateStart.Month,
                                day: rdate.DateStart.Day),
                            daylightTransitionEnd: TimeZoneInfo.TransitionTime.CreateFixedDateRule(
                                timeOfDay: standardTimeOfDay,
                                month: ruleEndDate.Month,
                                day: ruleEndDate.Day));
                    }
                }
            }
        }

        private static int? GetWeekOfMonth(WeekDay? date, int? month)
        {
            if (date == null || month is null) return null;
            if (date.Offset > 0) return date.Offset;
            int weeksInMonth = (int)Math.Ceiling(DateTime.DaysInMonth(2000, month.Value) / 7.0);
            return weeksInMonth + date.Offset + 1;
        }

        private static int GetOffsetMinutes(string? timezoneOffsetTo)
        {
            int offsetMinutes = 0;
            if (timezoneOffsetTo is null ||
                timezoneOffsetTo.Length is < MinTimezoneOffsetLength or > MaxTimezoneOffsetLength)
                return offsetMinutes;
            offsetMinutes = ConvertToOffsetInMinutes(timezoneOffsetTo);
            return offsetMinutes;
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
    }
}
