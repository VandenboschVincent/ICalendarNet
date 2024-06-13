using ICalendarNet.Base;
using ICalendarNet.Extensions;
using ICalendarNet.Serialization;
using static ICalendarNet.Statics;

namespace ICalendarNet.Components
{
    /// <summary>
    ///   <see cref="ICalComponent.VCALENDAR" />
    /// </summary>
    public class Calendar : CalendarObject
    {
        public static Calendar? LoadCalendar(string source)
        {
            using ICalSerializor serializor = new();
            return serializor.DeserializeCalendar(source);
        }

        public static IEnumerable<Calendar> LoadCalendars(string source)
        {
            using ICalSerializor serializor = new();
            return serializor.DeserializeCalendars(source).ToList();
        }

        /// <summary>
        ///   <see cref="ICalComponent.VCALENDAR" />
        /// </summary>
        public override ICalComponent ComponentType => ICalComponent.VCALENDAR;

        /// <summary>
        ///   <see cref="ICalProperty.NAME" />
        /// </summary>
        public string? Name
        {
            get => Properties.GetContentlineValue(ICalProperty.NAME);
            set => Properties.UpdateLineProperty(value!, ICalProperty.NAME);
        }

        /// <summary>
        ///   <see cref="ICalProperty.SUMMARY" />
        /// </summary>
        public string? Summary
        {
            get => Properties.GetContentlineValue(ICalProperty.SUMMARY);
            set => Properties.UpdateLineProperty(value!, ICalProperty.SUMMARY);
        }

        /// <summary>
        ///   <see cref="ICalProperty.DESCRIPTION" />
        /// </summary>
        public string? Description
        {
            get => Properties.GetContentlineValue(ICalProperty.DESCRIPTION);
            set => Properties.UpdateLineProperty(value!, ICalProperty.DESCRIPTION);
        }

        /// <summary>
        ///   <see cref="ICalProperty.UID" />
        /// </summary>
        public string? Uid
        {
            get => Properties.GetContentlineValue(ICalProperty.UID);
            set => Properties.UpdateLineProperty(value!, ICalProperty.UID);
        }

        /// <summary>
        ///   <see cref="ICalProperty.URL" />
        /// </summary>
        public string? Url
        {
            get => Properties.GetContentlineValue(ICalProperty.URL);
            set => Properties.UpdateLineProperty(value!, ICalProperty.URL);
        }

        /// <summary>
        ///   <see cref="ICalProperty.X_OWNER" />
        /// </summary>
        public string? Owner
        {
            get => Properties.GetContentlineValue(ICalProperty.X_OWNER);
            set => Properties.UpdateLineProperty(value!, ICalProperty.X_OWNER);
        }

        /// <summary>
        ///   <see cref="ICalProperty.ETAG" />
        /// </summary>
        public string? ETag
        {
            get => Properties.GetContentlineValue(ICalProperty.ETAG);
            set => Properties.UpdateLineProperty(value!, ICalProperty.ETAG);
        }

        /// <summary>
        ///   <see cref="ICalProperty.CALSCALE" />
        /// </summary>
        public string? Scale
        {
            get => Properties.GetContentlineValue(ICalProperty.CALSCALE);
            set => Properties.UpdateLineProperty(value!, ICalProperty.CALSCALE);
        }

        /// <summary>
        ///   <see cref="ICalProperty.METHOD" />
        /// </summary>
        public string? Method
        {
            get => Properties.GetContentlineValue(ICalProperty.METHOD);
            set => Properties.UpdateLineProperty(value!, ICalProperty.METHOD);
        }

        /// <summary>
        ///   <see cref="ICalProperty.METHOD" />
        /// </summary>
        public string? ProdId
        {
            get => Properties.GetContentlineValue(ICalProperty.METHOD);
            set => Properties.UpdateLineProperty(value!, ICalProperty.METHOD);
        }

        /// <summary>
        ///   <see cref="ICalProperty.VERSION" />
        /// </summary>
        public string? Version
        {
            get => Properties.GetContentlineValue(ICalProperty.VERSION);
            set => Properties.UpdateLineProperty(value!, ICalProperty.VERSION);
        }

        /// <summary>
        ///   <see cref="ICalProperty.LAST_MODIFIED" />
        /// </summary>
        public DateTimeOffset? LastModified
        {
            get => Properties.GetContentlineDateTime(ICalProperty.LAST_MODIFIED);
            set => Properties.UpdateLineProperty(value!, ICalProperty.LAST_MODIFIED);
        }

        /// <summary>
        ///   <see cref="ICalProperty.SYNCTOKEN" />
        /// </summary>
        public string? SyncToken
        {
            get => Properties.GetContentlineValue(ICalProperty.SYNCTOKEN);
            set => Properties.UpdateLineProperty(value!, ICalProperty.SYNCTOKEN);
        }

        /// <summary>
        ///   <see cref="ICalProperty.COLOR" />
        /// </summary>
        public string? Color
        {
            get => Properties.GetContentlineValue(ICalProperty.COLOR);
            set => Properties.UpdateLineProperty(value!, ICalProperty.COLOR);
        }

        /// <summary>
        ///   <see cref="ICalProperty.CREATED" />
        /// </summary>
        public DateTimeOffset? Created
        {
            get => Properties.GetContentlineDateTime(ICalProperty.CREATED);
            set => Properties.UpdateLineProperty(value!, ICalProperty.CREATED);
        }

        /// <summary>
        ///   <see cref="ICalComponent.VEVENT" />
        /// </summary>
        public IEnumerable<CalendarEvent> GetEvents() => SubComponents.Where(t => t.ComponentType == ICalComponent.VEVENT).Cast<CalendarEvent>();

        /// <summary>
        ///   <see cref="ICalComponent.VTODO" />
        /// </summary>
        public IEnumerable<CalendarTodo> GetTodos() => SubComponents.Where(t => t.ComponentType == ICalComponent.VTODO).Cast<CalendarTodo>();

        /// <summary>
        ///   <see cref="ICalComponent.VJOURNAL" />
        /// </summary>
        public IEnumerable<CalendarJournal> GetJournals() => SubComponents.Where(t => t.ComponentType == ICalComponent.VJOURNAL).Cast<CalendarJournal>();

        /// <summary>
        ///   <see cref="ICalComponent.VFREEBUSY" />
        /// </summary>
        public IEnumerable<CalendarFreeBusy> GetFreeBusy() => SubComponents.Where(t => t.ComponentType == ICalComponent.VFREEBUSY).Cast<CalendarFreeBusy>();

        /// <summary>
        ///   <see cref="ICalComponent.VTIMEZONE" />
        /// </summary>
        public IEnumerable<CalendarTimeZone> GetTimeZones() => SubComponents.Where(t => t.ComponentType == ICalComponent.VTIMEZONE).Cast<CalendarTimeZone>();
    }
}