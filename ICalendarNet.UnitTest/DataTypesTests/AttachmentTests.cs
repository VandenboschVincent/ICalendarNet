using FluentAssertions;
using ICalendarNet.Base;
using ICalendarNet.Components;
using ICalendarNet.DataTypes;
using ICalendarNet.Serialization;
using ICalendarNet.UnitTest.Base;
using iCalNET;

namespace ICalendarNet.UnitTest.DataTypesTests
{
    public class AttachmentTests : UnitTestBase
    {
        [TestCase("ATTACH;VALUE=BINARY;ENCODING=BASE64:VGhpcyBpcyBhIHRlc3QgdG8gdHJ5IG91dCBiYXNlNjQgZW5jb2Rpbmcgd2l0aG91dCBiZW==", "VGhpcyBpcyBhIHRlc3QgdG8gdHJ5IG91dCBiYXNlNjQgZW5jb2Rpbmcgd2l0aG91dCBiZW==")]
        [TestCase("ATTACH;VALUE=BINARY;ENCODING=BASE64:\r\n VGhpcyBpcyBhIHRlc3QgdG8gdHJ5IG91dCBiYXNlNjQgZW5jb2Rpbmcgd2l0aG91dCBiZW==", "VGhpcyBpcyBhIHRlc3QgdG8gdHJ5IG91dCBiYXNlNjQgZW5jb2Rpbmcgd2l0aG91dCBiZW==")]
        public void Test_Attachment_Byte_Deserialize(string value, string bytevalue)
        {
            ICalSerializor calSerializor = new();
            ICalendarProperty prop = calSerializor.DeserializeICalProperty(value);
            prop.Should().BeOfType<CalendarAttachment>();
            CalendarAttachment attachment = (CalendarAttachment)prop;
            attachment.Should().NotBeNull();
            attachment.GetData().Should().NotBeNull();
            attachment.Value.Should().Be(bytevalue);
        }

        [TestCase("ATTACH:http://ical.mac.com/ical/US32Holidays.ics", "http://ical.mac.com/ical/US32Holidays.ics")]
        [TestCase("ATTACH:CID:jsmith.part3.960817T083000.xyzMail@example.com", "CID:jsmith.part3.960817T083000.xyzMail@example.com")]
        [TestCase("ATTACH;FMTTYPE=application/postscript:ftp://example.com/pub/reports/r-960812.ps", "ftp://example.com/pub/reports/r-960812.ps")]
        public void Test_Attachment_Uri_Deserialize(string value, string uri)
        {
            ICalSerializor calSerializor = new();
            ICalendarProperty prop = calSerializor.DeserializeICalProperty(value);
            prop.Should().BeOfType<CalendarAttachment>();
            CalendarAttachment attachment = (CalendarAttachment)prop;
            attachment.Should().NotBeNull();
            attachment.GetUri().Should().NotBeNull();
            attachment.GetUri()!.ToString().Should().BeEquivalentTo(uri);
            calSerializor.SerializeICalProperty(attachment).Should().Be(value);
        }
        [Test]
        public async Task Test_Attachment_FromEvent()
        {
            ICalSerializor calSerializor = new();
            foreach (var icalvar in GetIcalStrings("Serialization\\Attachment*"))
            {
                Calendar? calendar = await calSerializor.DeserializeCalendar(icalvar);
                calendar.Should().NotBeNull();
                CalendarEvent? calendarEvent = calendar!.GetEvents().FirstOrDefault();
                calendarEvent.Should().NotBeNull();
                var attachments = calendarEvent!.GetAttachments();
                attachments.Should().NotBeEmpty();
                var attachment = attachments!.FirstOrDefault();
                attachment.Should().NotBeNull();
                attachment.Should().Match<CalendarAttachment>(x => x.GetData() != null || x.GetUri() != null);
            }
        }
    }
}
