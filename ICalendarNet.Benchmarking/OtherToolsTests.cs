using BenchmarkDotNet.Attributes;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using ICalendarNet.Benchmarking.Data;

namespace ICalendarNet.Benchmarking
{
    [MemoryDiagnoser]
    public class OtherToolsTests
    {
        private readonly SampleData _sampleData = new();

        [GlobalSetup]
        public async Task Setup()
        {
            await _sampleData.Setup();
        }

        private const string _aTzid = "America/New_York";

        private static Calendar SimpleCalendar()
        {
            //Add an event
            CalendarEvent calendarEvent = new()
            {
                Start = new CalDateTime(DateTime.Now, _aTzid),
                End = new CalDateTime(DateTime.Now + TimeSpan.FromHours(1), _aTzid),
                RecurrenceRule =
                    new RecurrencePattern(FrequencyType.Daily, 1)
                    {
                        Count = 100,
                    },
                Location = "The Exceptionally Long Named Meeting Room",
                Priority = 0
            };
            Calendar calendar = new()
            {
                Events = { calendarEvent },
            };
            return calendar;
        }

        [Benchmark]
        public CalendarEvent ICal_Net_DeserializeCalendar() => Calendar.Load(SampleData.SampleCalendar)!.Events.First();

        [Benchmark]
        public string? ICal_Net_SerializeCalendar() => new CalendarSerializer().SerializeToString(SimpleCalendar());

        [Benchmark]
        public List<string?> ICal_Net_Deserialize_And_Serialize_all_Calendars()
        {
            var serializer = new CalendarSerializer();
            var calendars = CalendarCollection.Load(string.Join(Environment.NewLine, _sampleData.ICalStrings));
            return [.. calendars.Select(serializer.SerializeToString)];
        }

        [Benchmark]
        public string? ICal_Net_Deserialize_And_Serialize_Big_Calendar()
        {
            Calendar? calendar = Calendar.Load(_sampleData.AmericanAwernessDays);
            return new CalendarSerializer().SerializeToString(calendar);
        }
    }
}