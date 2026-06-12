using ICalendarNet.Logic;
using ICalendarNet.Models.Base;
using System;
using static ICalendarNet.Models.Enum.Statics;

namespace ICalendarNet.Models.DataTypes
{
    public class CalendarTrigger : ContentLine
    {
        public TimeSpan? TimeValue
        {
            get
            {
                return TypeConverters.ConvertToTimeSpan(Value);
            }
            set
            {
                if (value == null) return;
                Value = TypeConverters.ConvertFromTimeSpan(value.Value);
            }
        }

        public DateTimeOffset? DateValue
        {
            get { return TypeConverters.ConvertToDateTimeOffset(Value, GetTimeZone()); }
            set
            {
                if (value == null) return;
                Value = TypeConverters.ConvertFromDateTimeOffset(value.Value, GetTimeZone());
            }
        }

        public CalendarTrigger(string name, string value, ContentLineParameters? parameter) : base(name, value, parameter)
        {
        }

        public CalendarTrigger(DateTimeOffset dateTime)
            : base(nameof(ICalProperty.TRIGGER),
                  TypeConverters.ConvertFromDateTimeOffset(dateTime),
                  null)
        {
            Parameters = new ContentLineParameters(
            [
                new("VALUE", ["DATE_TIME"])
            ]);
        }

        public CalendarTrigger(TimeSpan timeSpan, TriggerStartEnd triggerStartEnd = TriggerStartEnd.START)
            : base(nameof(ICalProperty.TRIGGER),
                  TypeConverters.ConvertFromTimeSpan(timeSpan),
                  null)
        {
            Parameters = new ContentLineParameters(
            [
                new("RELATED", [triggerStartEnd.ToString()])
            ]);
        }
    }

    public enum TriggerStartEnd
    {
        START,
        END,
    }
}