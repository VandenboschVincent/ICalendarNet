using ICalendarNet.Components;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Models
{
    public class MetadataContainer
    {
        private readonly List<CalendarTimeZone> _TimeZonedata = [];

        public void SetTimeZone(CalendarTimeZone value)
        {
            _TimeZonedata.Add(value);
        }

        public void SetTimeZones(List<CalendarTimeZone> values)
        {
            _TimeZonedata.AddRange(values);
        }

        public CalendarTimeZone? GetTimeZone(string key)
        {
            return _TimeZonedata.Find(t => t.TimeZoneId == key);
        }

        public IEnumerable<string?> GetTimeZones()
        {             
            return _TimeZonedata.Select(t => t.TimeZoneId);
        }
    }
}
