using ICalendarNet.Base;
using ICalendarNet.DataTypes;
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
            foreach (var standard in components)
            {
                var start = standard.DTSTART;
                var timezoneOffsetTo = standard.TimezoneOffsetTo;
                if (start is null || start > dateTime || timezoneOffsetTo is null)
                    continue;
                if (timezoneOffsetTo.Length < 4 || timezoneOffsetTo.Length > 5)
                    continue;
                if (standard.GetRecurrenceRule()?.Until is DateTimeOffset until && until < dateTime)
                    continue;
                DateTimeOffset startCalculalte = dateTime.Value.AddYears(-1);
                startCalculalte = start.Value > startCalculalte ? start.Value : startCalculalte;
                var foundDates = standard.GetRecurrenceDates(10, startCalculalte, false, dateTime);
                if (foundDates is not null)
                    periods.AddRange(foundDates.Select(t => new DateTimeAndOffset(timezoneOffsetTo, t.DateStart)));
                else
                {
                    var savedProperties = standard.RecurrenceDates?
                        .Where(t => t.DateEnd is null || t.DateEnd >= dateTime);
                    if (savedProperties is not null && savedProperties.Any())
                        periods.AddRange(savedProperties.Select(t => new DateTimeAndOffset(timezoneOffsetTo, t.DateStart)));
                    else if (components.Count() == 1)
                        periods.Add(new DateTimeAndOffset(timezoneOffsetTo, start.Value));
                }
            }

            var currentPeriod = periods.OrderByDescending(t => t.date).FirstOrDefault(t => t.date <= dateTime);
            return currentPeriod is null ? 0 : ConvertToOffsetInMinutes(currentPeriod.offset);
        }

        private int ConvertToOffsetInMinutes(ReadOnlySpan<char> offset)
        {
            var sign = offset.StartsWith("-") ? -1 : 1;
            var hourParts = offset.TrimStart("+-").Slice(0, 2);
            var minuteParts = offset.TrimStart("+-").Slice(2, 2);
            if (int.TryParse(hourParts, out int hours) && int.TryParse(minuteParts, out int minutes))
                return sign * (hours * 60 + minutes);
            return 0;
        }

        private class DateTimeAndOffset
        {
            public DateTimeAndOffset(string offset, DateTimeOffset date)
            {
                this.offset = offset;
                this.date = date;
            }

            public string offset { get; set; }
            public DateTimeOffset date { get; set; }
        }
    }
}