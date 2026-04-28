using ICalendarNet.Converters;
using ICalendarNet.DataTypes;
using ICalendarNet.DataTypes.Recurrence;
using ICalendarNet.UnitTest.Base;
using System.Reflection;

namespace ICalendarNet.UnitTest.ComponentsTests
{
    [TestFixture]
    internal class RecurrenceIdentifierTests : UnitTestBase
    {
        static IEnumerable<string> RecurrenceIcal => GetIcalFiles("Recurrence/*");
        static IEnumerable<RecurrenceTest> RecurrenceTestCases()
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            string topLevelIcsPath = Path.GetFullPath(Path.Combine(currentDirectory, "Calendars", "Recurrence"));
            var file = File.ReadAllText(Path.Combine(topLevelIcsPath, "RecurrenceTestCases.txt"));
            return RecurrenceParser.Parse(file);
        }
        static IEnumerable<RecurrenceTest> FaultyRecurrenceTestCases()
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            string topLevelIcsPath = Path.GetFullPath(Path.Combine(currentDirectory, "Calendars", "Recurrence"));
            var file = File.ReadAllText(Path.Combine(topLevelIcsPath, "FaultyTestCases.txt"));
            return RecurrenceParser.Parse(file);
        }

        private void TestCase(RecurrenceTest exampleCase)
        {
            var rrule = new CalendarRecurrenceRule(exampleCase.RRule);
            var evaluator = new RecurrenceRuleEvaluator(rrule);
            var startdate = new DateTimeOffset(exampleCase.DtStart!.Value, TimeSpan.Zero);
            var refDate = new DateTimeOffset(exampleCase.DtStart!.Value, TimeSpan.Zero);
            var datesFound = evaluator.Evaluate(startdate, refDate, new()).Select(t => t.DateStart.DateTime).ToList();
            if (exampleCase.Instances.Count > 0)
            {
                datesFound.Should().Equal(exampleCase.Instances);
            }
            else
            {
                datesFound.Should().BeEmpty();
            }
        }

        [TestCaseSource(nameof(RecurrenceIcal))]
        public void TestAllRecurrence(string file)
        {
            string icalvar = File.ReadAllText(file);
            Calendar? calendar = Calendar.LoadCalendar(icalvar);
            calendar.Should().NotBeNull();
            foreach (var calEvent in calendar.GetEvents())
            {
                try
                {
                    var recurrence = calEvent.GetRecurrence(10);
                    recurrence.Should().NotBeEmpty(icalvar);
                    recurrence.Should().HaveCountGreaterThan(1, icalvar);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Failed to get recurrence for event with ical:{Environment.NewLine}{icalvar}{Environment.NewLine}Exception: {ex}");
                }
            }
        }

        [TestCaseSource(nameof(RecurrenceTestCases))]
        public void TestCases(RecurrenceTest exampleCase)
        {
            try
            {
                TestCase(exampleCase);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(exampleCase.Exception))
                {
                    Assert.Fail($"Unexpected exception for case: {exampleCase.Comment}{Environment.NewLine}RRule: {exampleCase.RRule}{Environment.NewLine}DTSTART: {exampleCase.DtStart}{Environment.NewLine}Expected Instances: {string.Join(", ", exampleCase.Instances)}{Environment.NewLine}Exception: {ex}");
                }
            }
        }

        [Ignore("These currently do not work")]
        [TestCaseSource(nameof(FaultyRecurrenceTestCases))]
        public void FaultyTestCases(RecurrenceTest exampleCase)
        {
            try
            {
                TestCase(exampleCase);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(exampleCase.Exception))
                {
                    Assert.Fail($"Unexpected exception for case: {exampleCase.Comment}{Environment.NewLine}RRule: {exampleCase.RRule}{Environment.NewLine}DTSTART: {exampleCase.DtStart}{Environment.NewLine}Expected Instances: {string.Join(", ", exampleCase.Instances)}{Environment.NewLine}Exception: {ex}");
                }
            }
        }
    }
    public class RecurrenceTest
    {
        public string Comment { get; set; }
        public string RRule { get; set; }
        public DateTime? DtStart { get; set; }
        public List<DateTime> Instances { get; set; } = new();
        public string Exception { get; set; }

        override public string ToString()
        {
            return RRule;
        }
    }

    static class RecurrenceParser
    {
        public static List<RecurrenceTest> Parse(string input)
        {
            var result = new List<RecurrenceTest>();

            var blocks = input.Split(
                new[] { "\r\n\r\n", "\n\n" },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var block in blocks)
            {
                var test = new RecurrenceTest();

                var lines = block.Split(
                    new[] { "\r\n", "\n" },
                    StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var trimmed = line.Trim();

                    if (trimmed.StartsWith("#"))
                    {
                        test.Comment = trimmed.Substring(1).Trim();
                    }
                    else if (trimmed.StartsWith("RRULE:"))
                    {
                        test.RRule = trimmed.Substring("RRULE:".Length);
                    }
                    else if (trimmed.StartsWith("DTSTART:"))
                    {
                        test.DtStart = ICalTypeConverters.ConvertToDateTimeOffset((trimmed.Substring("DTSTART:".Length) + "Z")
                            .Replace("ZZ","Z"))?.DateTime;
                    }
                    else if (trimmed.StartsWith("INSTANCES:"))
                    {
                        var values = trimmed.Substring("INSTANCES:".Length)
                            .Split(',', StringSplitOptions.RemoveEmptyEntries);

                        test.Instances = values
                            .Select(v => ICalTypeConverters.ConvertToDateTimeOffset(v + "Z"))
                            .Where(d => d.HasValue)
                            .Select(d => d.Value.DateTime)
                            .ToList();
                    }
                    else if (trimmed.StartsWith("EXCEPTION:"))
                    {
                        test.Exception = trimmed.Substring("EXCEPTION:".Length);
                    }
                }

                result.Add(test);
            }

            return result;
        }

    }
}