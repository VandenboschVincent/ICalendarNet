using ICalendarNet.Base;
using ICalendarNet.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Serialization
{
    public static class CalSerializor
    {
        public static Calendar? DeserializeCalendar(ReadOnlySpan<char> source)
        {
            return DeserializeICalComponent<Calendar>(source);
        }

        public static List<Calendar> DeserializeCalendars(ReadOnlySpan<char> source)
        {
            return DeserializeICalComponents<Calendar>(source);
        }

        public static T? DeserializeICalComponent<T>(ReadOnlySpan<char> source) where T : ICalendarComponent, new()
        {
            return DeserializeICalComponents<T>(source).FirstOrDefault();
        }

        public static List<T> DeserializeICalComponents<T>(ReadOnlySpan<char> source) where T : ICalendarComponent, new()
        {
            StringHandler handler = new(source);
            if (handler.BlocksLeft < 1)
                throw new ArgumentException("Could not deserialize source");

            return CalComponentSerializor.InternalDeserializeComponents<T>(ref handler);
        }

        public static ICalendarProperty? DeserializeICalProperty(ReadOnlySpan<char> source)
        {
            return CalPropertySerializor.InternalDeserializeContentLines(source).FirstOrDefault();
        }

        public static string SerializeCalendar(Calendar calendar)
        {
            return CalComponentSerializor.SerializeComponent(calendar).Trim();
        }

        public static string SerializeICalObjec(ICalendarComponent calendarObject)
        {
            return CalComponentSerializor.SerializeComponent(calendarObject).Trim();
        }

        public static string SerializeICalProperty(ICalendarProperty contentLine)
        {
            return CalPropertyParameterSerializor.SerializeProperty(contentLine).Trim();
        }
    }
}