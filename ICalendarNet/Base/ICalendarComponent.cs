namespace ICalendarNet.Base
{
    public interface ICalendarComponent
    {
        ICalComponent ComponentType { get; }
        List<ICalendarProperty> Properties { get; }
        List<ICalendarComponent> SubComponents { get; }
        void AddProperty(string key, string value);
        void UpdateProperty(string key, string value);
        void UpdateProperty(string key, IEnumerable<string> value);
    }
}