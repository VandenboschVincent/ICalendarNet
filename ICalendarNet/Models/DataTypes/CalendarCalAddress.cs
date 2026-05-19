using ICalendarNet.Models.Base;

namespace ICalendarNet.Models.DataTypes
{
    public class CalendarCalAddress(string name, string value, ContentLineParameters? parameter) : ContentLine(name, value, parameter)
    {
    }
}