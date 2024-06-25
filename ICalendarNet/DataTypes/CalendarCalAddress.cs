using ICalendarNet.Base;

namespace ICalendarNet.DataTypes
{
    public class CalendarCalAddress : ContentLine
    {
        public CalendarCalAddress(string name, string value, ContentLineParameters? parameter) : base(name, value, parameter)
        {
        }
    }
}