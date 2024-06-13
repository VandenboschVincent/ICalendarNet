using ICalendarNet.Base;

namespace ICalendarNet.Components
{
    /// <summary>
    /// https://datatracker.ietf.org/doc/html/rfc5545#section-3.6.5
    /// </summary>
    public class CalendarTimeZone : CalendarObject
    {
        public override ICalComponent ComponentType => ICalComponent.VTIMEZONE;
    }
}