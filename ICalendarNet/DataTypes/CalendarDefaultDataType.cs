using ICalendarNet.Base;

namespace ICalendarNet.DataTypes
{
    public class CalendarDefaultDataType : ContentLine
    {
        public CalendarDefaultDataType(Statics.ICalProperty key, string value, ContentLineParameters? param) : base(Statics.ICalProperties[(int)key], value, param)
        {
        }
        public CalendarDefaultDataType(string key, string value, ContentLineParameters? param) : base(key, value, param)
        {
        }

        public override string ToString()
        {
            return $"{Name}:{Value}";
        }
    }
}
