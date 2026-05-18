using ICalendarNet.Extensions;
using System;
using static ICalendarNet.Statics;

namespace ICalendarNet.Base
{
    public abstract class CalendarOccurableObject : CalendarObject
    {
        /// <summary>
        ///   <see cref="ICalProperty.DTSTART" />
        /// </summary>
        public virtual DateTimeOffset? DTSTART
        {
            get => Properties.GetContentlineDateTime(ICalProperty.DTSTART, Metadata);
            set => Properties.UpdateLineProperty(value!, ICalProperty.DTSTART);
        }
    }
}
