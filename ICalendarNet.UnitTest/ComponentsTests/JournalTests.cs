using ICalendarNet.Extensions;
using ICalendarNet.UnitTest.Base;

namespace ICalendarNet.UnitTest.ComponentsTests
{
    public class JournalTests : UnitTestBase
    {
        [Test]
        public void Test_Serialize_Journal()
        {
            ICalSerializor calSerializor = new();
            string icalvar = @"BEGIN:VJOURNAL
DTSTAMP:19970324T120000Z
UID:uid5@host1.com
ORGANIZER;SENT-BY=""MAILTO:jane_doe@host.com"";CN=JohnSmith
STATUS:FINAL
CLASS:PRIVATE
CATEGORY:Project Report, XYZ, Weekly Meeting
DESCRIPTION:Project xyz Review Meeting Minutes.
SUMMARY:Project xyz Review Meeting
END:VJOURNAL";
            CalendarJournal? calendar = calSerializor.DeserializeICalComponent<CalendarJournal>(icalvar);
            calendar!.Properties.Should().HaveCount(8);
            calendar.DTSTAMP.Should().Be(DateTimeOffset.FromUnixTimeSeconds(859204800));
            calendar.Uid.Should().Be("uid5@host1.com");
            calendar.Description.Should().Be("Project xyz Review Meeting Minutes.");
            //TODO Add organizer
            //calendar.Properties.GetContentlineValue(Statics.ICalProperty.ORGANIZER).Should().Be("")
            calendar.Properties.GetContentlineValue(Statics.ICalProperty.STATUS).Should().Be("FINAL");
            calendar.Properties.GetContentlineValue(Statics.ICalProperty.CATEGORY).Should().Be("Project Report, XYZ, Weekly Meeting");
            calendar.Properties.GetContentlineValue(Statics.ICalProperty.SUMMARY).Should().Be("Project xyz Review Meeting");
        }

        [Test]
        public void Test_ChangeProperty_Journal()
        {
            ICalSerializor calSerializor = new();
            string calDescr = "Test123456789,&é\"'(§èo!çà)'§è!çà)à_°98^$¨*ù%+:;,+/.?*//";
            foreach (var icalvar in GetIcalStrings("Journal*"))
            {
                Calendar? calendar = calSerializor.DeserializeCalendar(icalvar);

                calendar!.GetJournals().First().Description = calDescr;

                string serializedCalendar = calSerializor.SerializeCalendar(calendar);

                Calendar? serializedCalender = calSerializor.DeserializeCalendar(serializedCalendar);

                serializedCalender!.GetJournals().Any(t => t.Description == calDescr).Should().BeTrue();
                serializedCalender.GetJournals().First().Properties.Should().HaveCountGreaterThan(1);
            }
        }
    }
}