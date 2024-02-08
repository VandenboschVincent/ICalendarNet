using FluentAssertions;
using ICalendarNet.Components;
using ICalendarNet.Serialization;
using ICalendarNet.UnitTest.Base;

namespace ICalendarNet.UnitTest.ComponentsTests
{
    public class AlarmTests : UnitTestBase
    {
        [Test]
        public async Task Test_Serialize_Alarm()
        {
            ICalSerializor calSerializor = new();
            foreach (var icalvar in GetIcalStrings("Alarm*"))
            {
                Calendar? calendar = await calSerializor.DeserializeCalendar(icalvar);
                calendar.Should().NotBeNull();
                calendar!.Properties.Should().NotBeEmpty();
                string serializedCalendar = calSerializor.SerializeCalendar(calendar);

                serializedCalendar.Should().Be(icalvar);

                foreach (var calEvent in calendar.GetEvents())
                {
                    calEvent.GetAlarms().Should().NotBeEmpty();
                }
            }
        }

        [Test]
        public async Task Test_ChangeProperty_Alarm()
        {
            ICalSerializor calSerializor = new();
            string calDescr = "Test123456789,&é\"'(§èo!çà)'§è!çà)à_°98^$¨*ù%+:;,+/.?*//";
            foreach (var icalvar in GetIcalStrings("Alarm*"))
            {
                Calendar? calendar = await Calendar.LoadCalendarAsync(icalvar);
                calendar.Should().NotBeNull();
                calendar!.GetEvents().First().GetAlarms().First().Description = calDescr;

                string serializedCalendar = calSerializor.SerializeCalendar(calendar);

                Calendar? serializedCalender = await Calendar.LoadCalendarAsync(serializedCalendar);
                serializedCalender.Should().NotBeNull();
                serializedCalender!.GetEvents().Any(t => t.GetAlarms().Any(x => x.Description == calDescr)).Should().BeTrue();

            }
        }
    }
}
