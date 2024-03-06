using ICalendarNet.Base;
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
        /// <summary>
        /// https://datatracker.ietf.org/doc/html/rfc5545#section-3.6.3
        /// </summary>
        public CalendarJournal()
        {
        }
    }
}
