using ICalendarNet.Base;

namespace ICalendarNet.Components
{
    public class CalendarDaylight : CalendarObject
    {
        public override ICalComponent ComponentType => ICalComponent.DAYLIGHT;
        public CalendarDaylight()
        {
        }

    }
}
