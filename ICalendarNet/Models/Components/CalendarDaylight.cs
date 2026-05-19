using ICalendarNet.Models.Enum;

namespace ICalendarNet.Models.Components
{
    public class CalendarDaylight : CalendarStandard
    {
        public override ICalComponent ComponentType => ICalComponent.DAYLIGHT;
    }
}