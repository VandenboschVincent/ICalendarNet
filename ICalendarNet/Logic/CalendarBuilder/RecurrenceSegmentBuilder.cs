using ICalendarNet.Extensions;
using ICalendarNet.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using static ICalendarNet.Models.Enum.Statics;

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
        public static IEnumerable<GenerationSegment> Build(
            CalendarRecurrableObject master,
            IEnumerable<CalendarRecurrableObject>? futureOverrides,
            DateTimeOffset start,
            DateTimeOffset end)
        {
            var splitters = futureOverrides?
                .Where(o => o.Properties.HasProperty(ICalProperty.RRULE))
                .OrderBy(o => o.RecurrenceID).ToList() ?? [];

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
                        yield return new GenerationSegment(
                            Generator: current,
                            AnchorStart: currentAnchor,
                            WindowStart: windowStart,
                            WindowEnd: segEnd);
                    }
                }

                if (i < splitters.Count)
                {
                    current = splitters[i];
                    currentAnchor = splitters[i].RecurrenceID!.Value;
                }
            }
        }
    }
}