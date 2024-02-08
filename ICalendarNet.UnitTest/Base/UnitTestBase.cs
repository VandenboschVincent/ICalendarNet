using System.Reflection;

namespace ICalendarNet.UnitTest.Base
{
    public class UnitTestBase
    {
        internal static List<string> GetIcalStrings(string? fileName = null)
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            string topLevelIcsPath = Path.GetFullPath(Path.Combine(currentDirectory, @"Calendars"));
            return
            [
                .. Directory.EnumerateFiles(topLevelIcsPath, (fileName ?? "*") + ".ics", SearchOption.AllDirectories)
                                .Select(File.ReadAllText)
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .OrderByDescending(s => s.Length)
,
            ];
        }

    }
}
