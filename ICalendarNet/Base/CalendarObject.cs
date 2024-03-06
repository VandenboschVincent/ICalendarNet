
using ICalendarNet.Extensions;
using static ICalendarNet.Statics;
using System.Runtime.InteropServices;

namespace ICalendarNet.Base
{
    public abstract class CalendarObject : ICalendarComponent, IDisposable
    {
        public abstract ICalComponent ComponentType { get; }
        public List<ICalendarProperty> Properties { get; } = [];
        public List<ICalendarComponent> SubComponents { get; } = [];

        public void AddProperty(ICalProperty key, string value, ContentLineParameters? parameters = null)
        {
            Properties.UpdateLineProperty(value!, key, parameters);
        }

        public void UpdateProperty(ICalProperty key, IEnumerable<string> value, ContentLineParameters? parameters = null)
        {
            Properties.UpdateLinesProperty(value!, key, parameters);
        }

        protected virtual void Dispose(bool disposing)
        {
            Properties.Clear();
            SubComponents.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}