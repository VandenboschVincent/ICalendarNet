using ICalendarNet.Base;
using ICalendarNet.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ICalendarNet.Statics;

namespace ICalendarNet.DataTypes
{
    public class CalendarPeriod : ContentLine
    {
        public DateTimeOffset DateStart
        {
            get 
            {
                return TypeConverters.ConvertToDateTimeOffset(Value.Split('/').FirstOrDefault()) ??
                    throw new ArgumentException($"Could not parse {Value} to period"); 
            }
            set 
            { 
                Value = TypeConverters.ConvertFromDateTimeOffset(value) + "/" + Value.Split('/')[^1];
            }
        }

        public DateTimeOffset DateEnd
        {
            get
            {
                string endValue = Value.Split('/').LastOrDefault()
                    ?? throw new ArgumentException($"Could not parse {Value} to period");
                if (endValue.StartsWith('P'))
                    return DateStart.Add(TypeConverters.ConvertToTimeSpan(endValue) ?? throw new ArgumentException($"Could not parse {Value} to period"));
                return TypeConverters.ConvertToDateTimeOffset(endValue) ?? throw new ArgumentException($"Could not parse {Value} to period");
            }
            set
            {
                Value = Value.Split('/')[0] + "/" + TypeConverters.ConvertFromDateTimeOffset(value);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                string endValue = Value.Split('/').LastOrDefault()
                    ?? throw new ArgumentException($"Could not parse {Value} to period");
                if (endValue.StartsWith('P'))
                    return TypeConverters.ConvertToTimeSpan(endValue) ?? throw new ArgumentException($"Could not parse {Value} to period");
                return (TypeConverters.ConvertToDateTimeOffset(endValue) ?? throw new ArgumentException($"Could not parse {Value} to period"))
                    .Subtract(DateStart);

            }
            set
            {
                Value = Value.Split('/')[0] + "/" + TypeConverters.ConvertFromTimeSpan(value);
            }
        }


        public CalendarPeriod(ICalProperty key, string value, ContentLineParameters? param) : base(ICalProperties[(int)key], value, param)
        { }

        public CalendarPeriod(string key, string value, ContentLineParameters? param) : base(key, value, param)
        { }

        public CalendarPeriod(ICalProperty key, DateTimeOffset dateStart, DateTimeOffset dateEnd) : base(ICalProperties[(int)key], string.Empty, null)
        {
            Value = TypeConverters.ConvertFromDateTimeOffset(dateStart) + "/" + TypeConverters.ConvertFromDateTimeOffset(dateEnd);
        }
        public CalendarPeriod(ICalProperty key, DateTimeOffset dateStart, TimeSpan duration) : base(ICalProperties[(int)key], string.Empty, null)
        {
            Value = TypeConverters.ConvertFromDateTimeOffset(dateStart) + "/" + TypeConverters.ConvertFromTimeSpan(duration);
        }
    }

}
