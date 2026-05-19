using ICalendarNet.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Logic.CalendarBuilder
{
    /// <summary>
    /// Splits a master's lifetime into segments where each segment is
    /// owned by exactly one "generator" (master or an RRULE-carrying
    /// THISANDFUTURE override). Property-only THISANDFUTURE overrides
    /// do NOT split the timeline — they decorate occurrences instead.
    /// </summary>
    internal static class RecurrenceSegmentBuilder
    {
        public static List<GenerationSegment> Build(
            CalendarRecurrableObject master,
            List<CalendarRecurrableObject>? futureOverrides,
            DateTimeOffset start,
            DateTimeOffset end)
        {
            var splitters = futureOverrides?
                .Where(o => o.GetRecurrenceRule() != null)
                .OrderBy(o => o.RecurrenceID!.Value)
                .ToList() ?? [];

            var segments = new List<GenerationSegment>();

            CalendarRecurrableObject current = master;
            DateTimeOffset currentAnchor = master.DateTimeStart!.Value;

            for (int i = 0; i <= splitters.Count; i++)
            {
                // Exclusive end: the next splitter (if any) takes over here.
                DateTimeOffset segEnd = i < splitters.Count
                    ? splitters[i].RecurrenceID!.Value
                    : end;

                if (segEnd > start)
                {
                    var windowStart = currentAnchor > start ? currentAnchor : start;
                    if (windowStart < segEnd)
                    {
                        segments.Add(new GenerationSegment(
                            Generator: current,
                            AnchorStart: currentAnchor,
                            WindowStart: windowStart,
                            WindowEnd: segEnd));
                    }
                }

                if (i < splitters.Count)
                {
                    current = splitters[i];
                    currentAnchor = splitters[i].RecurrenceID!.Value;
                }
            }

            return segments;
        }
    }
}