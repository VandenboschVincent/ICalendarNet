using ICalendarNet.Extensions;
using ICalendarNet.Logic;
using ICalendarNet.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
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
            : base(ICalProperties[(int)ICalProperty.TRIGGER],
                  TypeConverters.ConvertFromDateTimeOffset(dateTime),
                  null)
        {
            Parameters = new List<KeyValuePair<string, IEnumerable<string>>>()
            {
                new("VALUE", ["DATE_TIME"])
            }.ToDictionary();
        }

        public CalendarTrigger(TimeSpan timeSpan, TriggerStartEnd triggerStartEnd = TriggerStartEnd.START)
            : base(ICalProperties[(int)ICalProperty.TRIGGER],
                  TypeConverters.ConvertFromTimeSpan(timeSpan),
                  null)
        {
            Parameters = new List<KeyValuePair<string, IEnumerable<string>>>()
            {
                new("RELATED", [triggerStartEnd.ToString()])
            }.ToDictionary();
        }
    }

    public enum TriggerStartEnd
    {
        START,
        END,
    }
}