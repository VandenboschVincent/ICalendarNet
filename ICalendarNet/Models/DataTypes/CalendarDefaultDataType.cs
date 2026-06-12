using ICalendarNet.Extensions;
using ICalendarNet.Models.Base;
using ICalendarNet.Models.Enum;

namespace ICalendarNet.Models.DataTypes
{
    public class CalendarDefaultDataType : ContentLine
    {
        public CalendarDefaultDataType(Statics.ICalProperty key, string value, ContentLineParameters? param) : base(key.GetString(), value, param)
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