namespace ICalendarNet.Serialization
{
    public partial class ICalSerializor
    {
        public static int GetEndLength(ICalComponent component)
        {
            switch (component)
            {
                case ICalComponent.VCALENDAR:
                    return 13;

                case ICalComponent.VEVENT:
                    return 10;

                case ICalComponent.VTODO:
                    return 9;

                case ICalComponent.VJOURNAL:
                    return 12;

                case ICalComponent.VFREEBUSY:
                    return 13;

                case ICalComponent.VTIMEZONE:
                    return 13;

                case ICalComponent.STANDARD:
                    return 12;

                case ICalComponent.DAYLIGHT:
                    return 12;

                case ICalComponent.VALARM:
                    return 10;

                default:
                    break;
            }
            throw new NotSupportedException(component.ToString());
        }

        internal const string vCalendarEnd = "END:VCALENDAR";
        internal const string vEventEnd = "END:VEVENT";
        internal const string vTodoEnd = "END:VTODO";
        internal const string vJournalEnd = "END:VJOURNAL";
        internal const string vFreeBusyEnd = "END:VFREEBUSY";
        internal const string vTimezoneEnd = "END:VTIMEZONE";
        internal const string vStandardEnd = "END:STANDARD";
        internal const string vDaylightEnd = "END:DAYLIGHT";
        internal const string vAlarmEnd = "END:VALARM";
        public const string vBeginString = "BEGIN:";
        internal const string vCalendarBegin = "BEGIN:VCALENDAR";
        internal const string vEventBegin = "BEGIN:VEVENT";
        internal const string vTodoBegin = "BEGIN:VTODO";
        internal const string vJournalBegin = "BEGIN:VJOURNAL";
        internal const string vFreeBusyBegin = "BEGIN:VFREEBUSY";
        internal const string vTimezoneBegin = "BEGIN:VTIMEZONE";
        internal const string vStandardBegin = "BEGIN:STANDARD";
        internal const string vDaylightBegin = "BEGIN:DAYLIGHT";
        internal const string vAlarmBegin = "BEGIN:VALARM";

        internal static readonly string[] searchBeginKeys = [vCalendarBegin, vEventBegin, vTodoBegin, vJournalBegin, vFreeBusyBegin, vTimezoneBegin, vStandardBegin, vDaylightBegin, vAlarmBegin];
        //internal static readonly string[] searchEndKeys = [vCalendarEnd, vEventEnd, vTodoEnd, vJournalEnd, vFreeBusyEnd, vTimezoneEnd, vStandardEnd, vDaylightEnd, vAlarmEnd]
        //internal static readonly List<string> allKeys = [.. searchBeginKeys, .. searchEndKeys]

        //[GeneratedRegex(@"\r\n?|\n", RegexOptions.Compiled)]
        //internal static partial Regex ReplaceAllNewLinesRegex()

        //[GeneratedRegex("(.+?)((;(.+?)=(.+?))*):(.+)", RegexOptions.Singleline | RegexOptions.Compiled)]
        //internal static partial Regex ContentLineRegex()
    }
}