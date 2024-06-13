using ICalendarNet.Base;

namespace ICalendarNet.Components
{
    /// <summary>
    /// https://datatracker.ietf.org/doc/html/rfc5545#section-3.6.4
    /// </summary>
    public class CalendarFreeBusy : CalendarObject
    {
        public override ICalComponent ComponentType => ICalComponent.VFREEBUSY;
    }
}