using ICalendarNet.Models.Base;
using ICalendarNet.Models.Components;

namespace ICalendarNet.UnitTest.DataTypesTests
{
    internal class BuildCalendarTests
    {
        private static readonly DateTimeOffset RangeStart =
        new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        private static readonly DateTimeOffset RangeEnd =
            new(2026, 12, 31, 23, 59, 59, TimeSpan.Zero);

        private static Calendar? Parse(string ical)
            => Calendar.LoadCalendar(ical);

        private static IEnumerable<ICalendarComponent> BuildCalendar(
            Calendar? calendar, DateTimeOffset start, DateTimeOffset end) =>
            calendar?.BuildCalendar(start, end) ?? [];

        [Test]
        public void BuildCalendar_EmptyCalendar_ReturnsNoComponents()
        {
            const string ical = """
            BEGIN:VCALENDAR
            VERSION:2.0
            PRODID:-//Test//EN
            END:VCALENDAR
            """;

            var result = BuildCalendar(Parse(ical), RangeStart, RangeEnd).ToList();

            result.Should().BeEmpty();
        }

        [Test]
        public void BuildCalendar_SingleNonRecurringEvent_InRange_IsReturned()
        {
            const string ical = """
            BEGIN:VCALENDAR
            VERSION:2.0
            PRODID:-//Test//EN
            BEGIN:VEVENT
            UID:plain-1@test
            DTSTAMP:20260101T000000Z
            DTSTART:20260301T090000Z
            DTEND:20260301T100000Z
            SUMMARY:Plain event
            END:VEVENT
            END:VCALENDAR
            """;

            var result = BuildCalendar(Parse(ical), RangeStart, RangeEnd).ToList();

            result.Should().ContainSingle()
                .Which.Should().BeOfType<CalendarRecurrableObject>()
                .Which.Uid.Should().Be("plain-1@test");
        }

        [Test]
        public void BuildCalendar_NonRecurringEvent_OutOfRange_IsExcluded()
        {
            const string ical = """
            BEGIN:VCALENDAR
            VERSION:2.0
            PRODID:-//Test//EN
            BEGIN:VEVENT
            UID:out-of-range@test
            DTSTAMP:20250101T000000Z
            DTSTART:20250601T090000Z
            DTEND:20250601T100000Z
            SUMMARY:Last year
            END:VEVENT
            END:VCALENDAR
            """;

            var result = BuildCalendar(Parse(ical), RangeStart, RangeEnd).ToList();

            result.Should().BeEmpty();
        }

        [Test]
        public void BuildCalendar_DailyRecurringMaster_ExpandsAllOccurrencesInRange()
        {
            // Daily for 5 days starting Jan 10, 2026.
            const string ical = """
            BEGIN:VCALENDAR
            VERSION:2.0
            PRODID:-//Test//EN
            BEGIN:VEVENT
            UID:daily-1@test
            DTSTAMP:20260101T000000Z
            DTSTART:20260110T080000Z
            DTEND:20260110T090000Z
            RRULE:FREQ=DAILY;COUNT=5
            SUMMARY:Daily standup
            END:VEVENT
            END:VCALENDAR
            """;

            var result = BuildCalendar(Parse(ical), RangeStart, RangeEnd)
                .Cast<CalendarRecurrableObject>()
                .OrderBy(c => c.DateTimeStart)
                .ToList();

            result.Should().HaveCount(5);
            result.Select(c => c.DateTimeStart!.Value.UtcDateTime)
                  .Should().BeEquivalentTo(
                  [
                  new DateTime(2026, 1, 10, 8, 0, 0, DateTimeKind.Utc),
                  new DateTime(2026, 1, 11, 8, 0, 0, DateTimeKind.Utc),
                  new DateTime(2026, 1, 12, 8, 0, 0, DateTimeKind.Utc),
                  new DateTime(2026, 1, 13, 8, 0, 0, DateTimeKind.Utc),
                  new DateTime(2026, 1, 14, 8, 0, 0, DateTimeKind.Utc),
                  ], opt => opt.WithStrictOrdering());
        }

        [Test]
        public void BuildCalendar_RecurringMaster_OutsideQueryWindow_ExpandsNothing()
        {
            const string ical = """
            BEGIN:VCALENDAR
            VERSION:2.0
            PRODID:-//Test//EN
            BEGIN:VEVENT
            UID:daily-2@test
            DTSTAMP:20240101T000000Z
            DTSTART:20240110T080000Z
            DTEND:20240110T090000Z
            RRULE:FREQ=DAILY;COUNT=3
            SUMMARY:Old series
            END:VEVENT
            END:VCALENDAR
            """;

            var result = BuildCalendar(Parse(ical), RangeStart, RangeEnd).ToList();

            result.Should().BeEmpty();
        }

        [Test]
        public void BuildCalendar_OverrideReplacesMatchingOccurrence()
        {
            // Master: 3 daily occurrences starting Feb 1 09:00.
            // Override: move the Feb 2 occurrence to 11:00 with a new summary.
            const string ical = """
            BEGIN:VCALENDAR
            VERSION:2.0
            PRODID:-//Test//EN
            BEGIN:VEVENT
            UID:evt-with-override@test
            DTSTAMP:20260101T000000Z
            DTSTART:20260201T090000Z
            DTEND:20260201T100000Z
            RRULE:FREQ=DAILY;COUNT=3
            SUMMARY:Original
            END:VEVENT
            BEGIN:VEVENT
            UID:evt-with-override@test
            RECURRENCE-ID:20260202T090000Z
            DTSTAMP:20260101T000000Z
            DTSTART:20260202T110000Z
            DTEND:20260202T120000Z
            SUMMARY:Moved instance
            END:VEVENT
            END:VCALENDAR
            """;

            var result = BuildCalendar(Parse(ical), RangeStart, RangeEnd)
                .Cast<CalendarRecurrableObject>()
                .OrderBy(c => c.DateTimeStart)
                .ToList();

            result.Should().HaveCount(3);

            // Feb 2 09:00 should be replaced by Feb 2 11:00 "Moved instance".
            result.OfType<CalendarEvent>().Should().ContainSingle(c =>
                c.DateTimeStart == new DateTimeOffset(2026, 2, 2, 11, 0, 0, TimeSpan.Zero)
                && c.Summary == "Moved instance");

            result.OfType<CalendarEvent>().Should().NotContain(c =>
                c.DateTimeStart == new DateTimeOffset(2026, 2, 2, 9, 0, 0, TimeSpan.Zero));
        }

        [Test]
        public void BuildCalendar_OrphanOverride_WithoutMaster_IsReturned()
        {
            // Override references a UID with no corresponding master VEVENT.
            const string ical = """
            BEGIN:VCALENDAR
            VERSION:2.0
            PRODID:-//Test//EN
            BEGIN:VEVENT
            UID:ghost-uid@test
            RECURRENCE-ID:20260401T090000Z
            DTSTAMP:20260101T000000Z
            DTSTART:20260401T100000Z
            DTEND:20260401T110000Z
            SUMMARY:Orphan override
            END:VEVENT
            END:VCALENDAR
            """;

            var result = BuildCalendar(Parse(ical), RangeStart, RangeEnd).ToList();

            result.Should().ContainSingle()
                .Which.Should().BeOfType<CalendarEvent>()
                .Which.Summary.Should().Be("Orphan override");
        }

        [Test]
        public void BuildCalendar_OrphanOverride_OutsideRange_IsExcluded()
        {
            const string ical = """
            BEGIN:VCALENDAR
            VERSION:2.0
            PRODID:-//Test//EN
            BEGIN:VEVENT
            UID:ghost-uid@test
            RECURRENCE-ID:20200401T090000Z
            DTSTAMP:20200101T000000Z
            DTSTART:20200401T100000Z
            DTEND:20200401T110000Z
            SUMMARY:Old orphan
            END:VEVENT
            END:VCALENDAR
            """;

            var result = BuildCalendar(Parse(ical), RangeStart, RangeEnd).ToList();

            result.Should().BeEmpty();
        }

        [Test]
        public void BuildCalendar_ExDate_RemovesSpecificOccurrence()
        {
            // Daily for 5 days, but EXDATE the third occurrence.
            const string ical = """
            BEGIN:VCALENDAR
            VERSION:2.0
            PRODID:-//Test//EN
            BEGIN:VEVENT
            UID:daily-exdate@test
            DTSTAMP:20260101T000000Z
            DTSTART:20260110T080000Z
            DTEND:20260110T090000Z
            RRULE:FREQ=DAILY;COUNT=5
            EXDATE:20260112T080000Z
            SUMMARY:With exdate
            END:VEVENT
            END:VCALENDAR
            """;

            var result = BuildCalendar(Parse(ical), RangeStart, RangeEnd)
                .Cast<CalendarRecurrableObject>()
                .ToList();

            result.Should().HaveCount(4);
            result.Should().NotContain(c =>
                c.DateTimeStart == new DateTimeOffset(2026, 1, 12, 8, 0, 0, TimeSpan.Zero));
        }

        [Test]
        public void BuildCalendar_MixedScenario_ReturnsExpandedOrphanAndPlainItems()
        {
            // - Weekly master (4 instances) starting Jan 5, 2026.
            // - Override of the 3rd instance (Jan 19) moved to 14:00.
            // - Orphan override with no master.
            // - Plain non-recurring event.
            const string ical = """
            BEGIN:VCALENDAR
            VERSION:2.0
            PRODID:-//Test//EN
            BEGIN:VEVENT
            UID:weekly-1@test
            DTSTAMP:20260101T000000Z
            DTSTART:20260105T100000Z
            DTEND:20260105T110000Z
            RRULE:FREQ=WEEKLY;COUNT=4
            SUMMARY:Weekly sync
            END:VEVENT
            BEGIN:VEVENT
            UID:weekly-1@test
            RECURRENCE-ID:20260119T100000Z
            DTSTAMP:20260101T000000Z
            DTSTART:20260119T140000Z
            DTEND:20260119T150000Z
            SUMMARY:Rescheduled
            END:VEVENT
            BEGIN:VEVENT
            UID:orphan-1@test
            RECURRENCE-ID:20260501T090000Z
            DTSTAMP:20260101T000000Z
            DTSTART:20260501T090000Z
            DTEND:20260501T100000Z
            SUMMARY:Orphan
            END:VEVENT
            BEGIN:VEVENT
            UID:plain-1@test
            DTSTAMP:20260101T000000Z
            DTSTART:20260615T120000Z
            DTEND:20260615T130000Z
            SUMMARY:Plain
            END:VEVENT
            END:VCALENDAR
            """;

            var result = BuildCalendar(Parse(ical), RangeStart, RangeEnd)
                .Cast<CalendarRecurrableObject>()
                .ToList();

            // 4 weekly expansions (one is the override) + 1 orphan + 1 plain = 6
            result.Should().HaveCount(6);

            result.OfType<CalendarEvent>().Should().ContainSingle(c => c.Summary == "Rescheduled");
            result.OfType<CalendarEvent>().Should().ContainSingle(c => c.Summary == "Orphan");
            result.OfType<CalendarEvent>().Should().ContainSingle(c => c.Summary == "Plain");
            result.OfType<CalendarEvent>().Where(c => c.Summary == "Weekly sync").Should().HaveCount(3);
        }

        [Test]
        public void BuildCalendar_MultipleMasters_AllExpanded()
        {
            const string ical = """
            BEGIN:VCALENDAR
            VERSION:2.0
            PRODID:-//Test//EN
            BEGIN:VEVENT
            UID:m1@test
            DTSTAMP:20260101T000000Z
            DTSTART:20260101T080000Z
            DTEND:20260101T090000Z
            RRULE:FREQ=DAILY;COUNT=2
            SUMMARY:Series 1
            END:VEVENT
            BEGIN:VEVENT
            UID:m2@test
            DTSTAMP:20260101T000000Z
            DTSTART:20260201T090000Z
            DTEND:20260201T100000Z
            RRULE:FREQ=DAILY;COUNT=3
            SUMMARY:Series 2
            END:VEVENT
            END:VCALENDAR
            """;

            var result = BuildCalendar(Parse(ical), RangeStart, RangeEnd)
                .Cast<CalendarRecurrableObject>()
                .ToList();

            result.Should().HaveCount(5);
            result.Select(c => c.Uid).Distinct()
                  .Should().BeEquivalentTo("m1@test", "m2@test");
            result.Count(c => c.Uid == "m1@test").Should().Be(2);
            result.Count(c => c.Uid == "m2@test").Should().Be(3);
        }

        [Test]
        public void BuildCalendar_PartialWindow_OnlyOccurrencesInsideRangeReturned()
        {
            // Daily for 10 days, but query window covers only days 3..6.
            const string ical = """
            BEGIN:VCALENDAR
            VERSION:2.0
            PRODID:-//Test//EN
            BEGIN:VEVENT
            UID:partial@test
            DTSTAMP:20260301T000000Z
            DTSTART:20260301T080000Z
            DTEND:20260301T090000Z
            RRULE:FREQ=DAILY;COUNT=10
            SUMMARY:Partial
            END:VEVENT
            END:VCALENDAR
            """;

            var windowStart = new DateTimeOffset(2026, 3, 3, 0, 0, 0, TimeSpan.Zero);
            var windowEnd = new DateTimeOffset(2026, 3, 6, 23, 59, 59, TimeSpan.Zero);

            var result = BuildCalendar(Parse(ical), windowStart, windowEnd)
                .Cast<CalendarRecurrableObject>()
                .OrderBy(c => c.DateTimeStart)
                .ToList();

            result.Should().HaveCount(4);
            result[0].DateTimeStart!.Value.UtcDateTime
                  .Should().Be(new DateTime(2026, 3, 3, 8, 0, 0, DateTimeKind.Utc));
            result[^1].DateTimeStart!.Value.UtcDateTime
                  .Should().Be(new DateTime(2026, 3, 6, 8, 0, 0, DateTimeKind.Utc));
        }

        [Test]
        public void BuildCalendar_EndBeforeStart_ReturnsEmpty()
        {
            const string ical = """
            BEGIN:VCALENDAR
            VERSION:2.0
            PRODID:-//Test//EN
            BEGIN:VEVENT
            UID:reverse@test
            DTSTAMP:20260101T000000Z
            DTSTART:20260310T080000Z
            DTEND:20260310T090000Z
            RRULE:FREQ=DAILY;COUNT=5
            SUMMARY:Reverse window
            END:VEVENT
            END:VCALENDAR
            """;

            var result = BuildCalendar(Parse(ical), RangeEnd, RangeStart).ToList();

            result.Should().BeEmpty();
        }

        [Test]
        public void BuildCalendar_IsLazilyEvaluated()
        {
            const string ical = """
            BEGIN:VCALENDAR
            VERSION:2.0
            PRODID:-//Test//EN
            END:VCALENDAR
            """;

            var result = BuildCalendar(Parse(ical), RangeStart, RangeEnd);

            result.Should().BeAssignableTo<IEnumerable<ICalendarComponent>>();
            result.Should().NotBeOfType<List<ICalendarComponent>>();
        }
    }
}
