using ICalendarNet.Base;
using ICalendarNet.DataTypes;
using ICalendarNet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
        public virtual AlarmAction Action
        {
            get => Properties.GetContentlineValue<AlarmAction>(ICalProperty.ACTION, AlarmAction.DISPLAY.ToString());
            set => Properties.UpdateLineProperty(value!, ICalProperty.ACTION);
        }

        /// <summary>
        ///   <see cref="ICalProperty.Trigger" />
        /// </summary>
        public virtual CalendarTrigger? Trigger
        {
            get => (CalendarTrigger?)Properties.GetContentlines(ICalProperty.TRIGGER).FirstOrDefault();
            set => Properties.UpdateLineProperty(value!.Value, ICalProperty.TRIGGER, value!.Parameters);
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

        public CalendarAlarm()
        { }

        /// <summary>
        /// Add the Alarm as an email
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="emailAdresses"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public CalendarAlarm(CalendarTrigger trigger, IEnumerable<string> emailAdresses, string subject, string body)
        {
            Trigger = trigger;
            Action = AlarmAction.EMAIL;
            Description = body;
            Attendee = emailAdresses;
            Summary = subject;
        }

        /// <summary>
        /// Add the alarm as a notification
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="notification"></param>
        public CalendarAlarm(CalendarTrigger trigger, string notification)
        {
            Trigger = trigger;
            Action = AlarmAction.DISPLAY;
            Description = notification;
        }
    }
}