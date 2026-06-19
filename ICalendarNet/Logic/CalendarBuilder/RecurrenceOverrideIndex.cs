using ICalendarNet.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Logic.CalendarBuilder
{
    /// <summary>
    /// Indexes recurrence overrides by UID, separating single-instance
    /// overrides from THISANDFUTURE overrides. Within each bucket the
    /// override with the highest SEQUENCE wins.
    /// </summary>
    internal sealed class RecurrenceOverrideIndex
    {
        private readonly Dictionary<string, Dictionary<DateTime, CalendarRecurrableObject>> _singleByUid;
        private readonly Dictionary<string, IOrderedEnumerable<CalendarRecurrableObject>> _futureByUid;

        public RecurrenceOverrideIndex(
            Dictionary<string, Dictionary<DateTime, CalendarRecurrableObject>> single,
            Dictionary<string, IOrderedEnumerable<CalendarRecurrableObject>> future)
        {
            _singleByUid = single;
            _futureByUid = future;
        }

        public static RecurrenceOverrideIndex Build(IEnumerable<CalendarRecurrableObject> recurrable)
        {
            var withRecurrenceId = recurrable
                .Where(t => t.RecurrenceID != null && !string.IsNullOrEmpty(t.Uid));

            // Single-instance overrides (RANGE != THISANDFUTURE)
            var single = withRecurrenceId
                .Where(t => !t.OverwritesRecurrence())
                .GroupBy(t => t.Uid!)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(i => i.RecurrenceID!.Value.UtcDateTime)
                          .ToDictionary(
                              gg => gg.Key,
                              gg => gg.OrderByDescending(i => i.Sequence).First()));

            // THISANDFUTURE overrides, sorted ascending by RECURRENCE-ID
            var future = withRecurrenceId
                .Where(t => t.OverwritesRecurrence())
                .GroupBy(t => t.Uid!)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(i => i.RecurrenceID!.Value.UtcDateTime)
                          .Select(gg => gg.OrderByDescending(i => i.Sequence).First())
                          .OrderBy(i => i.RecurrenceID!.Value));

            return new RecurrenceOverrideIndex(single, future);
        }

        public Dictionary<DateTime, CalendarRecurrableObject>? GetSingleOverrides(string uid) =>
            _singleByUid.TryGetValue(uid, out var v) ? v : null;

        public IEnumerable<CalendarRecurrableObject>? GetFutureOverrides(string uid) =>
            _futureByUid.TryGetValue(uid, out var v) ? v : null;
    }
}