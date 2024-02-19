using ICalendarNet.Base;
using ICalendarNet.Components;

namespace ICalendarNet.Serialization
{
    public partial class ICalSerializor
    {
        public async Task<Calendar?> DeserializeCalendar(string source)
        {
            return (await DeserializeCalendars(source)).FirstOrDefault();
        }
        public async Task<IEnumerable<Calendar>> DeserializeCalendars(string source)
        {
            source = ReplaceAllNewLinesRegex().Replace(source, Environment.NewLine);
            return await Task.WhenAll(
                GetObjectSources(source, ICalComponent.VCALENDAR)
                    .Select(t => DeserializeComponentsToICalObject(string.Join(Environment.NewLine, t.Skip(1)), new Calendar())));
        }
        public Task<T> DeserializeICalComponent<T>(string source) where T : ICalendarComponent, new()
        {
            return DeserializeComponentsToICalObject(source, new T());
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
