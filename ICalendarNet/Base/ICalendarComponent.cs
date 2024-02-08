using System.Collections.Concurrent;

namespace ICalendarNet.Base
{
    public interface ICalendarComponent
    {
        internal ConcurrentQueue<ICalendarProperty> contentLines { get; set; }
        internal ConcurrentBag<ICalendarComponent> components { get; set; }


        ICalComponent ComponentType { get; }
        List<ICalendarProperty> Properties { get; }
        List<ICalendarComponent> SubComponents { get; }
        void AddProperty(string key, string value);
        void UpdateProperty(string key, string value);
        void UpdateProperty(string key, IEnumerable<string> value);
    }
}