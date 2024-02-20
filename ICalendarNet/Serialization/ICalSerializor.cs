using ICalendarNet.Base;
using ICalendarNet.Components;

namespace ICalendarNet.Serialization
{
    public partial class ICalSerializor
    {
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
            source = ReplaceAllNewLinesRegex().Replace(source, Environment.NewLine);
            string[] sourceLines = source.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            for (int i = 0; i < sourceLines.Length; i++)
            {
                if (allKeys.Contains(sourceLines[i], StringComparer.OrdinalIgnoreCase))
                    sourceLines[i] = sourceLines[i].ToUpper();
            }
            return InternalDeserializeComponents<T>(sourceLines);
        }
        public ICalendarProperty DeserializeICalProperty(string source)
        {
            return GetProperty(source);
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
    }
}
