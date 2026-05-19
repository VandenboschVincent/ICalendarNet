using ICalendarNet.Models.Components;
using System.Globalization;

namespace ICalendarNet.UnitTest.DataTypesTests
{
    public class EncodingTests
    {
        [Test]
        public void Test_Encoding_Deserialize_Serialize()
        {
            var icalvar = @"BEGIN:VEVENT
CATEGORY;ENCODING=BASE64:Mg==
CATEGORY:3
UID;ENCODING=BASE64:uuid1153170430406
SUMMARY;ENCODING=BASE64:VGVzdCBldmVudA0KTmV3bGluZSBUZXN0IGV2ZW50DQpodHRwczovL2xlYXJuLm1pY3Jvc29mdC5jb20vZW4tdXMvZG90bmV0L2FwaS9zeXN0ZW0uc3RyaW5nLmpvaW4/dmlldz1uZXQtOC4w
DTSTART;ENCODING=BASE64:MjAwNjA3MThUMTAwMDAw
LOCATION;ENCODING=BASE64:RGF5d2VzdA==
END:VEVENT";
            CalendarEvent? calendar = CalSerializor.DeserializeICalComponent<CalendarEvent>(icalvar);
            calendar!.Properties.Should().HaveCount(6);
            calendar.Summary.Should().Be($"Test event\r\nNewline Test event\r\nhttps://learn.microsoft.com/en-us/dotnet/api/system.string.join?view=net-8.0");
            calendar.Location.Should().Be("Daywest");
            calendar.Categories.Should().Contain("3");
            calendar.Categories.Should().Contain("2");
            calendar.DateTimeStart.Should().Be(DateTimeOffset.Parse("2006-07-18T10:00:00", CultureInfo.InvariantCulture));
            calendar.Summary = "Test new event";
            string serialized = CalSerializor.SerializeICalObject(calendar);
            serialized.Should().Be(@"BEGIN:VEVENT
CATEGORY;ENCODING=BASE64:Mg==
CATEGORY:3
UID;ENCODING=BASE64:uuid1153170430406
SUMMARY;ENCODING=BASE64:VGVzdCBuZXcgZXZlbnQ=
DTSTART;ENCODING=BASE64:MjAwNjA3MThUMTAwMDAw
LOCATION;ENCODING=BASE64:RGF5d2VzdA==
END:VEVENT");
        }
    }
}
