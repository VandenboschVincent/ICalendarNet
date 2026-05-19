using ICalendarNet.Models.Enum;
using System.Collections.Generic;

namespace ICalendarNet.Models.Base
{
    public interface ICalendarComponent
    {
        ICalComponent ComponentType { get; }
        List<ICalendarProperty> Properties { get; }
        List<ICalendarComponent> SubComponents { get; }
        MetadataContainer Metadata { get; }
    }
}