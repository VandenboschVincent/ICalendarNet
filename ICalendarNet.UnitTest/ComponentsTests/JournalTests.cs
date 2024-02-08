using FluentAssertions;
using ICalendarNet.Components;
using ICalendarNet.Serialization;
using ICalendarNet.UnitTest.Base;

namespace ICalendarNet.UnitTest.ComponentsTests
{
    public class JournalTests : UnitTestBase
    {
        [Test]
        public async Task Test_Serialize_Journal()
        {
            ICalSerializor calSerializor = new();
            foreach (var icalvar in GetIcalStrings("Journal*"))
            {
                Calendar? calendar = await calSerializor.DeserializeCalendar(icalvar);
                calendar!.Properties.Should().NotBeEmpty();
                string serializedCalendar = calSerializor.SerializeCalendar(calendar);

                serializedCalendar.Should().Be(icalvar);

                calendar.GetJournals().Should().NotBeEmpty();
            }
        }

        [Test]
        public async Task Test_ChangeProperty_Journal()
        {
            ICalSerializor calSerializor = new();
            string calDescr = "Test123456789,&é\"'(§èo!çà)'§è!çà)à_°98^$¨*ù%+:;,+/.?*//";
            foreach (var icalvar in GetIcalStrings("Journal*"))
            {
                Calendar? calendar = await calSerializor.DeserializeCalendar(icalvar);

                calendar!.GetJournals().First().Description = calDescr;

                string serializedCalendar = calSerializor.SerializeCalendar(calendar);

                Calendar? serializedCalender = await calSerializor.DeserializeCalendar(serializedCalendar);

                serializedCalender!.GetJournals().Any(t => t.Description == calDescr).Should().BeTrue();
            }
        }
    }
}
