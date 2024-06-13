using ICalendarNet.Base;
using ICalendarNet.DataTypes;
using ICalendarNet.Extensions;
using static ICalendarNet.Statics;

namespace ICalendarNet.Components
{
    /// <summary>
    /// https://datatracker.ietf.org/doc/html/rfc5545#section-3.6.1
    /// </summary>
    public class CalendarEvent : CalendarObject
    {
        public override ICalComponent ComponentType => ICalComponent.VEVENT;

        public string? Name
        {
            get => Properties.GetContentlineValue(ICalProperty.NAME);
            set => Properties.UpdateLineProperty(value!, ICalProperty.NAME);
        }

        public string? Summary
        {
            get => Properties.GetContentlineValue(ICalProperty.SUMMARY);
            set => Properties.UpdateLineProperty(value!, ICalProperty.SUMMARY);
        }

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

        public string? Url
        {
            get => Properties.GetContentlineValue(ICalProperty.URL);
            set => Properties.UpdateLineProperty(value!, ICalProperty.URL);
        }

        public string? Location
        {
            get => Properties.GetContentlineValue(ICalProperty.LOCATION);
            set => Properties.UpdateLineProperty(value!, ICalProperty.LOCATION);
        }

        /// <summary>
        /// This property defines the categories for a calendar component.
        /// </summary>
        public IEnumerable<string> Categories
        {
            get => Properties.GetContentlinesValue(ICalProperty.CATEGORIES);
            set => Properties.UpdateLinesProperty(value!, ICalProperty.CATEGORIES);
        }

        public DateTimeOffset? DTSTART
        {
            get => Properties.GetContentlineDateTime(ICalProperty.DTSTART);
            set => Properties.UpdateLineProperty(value!, ICalProperty.DTSTART);
        }

        public DateTimeOffset? DTEND
        {
            get => Properties.GetContentlineDateTime(ICalProperty.DTEND);
            set => Properties.UpdateLineProperty(value!, ICalProperty.DTEND);
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

        public IEnumerable<CalendarAlarm> GetAlarms() => SubComponents.Where(t => t.ComponentType == ICalComponent.VALARM).Cast<CalendarAlarm>();

        public override string ToString()
        {
            return $"VEVENT: {Summary} {DTSTART.GetValueOrDefault():dd/MM/yy HH:mm} - {DTEND.GetValueOrDefault():dd/MM/yy HH:mm}";
        }
    }
}