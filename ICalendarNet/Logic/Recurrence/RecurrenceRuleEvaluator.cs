using ICalendarNet.Extensions;
using ICalendarNet.Models.DataTypes;
using ICalendarNet.Models.DataTypes.Recurrence;
using ICalendarNet.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Logic.Recurrence
{
    /// <summary>
    /// Evaluates an iCalendar RRULE (RFC 5545) against a reference date and an
    /// optional period start, producing the resulting occurrences as
    /// <see cref="CalendarPeriod"/> instances.
    /// </summary>
    public static partial class RecurrenceRuleEvaluator
    {
        /// <summary>
        /// Evaluate the occurrences of this recurrence pattern.
        /// </summary>
        /// <param name="rule">The recurrence rule.</param>
        /// <param name="referenceDate">The reference date, i.e. DTSTART.</param>
        /// <param name="periodStart">Start (incl.) of the period occurrences are generated for.</param>
        /// <param name="options">Evaluation options (limits, AddStartDate, etc.).</param>
        public static IEnumerable<DateTimeOffset> Evaluate(
            CalendarRecurrenceRule rule,
            DateTimeOffset referenceDate,
            DateTimeOffset? periodStart,
            EvaluationOptions? options)
        {
            options ??= new();
            options.MaxDateTime ??= GetMaxYear(periodStart ?? referenceDate);

            if (rule.Frequency < FrequencyType.Daily && !referenceDate.HasTime())
            {
                // This case is not defined by RFC 5545. We handle it by evaluating the rule
                // as if referenceDate had a time (i.e. set to midnight).
                referenceDate = new DateTimeOffset(referenceDate.Date, referenceDate.Offset);
            }

            // Create a recurrence pattern suitable for use during evaluation.
            var pattern = ProcessRecurrencePattern(referenceDate, rule);

            var periodQuery = GetDates(referenceDate, periodStart, pattern, options)
                .TakeWhile((p, x) =>
                    p <= (pattern.Until ?? options.MaxDateTime)
                    && x < options.MaxOccurrencesLimit)
                .ToList();

            if (options.AddStartDate && !periodQuery.Contains(referenceDate) &&
                periodStart != null && referenceDate >= periodStart)
                periodQuery.Add(referenceDate);
            else if (options?.AddStartDate == false)
                periodQuery.RemoveAll(p => p.Equals(referenceDate));

            return periodQuery.OrderBy(t => t);
        }

        internal static IEnumerable<DateTimeOffset>? GetRecurrenceDates(
            CalendarRecurrenceRule rrule,
            DateTimeOffset dtstart,
            int amount = 1,
            DateTimeOffset? periodStart = null,
            bool addStartDate = true,
            DateTimeOffset? maxDate = null,
            IEnumerable<DateTimeOffset>? exceptionDates = null,
            string? timeZone = null)
        {
            var exdates = exceptionDates ?? [];
            return Evaluate(rrule, dtstart, periodStart, new EvaluationOptions
            {
                MaxOccurrencesLimit = amount,
                AddStartDate = addStartDate,
                MaxDateTime = maxDate,
                TimeZone = timeZone
            }).Where(t => !exdates.Contains(t));
        }
    }
}