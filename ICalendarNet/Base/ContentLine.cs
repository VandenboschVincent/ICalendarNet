namespace ICalendarNet.Base
{
    public abstract class ContentLine : ICalendarProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public ContentLineParameters Parameters { get; set; }
        protected ContentLine(string name, string value, ContentLineParameters? parameter)
        {
            Name = name;
            Value = value;
            Parameters = parameter ?? new ContentLineParameters();
        }
    }
}