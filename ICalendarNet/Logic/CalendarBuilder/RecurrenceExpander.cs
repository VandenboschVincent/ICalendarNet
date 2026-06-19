using ICalendarNet.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Logic.CalendarBuilder
{
    /// <summary>
    /// Expands a single master into concrete occurrences within
    /// [start, end), applying single-instance and THISANDFUTURE overrides.
    /// </summary>
    internal static class RecurrenceExpander
    {
        public static IEnumerable<CalendarRecurrableObject> Expand(
            CalendarRecurrableObject master,
            RecurrenceOverrideIndex overrides,
            DateTimeOffset start,
            DateTimeOffset end)
        {
            var uid = master.Uid ?? string.Empty;
            var singleOverrides = overrides.GetSingleOverrides(uid);
            var futureOverrides = overrides.GetFutureOverrides(uid);

            var segments = RecurrenceSegmentBuilder.Build(master, futureOverrides, start, end);

            foreach (var seg in segments)
            {
                var segOccurrences = seg.Generator.GetOccuring(
                    int.MaxValue,
                    seg.WindowStart,
                    true,
                    seg.WindowEnd);

                foreach (var occur in segOccurrences)
                {
                    var resolved = ResolveOccurrence(occur, seg, singleOverrides, futureOverrides);
                    yield return resolved;
                }
            }
        }

        private static CalendarRecurrableObject ResolveOccurrence(
            CalendarRecurrableObject occur,
            GenerationSegment seg,
            Dictionary<DateTime, CalendarRecurrableObject>? singleOverrides,
            IEnumerable<CalendarRecurrableObject>? futureOverrides)
        {
            var originalStart = seg.WindowStart;

            // 1. Exact single-instance override beats everything else.
            if (singleOverrides != null &&
                singleOverrides.TryGetValue(occur.DateTimeStart!.Value.UtcDateTime, out var singleOverride))
            {
                return singleOverride;
            }

            // 2. Property-only THISANDFUTURE decoration: latest such override
            //    <= originalStart that did NOT open its own segment.
            //    (If the segment's generator itself is a THISANDFUTURE override
            //    with an RRULE, its property edits are already baked into `occur`.)
            var decoration = futureOverrides?
                .LastOrDefault(o =>
                    o.RecurrenceID!.Value <= originalStart &&
                    o.GetRecurrenceRule() == null &&
                    o.RecurrenceID!.Value >= seg.AnchorStart);

            return decoration ?? occur;
        }
    }
}