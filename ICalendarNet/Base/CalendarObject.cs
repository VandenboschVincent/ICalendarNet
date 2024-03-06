using ICalendarNet.Extensions;
using System.Runtime.InteropServices;

namespace ICalendarNet.Base
{
    public abstract class CalendarObject : ICalendarComponent, IDisposable
    {
        public abstract ICalComponent ComponentType { get; }
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {

        }
    }
}