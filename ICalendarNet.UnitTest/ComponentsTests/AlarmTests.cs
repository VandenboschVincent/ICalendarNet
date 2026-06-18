using ICalendarNet.Models.Components;
using ICalendarNet.Models.Enum;
using ICalendarNet.UnitTest.Base;

namespace ICalendarNet.UnitTest.ComponentsTests
{
    internal class AlarmTests : UnitTestBase
    {
        private static IEnumerable<string> IcalFiles => GetIcalFiles("Alarm*");

        [Test]
        public void Test_Serialize_Alarm()
        {
            var icalvar = @"BEGIN:VALARM
TRIGGER;RELATED=END:-PT30M
ACTION:DISPLAY
DESCRIPTION:Breakfast meeting with executive\nteam at 8:30 AM EST.
END:VALARM";
            CalendarAlarm? calendar = CalSerializor.DeserializeICalComponent<CalendarAlarm>(icalvar);
            calendar.Should().NotBeNull();
            calendar!.Properties.Should().HaveCount(3);
            calendar.Trigger!.TimeValue.Should().Be(TimeSpan.FromMinutes(-30));
            calendar.Action.Should().Be(AlarmAction.DISPLAY);
            calendar.Description.Should().Be("Breakfast meeting with executive\\nteam at 8:30 AM EST.");
            string serialized = CalSerializor.SerializeICalObject(calendar);
            serialized.Should().Be(@"BEGIN:VALARM
TRIGGER;RELATED=END:-PT30M
ACTION:DISPLAY
DESCRIPTION:Breakfast meeting with executive\nteam at 8:30 AM EST.
END:VALARM");
        }

        [TestCaseSource(nameof(IcalFiles))]
        public void Test_ChangeProperty_Alarm(string file)
        {
            string icalvar = File.ReadAllText(file);
            string calDescr = "Test123456789,&é\"'(§èo!çà)'§è!çà)à_°98^$¨*ù%+:;,+/.?*//";
            Calendar? calendar = Calendar.LoadCalendar(icalvar);
            calendar.Should().NotBeNull();
            calendar!.GetEvents().First().GetAlarms().First().Description = calDescr;
            calendar!.GetEvents().First().DateTimeStart.Should().NotBeNull();

            string serializedCalendar = CalSerializor.SerializeCalendar(calendar);

            Calendar? serializedCalender = Calendar.LoadCalendar(serializedCalendar);
            serializedCalender.Should().NotBeNull();
            serializedCalender!.GetEvents().Any(t => t.GetAlarms().Any(x => x.Description == calDescr)).Should().BeTrue();
            serializedCalender.GetEvents().First().GetAlarms().First().Properties.Should().HaveCountGreaterThan(1);
        }
    }
}