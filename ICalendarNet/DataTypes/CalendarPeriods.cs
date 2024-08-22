using ICalendarNet.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ICalendarNet.Statics;

namespace ICalendarNet.DataTypes
{
    public class CalendarPeriods : ContentLine
    {
        public IEnumerable<CalendarPeriod> GetPeriods() => Value.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(t => new CalendarPeriod(Name, t, Parameters));

        public CalendarPeriods(ICalProperty key, string value, ContentLineParameters? param) : base(ICalProperties[(int)key], value, param)
        { }

        public CalendarPeriods(string key, string value, ContentLineParameters? param) : base(key, value, param)
        { }

        public CalendarPeriods(ICalProperty key, IEnumerable<CalendarPeriod> calendarPeriods) : base(ICalProperties[(int)key], string.Join(',', calendarPeriods.Select(t => t.Value)), null)
        { }

    }
}
