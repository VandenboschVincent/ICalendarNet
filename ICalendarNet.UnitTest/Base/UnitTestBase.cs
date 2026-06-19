using System.Reflection;

namespace ICalendarNet.UnitTest
{
    internal abstract class UnitTestBase
    {
        protected static List<string> GetIcalStrings(string? fileName = null)
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            string topLevelIcsPath = Path.GetFullPath(Path.Combine(currentDirectory, @"Calendars"));
            return
            [
                .. Directory.EnumerateFiles(topLevelIcsPath, (fileName ?? "*") + ".ics", SearchOption.AllDirectories)
                                .Select(File.ReadAllText)
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .OrderByDescending(s => s.Length)
                                ];
        }

        protected static List<string> GetIcalFiles(string? fileName = null)
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            string topLevelIcsPath = Path.GetFullPath(Path.Combine(currentDirectory, @"Calendars"));
            return
            [
                .. Directory.EnumerateFiles(topLevelIcsPath, (fileName ?? "*") + ".ics", SearchOption.AllDirectories)
            ];
        }
    }

}