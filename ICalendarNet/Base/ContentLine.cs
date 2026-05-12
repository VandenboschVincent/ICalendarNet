using ICalendarNet.Components;
using ICalendarNet.Extensions;
using ICalendarNet.Models;
using static ICalendarNet.Statics;

namespace ICalendarNet.Base
{
    public abstract class ContentLine : ICalendarProperty
    {
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
        protected ContentLine(string name, string value, ContentLineParameters? parameter)
        {
            Name = name;
            Value = value;
            Parameters = parameter ?? new ContentLineParameters();
        }
    }
}