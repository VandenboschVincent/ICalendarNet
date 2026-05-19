using ICalendarNet.Extensions;
using ICalendarNet.Models.Components;
using static ICalendarNet.Models.Enum.Statics;

namespace ICalendarNet.Models.Base
{
    public abstract class ContentLine : ICalendarProperty
    {
        protected ContentLine(string name, string value, ContentLineParameters? parameters = null)
        {
            Name = name;
            Value = value;
            Parameters = parameters ?? new ContentLineParameters();
        }

        public string Name { get; set; }
        public virtual string Value { get; set; }
        public ContentLineParameters Parameters { get; set; }
        public MetadataContainer Metadata { get; } = new MetadataContainer();

        protected CalendarTimeZone? GetTimeZone()
        {
            if (Parameters.GetValue(ICalProperties[(int)ICalProperty.TZID]) is string tzid)
            {
                return Metadata.GetTimeZone(tzid);
            }
            return null;
        }
    }
}