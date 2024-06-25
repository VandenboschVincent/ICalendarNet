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
        ///   <see cref="ICalProperty.SUMMARY" />
        /// </summary>
        public virtual string? Summary
        {
            get => Properties.GetContentlineValue(ICalProperty.SUMMARY);
            set => Properties.UpdateLineProperty(value!, ICalProperty.SUMMARY);
        }

        /// <summary>
        ///   <see cref="ICalProperty.ACTION" />
        /// </summary>
        public virtual string? Action
        {
            get => Properties.GetContentlineValue(ICalProperty.ACTION);
            set => Properties.UpdateLineProperty(value!, ICalProperty.ACTION);
        }

        /// <summary>
        ///   <see cref="ICalProperty.Trigger" />
        /// </summary>
        public virtual string? Trigger
        {
            get => Properties.GetContentlineValue(ICalProperty.TRIGGER);
            set => Properties.UpdateLineProperty(value!, ICalProperty.TRIGGER);
        }

        /// <summary>
        ///   <see cref="ICalProperty.Duration" />
        /// </summary>
        public virtual TimeSpan? Duration
        {
            get => Properties.GetContentlineTimeSpan(ICalProperty.DURATION);
            set => Properties.UpdateLineProperty(value!, ICalProperty.DURATION);
        }

        /// <summary>
        ///   <see cref="ICalProperty.REPEAT" />
        /// </summary>
        public virtual double? Repeat
        {
            get => Properties.GetContentlineDouble(ICalProperty.REPEAT);
            set => Properties.UpdateLineProperty(value!, ICalProperty.REPEAT);
        }

        /// <summary>
        ///   <see cref="ICalProperty.ATTENDEE" />
        /// </summary>
        public virtual IEnumerable<string>? Attendee
        {
            get => Properties.GetContentlinesSeperatedValue(ICalProperty.ATTENDEE);
            set => Properties.UpdateLinesSeperatedProperty(value!.ToList(), ICalProperty.ATTENDEE);
        }

        /// <summary>
        ///   <see cref="ICalProperty.ATTACH" />
        /// </summary>
        public IEnumerable<CalendarAttachment> GetAttachments()
        {
            return Properties.GetContentlines(ICalProperty.ATTACH).Cast<CalendarAttachment>();
        }

        /// <summary>
        ///   <see cref="ICalProperty.ATTACH" />
        /// </summary>
        public void SetAttachments(IEnumerable<CalendarAttachment> attachments)
        {
            Properties.UpdateLineProperty(attachments, ICalProperty.ATTACH);
        }
    }
}