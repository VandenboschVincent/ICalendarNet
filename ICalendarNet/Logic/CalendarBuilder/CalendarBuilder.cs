using ICalendarNet.Extensions;
using ICalendarNet.Models.Base;
using ICalendarNet.Models.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Logic.CalendarBuilder
{
    internal static class CalendarBuilder
    {
        public static IEnumerable<ICalendarComponent> BuildCalendar(
            Calendar calendar, DateTimeOffset start, DateTimeOffset end)
        {
            var recurrable = calendar.SubComponents
                .OfType<CalendarRecurrableObject>()
                .ToList();

            var overrides = RecurrenceOverrideIndex.Build(recurrable);
            var masters = GetMasters(recurrable);

            // 1. Recurring components: expand each master.
            foreach (var master in masters)
                foreach (var component in RecurrenceExpander.Expand(master, overrides, start, end))
                    yield return component;

            // 2. Orphan overrides (override exists but its master does not).
            foreach (var orphan in GetOrphanOverrides(recurrable, masters, start, end))
                yield return orphan;

            // 3. Plain, non-recurring components.
            foreach (var item in GetNonRecurringItems(calendar, start, end))
                yield return item;
        }

        private static List<CalendarRecurrableObject> GetMasters(
            List<CalendarRecurrableObject> recurrable) =>
            [.. recurrable.Where(t =>
                    t.RecurrenceID is null &&
                    t.DateTimeStart is not null &&
                    (t.RecurrenceDates?.Any() == true || t.GetRecurrenceRule() != null))];

        private static IEnumerable<CalendarRecurrableObject> GetOrphanOverrides(
            List<CalendarRecurrableObject> recurrable,
            List<CalendarRecurrableObject> masters,
            DateTimeOffset start,
            DateTimeOffset end)
        {
            var masterUids = masters.Select(m => m.Uid).ToHashSet();
            return recurrable.Where(t =>
                t.RecurrenceID != null &&
                !masterUids.Contains(t.Uid) &&
                t.DateTimeStart.Between(start, end));
        }

        private static IEnumerable<CalendarOccurableObject> GetNonRecurringItems(
            Calendar calendar, DateTimeOffset start, DateTimeOffset end) =>
            calendar.SubComponents
                .OfType<CalendarOccurableObject>()
                .Where(o => !IsRecurring(o) && o.DateTimeStart.Between(start, end));

        private static bool IsRecurring(CalendarOccurableObject o) =>
            o is CalendarRecurrableObject r &&
            (r.RecurrenceID != null ||
             r.RecurrenceDates?.Any() == true ||
             r.GetRecurrenceRule() != null);
    }
}