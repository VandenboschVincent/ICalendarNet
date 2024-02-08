using ICalendarNet.Extensions;
using System.Collections.Concurrent;

namespace ICalendarNet.Base
{
    public abstract class CalendarObject : ICalendarComponent
    {
        public abstract ICalComponent ComponentType { get; }
        ConcurrentQueue<ICalendarProperty> ICalendarComponent.contentLines { get; set; } = [];
        ConcurrentBag<ICalendarComponent> ICalendarComponent.components { get; set; } = [];

        public List<ICalendarProperty> Properties { get; } = [];
        public List<ICalendarComponent> SubComponents { get; } = [];

        public void AddProperty(string key, string value)
        {
            Properties.UpdateLineProperty(value, key);
        }

        public void UpdateProperty(string key, string value)
        {
            Properties.UpdateLineProperty(value, key);
        }

        public void UpdateProperty(string key, IEnumerable<string> value)
        {
            Properties.UpdateLinesProperty(value.ToList(), key);
        }
    }
}