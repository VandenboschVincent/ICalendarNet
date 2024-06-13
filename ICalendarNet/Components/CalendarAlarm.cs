using ICalendarNet.Base;
using ICalendarNet.DataTypes;
using ICalendarNet.Extensions;
using static ICalendarNet.Statics;

namespace ICalendarNet.Components
{
    /// <summary>
    ///   <see cref="ICalComponent.VALARM" />
    /// </summary>
    public class CalendarAlarm : CalendarObject
    {
        /// <summary>
        ///   <see cref="ICalComponent.VALARM" />
        /// </summary>
        public override ICalComponent ComponentType => ICalComponent.VALARM;

        /// <summary>
        ///   <see cref="ICalProperty.DESCRIPTION" />
        /// </summary>
        public virtual string? Description
        {
            get => Properties.GetContentlineValue(ICalProperty.DESCRIPTION);
            set => Properties.UpdateLineProperty(value!, ICalProperty.DESCRIPTION);
        }

        /// <summary>
        ///   <see cref="ICalProperty.DESCRIPTION" />
        /// </summary>
        public virtual string? Summary
        {
            get => Properties.GetContentlineValue(ICalProperty.SUMMARY);
            set => Properties.UpdateLineProperty(value!, ICalProperty.SUMMARY);
        }

        /// <summary>
        ///   <see cref="ICalProperty.DESCRIPTION" />
        /// </summary>
        public virtual string? Action
        {
            get => Properties.GetContentlineValue(ICalProperty.ACTION);
            set => Properties.UpdateLineProperty(value!, ICalProperty.ACTION);
        }

        /// <summary>
        /// Relative to the DTStart or DTEnd or an absolute datetime of the parent object
        /// Can be repeated if Duration(delay) and Repeat are present
        /// </summary>
        public virtual string? Trigger
        {
            get => Properties.GetContentlineValue(ICalProperty.TRIGGER);
            set => Properties.UpdateLineProperty(value!, ICalProperty.TRIGGER);
        }

        /// <summary>
        /// Delay for the next alarm
        /// </summary>
        public virtual TimeSpan? Duration
        {
            get => Properties.GetContentlineTimeSpan(ICalProperty.DURATION);
            set => Properties.UpdateLineProperty(value!, ICalProperty.DURATION);
        }

        /// <summary>
        /// Sets how many times the action will repeat
        /// </summary>
        public virtual double? Repeat
        {
            get => Properties.GetContentlineDouble(ICalProperty.REPEAT);
            set => Properties.UpdateLineProperty(value!, ICalProperty.REPEAT);
        }

        /// <summary>
        /// Sets the receivers when Action is set to Email
        /// </summary>
        public virtual IEnumerable<string>? Attendee
        {
            get => Properties.GetContentlinesValue(ICalProperty.ATTENDEE);
            set => Properties.UpdateLinesProperty(value!.ToList(), ICalProperty.ATTENDEE);
        }

        public IEnumerable<CalendarAttachment> GetAttachments()
        {
            return Properties.GetContentlines(ICalProperty.ATTACH).Cast<CalendarAttachment>();
        }

        public void SetAttachments(IEnumerable<CalendarAttachment> attachments)
        {
            Properties.UpdateLineProperty(attachments, ICalProperty.ATTACH);
        }
    }
}