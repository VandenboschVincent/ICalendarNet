using ICalendarNet.Base;
using ICalendarNet.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Serialization
{
    public partial class CalSerializor
    {
        public Calendar? DeserializeCalendar(ReadOnlySpan<char> source)
        {
            return DeserializeICalComponent<Calendar>(source);
        }

        public List<Calendar> DeserializeCalendars(ReadOnlySpan<char> source)
        {
            return DeserializeICalComponents<Calendar>(source);
        }

        public T DeserializeICalComponent<T>(ReadOnlySpan<char> source) where T : ICalendarComponent, new()
        {
            return DeserializeICalComponents<T>(source).FirstOrDefault();
        }

        public List<T> DeserializeICalComponents<T>(ReadOnlySpan<char> source) where T : ICalendarComponent, new()
        {
            StringHandler handler = new StringHandler(source);
            if (handler.BlocksLeft < 1)
                throw new ArgumentException("Could not deserialize source");

            return InternalDeserializeComponents<T>(ref handler);
        }

        public ICalendarProperty? DeserializeICalProperty(ReadOnlySpan<char> source)
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
    }
}