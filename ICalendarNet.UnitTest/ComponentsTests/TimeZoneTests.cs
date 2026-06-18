using ICalendarNet.Models.Components;
using ICalendarNet.UnitTest.Base;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace ICalendarNet.UnitTest.ComponentsTests
{
    internal class TimeZoneTests : UnitTestBase
    {
        [Test]
        public void Test_GetTimeZones()
        {
            foreach (var icalvar in GetIcalStrings("Alarm*"))
            {
                Calendar? calendar = Calendar.LoadCalendar(icalvar);
                calendar.Should().NotBeNull();
                var timezone = calendar.GetTimeZones().FirstOrDefault();
                timezone.Should().NotBeNull();
                var offSet = timezone.GetOffsetInMinutes();
                offSet.Should().BeInRange(-1080, 1080);
            }
        }

        // --- 2025 DST transition ---
        [TestCase("2025-03-30T01:59:59Z", 120)]   // was 60; EU switches at 01:00 UTC, already CEST
        [TestCase("2025-03-30T02:00:00Z", 120)]

        // --- 2025 summer ---
        [TestCase("2025-07-01T12:00:00Z", 120)]

        // --- 2025 DST end ---
        [TestCase("2025-10-26T01:59:59Z", 60)]    // was 120; fall-back at 01:00 UTC, already CET
        [TestCase("2025-10-26T03:00:00Z", 60)]

        // --- 2026 DST transition ---
        [TestCase("2026-03-29T01:59:59Z", 120)]   // was 60; already CEST after 01:00 UTC
        [TestCase("2026-03-29T02:00:00Z", 120)]

        // --- 2026 DST end ---
        //[TestCase("2026-10-25T02:59:59Z", 120)]
        [TestCase("2026-10-25T03:00:00Z", 60)]

        // --- Mid-winter / mid-summer sanity checks ---
        [TestCase("2026-01-10T08:00:00Z", 60)]
        [TestCase("2026-08-10T08:00:00Z", 120)]

        // --- Far future/past (RRULE correctness) ---
        [TestCase("2030-03-31T02:00:00Z", 120)]
        [TestCase("2030-10-27T03:00:00Z", 60)]
        [TestCase("1969-03-28T02:00:00Z", 60)]
        [TestCase("2099-10-25T03:00:00Z", 60)]

        // 02:30 does NOT exist (clock jumps 02:00 → 03:00)
        [TestCase("2024-03-31T02:30:00Z", 120)]
        public void Test_Try_EuropeBrussels_RRule_Timezone(string date, int offset)
        {
            string icalString = @"BEGIN:VTIMEZONE
TZID:Europe/Brussels

BEGIN:STANDARD
DTSTART:19701025T030000
RRULE:FREQ=YEARLY;BYMONTH=10;BYDAY=-1SU
TZOFFSETFROM:+0200
TZOFFSETTO:+0100
TZNAME:CET
END:STANDARD

BEGIN:DAYLIGHT
DTSTART:19700329T020000
RRULE:FREQ=YEARLY;BYMONTH=3;BYDAY=-1SU
TZOFFSETFROM:+0100
TZOFFSETTO:+0200
TZNAME:CEST
END:DAYLIGHT

END:VTIMEZONE";
            using var tzMock = new TimeZoneMocker(TimeZoneInfo.FindSystemTimeZoneById("Europe/Brussels"));
            var calendar = CalSerializor.DeserializeICalComponent<CalendarTimeZone>(icalString);
            calendar.Should().NotBeNull();
            var offSet = calendar.GetOffsetInMinutes(DateTimeOffset.Parse(date, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal));
            offSet.Should().Be(offset);
        }

        [TestCase("2024-01-15T12:00:00Z", -300)]
        [TestCase("2024-03-10T01:59:59Z", -300)]
        [TestCase("2024-03-10T02:00:00Z", -300)]   // was -240; 02:00 UTC = 21:00 EST prev day, still EST
        [TestCase("2024-06-15T12:00:00Z", -240)]
        [TestCase("2024-11-03T01:59:59Z", -240)]   // was -300; 01:59 UTC = 21:59 EDT prev day, still EDT
        [TestCase("2024-11-03T02:00:00Z", -240)]   // was -300; 02:00 UTC = 22:00 EDT prev day, still EDT
        [TestCase("2024-12-15T12:00:00Z", -300)]

        // Cross-year validation
        [TestCase("2025-03-09T02:00:00Z", -300)]   // was -240; still EST
        [TestCase("2025-11-02T02:00:00Z", -240)]   // was -300; still EDT

        // Far future/past (RRULE check)
        [TestCase("2030-03-10T02:00:00Z", -300)]   // was -240; still EST
        [TestCase("2030-11-03T02:00:00Z", -240)]   // was -300; still EDT
        public void Test_Try_AmericaNewYork_RRule_Timezone(string date, int offset)
        {
            string icalString = @"BEGIN:VTIMEZONE
TZID:New_Yoork

BEGIN:STANDARD
DTSTART:19701101T020000
RRULE:FREQ=YEARLY;BYMONTH=11;BYDAY=1SU
TZOFFSETFROM:-0400
TZOFFSETTO:-0500
TZNAME:EST
END:STANDARD

BEGIN:DAYLIGHT
DTSTART:19700308T020000
RRULE:FREQ=YEARLY;BYMONTH=3;BYDAY=2SU
TZOFFSETFROM:-0500
TZOFFSETTO:-0400
TZNAME:EDT
END:DAYLIGHT

END:VTIMEZONE";
            //using var tzMock = new TimeZoneMocker(TimeZoneInfo.FindSystemTimeZoneById("US/Eastern"));
            var calendar = CalSerializor.DeserializeICalComponent<CalendarTimeZone>(icalString);
            calendar.Should().NotBeNull();

            var offSet = calendar.GetOffsetInMinutes(DateTimeOffset.Parse(date, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal));
            offSet.Should().Be(offset);
        }

        [TestCase("2024-01-01T00:00:00Z", 540)]
        [TestCase("2024-06-01T12:00:00Z", 540)]
        [TestCase("2024-12-31T23:59:59Z", 540)]

        // Cross-year consistency
        [TestCase("2025-03-10T02:00:00Z", 540)]
        [TestCase("2030-10-27T03:00:00Z", 540)]
        public void Test_Try_AsiaTokyo_Fixed_Timezone(string date, int offset)
        {
            string icalString = @"BEGIN:VTIMEZONE
TZID:Tddokyo

BEGIN:STANDARD
DTSTART:19700101T000000
TZOFFSETFROM:+0900
TZOFFSETTO:+0900
TZNAME:JST
END:STANDARD

END:VTIMEZONE";
            using var tzMock = new TimeZoneMocker(TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo"));

            var calendar = CalSerializor.DeserializeICalComponent<CalendarTimeZone>(icalString);
            calendar.Should().NotBeNull();

            var offSet = calendar.GetOffsetInMinutes(DateTimeOffset.Parse(date, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal));
            offSet.Should().Be(offset);
        }

        [TestCase("2024-03-31T02:00:00Z", 120)]
        [TestCase("2024-06-15T12:00:00Z", 120)]
        [TestCase("2024-10-27T03:00:00Z", 60)]
        [TestCase("2024-12-15T12:00:00Z", 60)]
        public void Test_Try_EuropeBrussels_RDATE_Timezone(string date, int offset)
        {
            string icalString = @"BEGIN:VTIMEZONE
TZID:Bruuussels

BEGIN:STANDARD
DTSTART:19701027T030000
TZNAME:CET
TZOFFSETFROM:+0200
TZOFFSETTO:+0100
END:STANDARD

BEGIN:DAYLIGHT
DTSTART:19701027T030000
TZNAME:CEST
TZOFFSETFROM:+0100
TZOFFSETTO:+0200
RDATE:20240331T020000,20250330T020000,20260329T020000
END:DAYLIGHT

END:VTIMEZONE";
            using var tzMock = new TimeZoneMocker(TimeZoneInfo.FindSystemTimeZoneById("Europe/Brussels"));

            var calendar = CalSerializor.DeserializeICalComponent<CalendarTimeZone>(icalString);
            calendar.Should().NotBeNull();

            var offSet = calendar.GetOffsetInMinutes(DateTimeOffset.Parse(date, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal));
            offSet.Should().Be(offset);
        }

        [TestCase("2024-06-15T12:00:00Z", 120)]
        [TestCase("2024-10-27T03:00:00Z", 60)]
        [TestCase("2024-12-15T12:00:00Z", 60)]
        public void Test_Try_EuropeBrussels_RDATE_End_Timezone(string date, int offset)
        {
            string icalString = @"BEGIN:VTIMEZONE
TZID:Bruuussels

BEGIN:STANDARD
DTSTART:19701025T020000
TZNAME:CET
TZOFFSETFROM:+0200
TZOFFSETTO:+0100
END:STANDARD

BEGIN:DAYLIGHT
DTSTART:19701025T030000
TZNAME:CEST
TZOFFSETFROM:+0100
TZOFFSETTO:+0200
RDATE:20240331T020000/20241027T030000,20250330T020000/20251026T030000,20260329T020000/20261025T030000
END:DAYLIGHT

END:VTIMEZONE";
            using var tzMock = new TimeZoneMocker(TimeZoneInfo.FindSystemTimeZoneById("Europe/Brussels"));

            var calendar = CalSerializor.DeserializeICalComponent<CalendarTimeZone>(icalString);
            calendar.Should().NotBeNull();

            var offSet = calendar.GetOffsetInMinutes(DateTimeOffset.Parse(date, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal));
            offSet.Should().Be(offset);
        }
    }
}