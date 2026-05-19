using ICalendarNet.Extensions;
using ICalendarNet.Models.Enum;
using System.Collections.Generic;
using static ICalendarNet.Models.Enum.Statics;

namespace ICalendarNet.Models.Base
{
    public abstract class CalendarObject : ICalendarComponent
    {
        public abstract ICalComponent ComponentType { get; }
        public List<ICalendarProperty> Properties { get; } = [];
        public List<ICalendarComponent> SubComponents { get; } = [];
        public MetadataContainer Metadata { get; } = new MetadataContainer();

        public void AddProperty(ICalProperty key, string value, ContentLineParameters? parameters = null)
        {
            Properties.UpdateLineProperty(value, key, parameters);
        }

        public void UpdateProperty(ICalProperty key, IEnumerable<string> value, ContentLineParameters? parameters = null)
        {
            Properties.UpdateLinesProperty(value, key, parameters);
        }
    }
}