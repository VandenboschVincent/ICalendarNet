using System;

namespace ICalendarNet.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool HasTime(this DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.TimeOfDay != TimeSpan.Zero;
        }

        public static DateTimeOffset AddWeeks(this DateTimeOffset dt, int interval, DayOfWeek firstDayOfWeek)
        {
            dt = dt.AddDays(interval * 7);
            while (dt.DayOfWeek != firstDayOfWeek)
            {
                dt = dt.AddDays(-1);
            }

            return dt;
        }
    }
}
