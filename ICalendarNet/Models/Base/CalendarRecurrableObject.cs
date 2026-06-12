using ICalendarNet.Extensions;
using ICalendarNet.Logic.Recurrence;
using ICalendarNet.Models.DataTypes;
using ICalendarNet.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using static ICalendarNet.Models.Enum.Statics;

namespace ICalendarNet.Models.Base
{
    public abstract class CalendarRecurrableObject : CalendarOccurableObject
    {
        private static readonly List<string> rruleProperties = [nameof(ICalProperty.EXDATE)
            , nameof(ICalProperty.RRULE)
            , nameof(ICalProperty.EXRULE)
            , nameof(ICalProperty.RDATE)];

        /// <summary>
        ///   <see cref="ICalProperty.EXDATE" />
        /// </summary>
        public virtual IEnumerable<DateTimeOffset>? ExceptionDateTimes
        {
            get => Properties.GetContentlineDateTimes(ICalProperty.EXDATE, Metadata);
            set => Properties.UpdateLineProperty(value, ICalProperty.EXDATE);
        }

        /// <summary>
        ///   <see cref="ICalProperty.RECURRENCE_ID" />
        /// </summary>
        public virtual DateTimeOffset? RecurrenceID
        {
            get => Properties.GetContentlineDateTime(ICalProperty.RECURRENCE_ID);
            set => Properties.UpdateLineProperty(value, ICalProperty.RECURRENCE_ID);
        }

        public virtual bool OverwritesRecurrence()
        {
            return Properties.GetContentlines(ICalProperty.RECURRENCE_ID).FirstOrDefault()?.Parameters.GetValue("RANGE") ==
                "THISANDFUTURE";
        }

        /// <summary>
        ///   <see cref="ICalProperty.SEQUENCE" />
        /// </summary>
        public int? Sequence
        {
            get => Properties.GetContentlineInt(ICalProperty.SEQUENCE);
            set => Properties.UpdateLineProperty(value, ICalProperty.SEQUENCE);
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

        /// <summary>
        ///   <see cref="ICalProperty.RDATE" />
        /// </summary>
        public IEnumerable<CalendarPeriod>? RecurrenceDates
        {
            get => Properties.GetContentlines(ICalProperty.RDATE).Cast<CalendarPeriods>()
                .SelectMany(t => t.GetPeriods());
            set => Properties.UpdateLineProperty(value, ICalProperty.RDATE);
        }

        public IEnumerable<DateTimeOffset>? GetRecurrence(int amount = 1, DateTimeOffset? start = null, bool addStartDay = true, DateTimeOffset? end = null)
        {
            var rrule = GetRecurrenceRule();
            var dtstart = DateTimeStart;
            var tzstart = Properties.GetContentlines(ICalProperty.DTSTART)
                .FirstOrDefault()?.Parameters.GetValue(nameof(ICalProperty.TZID));
            if (rrule is null || dtstart is null)
                return null;
            if (rrule.Until < start)
                return null;
            var exdates = ExceptionDateTimes;
            return RecurrenceRuleEvaluator.GetRecurrenceDates(rrule, dtstart.Value, amount, start, addStartDay, end, exceptionDates: exdates, timeZone: tzstart) ??
                RecurrenceDates?
                    .Where(t => (start == null || t.DateStart >= start) && (end == null || t.DateStart <= end)).Select(t => t.DateStart)
                    .OrderBy(t => t)
                    .Take(amount);
        }

        public IEnumerable<CalendarRecurrableObject> GetOccuring(int amount = 1, DateTimeOffset? start = null, bool addStartDay = true, DateTimeOffset? end = null)
        {
            var recurrences = GetRecurrence(amount, start, addStartDay, end);
            return recurrences?.Select(Clone) ?? [];
        }

        protected abstract CalendarRecurrableObject Clone(DateTimeOffset occurence);

        protected static T CloneComponent<T>(T obj, DateTimeOffset occurence) where T : CalendarRecurrableObject, new()
        {
            var serialized = CalSerializor.SerializeICalObject(obj);
            var clone = CalSerializor.DeserializeICalComponent<T>(serialized, obj.Metadata.GetTimeZones()) ?? throw new InvalidOperationException("cloning object failed");
            clone.DateTimeStart = occurence;
            clone.Properties.RemoveAll(t => rruleProperties.Contains(t.Name));
            return clone;
        }
    }
}