using ICalendarNet.Base;
using ICalendarNet.DataTypes;
using ICalendarNet.UnitTest.Base;

namespace ICalendarNet.UnitTest.DataTypesTests
{
    public class AttachmentTests : UnitTestBase
    {
        [TestCase("ATTACH;VALUE=BINARY;ENCODING=BASE64:VGhpcyBpcyBhIHRlc3QgdG8gdHJ5IG91dCBiYXNlNjQgZW5jb2Rpbmcgd2l0aG91dCBiZW==", "VGhpcyBpcyBhIHRlc3QgdG8gdHJ5IG91dCBiYXNlNjQgZW5jb2Rpbmcgd2l0aG91dCBiZW==")]
        [TestCase("ATTACH;VALUE=BINARY;ENCODING=BASE64:\r\nVGhpcyBpcyBhIHRlc3QgdG8gdHJ5IG91dCBiYXNlNjQgZW5jb2Rpbmcgd2l0aG91dCBiZW==", "VGhpcyBpcyBhIHRlc3QgdG8gdHJ5IG91dCBiYXNlNjQgZW5jb2Rpbmcgd2l0aG91dCBiZW==")]
        public void Test_Attachment_Byte_Deserialize(string value, string bytevalue)
        {
            ICalSerializor calSerializor = new();
            ICalendarProperty? prop = calSerializor.DeserializeICalProperty(value);
            prop.Should().NotBeNull();
            CalendarAttachment attachment = prop.Should().BeOfType<CalendarAttachment>().Subject;
            attachment.Should().NotBeNull();
            attachment.GetData().Should().NotBeNull();
            attachment.Value.Should().Be(bytevalue);
            attachment.ENCODING.Should().Be("BASE64");
            attachment.ValueType.Should().Be("BINARY");
        }

        [TestCase("ATTACH:http://ical.mac.com/ical/US32Holidays.ics", "http://ical.mac.com/ical/US32Holidays.ics")]
        [TestCase("ATTACH:CID:jsmith.part3.960817T083000.xyzMail@example.com", "CID:jsmith.part3.960817T083000.xyzMail@example.com")]
        [TestCase("ATTACH;FMTTYPE=application/postscript:ftp://example.com/pub/reports/r-960812.ps", "ftp://example.com/pub/reports/r-960812.ps")]
        public void Test_Attachment_Uri_Deserialize(string value, string uri)
        {
            ICalSerializor calSerializor = new();
            ICalendarProperty? prop = calSerializor.DeserializeICalProperty(value);
            prop.Should().NotBeNull();
            CalendarAttachment attachment = prop.Should().BeOfType<CalendarAttachment>().Subject;
            attachment.Should().NotBeNull();
            attachment.GetUri().Should().NotBeNull();
            attachment.GetUri()!.ToString().Should().BeEquivalentTo(uri);
            calSerializor.SerializeICalProperty(attachment).Should().Be(value);
        }

        [TestCase("ATTACH;FMTTYPE=application/postscript:ftp://example.com/pub/reports/r-960812.ps", "application/postscript")]
        public void Test_Attachment_Uri_Deserialize_FMTTYPE(string value, string type)
        {
            ICalSerializor calSerializor = new();
            ICalendarProperty? prop = calSerializor.DeserializeICalProperty(value);
            prop.Should().NotBeNull();
            CalendarAttachment attachment = prop.Should().BeOfType<CalendarAttachment>().Subject;
            attachment.Should().NotBeNull();
            attachment.FMTTYPE.Should().Be(type);
        }

        [Test]
        public void Test_Attachment_FromEvent()
        {
            ICalSerializor calSerializor = new();
            foreach (var icalvar in GetIcalStrings("Serialization\\Attachment*"))
            {
                Calendar? calendar = calSerializor.DeserializeCalendar(icalvar);
                calendar.Should().NotBeNull();
                CalendarEvent? calendarEvent = calendar!.GetEvents().FirstOrDefault();
                calendarEvent.Should().NotBeNull();
                IEnumerable<CalendarAttachment> attachments = calendarEvent!.GetAttachments();
                attachments.Should().NotBeEmpty();
                CalendarAttachment? attachment = attachments!.FirstOrDefault();
                attachment.Should().NotBeNull();
                attachment.Should().Match<CalendarAttachment>(x => x.GetData() != null || x.GetUri() != null);
            }
        }
    }
}