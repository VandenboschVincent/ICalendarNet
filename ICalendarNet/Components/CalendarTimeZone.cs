using ICalendarNet.Base;
using ICalendarNet.Extensions;
using System;
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
            get => Properties.GetContentlineDateTime(ICalProperty.LAST_MODIFIED);
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
    }
}