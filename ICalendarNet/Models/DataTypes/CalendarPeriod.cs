using ICalendarNet.Logic;
using ICalendarNet.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using static ICalendarNet.Models.Enum.Statics;

namespace ICalendarNet.Models.DataTypes
{
    public class CalendarPeriod : ContentLine
    {
        private IEnumerable<string> ValueParts => Value.Split('/').Select(t => t.Trim());

        public DateTimeOffset DateStart
        {
            get
            {
                return TypeConverters.ConvertToDateTimeOffset(Value.Split('/').FirstOrDefault(), GetTimeZone()) ??
                    throw new ArgumentException($"Could not parse {Value} to period");
            }
            set
            {
                Value = TypeConverters.ConvertFromDateTimeOffset(value, GetTimeZone()) + "/" + Value.Split('/')[^1];
            }
        }

        public DateTimeOffset? DateEnd
        {
            get
            {
                if (!Value.Contains('/'))
                    return null;
                string endValue = ValueParts.LastOrDefault()
                    ?? throw new ArgumentException($"Could not parse {Value} to period");
                if (endValue.StartsWith('P'))
                    return DateStart.Add(TypeConverters.ConvertToTimeSpan(endValue) ?? throw new ArgumentException($"Could not parse {Value} to period"));
                return TypeConverters.ConvertToDateTimeOffset(endValue, GetTimeZone()) ?? throw new ArgumentException($"Could not parse {Value} to period");
            }
            set
            {
                Value = ValueParts.First() + (value.HasValue ? ("/" + TypeConverters.ConvertFromDateTimeOffset(value.Value, GetTimeZone())) : string.Empty);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                string endValue = ValueParts.LastOrDefault()
                    ?? throw new ArgumentException($"Could not parse {Value} to period");
                if (endValue.StartsWith('P'))
                    return TypeConverters.ConvertToTimeSpan(endValue) ?? throw new ArgumentException($"Could not parse {Value} to period");
                return (TypeConverters.ConvertToDateTimeOffset(endValue, GetTimeZone()) ?? throw new ArgumentException($"Could not parse {Value} to period"))
                    .Subtract(DateStart);
            }
            set
            {
                Value = ValueParts.First() + "/" + TypeConverters.ConvertFromTimeSpan(value);
            }
        }

        public CalendarPeriod(ICalProperty key, string value, ContentLineParameters? param) : base(ICalProperties[(int)key], value, param)
        { }

        public CalendarPeriod(string key, string value, ContentLineParameters? param) : base(key, value, param)
        { }

        public CalendarPeriod(ICalProperty key, DateTimeOffset dateStart, DateTimeOffset dateEnd) : base(ICalProperties[(int)key], string.Empty, null)
        {
            Value = TypeConverters.ConvertFromDateTimeOffset(dateStart, GetTimeZone()) + "/" + TypeConverters.ConvertFromDateTimeOffset(dateEnd, GetTimeZone());
        }

        public CalendarPeriod(ICalProperty key, DateTimeOffset dateStart, TimeSpan duration) : base(ICalProperties[(int)key], string.Empty, null)
        {
            Value = TypeConverters.ConvertFromDateTimeOffset(dateStart, GetTimeZone()) + "/" + TypeConverters.ConvertFromTimeSpan(duration);
        }

        public override string ToString()
        {
            if (DateEnd is null)
                return DateStart.ToString("o");
            return DateStart.ToString("o") + "/" + DateEnd.Value.ToString("o");
        }
    }
}