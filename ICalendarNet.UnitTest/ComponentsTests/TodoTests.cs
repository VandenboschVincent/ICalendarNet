using ICalendarNet.UnitTest.Base;

namespace ICalendarNet.UnitTest.ComponentsTests
{
    public class TodoTests : UnitTestBase
    {
        [Test]
        public void Test_Serialize_Todo()
        {
            ICalSerializor calSerializor = new();
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
            CalendarTodo? calendar = calSerializor.DeserializeICalComponent<CalendarTodo>(icalvar);
            calendar!.Properties.Should().HaveCount(9);
        }

        [Test]
        public void Test_ChangeProperty_Todo()
        {
            ICalSerializor calSerializor = new();
            string calDescr = "Test123456789,&é\"'(§èo!çà)'§è!çà)à_°98^$¨*ù%+:;,+/.?*//";
            foreach (var icalvar in GetIcalStrings("Todo*"))
            {
                Calendar? calendar = calSerializor.DeserializeCalendar(icalvar);

                calendar!.GetTodos().First().Summary = calDescr;

                string serializedCalendar = calSerializor.SerializeCalendar(calendar);

                Calendar? serializedCalender = calSerializor.DeserializeCalendar(serializedCalendar);

                serializedCalender!.GetTodos().Any(t => t.Summary == calDescr).Should().BeTrue();
                serializedCalender.GetTodos().First().Properties.Should().HaveCountGreaterThan(1);
            }
        }
    }
}
