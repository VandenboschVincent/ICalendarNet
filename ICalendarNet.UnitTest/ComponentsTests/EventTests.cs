using ICalendarNet.UnitTest.Base;

namespace ICalendarNet.UnitTest.ComponentsTests
{
    public class EventTests : UnitTestBase
    {
        [Test]
        public async Task Test_Serialize_Event()
        {
            ICalSerializor calSerializor = new();
            var icalvar = @"
CREATED:20060717T210517Z
LAST-MODIFIED:20060717T210718Z
DTSTAMP:20060717T210718Z
UID:uuid1153170430406
SUMMARY:Test event
Newline Test event
https://learn.microsoft.com/en-us/dotnet/api/system.string.join?view=net-8.0
DTSTART:20060718T100000
DTEND:20060718T110000
LOCATION:Daywest";
            CalendarEvent calendar = await calSerializor.DeserializeICalComponent<CalendarEvent>(icalvar);
            calendar.Properties.Should().HaveCount(8);
        }
    }
}
