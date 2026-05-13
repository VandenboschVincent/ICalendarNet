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
        static List<RecurrenceTest> RecurrenceTestCases()
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            string topLevelIcsPath = Path.GetFullPath(Path.Combine(currentDirectory, "Calendars", "Recurrence"));
            var file = File.ReadAllText(Path.Combine(topLevelIcsPath, "RecurrenceTestCases.txt"));
            return RecurrenceParser.Parse(file);
        }

        private static void TestCase(RecurrenceTest exampleCase, int limit = 10, bool addstart = true)
        {
            var rrule = new CalendarRecurrenceRule(exampleCase.RRule);
            var evaluator = new RecurrenceRuleEvaluator(rrule);
            var startdate = new DateTimeOffset(exampleCase.DtStart!.Value, TimeSpan.Zero);
            var refDate = new DateTimeOffset(exampleCase.DtStart!.Value, TimeSpan.Zero);
            var datesFound = evaluator.Evaluate(startdate, refDate, new() 
            { 
                MaxOccurrencesLimit = limit,
                AddStartDate = addstart
            }).Select(t => t.DateStart.DateTime).ToList();
            if (exampleCase.Instances.Count > 0)
            {
                datesFound.Should().Equal(exampleCase.Instances);
            }
            else
            {
                datesFound.Should().BeEmpty();
            }
        }

        [Test]
        public void Test_Serialization_Deserialization_Of_Recurrence_Rule()
        {
            var calendar = new Calendar();
            var calEvent = new CalendarEvent();
            var rrule = new CalendarRecurrenceRule(string.Empty)
            {
                Frequency = FrequencyType.Monthly,
                Interval = 2,
                ByDay = [new WeekDay(DayOfWeek.Sunday)],
                BySetPosition = [4]
            };
            calEvent.SetRecurrenceRule(rrule);
            calendar.SubComponents.Add(calEvent);
            var serialized = CalSerializor.SerializeCalendar(calendar);
            serialized.Should().Contain("FREQ=MONTHLY;INTERVAL=2;BYDAY=SU;BYSETPOS=4");

            var newCalendar = CalSerializor.DeserializeCalendar(serialized);
            newCalendar.Should().NotBeNull();
            var newRrule = newCalendar.GetEvents().First().GetRecurrenceRule();
            newRrule.Should().NotBeNull();
            newRrule.Frequency.Should().Be(FrequencyType.Monthly);
            newRrule.Interval.Should().Be(2);
            newRrule.ByDay.Should().HaveCount(1);
            newRrule.ByDay[0].DayOfWeek.Should().Be(DayOfWeek.Sunday);
            newRrule.BySetPosition.Should().HaveCount(1);
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

        [TestCase("FREQ=YEARLY;BYMONTH=4;BYDAY=SU;BYSETPOS=3", "20270418", "20280416")]
        [TestCase("FREQ=YEARLY;BYMONTH=10;BYDAY=MO;BYSETPOS=1,2", "20261005", "20261012", "20271004", "20271011")]
        [TestCase("FREQ=MONTHLY;INTERVAL=2;BYMONTHDAY=29", "20260529", "20260729", "20260929", "20261129", "20270129", "20270329")]
        [TestCase("FREQ=MONTHLY;INTERVAL=2;BYMONTHDAY=31", "20260531", "20260731", "20270131", "20270331")]
        [TestCase("FREQ=MONTHLY;BYDAY=MO,TU,WE,TH,FR;BYSETPOS=-1", "20260529", "20260630", "20260731")]
        [TestCase("FREQ=MONTHLY;INTERVAL=3;BYDAY=SU;BYSETPOS=4", "20260524", "20260823", "20261122", "20270228")]
        [TestCase("FREQ=YEARLY;BYMONTH=2;BYDAY=-1TH", "20270225", "20280224", "20290222")]
        [TestCase("FREQ=YEARLY;BYDAY=TH;BYMONTH=6,7", "20260604", "20260611", "20260618", "20260625", "20260702", "20260709", "20260716", "20260723", "20260730", "20270603")]
        [TestCase("FREQ=MONTHLY;BYDAY=SA;BYMONTHDAY=7,8,9,10,11,12,13", "20260509", "20260613", "20260711")]
        public void IndividualDateRecurrenceTests(string rrule, params string[] date)
        {
            var test = new RecurrenceTest()
            {
                Instances = [.. date.Select(d => ICalTypeConverters.ConvertToDateTimeOffset(d + "Z", null)?.DateTime ?? default)],
                DtStart = new DateTime(2026,5,5,0,0,0, DateTimeKind.Utc),
                Comment = rrule,
                RRule = rrule
            };
            try
            {
                TestCase(test, date.Length, false);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(test.Exception))
                {
                    Assert.Fail($"Unexpected exception for case: {test.Comment}{Environment.NewLine}RRule: {test.RRule}{Environment.NewLine}DTSTART: {test.DtStart}{Environment.NewLine}Expected Instances: {string.Join(", ", test.Instances)}{Environment.NewLine}Exception: {ex}");
                }
            }
        }

        [TestCase("FREQ=HOURLY;INTERVAL=2;UNTIL=20260508T210000Z", "20260508T170000", "20260508T190000", "20260508T210000")]
        [TestCase("FREQ=MINUTELY;INTERVAL=15;UNTIL=20260508T160000Z", "20260508T151500", "20260508T153000", "20260508T154500", "20260508T160000")]
        [TestCase("FREQ=SECONDLY;INTERVAL=30;UNTIL=20260508T150200Z", "20260508T150030", "20260508T150100", "20260508T150130", "20260508T150200")]
        [TestCase("FREQ=HOURLY;INTERVAL=2;BYHOUR=16,17;UNTIL=20260508T210000Z", "20260508T170000")]
        public void IndividualDateTimeRecurrenceTests(string rrule, params string[] date)
        {
            var test = new RecurrenceTest()
            {
                Instances = [.. date.Select(d => ICalTypeConverters.ConvertToDateTimeOffset(d + "Z", null)?.DateTime ?? default)],
                DtStart = new DateTime(2026, 5, 8, 15, 0, 0, DateTimeKind.Utc),
                Comment = rrule,
                RRule = rrule
            };
            try
            {
                TestCase(test, 10, false);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(test.Exception))
                {
                    Assert.Fail($"Unexpected exception for case: {test.Comment}{Environment.NewLine}RRule: {test.RRule}{Environment.NewLine}DTSTART: {test.DtStart}{Environment.NewLine}Expected Instances: {string.Join(", ", test.Instances)}{Environment.NewLine}Exception: {ex}");
                }
            }
        }

        [TestCase("FREQ=DAILY;UNTIL=20260511Z", "20260509", "20260510", "20260511")]
        [TestCase("FREQ=YEARLY;UNTIL=20280509Z", "20270508", "20280508")]
        [TestCase("FREQ=WEEKLY;UNTIL=20260608Z", "20260515", "20260522", "20260529", "20260605")]
        [TestCase("FREQ=WEEKLY;INTERVAL=2;UNTIL=20260608Z", "20260522", "20260605")]
        [TestCase("FREQ=WEEKLY;UNTIL=20260520Z;WKST=SU;BYDAY=TU,TH", "20260512", "20260514", "20260519")]
        [TestCase("FREQ=WEEKLY;INTERVAL=2;UNTIL=20260530Z;WKST=SU;BYDAY=MO,WE,FR", "20260518", "20260520", "20260522")]
        [TestCase("FREQ=DAILY;COUNT=3", "20260509", "20260510")]
        [TestCase("FREQ=YEARLY;COUNT=3", "20270508", "20280508")]
        [TestCase("FREQ=WEEKLY;COUNT=3", "20260515", "20260522")]
        [TestCase("FREQ=WEEKLY;INTERVAL=2;COUNT=3", "20260522", "20260605")]
        [TestCase("FREQ=WEEKLY;WKST=SU;BYDAY=TU,TH;COUNT=3", "20260512", "20260514", "20260519")]
        [TestCase("FREQ=WEEKLY;INTERVAL=2;WKST=SU;BYDAY=MO,WE,FR;COUNT=3", "20260518", "20260520")]
        [TestCase("FREQ=WEEKLY;INTERVAL=2;COUNT=4;BYDAY=TU,SU;WKST=MO", "20260510", "20260519", "20260524", "20260602")]
        [TestCase("FREQ=WEEKLY;INTERVAL=2;COUNT=4;BYDAY=TU,SU;WKST=SU", "20260517", "20260519", "20260531", "20260602")]
        public void IndividualDateLimitRecurrenceTests(string rrule, params string[] date)
        {
            var test = new RecurrenceTest()
            {
                Instances = [.. date.Select(d => ICalTypeConverters.ConvertToDateTimeOffset(d + "Z", null)?.DateTime ?? default)],
                DtStart = new DateTime(2026, 5, 8, 0, 0, 0, DateTimeKind.Utc),
                Comment = rrule,
                RRule = rrule
            };
            try
            {
                TestCase(test, 10, false);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(test.Exception))
                {
                    Assert.Fail($"Unexpected exception for case: {test.Comment}{Environment.NewLine}RRule: {test.RRule}{Environment.NewLine}DTSTART: {test.DtStart}{Environment.NewLine}Expected Instances: {string.Join(", ", test.Instances)}{Environment.NewLine}Exception: {ex}");
                }
            }
        }
    }
    public class RecurrenceTest
    {
        public string? Comment { get; set; }
        public string RRule { get; set; } = string.Empty;
        public DateTime? DtStart { get; set; }
        public List<DateTime> Instances { get; set; } = [];
        public string? Exception { get; set; }

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
                ["\r\n\r\n", "\n\n"],
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var block in blocks)
            {
                var test = new RecurrenceTest();

                var lines = block.Split(
                    ["\r\n", "\n"],
                    StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var trimmed = line.Trim();

                    if (trimmed.StartsWith('#'))
                    {
                        test.Comment = trimmed[1..].Trim();
                    }
                    else if (trimmed.StartsWith("RRULE:"))
                    {
                        test.RRule = trimmed["RRULE:".Length..];
                    }
                    else if (trimmed.StartsWith("DTSTART:"))
                    {
                        test.DtStart = ICalTypeConverters.ConvertToDateTimeOffset(string.Concat(trimmed.AsSpan("DTSTART:".Length), "Z")
                            .Replace("ZZ","Z"), null)?.DateTime;
                    }
                    else if (trimmed.StartsWith("INSTANCES:"))
                    {
                        var values = trimmed["INSTANCES:".Length..]
                            .Split(',', StringSplitOptions.RemoveEmptyEntries);

                        test.Instances = [.. values
                            .Select(v => ICalTypeConverters.ConvertToDateTimeOffset(v + "Z", null))
                            .Where(d => d.HasValue)
                            .Select(d => d!.Value.DateTime)];
                    }
                    else if (trimmed.StartsWith("EXCEPTION:"))
                    {
                        test.Exception = trimmed["EXCEPTION:".Length..];
                    }
                }

                result.Add(test);
            }

            return result;
        }

    }
}