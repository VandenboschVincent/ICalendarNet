using ICalendarNet.UnitTest.Base;

namespace ICalendarNet.UnitTest.ComponentsTests
{
    public class AlarmTests : UnitTestBase
    {
        [Test]
        public void Test_Serialize_Alarm()
        {
            ICalSerializor calSerializor = new();
            var icalvar = @"BEGIN:VALARM
TRIGGER;RELATED=END:-PT30M
ACTION:DISPLAY
DESCRIPTION:Breakfast meeting with executive\nteam at 8:30 AM EST.
END:VALARM";
            CalendarAlarm? calendar = calSerializor.DeserializeICalComponent<CalendarAlarm>(icalvar);
            calendar.Should().NotBeNull();
            calendar!.Properties.Should().HaveCount(3);
            //TODO create other class for trigger
            calendar.Trigger.Should().Be("-PT30M");
            calendar.Action.Should().Be("DISPLAY");
            calendar.Description.Should().Be("Breakfast meeting with executive\\nteam at 8:30 AM EST.");
            string serialized = calSerializor.SerializeICalObjec(calendar);
            serialized.Should().Be(@"BEGIN:VALARM
TRIGGER;RELATED=END:-PT30M
ACTION:DISPLAY
DESCRIPTION:Breakfast meeting with executive\nteam at 8:30 AM EST.
END:VALARM");
        }

        [Test]
        public void Test_ChangeProperty_Alarm()
        {
            ICalSerializor calSerializor = new();
            string calDescr = "Test123456789,&é\"'(§èo!çà)'§è!çà)à_°98^$¨*ù%+:;,+/.?*//";
            foreach (var icalvar in GetIcalStrings("Alarm*"))
            {
                Calendar? calendar = Calendar.LoadCalendar(icalvar);
                calendar.Should().NotBeNull();
                calendar!.GetEvents().First().GetAlarms().First().Description = calDescr;

                string serializedCalendar = calSerializor.SerializeCalendar(calendar);

                Calendar? serializedCalender = Calendar.LoadCalendar(serializedCalendar);
                serializedCalender.Should().NotBeNull();
                serializedCalender!.GetEvents().Any(t => t.GetAlarms().Any(x => x.Description == calDescr)).Should().BeTrue();
                serializedCalender.GetEvents().First().GetAlarms().First().Properties.Should().HaveCountGreaterThan(1);
            }
        }
    }
}