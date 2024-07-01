using BenchmarkDotNet.Attributes;
using ICalendarNet.Base;
using ICalendarNet.Components;
using ICalendarNet.DataTypes;
using ICalendarNet.Serialization;
using System.Data.SqlTypes;
using System.Reflection;
using System.Text;

namespace ICalendarNet.Benchmarking
{
    [MemoryDiagnoser]
    public class ICalSerializationTests
    {
        [GlobalSetup]
        public async Task Setup()
        {
            ICalStrings = GetIcalStrings();
            using var httpClient = new HttpClient();
            AmericanAwernessDays = await httpClient.GetStringAsync("https://www.webcal.guru/en-US/download_calendar?calendar_instance_id=10");
        }

        private string AmericanAwernessDays = "";
        private List<string> ICalStrings = []; 
        private static List<string> GetIcalStrings()
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            string topLevelIcsPath = Path.GetFullPath(Path.Combine(currentDirectory, @"Calendars"));
            return
            [
                .. Directory.EnumerateFiles(topLevelIcsPath, "*.ics", SearchOption.AllDirectories)
                                .Select(File.ReadAllText)
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .OrderByDescending(s => s.Length)
,
            ];
        }
        private const string _sampleEvent = @"BEGIN:VCALENDAR
PRODID:-//Microsoft Corporation//Outlook 12.0 MIMEDIR//EN
VERSION:2.0
METHOD:PUBLISH
X-CALSTART:20090621T000000
X-CALEND:20090622T000000
X-WR-RELCALID:{0000002E-6380-7FD2-FED7-97EAE70D6611}
X-WR-CALNAME:Parse Error Calendar
BEGIN:VEVENT
ATTENDEE;CN=some.attendee@event.com;RSVP=TRUE:mailto:some.attendee@event.co
	m
ATTENDEE;CN=event@calendardemo.net;RSVP=TRUE:mailto:event@calendardemo.net
ATTENDEE;CN=""4th Floor Meeting Room"";CUTYPE=RESOURCE;ROLE=NON-PARTICIPANT;R
	SVP=TRUE:mailto:4th.floor.meeting.room@somewhere.com
CLASS:PUBLIC
CREATED:20090621T201527Z
DESCRIPTION:\n
DTEND;VALUE=DATE:20090622
DTSTAMP:20090621T201612Z
DTSTART;VALUE=DATE:20090621
LAST-MODIFIED:20090621T201618Z
LOCATION:The Exceptionally Long Named Meeting Room Whose Name Wraps Over Se
	veral Lines When Exported From Leading Calendar and Office Software App
	lication Microsoft Office 2007
ORGANIZER;CN=""Event Organizer"":mailto:some.attendee@somewhere.com
PRIORITY:5
SEQUENCE:0
SUMMARY;LANGUAGE=en-gb:Example Calendar Export that Blows Up DDay.iCal
TRANSP:TRANSPARENT
UID:040000008200E00074C5B7101A82E00800000000900AD080B5F2C901000000000000000
	010000000B29680BF9E5DC246B5EDDE228038E71F
X-ALT-DESC;FMTTYPE=text/html:<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 3.2//E
	N"">\n<HTML>\n<HEAD>\n<META NAME=""Generator"" CONTENT=""MS Exchange Server ve
	rsion 08.00.0681.000"">\n<TITLE></TITLE>\n</HEAD>\n<BODY>\n<!-- Converted f
	rom text/rtf format -->\n\n<P DIR=LTR><SPAN LANG=""en-gb""></SPAN></P>\n\n</
	BODY>\n</HTML>
X-MICROSOFT-CDO-BUSYSTATUS:FREE
X-MICROSOFT-CDO-IMPORTANCE:1
X-MICROSOFT-DISALLOW-COUNTER:FALSE
X-MS-OLK-ALLOWEXTERNCHECK:TRUE
X-MS-OLK-APPTSEQTIME:20090621T201612Z
X-MS-OLK-AUTOFILLLOCATION:TRUE
X-MS-OLK-CONFTYPE:0
BEGIN:VALARM
TRIGGER:-PT1080M
ACTION:DISPLAY
DESCRIPTION:Reminder
END:VALARM
END:VEVENT
END:VCALENDAR
";
        private static Calendar SimpleCalendar()
        {
            Calendar calendar = new Calendar();
            //Add an event
            CalendarEvent calendarEvent = new CalendarEvent()
            {
                DTSTART = DateTimeOffset.UtcNow,
                DTEND = DateTimeOffset.UtcNow.AddHours(1),
                Location = "The Exceptionally Long Named Meeting Room",
                Priority = 0
            };
            calendarEvent.SetAttachments(new List<CalendarAttachment>()
            {
                //Add url attachment
                new CalendarAttachment(new Uri("ldap://example.com:3333/o=eExample Industries,c=3DUS??(cn=3DBJohn Smith)"), ""),
                //Add byte attachment
                new CalendarAttachment(Encoding.UTF8.GetBytes(""), "application/msword")
            });
            calendar.SubComponents.Add(calendarEvent);
            //Add an alarm
            calendar.SubComponents.Add(
                new CalendarAlarm()
                {
                    Trigger = new CalendarTrigger(TimeSpan.FromMinutes(-108)),
                    Action = "DISPLAY",
                    Description = "Reminder"
                });
            calendar.SubComponents.Add(
                new CalendarAlarm()
                {
                    //As time value
                    Trigger = new CalendarTrigger(DateTime.Now.AddYears(1)),
                    Action = "DISPLAY",
                    Description = "Reminder"
                });
            //Add a t_odo
            calendar.SubComponents.Add(
                new CalendarTodo()
                {
                    DTSTART = DateTimeOffset.UtcNow,
                    Completed = DateTimeOffset.UtcNow.AddHours(1),
                    Location = "The Exceptionally Long Named Meeting Room"
                });
            //Add a freebusy
            calendar.SubComponents.Add(
                new CalendarFreeBusy()
                {
                    DTSTART = DateTimeOffset.UtcNow,
                    DTEND = DateTimeOffset.UtcNow.AddHours(1),
                });
            return calendar;
        }

        [Benchmark]
        public void DeserializeCalendar() => Calendar.LoadCalendar(_sampleEvent)!.GetEvents().First();

        [Benchmark]
        public void SerializeCalendar() => new CalSerializor().SerializeCalendar(SimpleCalendar());


        [Benchmark]
        public string Deserialize_And_Serialize_Tiny_Calendar()
        {
            var icalvar = ICalStrings[^1];
            Calendar? calendar = Calendar.LoadCalendar(icalvar);
            return new CalSerializor().SerializeCalendar(calendar!);
        }

        [Benchmark]
        public void Deserialize_And_Serialize_all_Calendars()
        {
            var serializer = new CalSerializor();
            var calendars = Calendar.LoadCalendars(string.Join(Environment.NewLine, ICalStrings));
            _ = calendars.Select(serializer.SerializeCalendar);

        }

        [Benchmark]
        public string Deserialize_And_Serialize_Event()
        {
            var serializer = new CalSerializor();
            var icalvar = $"BEGIN:VEVENT\r\nCREATED:20060717T210517Z\r\nLAST-MODIFIED:20060717T210718Z\r\nDTSTAMP:20060717T210718Z\r\nUID:uuid1153170430406\r\nSUMMARY:Test event\r\nDTSTART:20060718T100000\r\nDTEND:20060718T110000\r\nLOCATION:Daywest\r\nEND:VEVENT";
            ICalendarComponent? calendar = serializer.DeserializeICalComponent<CalendarEvent>(icalvar);
            return new CalSerializor().SerializeICalObjec(calendar!);
        }

        [Benchmark]
        public string Deserialize_And_Serialize_Big_Calendar()
        {
            Calendar? calendar = Calendar.LoadCalendar(AmericanAwernessDays);
            return new CalSerializor().SerializeCalendar(calendar!);
        }
    }
}
