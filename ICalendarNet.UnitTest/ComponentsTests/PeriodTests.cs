using ICalendarNet.Models.Components;
using ICalendarNet.Models.DataTypes;
using ICalendarNet.Models.Enum;
using ICalendarNet.UnitTest.Base;
using static ICalendarNet.Models.Enum.Statics;

namespace ICalendarNet.UnitTest.ComponentsTests
{
    public class PeriodTests : UnitTestBase
    {
        [Test]
        public void Test_Serialize_Period()
        {
            var icalvar = @"BEGIN:VFREEBUSY
UID:19970901T095957Z-76A912@example.com
ORGANIZER:mailto:jane_doe@example.com
ATTENDEE:mailto:john_public@example.com
DTSTAMP:19970901T100000Z
FREEBUSY:19971015T050000Z/PT8H30M,19971015T160000Z/PT5H30M,19971015T223000Z/PT6H30M
URL:http://example.com/pub/busy/jpublic-01.ifb
COMMENT:This iCalendar file contains busy time information for the next three months.
END:VFREEBUSY";

            CalendarFreeBusy? calendar = CalSerializor.DeserializeICalComponent<CalendarFreeBusy>(icalvar);
            calendar!.Properties.Should().HaveCount(7);
            calendar.Uid.Should().Be("19970901T095957Z-76A912@example.com");
            calendar.GetFreeBusy().Should().HaveCount(3);
            var freeBusy = calendar.GetFreeBusy().First();
            freeBusy.DateStart.Should().Be(DateTimeOffset.FromUnixTimeSeconds(876891600));
            freeBusy.DateEnd.Should().Be(DateTimeOffset.FromUnixTimeSeconds(876922200));
            freeBusy.Duration.Should().Be(TimeSpan.FromSeconds(30600));

            string serialized = CalSerializor.SerializeICalObject(calendar);
            serialized.Should().Be(@"BEGIN:VFREEBUSY
UID:19970901T095957Z-76A912@example.com
ORGANIZER:mailto:jane_doe@example.com
ATTENDEE:mailto:john_public@example.com
DTSTAMP:19970901T100000Z
FREEBUSY:19971015T050000Z/PT8H30M,19971015T160000Z/PT5H30M,19971015T223000Z/PT6H30M
URL:http://example.com/pub/busy/jpublic-01.ifb
COMMENT:This iCalendar file contains busy time information for the next three months.
END:VFREEBUSY");
        }

        [Test]
        public void Test_ChangeProperty_Period()
        {
            string calPeriod = "19971015T050000Z/PT8H30M,19971015T160000Z/PT5H30M";

            CalendarFreeBusy? calendar = CalSerializor.DeserializeICalComponent<CalendarFreeBusy>(@"BEGIN:VFREEBUSY
UID:19970901T095957Z-76A912@example.com
ORGANIZER:mailto:jane_doe@example.com
ATTENDEE:mailto:john_public@example.com
DTSTAMP:19970901T100000Z
FREEBUSY:19971015T050000Z/PT8H30M,19971015T160000Z/PT5H30M,19971015T223000Z/PT6H30M
URL:http://example.com/pub/busy/jpublic-01.ifb
COMMENT:This iCalendar file contains busy time information for the next three months.
END:VFREEBUSY");

            calendar.Should().NotBeNull();
            calendar!.SetFreeBusy([new(Statics.ICalProperty.FREEBUSY, calPeriod, null)]);

            string serializedCalendar = CalSerializor.SerializeICalObject(calendar);

            serializedCalendar.Split(Environment.NewLine).Should().Contain("FREEBUSY:19971015T050000Z/PT8H30M,19971015T160000Z/PT5H30M");
            CalendarFreeBusy? serializedCalender = CalSerializor.DeserializeICalComponent<CalendarFreeBusy>(serializedCalendar);
            serializedCalender.Should().NotBeNull();
            serializedCalender!.GetFreeBusy().Should().HaveCount(2);
        }

        [TestCase("20180219T000000Z/PT6H", 6, 0, 0)]
        [TestCase("20180219T000000Z/20180219T061005Z", 6, 10, 5)]
        [TestCase("20180219T010000Z/20180219T071005Z", 6, 10, 5)]
        public void Test_PeriodTime_ShouldBeCorrect(string value, int hours, int minutes, int seconds)
        {
            CalendarPeriod period = new(ICalProperty.FREEBUSY, value, null);
            period.Duration.Should().Be(TimeSpan.FromHours(hours).Add(TimeSpan.FromMinutes(minutes)).Add(TimeSpan.FromSeconds(seconds)));
        }
    }
}