using BenchmarkDotNet.Attributes;
using ICalendarNet.Benchmarking.Data;
using ICalendarNet.Models.Base;
using ICalendarNet.Models.Components;
using ICalendarNet.Serialization;

namespace ICalendarNet.Benchmarking
{
    [MemoryDiagnoser]
    public class ICalSerializationTests
    {
        private readonly SampleData _sampleData = new();

        [GlobalSetup]
        public async Task Setup()
        {
            await _sampleData.Setup();
        }

        private static Calendar SimpleCalendar()
        {
            Calendar calendar = new();
            //Add an event
            CalendarEvent calendarEvent = new()
            {
                DateTimeStart = DateTimeOffset.UtcNow,
                DateTimeEnd = DateTimeOffset.UtcNow.AddHours(1),
                Location = "The Exceptionally Long Named Meeting Room",
                Priority = 0
            };
            calendarEvent.SetRecurrenceRule(new Models.DataTypes.CalendarRecurrenceRule(string.Empty)
            {
                Frequency = Models.Enum.FrequencyType.Daily,
                Interval = 1,
                Count = 100
            });
            calendar.SubComponents.Add(calendarEvent);
            return calendar;
        }

        [Benchmark]
        public CalendarEvent DeserializeCalendar() => Calendar.LoadCalendar(SampleData.SampleCalendar)!.GetEvents().First();

        [Benchmark]
        public string SerializeCalendar() => CalSerializor.SerializeCalendar(SimpleCalendar());

        [Benchmark]
        public string Deserialize_And_Serialize_Tiny_Calendar()
        {
            var icalvar = _sampleData.ICalStrings[^1];
            Calendar? calendar = Calendar.LoadCalendar(icalvar);
            return CalSerializor.SerializeCalendar(calendar!);
        }

        [Benchmark]
        public List<string> Deserialize_And_Serialize_all_Calendars()
        {
            var calendars = Calendar.LoadCalendars(string.Join(Environment.NewLine, _sampleData.ICalStrings));
            return [.. calendars.Select(CalSerializor.SerializeCalendar)];
        }

        [Benchmark]
        public string Deserialize_And_Serialize_Event()
        {
            ICalendarComponent? calendar = CalSerializor.DeserializeICalComponent<CalendarEvent>(SampleData.SampleEvent);
            return CalSerializor.SerializeICalObject(calendar!);
        }

        [Benchmark]
        public string Deserialize_And_Serialize_Big_Calendar()
        {
            Calendar? calendar = Calendar.LoadCalendar(_sampleData.AmericanAwernessDays);
            return CalSerializor.SerializeCalendar(calendar!);
        }
    }
}