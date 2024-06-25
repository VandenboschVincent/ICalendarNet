using ICalendarNet.Extensions;
using ICalendarNet.UnitTest.Base;

namespace ICalendarNet.UnitTest.ComponentsTests
{
    public class EventTests : UnitTestBase
    {
        [Test]
        public void Test_Serialize_Event()
        {
            CalSerializor calSerializor = new();
            var icalvar = @"BEGIN:VEVENT
CREATED:20060717T210517Z
LAST-MODIFIED;testparam=paramvalue,paramvalue2:20060717T210718Z
DTSTAMP:20060717T210718Z
CATEGORY:3
UID:uuid1153170430406
SUMMARY:Test event
Newline Test event
https://learn.microsoft.com/en-us/dotnet/api/system.string.join?view=net-8.0
DTSTART:20060718T100000
DTEND:20060718T110000
LOCATION:Daywest
END:VEVENT";
            CalendarEvent? calendar = calSerializor.DeserializeICalComponent<CalendarEvent>(icalvar);
            calendar!.Properties.Should().HaveCount(9);
            calendar.Uid.Should().Be("uuid1153170430406");
            calendar.Summary.Should().Be($"Test event{Environment.NewLine}Newline Test event{Environment.NewLine}https://learn.microsoft.com/en-us/dotnet/api/system.string.join?view=net-8.0");
            calendar.Location.Should().Be("Daywest");
            calendar.Categories.Should().Contain("3");
            calendar!.Properties.GetContentlines(Statics.ICalProperty.LAST_MODIFIED).First().Parameters.Should().HaveCount(1);
            calendar!.Properties.GetContentlines(Statics.ICalProperty.LAST_MODIFIED).First().Parameters.First().Key.Should().Be("testparam");
            calendar!.Properties.GetContentlines(Statics.ICalProperty.LAST_MODIFIED).First().Parameters.First().Value.Should().BeEquivalentTo(new List<string>() { "paramvalue", "paramvalue2" });
            string serialized = calSerializor.SerializeICalObjec(calendar);
            serialized.Should().Be(@"BEGIN:VEVENT
CREATED:20060717T210517Z
LAST-MODIFIED;testparam=paramvalue,paramvalue2:20060717T210718Z
DTSTAMP:20060717T210718Z
CATEGORY:3
UID:uuid1153170430406
SUMMARY:Test event
Newline Test event
https://learn.microsoft.com/en-us/dotnet/api/system.string.join?view=net-8.0
DTSTART:20060718T100000
DTEND:20060718T110000
LOCATION:Daywest
END:VEVENT");
        }

        [Test]
        public void Test_ChangeProperty_Event()
        {
            CalSerializor calSerializor = new();
            string calDescr = "Test123456789,&é\"'(§èo!çà)'§è!çà)à_°98^$¨*ù%+:;,+/.?*//";
            foreach (var icalvar in GetIcalStrings("Event*"))
            {
                Calendar? calendar = Calendar.LoadCalendar(icalvar);
                calendar.Should().NotBeNull();
                calendar!.GetEvents().First().Description = calDescr;

                string serializedCalendar = calSerializor.SerializeCalendar(calendar);

                Calendar? serializedCalender = Calendar.LoadCalendar(serializedCalendar);
                serializedCalender.Should().NotBeNull();
                serializedCalender!.GetEvents().Any(t => t.Description == calDescr).Should().BeTrue();
                serializedCalender.GetEvents().First().Properties.Should().HaveCountGreaterThan(1);
            }
        }
    }
}