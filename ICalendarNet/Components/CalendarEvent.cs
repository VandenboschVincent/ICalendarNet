using ICalendarNet;
using ICalendarNet.Base;
using ICalendarNet.Components;
using ICalendarNet.DataTypes;
using ICalendarNet.Extensions;

namespace iCalNET
{
    public class CalendarEvent : CalendarObject
    {
        public override ICalComponent ComponentType => ICalComponent.VEVENT;
        public string? Name
        {
            get => Properties.GetContentlineProperty("Name");
            set => UpdateProperty("Name", value!);
        }
        public string? Summary
        {
            get => Properties.GetContentlineProperty("Summary");
            set => UpdateProperty("Summary", value!);
        }
        public string? Description
        {
            get => Properties.GetContentlineProperty("Description");
            set => UpdateProperty("Description", value!);
        }
        public string? Uid
        {
            get => Properties.GetContentlineProperty("Uid");
            set => UpdateProperty("Uid", value!);
        }
        public string? Url
        {
            get => Properties.GetContentlineProperty("URL");
            set => UpdateProperty("URL", value!);
        }
        public IEnumerable<string> Categories
        {
            get => Properties.GetContentlinesProperty("Categories");
            set => UpdateProperty("Categories", value!);
        }
        public DateTimeOffset? DTSTART
        {
            get => Properties.GetContentlineDateTime("DTSTART");
            set => Properties.UpdateLineProperty(value!, "DTSTART");
        }
        public DateTimeOffset? DTEND
        {
            get => Properties.GetContentlineDateTime("DTEND");
            set => Properties.UpdateLineProperty(value!, "DTEND");
        }
        public DateTimeOffset? DTSTAMP
        {
            get => Properties.GetContentlineDateTime("DTSTAMP");
            set => Properties.UpdateLineProperty(value!, "DTSTAMP");
        }
        public IEnumerable<CalendarAttachment> GetAttachments()
        {
            return Properties.GetContentlines("ATTACH").Cast<CalendarAttachment>();
        }
        public void SetAttachments(IEnumerable<CalendarAttachment> attachments)
        {
            Properties.UpdateLineProperty(attachments, "ATTACH");
        }

        public IEnumerable<CalendarAlarm> GetAlarms() => SubComponents.Where(t => t.ComponentType == ICalComponent.VALARM).Cast<CalendarAlarm>();

        /// <summary>
        /// https://datatracker.ietf.org/doc/html/rfc5545#section-3.6.1
        /// </summary>
        public CalendarEvent()
        {
        }

        public override string ToString()
        {
            return $"VEVENT: {Summary} {DTSTART.GetValueOrDefault():dd/MM/yy HH:mm} - {DTEND.GetValueOrDefault():dd/MM/yy HH:mm}";
        }
    }
}