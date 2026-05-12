using ICalendarNet.DataTypes;
using ICalendarNet.DataTypes.Recurrence;
using ICalendarNet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using static ICalendarNet.Statics;

namespace ICalendarNet.Base
{
    public abstract class CalendarRecurrableObject : CalendarObject
    {
        /// <summary>
        ///   <see cref="ICalProperty.DTSTART" />
        /// </summary>
        public virtual DateTimeOffset? DTSTART
        {
            get => Properties.GetContentlineDateTime(ICalProperty.DTSTART, Metadata);
            set => Properties.UpdateLineProperty(value!, ICalProperty.DTSTART);
        }

        /// <summary>
        ///   <see cref="ICalProperty.EXDATE" />
        /// </summary>
        public virtual IEnumerable<DateTimeOffset>? ExceptionDateTimes
        {
            get => Properties.GetContentlineDateTimes(ICalProperty.EXDATE, Metadata);
            set => Properties.UpdateLineProperty(value!, ICalProperty.EXDATE);
        }

        /// <summary>
        ///   <see cref="ICalProperty.RRULE" />
        /// </summary>
        public CalendarRecurrenceRule? GetRecurrenceRule()
        {
            return Properties.GetContentlines(ICalProperty.RRULE).Cast<CalendarRecurrenceRule>().FirstOrDefault();
        }

        /// <summary>
        ///   <see cref="ICalProperty.RRULE" />
        /// </summary>
        public void SetRecurrenceRule(CalendarRecurrenceRule rrule)
        {
            Properties.UpdateLineProperty([rrule], ICalProperty.RRULE);
        }


        [Obsolete("EXRULE is marked as deprecated in RFC 5545 and will be removed in a future version")]
        public IEnumerable<string>? ExceptionRules
        {
            get => Properties.GetContentlinesValue(ICalProperty.EXRULE);
            set => Properties.UpdateLinesProperty(value!, ICalProperty.EXRULE);
        }

        /// <summary>
        ///   <see cref="ICalProperty.RDATE" />
        /// </summary>
        public IEnumerable<CalendarPeriod>? RecurrenceDates
        {
            get => Properties.GetContentlines(ICalProperty.RDATE).Cast<CalendarPeriods>()
                .SelectMany(t => t.GetPeriods());
            set => Properties.UpdateLineProperty(value!, ICalProperty.RDATE);
        }

        public IEnumerable<CalendarPeriod>? GetRecurrence(int amount = 1, DateTimeOffset? start = null, bool addStartDay = true, DateTimeOffset? end = null)
        {
            var rrule = GetRecurrenceRule();
            var dtstart = DTSTART;
            if (rrule is null || dtstart is null)
                return null;
            var exdates = ExceptionDateTimes;
            return RecurrenceUtil.GetRecurrenceDates(rrule, dtstart.Value, amount, start, addStartDay, end, exceptionDates: exdates) ?? 
                Properties.GetContentlines(ICalProperty.RDATE).Cast<CalendarPeriods>()
                .SelectMany(t => t.GetPeriods());
        }
    }
}
