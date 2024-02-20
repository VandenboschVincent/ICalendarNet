using ICalendarNet.Base;

namespace ICalendarNet.DataTypes
{
    public class CalendarDefaultDataType : ContentLine
    {
        public CalendarDefaultDataType(string name, string value, ContentLineParameters? param) : base(name, value, param)
        {
        }

        public override string ToString()
        {
            return $"{Name}:{Value}";
        }
    }
}
