using ICalendarNet.Base;
using ICalendarNet.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICalendarNet.UnitTest.DataTypesTests
{
    internal class AlarmTriggerTests
    {
        [TestCase("TRIGGER;RELATED=START:PT5M", 300)]
        [TestCase("TRIGGER;RELATED=START:-PT5M", -300)]
        [TestCase("TRIGGER;RELATED=START:P15DT5H0M20S", 1314020)]
        [TestCase("TRIGGER;RELATED=START:P7W", 4233600)]
        public void Test_Duration_Deserialize(string value, int seconds)
        {
            CalSerializor calSerializor = new();
            ICalendarProperty? prop = calSerializor.DeserializeICalProperty(value);
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
            CalSerializor calSerializor = new();
            string serilized = calSerializor.SerializeICalProperty(new CalendarTrigger(TimeSpan.FromSeconds(seconds)));
            serilized.Should().Be(value);
        }
    }
}
