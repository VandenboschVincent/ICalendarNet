using ICalendarNet.Base;
using ICalendarNet.Components;
using System.Runtime.InteropServices;

namespace ICalendarNet.Serialization
{
    public partial class ICalSerializor : IDisposable
    {
        private StringHandler? handler;

        public Calendar? DeserializeCalendar(string source)
        {
            return DeserializeICalComponent<Calendar>(source);
        }
        public IEnumerable<Calendar> DeserializeCalendars(string source)
        {
            return DeserializeICalComponents<Calendar>(source);
        }
        public T? DeserializeICalComponent<T>(string source) where T : ICalendarComponent, new()
        {
            return DeserializeICalComponents<T>(source).FirstOrDefault();
        }

        public IEnumerable<T> DeserializeICalComponents<T>(string source) where T : ICalendarComponent, new()
        {
            handler = new(source);
            if (handler.BlocksLeft < 1)
                throw new ArgumentException("Could not desirialize source");

            return InternalDeserializeComponents<T>(handler);
        }
        public ICalendarProperty? DeserializeICalProperty(string source)
        {
            return InternalDeserializeContentLines(source).FirstOrDefault();
        }

        public string SerializeCalendar(Calendar calendar)
        {
            return SerializeComponent(calendar);
        }
        public string SerializeICalObjec(ICalendarComponent calendarObject)
        {
            return SerializeComponent(calendarObject);
        }
        public string SerializeICalProperty(ICalendarProperty contentLine)
        {
            return SerializeProperty(contentLine);
        }

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                handler?.Dispose();
            }
        }

    }
}
