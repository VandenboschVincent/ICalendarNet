using ICalendarNet.DataTypes;
using ICalendarNet.UnitTest.Base;

namespace ICalendarNet.UnitTest.ComponentsTests
{
    public class PeriodTests : UnitTestBase
    {
        [Test]
        public void Test_Serialize_Period()
        {
            CalSerializor calSerializor = new();
            var icalvar = @"BEGIN:VFREEBUSY
UID:19970901T095957Z-76A912@example.com
ORGANIZER:mailto:jane_doe@example.com
ATTENDEE:mailto:john_public@example.com
DTSTAMP:19970901T100000Z
FREEBUSY:19971015T050000Z/PT8H30M,19971015T160000Z/PT5H30M,19971015T223000Z/PT6H30M
URL:http://example.com/pub/busy/jpublic-01.ifb
COMMENT:This iCalendar file contains busy time information for the next three months.
END:VFREEBUSY";

            CalendarFreeBusy? calendar = calSerializor.DeserializeICalComponent<CalendarFreeBusy>(icalvar);
            calendar!.Properties.Should().HaveCount(7);
            calendar.Uid.Should().Be("19970901T095957Z-76A912@example.com");
            calendar.GetFreeBusy().Should().HaveCount(3);
            var freeBusy = calendar.GetFreeBusy().First();
            freeBusy.DateStart.Should().Be(DateTimeOffset.FromUnixTimeSeconds(876891600));
            freeBusy.DateEnd.Should().Be(DateTimeOffset.FromUnixTimeSeconds(876922200));
            freeBusy.Duration.Should().Be(TimeSpan.FromSeconds(30600));

            string serialized = calSerializor.SerializeICalObjec(calendar);
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
            CalSerializor calSerializor = new();
            string calPeriod = "19971015T050000Z/PT8H30M,19971015T160000Z/PT5H30M";

            CalendarFreeBusy? calendar = calSerializor.DeserializeICalComponent<CalendarFreeBusy>(@"BEGIN:VFREEBUSY
UID:19970901T095957Z-76A912@example.com
ORGANIZER:mailto:jane_doe@example.com
ATTENDEE:mailto:john_public@example.com
DTSTAMP:19970901T100000Z
FREEBUSY:19971015T050000Z/PT8H30M,19971015T160000Z/PT5H30M,19971015T223000Z/PT6H30M
URL:http://example.com/pub/busy/jpublic-01.ifb
COMMENT:This iCalendar file contains busy time information for the next three months.
END:VFREEBUSY");

            calendar.Should().NotBeNull();
            calendar!.SetFreeBusy(new List<CalendarPeriod>() { new CalendarPeriod(Statics.ICalProperty.FREEBUSY, calPeriod, null) });

            string serializedCalendar = calSerializor.SerializeICalObjec(calendar);

            serializedCalendar.Split(Environment.NewLine).Should().Contain("FREEBUSY:19971015T050000Z/PT8H30M,19971015T160000Z/PT5H30M");
            CalendarFreeBusy? serializedCalender = calSerializor.DeserializeICalComponent<CalendarFreeBusy>(serializedCalendar);
            serializedCalender.Should().NotBeNull();
            serializedCalender!.GetFreeBusy().Should().HaveCount(2);
        }
    }
}
