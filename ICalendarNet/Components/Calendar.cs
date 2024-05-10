using ICalendarNet.Base;
using ICalendarNet.Extensions;
using ICalendarNet.Serialization;
using System;
using System.Globalization;
using static ICalendarNet.Statics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ICalendarNet.Components
{
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


        public override ICalComponent ComponentType => ICalComponent.VCALENDAR;

        public string? Name
        {
            get => Properties.GetContentlineValue(ICalProperty.NAME);
            set => Properties.UpdateLineProperty(value!, ICalProperty.NAME);
        }

        public string? Summary
        {
            get => Properties.GetContentlineValue(ICalProperty.SUMMARY);
            set => Properties.UpdateLineProperty(value!, ICalProperty.SUMMARY);
        }

        public string? Description
        {
            get => Properties.GetContentlineValue(ICalProperty.DESCRIPTION);
            set => Properties.UpdateLineProperty(value!, ICalProperty.DESCRIPTION);
        }

        public string? Uid
        {
            get => Properties.GetContentlineValue(ICalProperty.UID);
            set => Properties.UpdateLineProperty(value!, ICalProperty.UID);
        }

        public string? Url
        {
            get => Properties.GetContentlineValue(ICalProperty.URL);
            set => Properties.UpdateLineProperty(value!, ICalProperty.URL);
        }

        public string? Owner
        {
            get => Properties.GetContentlineValue(ICalProperty.X_OWNER);
            set => Properties.UpdateLineProperty(value!, ICalProperty.X_OWNER);
        }

        public string? ETag
        {
            get => Properties.GetContentlineValue(ICalProperty.ETAG);
            set => Properties.UpdateLineProperty(value!, ICalProperty.ETAG);
        }

        /// <summary>
        /// This property defines the calendar scale used for the
        /// calendar information specified in the iCalendar object.
        /// default "GREGORIAN"
        /// </summary>
        public string? Scale
        {
            get => Properties.GetContentlineValue(ICalProperty.CALSCALE);
            set => Properties.UpdateLineProperty(value!, ICalProperty.CALSCALE);
        }

        /// <summary>
        /// This property defines the iCalendar object method
        /// associated with the calendar object.
        /// </summary>
        public string? Method
        {
            get => Properties.GetContentlineValue(ICalProperty.METHOD);
            set => Properties.UpdateLineProperty(value!, ICalProperty.METHOD);
        }

        /// <summary>
        /// This property specifies the identifier for the product that
        /// created the iCalendar object.
        /// </summary>
        public string? ProdId
        {
            get => Properties.GetContentlineValue(ICalProperty.METHOD);
            set => Properties.UpdateLineProperty(value!, ICalProperty.METHOD);
        }

        /// <summary>
        /// This property specifies the identifier corresponding to the
        /// highest version number or the minimum and maximum range of the
        /// iCalendar specification that is required in order to interpret the
        /// iCalendar object.
        /// </summary>
        public string? Version
        {
            get => Properties.GetContentlineValue(ICalProperty.VERSION);
            set => Properties.UpdateLineProperty(value!, ICalProperty.VERSION);
        }

        public DateTimeOffset? LastModified
        {
            get => Properties.GetContentlineDateTime(ICalProperty.LAST_MODIFIED);
            set => Properties.UpdateLineProperty(value!, ICalProperty.LAST_MODIFIED);
        }
        public string? SyncToken
        {
            get => Properties.GetContentlineValue(ICalProperty.SYNCTOKEN);
            set => Properties.UpdateLineProperty(value!, ICalProperty.SYNCTOKEN);
        }
        public string? Color
        {
            get => Properties.GetContentlineValue(ICalProperty.COLOR);
            set => Properties.UpdateLineProperty(value!, ICalProperty.COLOR);
        }
        public DateTimeOffset? Created
        {
            get => Properties.GetContentlineDateTime(ICalProperty.CREATED);
            set => Properties.UpdateLineProperty(value!, ICalProperty.CREATED);
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