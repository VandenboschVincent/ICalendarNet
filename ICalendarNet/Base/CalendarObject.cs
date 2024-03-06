
using ICalendarNet.Extensions;
using static ICalendarNet.Statics;

namespace ICalendarNet.Base
{
    public abstract class CalendarObject : ICalendarComponent
    {
        public abstract ICalComponent ComponentType { get; }
        public List<ICalendarProperty> Properties { get; } = [];
        public List<ICalendarComponent> SubComponents { get; } = [];

        public void AddProperty(ICalProperty key, string value, ContentLineParameters? parameters = null)
        {
            Properties.UpdateLineProperty(value!, key, parameters);
        }

        public void UpdateProperty(string key, string value, ContentLineParameters? parameters = null)
        {
            Properties.UpdateLineProperty(value!, key, parameters);
        }

        public void UpdateProperty(string key, IEnumerable<string> value, ContentLineParameters? parameters = null)
        {
            Properties.UpdateLinesProperty(value!, key, parameters);
        }
        public void UpdateProperty(ICalProperty key, IEnumerable<string> value, ContentLineParameters? parameters = null)
        {
            Properties.UpdateLinesProperty(value!, key, parameters);
        }
    }
}