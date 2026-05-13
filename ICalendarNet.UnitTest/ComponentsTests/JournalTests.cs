using ICalendarNet.UnitTest.Base;

namespace ICalendarNet.UnitTest.ComponentsTests
{
    public class JournalTests : UnitTestBase
    {
        static IEnumerable<string> IcalFiles => GetIcalFiles("Journal*");

        [Test]
        public void Test_Serialize_Journal()
        {
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
            CalendarJournal? calendar = CalSerializor.DeserializeICalComponent<CalendarJournal>(icalvar);
            calendar!.Properties.Should().HaveCount(8);
            calendar.DTSTAMP.Should().Be(DateTimeOffset.FromUnixTimeSeconds(859204800));
            calendar.Uid.Should().Be("uid5@host1.com");
            calendar.Descriptions.Should().OnlyContain(t => t == "Project xyz Review Meeting Minutes.");
            calendar.Organizer.Should().Be("jane_doe@host.com\";CN=JohnSmith");
            calendar.Status.Should().Be("FINAL");
            calendar.Categories.Should().BeEquivalentTo(new List<string>() { "Project Report", "XYZ", "Weekly Meeting", });
            calendar.Summary.Should().Be("Project xyz Review Meeting");
            string serialized = CalSerializor.SerializeICalObjec(calendar);
            serialized.Should().Be(@"BEGIN:VJOURNAL
DTSTAMP:19970324T120000Z
UID:uid5@host1.com
ORGANIZER;SENT-BY=""MAILTO:jane_doe@host.com"";CN=JohnSmith
STATUS:FINAL
CLASS:PRIVATE
CATEGORY:Project Report, XYZ, Weekly Meeting
DESCRIPTION:Project xyz Review Meeting Minutes.
SUMMARY:Project xyz Review Meeting
END:VJOURNAL");
        }

        [TestCaseSource(nameof(IcalFiles))]
        public void Test_ChangeProperty_Journal(string file)
        {
            string icalvar = File.ReadAllText(file);
            string calDescr = "Test123456789,&é\"'(§èo!çà)'§è!çà)à_°98^$¨*ù%+:;,+/.?*//";
            Calendar? calendar = CalSerializor.DeserializeCalendar(icalvar);
            CalendarJournal journal = calendar!.GetJournals().First();
            List<string> description = [.. journal.Descriptions!];
            description.Add(calDescr);
            journal.Descriptions = description;

            string serializedCalendar = CalSerializor.SerializeCalendar(calendar);

            Calendar? serializedCalender = CalSerializor.DeserializeCalendar(serializedCalendar);
            journal = serializedCalender!.GetJournals().First();
            journal.Descriptions!.Contains(calDescr).Should().BeTrue();
            serializedCalender.GetJournals().First().Properties.Should().HaveCountGreaterThan(1);
        }
    }
}