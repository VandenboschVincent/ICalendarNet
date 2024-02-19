using ICalendarNet.Base;
using ICalendarNet.Extensions;
using ICalendarNet.Serialization;

namespace ICalendarNet.Components
{
    public class Calendar : CalendarObject
    {
        public static async Task<Calendar?> LoadCalendarAsync(string source)
        {
            return await new ICalSerializor().DeserializeCalendar(source);
        }
        public static async Task<IEnumerable<Calendar>> LoadCalendarsAsync(string source)
        {
            return await new ICalSerializor().DeserializeCalendars(source);
        }


        public override ICalComponent ComponentType => ICalComponent.VCALENDAR;

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
        public string? Owner
        {
            get => Properties.GetContentlineProperty("X-Owner");
            set => UpdateProperty("X-Owner", value!);
        }
        public string? ETag
        {
            get => Properties.GetContentlineProperty("ETag");
            set => UpdateProperty("ETag", value!);
        }
        public DateTimeOffset? LastModified
        {
            get => Properties.GetContentlineDateTime("LAST-MODIFIED");
            set => Properties.UpdateLineProperty(value, "LAST-MODIFIED");
        }
        public string? SyncToken
        {
            get => Properties.GetContentlineProperty("SyncToken");
            set => UpdateProperty("SyncToken", value!);
        }
        public string? Color
        {
            get => Properties.GetContentlineProperty("Color");
            set => UpdateProperty("Color", value!);
        }
        public DateTimeOffset? Created
        {
            get => Properties.GetContentlineDateTime("Created");
            set => Properties.UpdateLineProperty(value, "Created");
        }

        public IEnumerable<CalendarEvent> GetEvents() => SubComponents.Where(t => t.ComponentType == ICalComponent.VEVENT).Cast<CalendarEvent>();
        public IEnumerable<CalendarTodo> GetTodos() => SubComponents.Where(t => t.ComponentType == ICalComponent.VTODO).Cast<CalendarTodo>();
        public IEnumerable<CalendarJournal> GetJournals() => SubComponents.Where(t => t.ComponentType == ICalComponent.VJOURNAL).Cast<CalendarJournal>();
        public IEnumerable<CalendarFreeBusy> GetFreeBusy() => SubComponents.Where(t => t.ComponentType == ICalComponent.VFREEBUSY).Cast<CalendarFreeBusy>();
        public IEnumerable<CalendarTimeZone> GetTimeZones() => SubComponents.Where(t => t.ComponentType == ICalComponent.VTIMEZONE).Cast<CalendarTimeZone>();

        public Calendar()
        {
        }
    }
}