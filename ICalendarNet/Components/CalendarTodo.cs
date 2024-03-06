using ICalendarNet.Base;
using ICalendarNet.Extensions;
using static ICalendarNet.Statics;

namespace ICalendarNet.Components
{
    public class CalendarTodo : CalendarObject
    {
        public override ICalComponent ComponentType => ICalComponent.VTODO;
        public string? Summary
        {
            get => Properties.GetContentlineValue(ICalProperty.SUMMARY);
            set => Properties.UpdateLineProperty(value!, ICalProperty.SUMMARY);
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
        /// https://datatracker.ietf.org/doc/html/rfc5545#section-3.6.2
        /// </summary>
        public CalendarTodo()
        {
        }
    }
}
