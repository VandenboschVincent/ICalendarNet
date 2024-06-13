using ICalendarNet.Base;
using ICalendarNet.Extensions;
using static ICalendarNet.Statics;

namespace ICalendarNet.Components
{
    /// <summary>
    /// https://datatracker.ietf.org/doc/html/rfc5545#section-3.6.4
    /// </summary>
    public class CalendarFreeBusy : CalendarObject
    {
        public override ICalComponent ComponentType => ICalComponent.VFREEBUSY;

        /// <summary>
        ///   <see cref="ICalProperty.DTEND" />
        /// </summary>
        public DateTimeOffset? DTEND
        {
            get => Properties.GetContentlineDateTime(ICalProperty.DTEND);
            set => Properties.UpdateLineProperty(value!, ICalProperty.DTEND);
        }

        /// <summary>
        ///   <see cref="ICalProperty.UID" />
        /// </summary>
        public string? Uid
        {
            get => Properties.GetContentlineValue(ICalProperty.UID);
            set => Properties.UpdateLineProperty(value!, ICalProperty.UID);
        }

        /// <summary>
        ///   <see cref="ICalProperty.URL" />
        /// </summary>
        public string? Url
        {
            get => Properties.GetContentlineValue(ICalProperty.URL);
            set => Properties.UpdateLineProperty(value!, ICalProperty.URL);
        }

        /// <summary>
        ///   <see cref="ICalProperty.COMMENT" />
        /// </summary>
        public virtual string? Comment
        {
            get => Properties.GetContentlineValue(ICalProperty.COMMENT);
            set => Properties.UpdateLineProperty(value!, ICalProperty.COMMENT);
        }

        /// <summary>
        ///   <see cref="ICalProperty.DTSTART" />
        /// </summary>
        public DateTimeOffset? DTSTART
        {
            get => Properties.GetContentlineDateTime(ICalProperty.DTSTART);
            set => Properties.UpdateLineProperty(value!, ICalProperty.DTSTART);
        }

        /// <summary>
        ///   <see cref="ICalProperty.CONTACT" />
        /// </summary>
        public string? Contact
        {
            get => Properties.GetContentlineValue(ICalProperty.CONTACT);
            set => Properties.UpdateLineProperty(value!, ICalProperty.CONTACT);
        }

        /// <summary>
        ///   <see cref="ICalProperty.DTSTAMP" />
        /// </summary>
        public DateTimeOffset? DTSTAMP
        {
            get => Properties.GetContentlineDateTime(ICalProperty.DTSTAMP);
            set => Properties.UpdateLineProperty(value!, ICalProperty.DTSTAMP);
        }

        /// <summary>
        ///   <see cref="ICalProperty.REQUEST_STATUS" />
        /// </summary>
        public string? RequestStatus
        {
            get => Properties.GetContentlineValue(ICalProperty.REQUEST_STATUS);
            set => Properties.UpdateLineProperty(value!, ICalProperty.REQUEST_STATUS);
        }
    }
}