using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.DataTypes.Recurrence
{
    /// <summary>
    /// Represents an RFC 5545 "BYDAY" value.
    /// </summary>
    public class WeekDay
    {
        public virtual int Offset { get; set; } = 0;

        public virtual DayOfWeek DayOfWeek { get; set; } = DayOfWeek.Sunday;

        public WeekDay(DayOfWeek day)
        {
            DayOfWeek = day;
        }

        public WeekDay(DayOfWeek day, int num) : this(day)
        {
            Offset = num;
        }

        public WeekDay(string value)
        {
            var partWithoutNumber = value;
            var number = new string(value.TakeWhile(char.IsDigit).ToArray());
            if (!string.IsNullOrEmpty(number))
            {
                var negative = value.StartsWith('-');
                partWithoutNumber = value[(negative ? 1 + number.Length : number.Length)..];
                if (int.TryParse(number, out var intValue))
                {
                    Offset = negative ? -intValue : intValue;
                }
            }
            if (CalendarRecurrenceRule.dayMap.TryGetValue(partWithoutNumber, out var dow))
                DayOfWeek = dow;
        }

        public override string ToString()
        {
            var day = CalendarRecurrenceRule.dayMap.FirstOrDefault(t => t.Value == DayOfWeek).Key;
            return Offset == 0 ? day : $"{Offset}{day}";
        }
    }
}
