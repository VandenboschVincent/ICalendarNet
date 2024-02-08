using ICalendarNet.Base;

namespace ICalendarNet.Components
{
    public class CalendarStandard : CalendarObject
    {
        public override ICalComponent ComponentType => ICalComponent.STANDARD;
        public CalendarStandard()
        {
        }
    }
}
