using ICalendarNet.Base;
using ICalendarNet.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICalendarNet.Models
{
    public class CalendarOccurrence
    {
        public DateTimeOffset Period { get; set; }
        public CalendarRecurrableObject Source { get; set; }

        public CalendarOccurrence(CalendarOccurrence ao)
        {
            Period = ao.Period;
            Source = ao.Source;
        }

        public CalendarOccurrence(CalendarRecurrableObject recurrable, DateTimeOffset period)
        {
            Source = recurrable;
            Period = period;
        }

        public override string ToString() => $"Occurrence {Source.GetType().Name} ({Period})";
    }
}
