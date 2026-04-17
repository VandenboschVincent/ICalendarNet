using ICalendarNet.Base;
using ICalendarNet.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using static ICalendarNet.Statics;

namespace ICalendarNet.DataTypes
{
    public class CalendarPeriod : ContentLine
    {
        private IEnumerable<string> ValueParts => Value.Split('/').Select(t => t.Trim());

        public DateTimeOffset DateStart
        {
            get
            {
                return ICalTypeConverters.ConvertToDateTimeOffset(Value.Split('/').FirstOrDefault()) ??
                    throw new ArgumentException($"Could not parse {Value} to period");
            }
            set
            {
                Value = ICalTypeConverters.ConvertFromDateTimeOffset(value) + "/" + Value.Split('/')[^1];
            }
        }

        public DateTimeOffset DateEnd
        {
            get
            {
                string endValue = ValueParts.LastOrDefault()
                    ?? throw new ArgumentException($"Could not parse {Value} to period");
                if (endValue.StartsWith('P'))
                    return DateStart.Add(ICalTypeConverters.ConvertToTimeSpan(endValue) ?? throw new ArgumentException($"Could not parse {Value} to period"));
                return ICalTypeConverters.ConvertToDateTimeOffset(endValue) ?? throw new ArgumentException($"Could not parse {Value} to period");
            }
            set
            {
                Value = ValueParts.First() + "/" + ICalTypeConverters.ConvertFromDateTimeOffset(value);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                string endValue = ValueParts.LastOrDefault()
                    ?? throw new ArgumentException($"Could not parse {Value} to period");
                if (endValue.StartsWith('P'))
                    return ICalTypeConverters.ConvertToTimeSpan(endValue) ?? throw new ArgumentException($"Could not parse {Value} to period");
                return (ICalTypeConverters.ConvertToDateTimeOffset(endValue) ?? throw new ArgumentException($"Could not parse {Value} to period"))
                    .Subtract(DateStart);
            }
            set
            {
                Value = ValueParts.First() + "/" + ICalTypeConverters.ConvertFromTimeSpan(value);
            }
        }

        public CalendarPeriod(ICalProperty key, string value, ContentLineParameters? param) : base(ICalProperties[(int)key], value, param)
        { }

        public CalendarPeriod(string key, string value, ContentLineParameters? param) : base(key, value, param)
        { }

        public CalendarPeriod(ICalProperty key, DateTimeOffset dateStart, DateTimeOffset dateEnd) : base(ICalProperties[(int)key], string.Empty, null)
        {
            Value = ICalTypeConverters.ConvertFromDateTimeOffset(dateStart) + "/" + ICalTypeConverters.ConvertFromDateTimeOffset(dateEnd);
        }

        public CalendarPeriod(ICalProperty key, DateTimeOffset dateStart, TimeSpan duration) : base(ICalProperties[(int)key], string.Empty, null)
        {
            Value = ICalTypeConverters.ConvertFromDateTimeOffset(dateStart) + "/" + ICalTypeConverters.ConvertFromTimeSpan(duration);
        }
    }
}