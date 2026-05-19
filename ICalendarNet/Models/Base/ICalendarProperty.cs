namespace ICalendarNet.Models.Base
{
    public interface ICalendarProperty
    {
        string Name { get; set; }
        string Value { get; set; }
        ContentLineParameters Parameters { get; set; }
        MetadataContainer Metadata { get; }
    }
}