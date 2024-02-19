using System.Text.RegularExpressions;

namespace ICalendarNet.Serialization
{
    public partial class ICalSerializor
    {
        private static int? GetFirstStringIndex(string source, string target)
        {
            int indexfound = source.IndexOf(target);
            return indexfound == -1 ? null : indexfound;
        }
        private static int? GetLastStringIndex(string source, string target)
        {
            int indexfound = source.LastIndexOf(target);
            return indexfound == -1 ? null : indexfound;
        }
        public static IEnumerable<IEnumerable<string>> SplitAndKeep(string s, string[] delims)
        {
            int start = 0, index;

            while ((index = s.IndexOf(delims, start)) != -1)
            {
                if (index - start > 0)
                    yield return s.Substring(start, index - start);
                yield return s.Substring(index, 1);
                start = index + 1;
            }

            if (start < s.Length)
            {
                yield return s.Substring(start);
            }
        }
        private static string[] GetStrings(string source, params string[] targets)
        {
            string[] splitted = SplitAndKeep(source, targets);
            return splitted;
        }
        private string[] GetObjectSources(string source, ICalComponent component)
        {
            switch (component)
            {
                case ICalComponent.VCALENDAR:
                    return GetStrings(source, vCalendarBegin, vCalendarEnd);
                case ICalComponent.VEVENT:
                    return GetStrings(source, vEventBegin, vEventEnd);
                case ICalComponent.VTODO:
                    return GetStrings(source, vTodoBegin, vTodoEnd);
                case ICalComponent.VJOURNAL:
                    return GetStrings(source, vJournalBegin, vJournalEnd);
                case ICalComponent.VFREEBUSY:
                    return GetStrings(source, vFreeBusyBegin, vFreeBusyEnd);
                case ICalComponent.VTIMEZONE:
                    return GetStrings(source, vTimezoneBegin, vTimezoneEnd);
                case ICalComponent.STANDARD:
                    return GetStrings(source, vStandardBegin, vStandardEnd);
                case ICalComponent.DAYLIGHT:
                    return GetStrings(source, vDaylightBegin, vDaylightEnd);
                case ICalComponent.VALARM:
                    return GetStrings(source, vAlarmBegin, vAlarmEnd);
                default:
                    break;
            }
            throw new NotSupportedException(component.ToString());
        }
        private string GetComponentContent(string source, ICalComponent component)
        {
            switch (component)
            {
                case ICalComponent.VCALENDAR:
                    return source[..(GetFirstStringIndex(source, vBegin) ?? GetFirstStringIndex(source, vCalendarEnd) ?? source.Length)];
                case ICalComponent.VEVENT:
                    return source[..(GetFirstStringIndex(source, vBegin) ?? GetFirstStringIndex(source, vEventEnd) ?? source.Length)];
                case ICalComponent.VTODO:
                    return source[..(GetFirstStringIndex(source, vBegin) ?? GetFirstStringIndex(source, vTodoEnd) ?? source.Length)];
                case ICalComponent.VJOURNAL:
                    return source[..(GetFirstStringIndex(source, vBegin) ?? GetFirstStringIndex(source, vJournalEnd) ?? source.Length)];
                case ICalComponent.VFREEBUSY:
                    return source[..(GetFirstStringIndex(source, vBegin) ?? GetFirstStringIndex(source, vFreeBusyEnd) ?? source.Length)];
                case ICalComponent.VTIMEZONE:
                    return source[..(GetFirstStringIndex(source, vBegin) ?? GetFirstStringIndex(source, vTimezoneEnd) ?? source.Length)];
                case ICalComponent.STANDARD:
                    return source[..(GetFirstStringIndex(source, vBegin) ?? GetFirstStringIndex(source, vStandardEnd) ?? source.Length)];
                case ICalComponent.DAYLIGHT:
                    return source[..(GetFirstStringIndex(source, vBegin) ?? GetFirstStringIndex(source, vDaylightEnd) ?? source.Length)];
                case ICalComponent.VALARM:
                    return source[..(GetFirstStringIndex(source, vBegin) ?? GetFirstStringIndex(source, vAlarmEnd) ?? source.Length)];
                default:
                    break;
            }
            throw new NotSupportedException(component.ToString());
        }
        internal const string vBegin = "BEGIN:";

        internal const string vCalendarEnd = "END:VCALENDAR";
        internal const string vEventEnd = "END:VEVENT";
        internal const string vTodoEnd = "END:VTODO";
        internal const string vJournalEnd = "END:VJOURNAL";
        internal const string vFreeBusyEnd = "END:VFREEBUSY";
        internal const string vTimezoneEnd = "END:VTIMEZONE";
        internal const string vStandardEnd = "END:STANDARD";
        internal const string vDaylightEnd = "END:DAYLIGHT";
        internal const string vAlarmEnd = "END:VALARM";

        internal const string vCalendarBegin = "BEGIN:VCALENDAR";
        internal const string vEventBegin = "BEGIN:VEVENT";
        internal const string vTodoBegin = "BEGIN:VTODO";
        internal const string vJournalBegin = "BEGIN:VJOURNAL";
        internal const string vFreeBusyBegin = "BEGIN:VFREEBUSY";
        internal const string vTimezoneBegin = "BEGIN:VTIMEZONE";
        internal const string vStandardBegin = "BEGIN:STANDARD";
        internal const string vDaylightBegin = "BEGIN:DAYLIGHT";
        internal const string vAlarmBegin = "BEGIN:VALARM";

        [GeneratedRegex("(\\r\\n )", RegexOptions.None, 200)]
        internal static partial Regex ReplaceNewLinesRegex();
        [GeneratedRegex(@"\r\n?|\n", RegexOptions.None, 200)]
        internal static partial Regex ReplaceAllNewLinesRegex();
        [GeneratedRegex("(?=[,;])", RegexOptions.IgnorePatternWhitespace, 200)]
        internal static partial Regex EscapeSpecialCharRegex();
        [GeneratedRegex("(.+?)((;.+?)*):(.+)", RegexOptions.Singleline, 200)]
        internal static partial Regex ContentLineRegex();
        [GeneratedRegex("(.+?)=(.+)", RegexOptions.None, 200)]
        internal static partial Regex ContentLineNameRegex();
        [GeneratedRegex("([^,]+)(?=,|$)", RegexOptions.None, 200)]
        internal static partial Regex ContentLineValuesRegex();
        [GeneratedRegex("([^;]+)(?=;|$)", RegexOptions.None, 200)]
        internal static partial Regex ContentLineParametersRegex();

        [GeneratedRegex(@"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None, 200)]
        public static partial Regex Base64Regex();
    }
}
