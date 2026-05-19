using ICalendarNet.Extensions;
using ICalendarNet.Models.Base;
using ICalendarNet.Models.DataTypes;
using ICalendarNet.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using static ICalendarNet.Models.Enum.Statics;

namespace ICalendarNet.Models.Components
{
    /// <summary>
    /// https://datatracker.ietf.org/doc/html/rfc5545#section-3.6.3
    /// </summary>
    public class CalendarJournal : CalendarRecurrableObject
    {
        public override ICalComponent ComponentType => ICalComponent.VJOURNAL;

        /// <summary>
        ///   <see cref="ICalProperty.DESCRIPTION" />
        /// </summary>
        public IEnumerable<string>? Descriptions
        {
            get => Properties.GetContentlinesValue(ICalProperty.DESCRIPTION);
            set => Properties.UpdateLinesProperty(value, ICalProperty.DESCRIPTION);
        }

        /// <summary>
        ///   <see cref="ICalProperty.DTSTAMP" />
        /// </summary>
        public DateTimeOffset? DateTimeStamp
        {
            get => Properties.GetContentlineDateTime(ICalProperty.DTSTAMP, Metadata);
            set => Properties.UpdateLineProperty(value, ICalProperty.DTSTAMP);
        }

        /// <summary>
        ///   <see cref="ICalProperty.CREATED" />
        /// </summary>
        public DateTimeOffset? Created
        {
            get => Properties.GetContentlineDateTime(ICalProperty.CREATED, Metadata);
            set => Properties.UpdateLineProperty(value, ICalProperty.CREATED);
        }

        /// <summary>
        ///   <see cref="ICalProperty.CATEGORIES" />
        /// </summary>
        public IEnumerable<string> Categories
        {
            get => Properties.GetContentlinesSeperatedValue(ICalProperty.CATEGORIES, ICalProperty.CATEGORY);
            set => Properties.UpdateLinesSeperatedProperty(value, ICalProperty.CATEGORIES);
        }

        /// <summary>
        ///   <see cref="ICalProperty.ORGANIZER" />
        /// </summary>
        public string? Organizer
        {
            get => Properties.GetContentlineValue(ICalProperty.ORGANIZER);
            set => Properties.UpdateLineProperty(value, ICalProperty.ORGANIZER);
        }

        /// <summary>
        ///   <see cref="ICalProperty.STATUS" />
        /// </summary>
        public string? Status
        {
            get => Properties.GetContentlineValue(ICalProperty.STATUS);
            set => Properties.UpdateLineProperty(value, ICalProperty.STATUS);
        }

        /// <summary>
        ///   <see cref="ICalProperty.SUMMARY" />
        /// </summary>
        public string? Summary
        {
            get => Properties.GetContentlineValue(ICalProperty.SUMMARY);
            set => Properties.UpdateLineProperty(value, ICalProperty.SUMMARY);
        }

        /// <summary>
        ///   <see cref="ICalProperty.CLASS" />
        /// </summary>
        public string? Class
        {
            get => Properties.GetContentlineValue(ICalProperty.CLASS);
            set => Properties.UpdateLineProperty(value, ICalProperty.CLASS);
        }

        /// <summary>
        ///   <see cref="ICalProperty.URL" />
        /// </summary>
        public string? Url
        {
            get => Properties.GetContentlineValue(ICalProperty.URL);
            set => Properties.UpdateLineProperty(value, ICalProperty.URL);
        }

        /// <summary>
        ///   <see cref="ICalProperty.LAST_MODIFIED" />
        /// </summary>
        public DateTimeOffset? LastModified
        {
            get => Properties.GetContentlineDateTime(ICalProperty.LAST_MODIFIED, Metadata);
            set => Properties.UpdateLineProperty(value, ICalProperty.LAST_MODIFIED);
        }

        /// <summary>
        ///   <see cref="ICalProperty.COMMENT" />
        /// </summary>
        public virtual string? Comment
        {
            get => string.Join(Environment.NewLine, Properties.GetContentlinesValue(ICalProperty.COMMENT));
            set => Properties.UpdateLineProperty(value, ICalProperty.COMMENT);
        }

        /// <summary>
        ///   <see cref="ICalProperty.CONTACT" />
        /// </summary>
        public string? Contact
        {
            get => Properties.GetContentlineValue(ICalProperty.CONTACT);
            set => Properties.UpdateLineProperty(value, ICalProperty.CONTACT);
        }

        /// <summary>
        ///   <see cref="ICalProperty.REQUEST_STATUS" />
        /// </summary>
        public string? RequestStatus
        {
            get => Properties.GetContentlineValue(ICalProperty.REQUEST_STATUS);
            set => Properties.UpdateLineProperty(value, ICalProperty.REQUEST_STATUS);
        }

        /// <summary>
        ///   <see cref="ICalProperty.ATTENDEE" />
        /// </summary>
        public virtual IEnumerable<string>? Attendee
        {
            get => Properties.GetContentlinesSeperatedValue(ICalProperty.ATTENDEE);
            set => Properties.UpdateLinesSeperatedProperty(value, ICalProperty.ATTENDEE);
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

        protected override CalendarRecurrableObject Clone(CalendarPeriod period)
        {
            return CloneComponent(this, period);
        }
    }
}