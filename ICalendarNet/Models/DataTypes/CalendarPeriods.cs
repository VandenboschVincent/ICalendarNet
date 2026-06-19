using ICalendarNet.Extensions;
using ICalendarNet.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using static ICalendarNet.Models.Enum.Statics;

namespace ICalendarNet.Models.DataTypes
{
    public class CalendarPeriods : ContentLine
    {
        public IEnumerable<CalendarPeriod> GetPeriods() => Value.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(t => new CalendarPeriod(Name, t, Parameters));

        public CalendarPeriods(ICalProperty key, string value, ContentLineParameters? param) : base(key.GetString(), value, param)
        { }

        public CalendarPeriods(string key, string value, ContentLineParameters? param) : base(key, value, param)
        { }

        public CalendarPeriods(ICalProperty key, IEnumerable<CalendarPeriod> calendarPeriods) : base(key.GetString(), string.Join(',', calendarPeriods.Select(t => t.Value)), null)
        { }
    }
}