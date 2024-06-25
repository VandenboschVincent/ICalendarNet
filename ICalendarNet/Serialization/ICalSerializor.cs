using ICalendarNet.Base;
using ICalendarNet.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Serialization
{
    public partial class CalSerializor : IDisposable
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

        public T DeserializeICalComponent<T>(string source) where T : ICalendarComponent, new()
        {
            return DeserializeICalComponents<T>(source).FirstOrDefault();
        }

        public IEnumerable<T> DeserializeICalComponents<T>(string source) where T : ICalendarComponent, new()
        {
            handler = new StringHandler(source);
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
            return SerializeComponent(calendar).Trim();
        }

        public string SerializeICalObjec(ICalendarComponent calendarObject)
        {
            return SerializeComponent(calendarObject).Trim();
        }

        public string SerializeICalProperty(ICalendarProperty contentLine)
        {
            return SerializeProperty(contentLine).Trim();
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