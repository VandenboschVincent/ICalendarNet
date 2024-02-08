using ICalendarNet.Base;

namespace ICalendarNet.Components
{
    public class CalendarTimeZone : CalendarObject
    {
        public override ICalComponent ComponentType => ICalComponent.VTIMEZONE;

        /// <summary>
        /// https://datatracker.ietf.org/doc/html/rfc5545#section-3.6.5
        /// </summary>
        public CalendarTimeZone()
        {
        }
    }
}
