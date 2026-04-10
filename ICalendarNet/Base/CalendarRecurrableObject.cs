using ICalendarNet.DataTypes;
using ICalendarNet.Extensions;
using ICalendarNet.Models;
using System;
using System.Collections.Generic;
using static ICalendarNet.Statics;

namespace ICalendarNet.Base
{
    public abstract class CalendarRecurrableObject : CalendarObject
    {
        /// <summary>
        ///   <see cref="ICalProperty.DTSTART" />
        /// </summary>
        public DateTimeOffset? DTSTART
        {
            get => Properties.GetContentlineDateTime(ICalProperty.DTSTART);
            set => Properties.UpdateLineProperty(value!, ICalProperty.DTSTART);
        }

        /// <summary>
        ///   <see cref="ICalProperty.EXDATE" />
        /// </summary>
        public virtual IEnumerable<DateTimeOffset>? ExceptionDateTimes
        {
            get => Properties.GetContentlineDateTimes(ICalProperty.EXDATE);
            set => Properties.UpdateLineProperty(value!, ICalProperty.EXDATE);
        }

        /// <summary>
        ///   <see cref="ICalProperty.RRULE" />
        /// </summary>
        public string? RRULE
        {
            get => Properties.GetContentlineValue(ICalProperty.RRULE);
            set => Properties.UpdateLineProperty(value!, ICalProperty.RRULE);
        }


        [Obsolete("EXRULE is marked as deprecated in RFC 5545 and will be removed in a future version")]
        IList<string> ExceptionRules { get; set; }

        //TODO
        //public IEnumerable<CalendarOccurrence> GetRecurrenceDates(int skip = 0, int amount = 1)
        //{

        //}
    }
}
