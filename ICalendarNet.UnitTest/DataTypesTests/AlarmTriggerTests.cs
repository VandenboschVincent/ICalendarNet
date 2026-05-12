using ICalendarNet.Base;
using ICalendarNet.DataTypes;
using ICalendarNet.UnitTest.Base;

namespace ICalendarNet.UnitTest.DataTypesTests
{
    public class AlarmTriggerTests : UnitTestBase
    {
        [TestCase("TRIGGER;RELATED=START:PT5M", 300)]
        [TestCase("TRIGGER;RELATED=START:-PT5M", -300)]
        [TestCase("TRIGGER;RELATED=START:P15DT5H0M20S", 1314020)]
        [TestCase("TRIGGER;RELATED=START:P7W", 4233600)]
        public void Test_Duration_Deserialize(string value, int seconds)
        {
            ICalendarProperty? prop = CalSerializor.DeserializeICalProperty(value);
            prop.Should().NotBeNull();
            CalendarTrigger attachment = prop.Should().BeOfType<CalendarTrigger>().Subject;
            attachment.Should().NotBeNull();
            attachment.TimeValue.Should().Be(TimeSpan.FromSeconds(seconds));
        }

        [TestCase("TRIGGER;RELATED=START:PT5M", 300)]
        [TestCase("TRIGGER;RELATED=START:-PT5M", -300)]
        [TestCase("TRIGGER;RELATED=START:P15DT5H0M20S", 1314020)]
        [TestCase("TRIGGER;RELATED=START:P7W", 4233600)]
        public void Test_Duration_Serialize(string value, int seconds)
        {
            string serilized = CalSerializor.SerializeICalProperty(new CalendarTrigger(TimeSpan.FromSeconds(seconds)));
            serilized.Should().Be(value);
        }

        [Test]
        public void Test_Trigger_Deserialize()
        {
            var icalvar = GetIcalStrings("Trigger2")[0];
            var calendar = Calendar.LoadCalendar(icalvar);
            calendar.Should().NotBeNull();
            var alarm = calendar.GetEvents().SelectMany(t => t.GetAlarms()).FirstOrDefault();
            alarm.Should().NotBeNull();
            alarm.Trigger.Should().NotBeNull();
            alarm.Trigger.DateValue.Should().NotBeNull();
            alarm.Trigger.DateValue.Value.Offset.Should().Be(TimeSpan.FromHours(-4));
        }
    }
}