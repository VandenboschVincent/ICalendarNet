using ICalendarNet.Base;
using ICalendarNet.Extensions;

namespace ICalendarNet.Components
{
    public class CalendarAlarm : CalendarObject
    {
        public override ICalComponent ComponentType => ICalComponent.VALARM;

        public virtual string? Description
        {
            get => Properties.GetContentlineProperty("Description");
            set => UpdateProperty("Description", value!);
        }
        /// <summary>
        /// https://datatracker.ietf.org/doc/html/rfc5545#section-3.6.6
        /// </summary>
        public CalendarAlarm()
        {
        }
    }
}
