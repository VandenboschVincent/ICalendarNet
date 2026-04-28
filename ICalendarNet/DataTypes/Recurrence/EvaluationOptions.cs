using System;

namespace ICalendarNet.DataTypes.Recurrence
{
    public class EvaluationOptions
    {
        /// <summary>
        /// The maximum number of increments to evaluate without finding a recurrence before
        /// evaluation is stopped exceptionally. If null, the evaluation will continue indefinitely.
        /// </summary>
        /// <remarks>
        /// This option only applies to the evaluation of RecurrencePatterns.
        /// <para/>
        /// If the specified number of increments is exceeded without finding a recurrence, an
        /// exception of type <see cref="Ical.Net.Evaluation.EvaluationLimitExceededException"/> will be thrown.
        /// </remarks>
        public int? MaxUnmatchedIncrementsLimit { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of occurrences allowed for the operation.
        /// </summary>
        /// <remarks>Set this property to limit the number of times an operation can be performed or
        /// results can be returned. Adjusting this value can help control resource usage and prevent excessive
        /// processing.</remarks>
        public int MaxOccurrencesLimit { get; set; } = 50;


        /// <summary>
        /// Gets or sets the maximum allowable date and time value for operations or data validation (overriden in rrule until).
        /// </summary>
        /// <remarks>This property can be used to enforce upper bounds on date and time values in
        /// scenarios such as scheduling, data entry, or validation logic. The default value is set far in the future to
        /// accommodate a wide range of use cases.</remarks>
        public DateTimeOffset? MaxDateTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a start date should always be added.
        /// </summary>
        public bool AddStartDate { get; set; } = true;
    }
}
