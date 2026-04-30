using ICalendarNet.Components;
using System.Collections.Generic;

namespace ICalendarNet.Models
{
    public class MetadataContainer
    {
        private readonly Dictionary<string, CalendarTimeZone> _TimeZonedata = [];

        public void SetTimeZone(string? key, CalendarTimeZone value)
        {
            if (key == null) return;
            _TimeZonedata[key] = value;
        }

        public CalendarTimeZone? GetTimeZone(string key)
        {
            return _TimeZonedata.GetValueOrDefault(key);
        }
    }
}
