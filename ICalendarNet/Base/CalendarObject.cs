using ICalendarNet.Extensions;
using static ICalendarNet.Statics;
using System.Collections.Generic;

namespace ICalendarNet.Base
{
    public abstract class CalendarObject : ICalendarComponent
    {
        public abstract ICalComponent ComponentType { get; }
        public List<ICalendarProperty> Properties { get; } = new List<ICalendarProperty>();
        public List<ICalendarComponent> SubComponents { get; } = new List<ICalendarComponent>();

        public void AddProperty(ICalProperty key, string value, ContentLineParameters? parameters = null)
        {
            Properties.UpdateLineProperty(value!, key, parameters);
        }

        public void UpdateProperty(ICalProperty key, IEnumerable<string> value, ContentLineParameters? parameters = null)
        {
            Properties.UpdateLinesProperty(value!, key, parameters);
        }

    }
}