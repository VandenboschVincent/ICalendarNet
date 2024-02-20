using BenchmarkDotNet.Attributes;
using ICalendarNet.Base;
using ICalendarNet.Components;
using ICalendarNet.Serialization;
using System.Reflection;

namespace ICalendarNet.Benchmarking
{
    [MemoryDiagnoser]
    public class ICalBenchmarkingTests
    {
        private static List<string> GetIcalStrings()
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            string topLevelIcsPath = Path.GetFullPath(Path.Combine(currentDirectory, @"Calendars"));
            return
            [
                .. Directory.EnumerateFiles(topLevelIcsPath, "*.ics", SearchOption.AllDirectories)
                                .Select(File.ReadAllText)
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .OrderByDescending(s => s.Length)
,
            ];
        }

        [Benchmark]
        public string BenchMark_Load_And_Serialize_Calendar()
        {
            var icalvar = GetIcalStrings()[0];
            Calendar? calendar = Calendar.LoadCalendar(icalvar);
            return new ICalSerializor().SerializeCalendar(calendar!);
        }

        [Benchmark]
        public void BenchMark_Load_And_Serialize_all_Calendars()
        {
            var serializer = new ICalSerializor();
            var calendars = Calendar.LoadCalendars(string.Join(Environment.NewLine, GetIcalStrings()));
            _ = calendars.Select(serializer.SerializeCalendar);

        }

        [Benchmark]
        public string BenchMark_Load_And_Serialize_Events()
        {
            var serializer = new ICalSerializor();
            var icalvar = $"BEGIN:VEVENT\r\nCREATED:20060717T210517Z\r\nLAST-MODIFIED:20060717T210718Z\r\nDTSTAMP:20060717T210718Z\r\nUID:uuid1153170430406\r\nSUMMARY:Test event\r\nDTSTART:20060718T100000\r\nDTEND:20060718T110000\r\nLOCATION:Daywest\r\nEND:VEVENT";
            ICalendarComponent? calendar = serializer.DeserializeICalComponent<CalendarEvent>(icalvar);
            return new ICalSerializor().SerializeICalObjec(calendar!);
        }

        [Benchmark]
        public async Task<string> BenchMark_Load_And_Serialize_Online_Calendar_Async()
        {
            using var httpClient = new HttpClient();
            string icalvar = await httpClient.GetStringAsync("https://www.webcal.guru/en-US/download_calendar?calendar_instance_id=10");

            Calendar? calendar = Calendar.LoadCalendar(icalvar);
            return new ICalSerializor().SerializeCalendar(calendar!);
        }
    }
}
