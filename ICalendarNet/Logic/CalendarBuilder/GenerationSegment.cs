using ICalendarNet.Models.Base;
using System;

namespace ICalendarNet.Logic.CalendarBuilder
{
 #if NET5_0_OR_GREATER
    /// <summary>
    /// A contiguous time window during which a single generator
    /// (the master, or a THISANDFUTURE override carrying its own RRULE)
    /// is responsible for producing occurrences.
    /// </summary>
    /// <param name="Generator">The component used to expand occurrences in this window.</param>
    /// <param name="AnchorStart">The generator's own DTSTART (or RECURRENCE-ID for overrides).</param>
    /// <param name="WindowStart">Effective start, clamped to the user-requested range start.</param>
    /// <param name="WindowEnd">Exclusive end: either the user range end or the next RRULE override's RECURRENCE-ID.</param>
    internal sealed record GenerationSegment(
        CalendarRecurrableObject Generator,
        DateTimeOffset AnchorStart,
        DateTimeOffset WindowStart,
        DateTimeOffset WindowEnd);
#else
    /// <summary>
    /// A contiguous time window during which a single generator
    /// (the master, or a THISANDFUTURE override carrying its own RRULE)
    /// is responsible for producing occurrences.
    /// </summary>
    internal sealed class GenerationSegment
    {
        public GenerationSegment(
            CalendarRecurrableObject Generator,
            DateTimeOffset AnchorStart,
            DateTimeOffset WindowStart,
            DateTimeOffset WindowEnd)
        {
            this.Generator = Generator;
            this.AnchorStart = AnchorStart;
            this.WindowStart = WindowStart;
            this.WindowEnd = WindowEnd;
        }

        /// <summary>The component used to expand occurrences in this window.</summary>
        public CalendarRecurrableObject Generator { get; }

        /// <summary>The generator's own DTSTART (or RECURRENCE-ID for overrides).</summary>
        public DateTimeOffset AnchorStart { get; }

        /// <summary>Effective start, clamped to the user-requested range start.</summary>
        public DateTimeOffset WindowStart { get; }

        /// <summary>Exclusive end: either the user range end or the next RRULE override's RECURRENCE-ID.</summary>
        public DateTimeOffset WindowEnd { get; }
    }
#endif
}