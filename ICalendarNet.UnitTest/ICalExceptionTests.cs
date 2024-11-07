namespace ICalendarNet.UnitTest
{
    public class ICalExceptionTests
    {
        [Test]
        public void Test_Exceptional_Organiser()
        {
            string ical = @"BEGIN:VEVENT
DTSTART;TZID=Europe/Moscow:20230919T172000
DTEND;TZID=Europe/Moscow:20230919T174000
SUMMARY:Team.me Offline rehearsal
UID:1qiki7damknn6qj7mc8ut0922p@google.com
SEQUENCE:0
DTSTAMP:20230929T111516Z
CREATED:20230914T230524Z
X-MICROSOFT-CDO-OWNERAPPTID:-1168347086
LOCATION:Kutuzovsky Ave\, 32\, Moskva\, Russia\, 121170
DESCRIPTION:&lt;p&gt;RUS&lt;/p&gt;&lt;p&gt;Приехать за 30 минут до начала своего слота!&lt;/p&gt;&lt;p&gt;Пасспорт + флешка с презентацией PDF&lt;/p&gt;&lt;p&gt;Форма одежды: свободная&lt;/p&gt;&lt;p&gt;Кутузовский проспект 32\, Г (После входа -&amp;gt\; направо\, по экскалатору на 2 этаж. Снова направо\, в малый конференции зал)&lt;/p&gt;&lt;br&gt;ENG&lt;br&gt;&lt;br&gt;Arrive 30 min before your slot!&lt;br&gt;&lt;br&gt;Passport + USB with PDF presentation&lt;br&gt;&lt;br&gt;Dress code: casual&lt;br&gt;&lt;br&gt;Kutuzovski prospect 32. (After entrance -&amp;gt\; turn right\, go up to the 2nd floor. Turn right again\, go to small conference hall)
URL:https://calendar.yandex.ru/event?event_id=1909162408
TRANSP:OPAQUE
CATEGORIES:Outlook
ORGANIZER;CN=Sber500 &amp; 2080 Acceleration Program:mailto:c_4eb66b106265305aa178a912be02479cb3c4a71159c9db935e8b515afff2f88f@group.calendar.google.com
ATTENDEE;PARTSTAT=NEEDS-ACTION;CN={email};ROLE=REQ-PARTICIPANT:mailto:{email}
ATTENDEE;PARTSTAT=NEEDS-ACTION;CN={email};ROLE=REQ-PARTICIPANT:mailto:{email}
ATTENDEE;PARTSTAT=ACCEPTED;CN=Sber500 &amp; 2080 Acceleration Program;ROLE=REQ-PARTICIPANT:mailto:c_4eb66b106265305aa178a912be02479cb3c4a71159c9db935e8b515afff2f88f@group.calendar.google.com
LAST-MODIFIED:20230914T230524Z
CLASS:PUBLIC
END:VEVENT";
            CalSerializor calSerializor = new();
            CalendarEvent? calendar = calSerializor.DeserializeICalComponent<CalendarEvent>(ical);
            calendar!.Properties.Should().NotBeEmpty();
            calendar.Organizer!.Value.Should().Be("mailto:c_4eb66b106265305aa178a912be02479cb3c4a71159c9db935e8b515afff2f88f@group.calendar.google.com");
            calendar.Organizer.Parameters.Should().HaveCount(2);
            calendar.Organizer.Parameters.First().Key.Should().Be("CN");
            calendar.Organizer.Parameters.First().Value.First().Should().Be("Sber500 &amp");
            calendar.Organizer.Parameters.Skip(1).First().Key.Should().Be(" 2080 Acceleration Program");
        }
    }
}