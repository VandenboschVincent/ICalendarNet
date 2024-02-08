using BenchmarkDotNet.Attributes;
using ICalendarNet.Base;
using ICalendarNet.Components;
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
        public async Task<string> BenchMark_Load_And_Serialize_Calendar_Async()
        {
            var icalvar = GetIcalStrings()[0];
            Calendar calendar = await Calendar.LoadCalendarAsync(icalvar);
            return calendar.Serialize();
        }

        [Benchmark]
        public async Task<string> BenchMark_Load_And_Serialize_Events_Async()
        {
            var icalvar = $"CREATED:20060717T210517Z\r\nLAST-MODIFIED:20060717T210718Z\r\nDTSTAMP:20060717T210718Z\r\nUID:uuid1153170430406\r\nSUMMARY:Test event\r\nDTSTART:20060718T100000\r\nDTEND:20060718T110000\r\nLOCATION:Daywest";
            ICalendarComponent calendar = await ICalComponent.VEVENT.ToObjectAsync(icalvar);
            return calendar.Serialize();
        }

        [Benchmark]
        public async Task<string> BenchMark_Load_And_Serialize_Online_Calendar_Async()
        {
            using var httpClient = new HttpClient();
            string icalvar = await httpClient.GetStringAsync("https://www.webcal.guru/en-US/download_calendar?calendar_instance_id=10");

            Calendar calendar = await Calendar.LoadCalendarAsync(icalvar);
            return calendar.Serialize();
        }
    }
}
