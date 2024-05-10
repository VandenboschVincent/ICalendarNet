using ICalendarNet.Base;
using ICalendarNet.DataTypes;
using ICalendarNet.Extensions;
using static ICalendarNet.Statics;

namespace ICalendarNet.Components
{
    public class CalendarJournal : CalendarObject
    {
        public override ICalComponent ComponentType => ICalComponent.VJOURNAL;
        public string? Description
        {
            get => Properties.GetContentlineValue(ICalProperty.DESCRIPTION);
            set => Properties.UpdateLineProperty(value!, ICalProperty.DESCRIPTION);
        }
        public string? Uid
        {
            get => Properties.GetContentlineValue(ICalProperty.UID);
            set => Properties.UpdateLineProperty(value!, ICalProperty.UID);
        }
        public DateTimeOffset? DTSTAMP
        {
            get => Properties.GetContentlineDateTime(ICalProperty.DTSTAMP);
            set => Properties.UpdateLineProperty(value!, ICalProperty.DTSTAMP);
        }
        public DateTimeOffset? Created
        {
            get => Properties.GetContentlineDateTime(ICalProperty.CREATED);
            set => Properties.UpdateLineProperty(value!, ICalProperty.CREATED);
        }

        /// <summary>
        /// This property defines the categories for a calendar component.
        /// </summary>
        public IEnumerable<string> Categories
        {
            get => Properties.GetContentlinesValue(ICalProperty.CATEGORIES);
            set => Properties.UpdateLinesProperty(value!, ICalProperty.CATEGORIES);
        }

        /// <summary>
        /// This property provides the capability to associate a
        /// document object with a calendar component.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CalendarAttachment> GetAttachments()
        {
            return Properties.GetContentlines(ICalProperty.ATTACH).Cast<CalendarAttachment>();
        }

        /// <summary>
        /// This property provides the capability to associate a
        /// document object with a calendar component.
        /// </summary>
        /// <returns></returns>
        public void SetAttachments(IEnumerable<CalendarAttachment> attachments)
        {
            Properties.UpdateLineProperty(attachments, ICalProperty.ATTACH);
        }

        /// <summary>
        /// https://datatracker.ietf.org/doc/html/rfc5545#section-3.6.3
        /// </summary>
        public CalendarJournal()
        {
        }
    }
}
