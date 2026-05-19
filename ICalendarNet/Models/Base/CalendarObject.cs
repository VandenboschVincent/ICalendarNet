using ICalendarNet.Models.Enum;
using System.Collections.Generic;

namespace ICalendarNet.Models.Base
{
    public abstract class CalendarObject : ICalendarComponent
    {
        public abstract ICalComponent ComponentType { get; }
        public List<ICalendarProperty> Properties { get; } = [];
        public List<ICalendarComponent> SubComponents { get; } = [];
        public MetadataContainer Metadata { get; } = new MetadataContainer();
    }
}