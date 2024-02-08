using ICalendarNet.Base;
using ICalendarNet.Extensions;

namespace ICalendarNet.Components
{
    public class CalendarTodo : CalendarObject
    {
        public override ICalComponent ComponentType => ICalComponent.VTODO;
        public string? Summary
        {
            get => Properties.GetContentlineProperty("Summary");
            set => UpdateProperty("Summary", value!);
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
        /// https://datatracker.ietf.org/doc/html/rfc5545#section-3.6.2
        /// </summary>
        public CalendarTodo()
        {
        }
    }
}
