using ICalendarNet.Extensions;
using ICalendarNet.Logic.TimeZone;
using ICalendarNet.Models.Base;
using ICalendarNet.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using static ICalendarNet.Models.Enum.Statics;

namespace ICalendarNet.Models.Components
{
    /// <summary>
    /// https://datatracker.ietf.org/doc/html/rfc5545#section-3.6.5
    /// </summary>
    public class CalendarTimeZone : CalendarObject
    {
        public override ICalComponent ComponentType => ICalComponent.VTIMEZONE;
        private TimeZoneInfo? cachedTimeZone = null;

        /// <summary>
        ///   <see cref="ICalProperty.LAST_MODIFIED" />
        /// </summary>
        public DateTimeOffset? LastModified
        {
            get => Properties.GetContentlineDateTime(ICalProperty.LAST_MODIFIED, Metadata);
            set => Properties.UpdateLineProperty(value, ICalProperty.LAST_MODIFIED);
        }

        /// <summary>
        ///   <see cref="ICalProperty.TZID" />
        /// </summary>
        public virtual string? TimeZoneId
        {
            get => Properties.GetContentlineValue(ICalProperty.TZID);
            set => Properties.UpdateLineProperty(value, ICalProperty.TZID);
        }

        /// <summary>
        ///   <see cref="ICalProperty.TZURL" />
        /// </summary>
        public virtual string? TimeZoneUrl
        {
            get => Properties.GetContentlineValue(ICalProperty.TZURL);
            set => Properties.UpdateLineProperty(value, ICalProperty.TZURL);
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
            DateTime dt = dateTime?.DateTime ?? DateTime.Now;
            cachedTimeZone ??= TimeZoneInfoHelper.GetTimeZone(this);
            if (cachedTimeZone == null) return 0;
            if (cachedTimeZone.IsInvalidTime(dt))
            {
                // Get the DST delta for this zone (typically +1:00)
                var adjustment = cachedTimeZone.GetAdjustmentRules()
                    .FirstOrDefault(r => r.DateStart <= dt && dt <= r.DateEnd);
                dt = dt + (adjustment?.DaylightDelta ?? cachedTimeZone.BaseUtcOffset);
            }
            var offset = Convert.ToInt32(cachedTimeZone.GetUtcOffset(dt).TotalMinutes);
            return offset;
        }
    }
}