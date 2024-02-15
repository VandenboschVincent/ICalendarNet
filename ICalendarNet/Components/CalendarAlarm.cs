using ICalendarNet.Base;
using ICalendarNet.DataTypes;
using ICalendarNet.Extensions;

namespace ICalendarNet.Components
{
    public class CalendarAlarm : CalendarObject
    {
        public override ICalComponent ComponentType => ICalComponent.VALARM;

        public virtual string? Description
        {
            get => Properties.GetContentlineProperty("Description");
            set => UpdateProperty("Description", value!);
        }

        /// <summary>
        /// Sets the subject of email when trigger is EMAIL
        /// </summary>
        public virtual string? Summary
        {
            get => Properties.GetContentlineProperty("SUMMARY");
            set => UpdateProperty("SUMMARY", value!);
        }

        /// <summary>
        /// Can be AUDIO, DISPLAY, EMAIL
        /// AUDIO: Raises sound found in the Attach property
        /// DISPLAY: displays text from Description property
        /// EMAIL: sends email to one or more Attendee properties, Summary as subject, Description as body, Attach as attachments
        /// </summary>
        public virtual string? Action
        {
            get => Properties.GetContentlineProperty("Action");
            set => UpdateProperty("Action", value!);
        }

        /// <summary>
        /// Relative to the DTStart of the parent object
        /// Can be repeated if Duration(delay) and Repeat are present
        /// </summary>
        public virtual DateTimeOffset? Trigger
        {
            get => Properties.GetContentlineDateTime("Trigger");
            set => Properties.UpdateLineProperty(value!, "Trigger");
        }

        public virtual TimeSpan? Duration
        {
            get => Properties.GetContentlineTimeSpan("Duration");
            set => Properties.UpdateLineProperty(value, "DURATION");
        }

        /// <summary>
        /// Sets how many times the action will repeat
        /// </summary>
        public virtual double? Repeat
        {
            get => Properties.GetContentlineDouble("Repeat");
            set => UpdateProperty("Repeat", value.ToString()!);
        }

        /// <summary>
        /// Sets the receivers when Action is set to Email
        /// </summary>
        public virtual IEnumerable<string>? Attendee
        {
            get => Properties.GetContentlinesProperty("ATTENDEE");
            set => UpdateProperty("ATTENDEE", value!);
        }

        public IEnumerable<CalendarAttachment> GetAttachments()
        {
            return Properties.GetContentlines("ATTACH").Cast<CalendarAttachment>();
        }

        public void SetAttachments(IEnumerable<CalendarAttachment> attachments)
        {
            Properties.UpdateLineProperty(attachments, "ATTACH");
        }

        /// <summary>
        /// https://datatracker.ietf.org/doc/html/rfc5545#section-3.6.6
        /// </summary>
        public CalendarAlarm()
        {
        }
    }
}
