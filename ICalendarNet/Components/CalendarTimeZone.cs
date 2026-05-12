using ICalendarNet.Base;
using ICalendarNet.DataTypes;
using ICalendarNet.DataTypes.Recurrence;
using ICalendarNet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using static ICalendarNet.Statics;

namespace ICalendarNet.Components
{
    /// <summary>
    /// https://datatracker.ietf.org/doc/html/rfc5545#section-3.6.5
    /// </summary>
    public class CalendarTimeZone : CalendarObject
    {
        public override ICalComponent ComponentType => ICalComponent.VTIMEZONE;

        /// <summary>
        ///   <see cref="ICalProperty.LAST_MODIFIED" />
        /// </summary>
        public DateTimeOffset? LastModified
        {
            get => Properties.GetContentlineDateTime(ICalProperty.LAST_MODIFIED, Metadata);
            set => Properties.UpdateLineProperty(value!, ICalProperty.LAST_MODIFIED);
        }

        /// <summary>
        ///   <see cref="ICalProperty.TZID" />
        /// </summary>
        public virtual string? TimeZoneId
        {
            get => Properties.GetContentlineValue(ICalProperty.TZID);
            set => Properties.UpdateLineProperty(value!, ICalProperty.TZID);
        }

        /// <summary>
        ///   <see cref="ICalProperty.TZURL" />
        /// </summary>
        public virtual string? TimeZoneUrl
        {
            get => Properties.GetContentlineValue(ICalProperty.TZURL);
            set => Properties.UpdateLineProperty(value!, ICalProperty.TZURL);
        }

        /// <summary>
        ///   <see cref="ICalComponent.DAYLIGHT" />
        /// </summary>
        public IEnumerable<CalendarDaylight> GetDaylights() => SubComponents.Where(t => t.ComponentType == ICalComponent.DAYLIGHT).Cast<CalendarDaylight>();

        /// <summary>
        ///   <see cref="ICalComponent.STANDARD" />
        /// </summary>
        public IEnumerable<CalendarStandard> GetStandards() => SubComponents.Where(t => t.ComponentType == ICalComponent.STANDARD).Cast<CalendarStandard>();

        public int GetOffsetInMinutes(DateTimeOffset? dateTime = null)
        {
            dateTime ??= DateTimeOffset.UtcNow;
            List<DateTimeAndOffset> periods = new();
            var components = SubComponents.Where(t => t.ComponentType == ICalComponent.STANDARD || t.ComponentType == ICalComponent.DAYLIGHT).Cast<CalendarStandard>();
            //Assume current offset for getting the correct time zone
            var assumeOffset = dateTime.Value.Offset.TotalMinutes;
            foreach (var standard in components)
            {
                var timezoneOffsetTo = standard.TimezoneOffsetTo;
                if (timezoneOffsetTo is null || timezoneOffsetTo.Length < 4 || timezoneOffsetTo.Length > 5)
                    continue;
                var start = ConvertToOffset(standard.DTSTART, assumeOffset);
                if (start is null || start > dateTime)
                    continue;
                var offset = ConvertToOffsetInMinutes(timezoneOffsetTo);
                var rrule = standard.GetRecurrenceRule();
                if (rrule is not null)
                {
                    var until = ConvertToOffset(rrule.Until, assumeOffset);
                    if (until is not null && until < dateTime)
                        continue;
                    var exDates = standard.ExceptionDateTimes?.Select(t => ConvertToOffset(t, assumeOffset));
                    DateTimeOffset startCalculalte = dateTime.Value.AddYears(-1);
                    startCalculalte = start.Value > startCalculalte ? start.Value : startCalculalte;
                    var foundDates = RecurrenceUtil.GetRecurrenceDates(rrule, start.Value, 10, startCalculalte, false, dateTime, exDates);
                    if (foundDates is not null)
                        periods.AddRange(foundDates.Select(t => new DateTimeAndOffset(offset, t.DateStart)));
                }
                var savedProperties = standard.RecurrenceDates?
                    .Where(t => t.DateEnd is null || ConvertToOffset(t.DateEnd, assumeOffset) >= dateTime);
                if (savedProperties is not null && savedProperties.Any())
                    periods.AddRange(savedProperties.Select(t => new DateTimeAndOffset(offset, t.DateStart)));
                else if (components.Count() == 1)
                    periods.Add(new DateTimeAndOffset(offset, start.Value));
            }

            var currentPeriod = periods.OrderByDescending(t => t.date).FirstOrDefault(t => ConvertToOffset(t.date, assumeOffset) <= dateTime.Value);
            return currentPeriod is null ? 0 : currentPeriod.offset;
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

        private sealed class DateTimeAndOffset
        {
            public DateTimeAndOffset(int offset, DateTimeOffset date)
            {
                this.offset = offset;
                this.date = date;
            }

            public int offset { get; set; }
            public DateTimeOffset date { get; set; }

            public override string ToString()
            {
                return date.ToString("o");
            }
        }
    }
}