using ICalendarNet.Extensions;
using System;
using static ICalendarNet.Models.Enum.Statics;

namespace ICalendarNet.Models.Base
{
    public abstract class CalendarOccurableObject : CalendarObject
    {
        /// <summary>
        ///   <see cref="ICalProperty.DTSTART" />
        /// </summary>
        public virtual DateTimeOffset? DateTimeStart
        {
            get => Properties.GetContentlineDateTime(ICalProperty.DTSTART, Metadata);
            set => Properties.UpdateLineProperty(value, ICalProperty.DTSTART);
        }

        /// <summary>
        ///   <see cref="ICalProperty.UID" />
        /// </summary>
        public string? Uid
        {
            get => Properties.GetContentlineValue(ICalProperty.UID);
            set => Properties.UpdateLineProperty(value, ICalProperty.UID);
        }
    }
}