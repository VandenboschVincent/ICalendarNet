using ICalendarNet.UnitTest.Base;

namespace ICalendarNet.UnitTest
{
    public class ICalNetTests : UnitTestBase
    {


        [TestCase("https://www.officeholidays.com/ics-all/belgium")]
        [TestCase("https://www.webcal.guru/en-US/download_calendar?calendar_instance_id=10")]
        [TestCase("https://www.webcal.guru/en-US/download_calendar?calendar_instance_id=142")]
        public async Task Test_Online_vCalendar_Should_Serialize(string icalString)
        {
            ICalSerializor calSerializor = new();
            using var httpClient = new HttpClient();
            string icalvar = await httpClient.GetStringAsync(icalString);

            Calendar? calendar = calSerializor.DeserializeCalendar(icalvar);
            calendar!.Properties.Should().NotBeEmpty();
            calendar.SubComponents.Should().NotBeEmpty();

            string serializedCalendar = calSerializor.SerializeCalendar(calendar);
            Calendar? calendarAfterSerialize = calSerializor.DeserializeCalendar(serializedCalendar);

            calendarAfterSerialize!.Properties.Should().BeEquivalentTo(calendar.Properties);
            calendarAfterSerialize.SubComponents.Count.Should().Be(calendar.SubComponents.Count);
            calendarAfterSerialize.SubComponents.Select(t => t.Properties.Count).Sum().Should().Be(calendar.SubComponents.Select(t => t.Properties.Count).Sum());
        }

        [Test]
        public void Test_Offline_vCalendar_Should_Serialize()
        {
            ICalSerializor calSerializor = new();
            var calendars = Calendar.LoadCalendars(string.Join(Environment.NewLine, GetIcalStrings()));
            calendars.Should().HaveCount(139);
            foreach (var calendar in calendars)
            {
                calendar!.Properties.Should().NotBeEmpty();

                string serializedCalendar = calSerializor.SerializeCalendar(calendar);
                Calendar? calendarAfterSerialize = calSerializor.DeserializeCalendar(serializedCalendar);
                calendarAfterSerialize!.Properties.Should().BeEquivalentTo(calendar.Properties);
                calendarAfterSerialize.SubComponents.Should().HaveCount(calendar.SubComponents.Count);

                //Assertions max size
                if (calendar.SubComponents.Exists(x => x.Properties.Count >= 30)
                    || calendar.SubComponents.Count >= 30)
                {
                    continue;
                }

                calendarAfterSerialize.SubComponents.Should().BeEquivalentTo(calendar.SubComponents);
            }
        }

        [TestCase("https://www.officeholidays.com/ics-all/belgium")]
        [TestCase("https://www.webcal.guru/en-US/download_calendar?calendar_instance_id=10")]
        [TestCase("https://www.webcal.guru/en-US/download_calendar?calendar_instance_id=142")]
        public async Task Test_Update_Param_Should_Serialize(string icalString)
        {
            ICalSerializor calSerializor = new();
            DateTime dt = DateTime.UtcNow;
            DateTime date = new(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, DateTimeKind.Utc);
            string calDescr = "Test123456789,&é\"'(§èo!çà)'§è!çà)à_°98^$¨*ù%+:;,+/.?*//";
            string eventDescr = "Test456789,&é\"'(§èo!çà)'§è!çà)à_°98^$¨*ù%+:;,+/.?*//";
            using var httpClient = new HttpClient();
            string icalvar = await httpClient.GetStringAsync(icalString);

            Calendar? calendar = calSerializor.DeserializeCalendar(icalvar);
            calendar!.Properties.Should().NotBeEmpty();
            calendar.SubComponents.Should().NotBeEmpty();

            calendar.Created = date;
            calendar.Description = calDescr;

            calendar.GetEvents().First().DTSTART = date;
            calendar.GetEvents().First().Description = eventDescr;

            string serializedCalendar = calSerializor.SerializeCalendar(calendar);
            Calendar? calendarAfterSerialize = calSerializor.DeserializeCalendar(serializedCalendar);

            calendarAfterSerialize!.Created.Should().Be(date);
            calendarAfterSerialize.Description.Should().Be(calDescr);

            calendarAfterSerialize.GetEvents().Any(t => t.DTSTART == date).Should().BeTrue();
            calendarAfterSerialize.GetEvents().Any(t => t.Description == eventDescr).Should().BeTrue();
        }
    }
}