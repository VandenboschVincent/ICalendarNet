using System.Collections.Generic;
using static ICalendarNet.Statics;

namespace ICalendarNet.Base
{
    public interface ICalendarComponent
    {
        ICalComponent ComponentType { get; }
        List<ICalendarProperty> Properties { get; }
        List<ICalendarComponent> SubComponents { get; }
        void AddProperty(ICalProperty key, string value, ContentLineParameters? parameters = null);
        void UpdateProperty(ICalProperty key, IEnumerable<string> value, ContentLineParameters? parameters = null);
    }
}