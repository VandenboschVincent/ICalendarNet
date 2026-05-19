using ICalendarNet.Models.Components;
using ICalendarNet.UnitTest.Base;

namespace ICalendarNet.UnitTest.ComponentsTests
{
    public class TodoTests : UnitTestBase
    {
        private static IEnumerable<string> IcalFiles => GetIcalFiles("Todo*");

        [Test]
        public void Test_Serialize_Todo()
        {
            string icalvar = @"BEGIN:VTODO
UID:fed50a1c-1e72-11db-a465-aae271be3660
SUMMARY:Test Todo
LOCATION:Test
STATUS:COMPLETED
COMPLETED;TZID=US-Eastern:20060730T090000
CLASS:PRIVATE
DTSTART;TZID=US-Eastern:20060728T090000
RRULE:FREQ=MONTHLY;COUNT=10;BYDAY=1FR
DTSTAMP:20060728T195437Z
END:VTODO";
            CalendarTodo? calendar = CalSerializor.DeserializeICalComponent<CalendarTodo>(icalvar);
            calendar!.Properties.Should().HaveCount(9);
            calendar.Uid.Should().Be("fed50a1c-1e72-11db-a465-aae271be3660");
            calendar.Summary.Should().Be("Test Todo");
            calendar.Location.Should().Be("Test");
            calendar.Status.Should().Be("COMPLETED");
            calendar.Completed!.Value.Year.Should().Be(2006);
            calendar.Class.Should().Be("PRIVATE");
            calendar.DateTimeStart!.Value.Year.Should().Be(2006);
            calendar.GetRecurrenceRule()!.Frequency.Should().Be(Models.Enum.FrequencyType.Monthly);
            calendar.GetRecurrenceRule()!.Count.Should().Be(10);
            calendar.GetRecurrenceRule()!.ByDay[0].DayOfWeek.Should().Be(DayOfWeek.Friday);
            calendar.GetRecurrenceRule()!.ByDay[0].Offset.Should().Be(1);
            calendar.DateTimeStamp.Should().Be(DateTimeOffset.FromUnixTimeSeconds(1154116477));
            string serialized = CalSerializor.SerializeICalObject(calendar);
            serialized.Should().Be(@"BEGIN:VTODO
UID:fed50a1c-1e72-11db-a465-aae271be3660
SUMMARY:Test Todo
LOCATION:Test
STATUS:COMPLETED
COMPLETED;TZID=US-Eastern:20060730T090000
CLASS:PRIVATE
DTSTART;TZID=US-Eastern:20060728T090000
RRULE:FREQ=MONTHLY;COUNT=10;BYDAY=1FR
DTSTAMP:20060728T195437Z
END:VTODO");
        }

        [TestCaseSource(nameof(IcalFiles))]
        public void Test_ChangeProperty_Todo(string file)
        {
            string icalvar = File.ReadAllText(file);
            string calDescr = "Test123456789,&é\"'(§èo!çà)'§è!çà)à_°98^$¨*ù%+:;,+/.?*//";
            Calendar? calendar = CalSerializor.DeserializeCalendar(icalvar);

            calendar!.GetTodos().First().Summary = calDescr;

            string serializedCalendar = CalSerializor.SerializeCalendar(calendar);

            Calendar? serializedCalender = CalSerializor.DeserializeCalendar(serializedCalendar);

            serializedCalender!.GetTodos().Any(t => t.Summary == calDescr).Should().BeTrue();
            serializedCalender.GetTodos().First().Properties.Should().HaveCountGreaterThan(1);
        }
    }
}