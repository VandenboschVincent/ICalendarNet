using ICalendarNet.Base;
using ICalendarNet.Extensions;

namespace ICalendarNet.Components
{
    public class CalendarJournal : CalendarObject
    {
        public override ICalComponent ComponentType => ICalComponent.VJOURNAL;
        public string? Description
        {
            get => Properties.GetContentlineProperty("Description");
            set => UpdateProperty("Description", value!);
        }
        public string? Uid
        {
            get => Properties.GetContentlineProperty("Uid");
            set => UpdateProperty("Uid", value!);
        }
        public DateTimeOffset? DTSTAMP
        {
            get => Properties.GetContentlineDateTime("DTSTAMP");
            set => Properties.UpdateLineProperty(value!, "DTSTAMP");
        }
        /// <summary>
        /// https://datatracker.ietf.org/doc/html/rfc5545#section-3.6.3
        /// </summary>
        public CalendarJournal()
        {
        }
    }
}
