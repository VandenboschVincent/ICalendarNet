using FluentAssertions;
using ICalendarNet.Components;
using ICalendarNet.Serialization;
using ICalendarNet.UnitTest.Base;

namespace ICalendarNet.UnitTest.ComponentsTests
{
    public class TodoTests : UnitTestBase
    {
        [Test]
        public async Task Test_Serialize_Todo()
        {
            ICalSerializor calSerializor = new();
            foreach (var icalvar in GetIcalStrings("Todo*"))
            {
                Calendar? calendar = await calSerializor.DeserializeCalendar(icalvar);
                calendar!.Properties.Should().NotBeEmpty();
                string serializedCalendar = calSerializor.SerializeCalendar(calendar);

                serializedCalendar.Should().Be(icalvar);

                calendar.GetTodos().Should().NotBeEmpty();
            }
        }

        [Test]
        public async Task Test_ChangeProperty_Todo()
        {
            ICalSerializor calSerializor = new();
            string calDescr = "Test123456789,&é\"'(§èo!çà)'§è!çà)à_°98^$¨*ù%+:;,+/.?*//";
            foreach (var icalvar in GetIcalStrings("Todo*"))
            {
                Calendar? calendar = await calSerializor.DeserializeCalendar(icalvar);

                calendar!.GetTodos().First().Summary = calDescr;

                string serializedCalendar = calSerializor.SerializeCalendar(calendar);

                Calendar? serializedCalender = await calSerializor.DeserializeCalendar(serializedCalendar);

                serializedCalender!.GetTodos().Any(t => t.Summary == calDescr).Should().BeTrue();
            }
        }
    }
}
