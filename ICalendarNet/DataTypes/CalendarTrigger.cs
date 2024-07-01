using ICalendarNet.Base;
using ICalendarNet.Converters;
using ICalendarNet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ICalendarNet.Statics;

namespace ICalendarNet.DataTypes
{
    public class CalendarTrigger : ContentLine
    {
        public TimeSpan? TimeValue { 
            get { 
                return TypeConverters.ConvertToTimeSpan(Value); 
            } 
            set 
            { 
                if (value == null) return;
                Value = TypeConverters.ConvertFromTimeSpan(value.Value);
            }
        }
        public DateTimeOffset? DateValue { 
            get { return TypeConverters.ConvertToDateTimeOffset(Value); }
            set
            {
                if (value == null) return;
                Value = TypeConverters.ConvertFromDateTimeOffset(value.Value);
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
                new KeyValuePair<string, IEnumerable<string>>("VALUE", new List<string>() { "DATE_TIME" })
            }.ToDictionary();
        }

        public CalendarTrigger(TimeSpan timeSpan, TriggerStartEnd triggerStartEnd = TriggerStartEnd.START)
            : base(ICalProperties[(int)ICalProperty.TRIGGER],
                  TypeConverters.ConvertFromTimeSpan(timeSpan),
                  null)
        {
            Parameters = new List<KeyValuePair<string, IEnumerable<string>>>()
            {
                new KeyValuePair<string, IEnumerable<string>>("RELATED", new List<string>() { triggerStartEnd.ToString() })
            }.ToDictionary();
        }
    }
    public enum TriggerStartEnd
    {
        START,
        END,
    }
}
