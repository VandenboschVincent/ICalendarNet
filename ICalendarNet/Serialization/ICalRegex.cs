using System.Text.RegularExpressions;

namespace ICalendarNet.Serialization
{
    public partial class ICalSerializor
    {
        private Regex GetObjectRegex(ICalComponent component)
        {
            switch (component)
            {
                case ICalComponent.VCALENDAR:
                    return vCalendarRegex();
                case ICalComponent.VEVENT:
                    return vEventRegex();
                case ICalComponent.VTODO:
                    return vTodoRegex();
                case ICalComponent.VJOURNAL:
                    return vJournalRegex();
                case ICalComponent.VFREEBUSY:
                    return vFreeBusyRegex();
                case ICalComponent.VTIMEZONE:
                    return vTimeZoneRegex();
                case ICalComponent.STANDARD:
                    return vStandardRegex();
                case ICalComponent.DAYLIGHT:
                    return vDaylightRegex();
                case ICalComponent.VALARM:
                    return vAlarmRegex();
                default:
                    break;
            }
            throw new NotSupportedException(component.ToString());
        }
        private Regex GetContentRegex(ICalComponent component)
        {
            switch (component)
            {
                case ICalComponent.VCALENDAR:
                    return vCalendarContentRegex();
                case ICalComponent.VEVENT:
                    return vEventContentRegex();
                case ICalComponent.VTODO:
                    return vTodoContentRegex();
                case ICalComponent.VJOURNAL:
                    return vJournalContentRegex();
                case ICalComponent.VFREEBUSY:
                    return vFreeBusyContentRegex();
                case ICalComponent.VTIMEZONE:
                    return vTimeZoneContentRegex();
                case ICalComponent.STANDARD:
                    return vStandardContentRegex();
                case ICalComponent.DAYLIGHT:
                    return vDaylightContentRegex();
                case ICalComponent.VALARM:
                    return vAlarmContentRegex();
                default:
                    break;
            }
            throw new NotSupportedException(component.ToString());
        }

        [GeneratedRegex("BEGIN:VCALENDAR(.+?)END:VCALENDAR", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
        internal static partial Regex vCalendarRegex();
        [GeneratedRegex("BEGIN:VEVENT(.+?)END:VEVENT", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
        internal static partial Regex vEventRegex();
        [GeneratedRegex("BEGIN:VTODO(.+?)END:VTODO", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
        internal static partial Regex vTodoRegex();
        [GeneratedRegex("BEGIN:VJOURNAL(.+?)END:VJOURNAL", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
        internal static partial Regex vJournalRegex();
        [GeneratedRegex("BEGIN:VALARM(.+?)END:VALARM", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
        internal static partial Regex vAlarmRegex();
        [GeneratedRegex("BEGIN:VTIMEZONE(.+?)END:VTIMEZONE", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
        internal static partial Regex vTimeZoneRegex();
        [GeneratedRegex("BEGIN:STANDARD(.+?)END:STANDARD", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
        internal static partial Regex vStandardRegex();
        [GeneratedRegex("BEGIN:DAYLIGHT(.+?)END:DAYLIGHT", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
        internal static partial Regex vDaylightRegex();
        [GeneratedRegex("BEGIN:VFREEBUSY(.+?)END:VFREEBUSY", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
        internal static partial Regex vFreeBusyRegex();


        [GeneratedRegex("(.+?)(?=(BEGIN:|END:VCALENDAR))", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
        internal static partial Regex vCalendarContentRegex();
        [GeneratedRegex("(.+?)(?=(BEGIN:|END:VEVENT))", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
        internal static partial Regex vEventContentRegex();
        [GeneratedRegex("(.+?)(?=(BEGIN:|END:VTODO))", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
        internal static partial Regex vTodoContentRegex();
        [GeneratedRegex("(.+?)(?=(BEGIN:|END:VJOURNAL))", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
        internal static partial Regex vJournalContentRegex();
        [GeneratedRegex("(.+?)(?=(BEGIN:|END:VALARM))", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
        internal static partial Regex vAlarmContentRegex();
        [GeneratedRegex("(.+?)(?=(BEGIN:|END:VTIMEZONE))", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
        internal static partial Regex vTimeZoneContentRegex();
        [GeneratedRegex("(.+?)(?=(BEGIN:|END:STANDARD))", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
        internal static partial Regex vStandardContentRegex();
        [GeneratedRegex("(.+?)(?=(BEGIN:|END:DAYLIGHT))", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
        internal static partial Regex vDaylightContentRegex();
        [GeneratedRegex("(.+?)(?=(BEGIN:|END:DAYLIGHT))", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
        internal static partial Regex vFreeBusyContentRegex();


        [GeneratedRegex("(\\r\\n )")]
        internal static partial Regex ReplaceNewLinesRegex();
        [GeneratedRegex(@"\r\n?|\n")]
        internal static partial Regex ReplaceAllNewLinesRegex();
        [GeneratedRegex("(?=[,;])", RegexOptions.IgnorePatternWhitespace)]
        internal static partial Regex EscapeSpecialCharRegex();
        [GeneratedRegex("(.+?)((;.+?)*):(.+)", RegexOptions.Singleline)]
        internal static partial Regex ContentLineRegex();
        [GeneratedRegex("(.+?)=(.+)")]
        internal static partial Regex ContentLineNameRegex();
        [GeneratedRegex("([^,]+)(?=,|$)")]
        internal static partial Regex ContentLineValuesRegex();
        [GeneratedRegex("([^;]+)(?=;|$)")]
        internal static partial Regex ContentLineParametersRegex();

        [GeneratedRegex(@"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None)]
        public static partial Regex Base64Regex();
    }
}
