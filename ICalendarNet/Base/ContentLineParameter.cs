namespace ICalendarNet.Base
{
    public class ContentLineParameter
    {
        public string Name { get; set; }
        public IEnumerable<string> Values { get; set; }

        public ContentLineParameter(string name, IEnumerable<string> values)
        {
            Name = name;
            Values = values;
        }
        public ContentLineParameter(string name, string value)
        {
            Name = name;
            Values = new[] { value };
        }

        public override string ToString()
        {
            return $"{Name}={string.Join(",", Values)}";
        }
    }
}