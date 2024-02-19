using System.Text.RegularExpressions;

namespace ICalendarNet.Serialization
{
    public partial class ICalSerializor
    {
        private static int? GetFirstStringIndex(string source, string target)
        {
            int indexfound = source.IndexOf(target, StringComparison.OrdinalIgnoreCase);
            return indexfound == -1 ? null : indexfound;
        }
        public static Tuple<int, int> FindIndexes(IEnumerable<KeyValuePair<int, string>> source, string[] delims)
        {
            return new(
                source.FirstOrDefault(t => t.Value.Equals(delims[0], StringComparison.OrdinalIgnoreCase)).Key,
                source.FirstOrDefault(t => t.Value.Equals(delims[1], StringComparison.OrdinalIgnoreCase)).Key);
        }
        private static IEnumerable<IEnumerable<string>> GetStrings(Dictionary<int, string> source, params string[] targets)
        {
            List<Tuple<int, int>> filterIndexes = [];
            while (true)
            {
                Tuple<int, int> indexesFound = FindIndexes(source.Skip((filterIndexes.LastOrDefault()?.Item2 + 1) ?? 0), targets);
                if (indexesFound.Item2 == 0)
                    break;
                filterIndexes.Add(indexesFound);
                if (filterIndexes[^1].Item2 >= source.Count)
                    break;
            }
            IEnumerable<IEnumerable<string>> splitted = filterIndexes.Select(x => source.Where(t => t.Key >= x.Item1 && t.Key <= x.Item2).Select(t => t.Value));
            return splitted;
        }
        private IEnumerable<IEnumerable<string>> GetObjectSources(string source, ICalComponent component)
        {
            Dictionary<int, string> allLines = source.Split(Environment.NewLine, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select((value, index) => new { value, index })
                .ToDictionary(pair => pair.index, pair => pair.value);
            switch (component)
            {
                case ICalComponent.VCALENDAR:
                    return GetStrings(allLines, vCalendarBegin, vCalendarEnd);
                case ICalComponent.VEVENT:
                    return GetStrings(allLines, vEventBegin, vEventEnd);
                case ICalComponent.VTODO:
                    return GetStrings(allLines, vTodoBegin, vTodoEnd);
                case ICalComponent.VJOURNAL:
                    return GetStrings(allLines, vJournalBegin, vJournalEnd);
                case ICalComponent.VFREEBUSY:
                    return GetStrings(allLines, vFreeBusyBegin, vFreeBusyEnd);
                case ICalComponent.VTIMEZONE:
                    return GetStrings(allLines, vTimezoneBegin, vTimezoneEnd);
                case ICalComponent.STANDARD:
                    return GetStrings(allLines, vStandardBegin, vStandardEnd);
                case ICalComponent.DAYLIGHT:
                    return GetStrings(allLines, vDaylightBegin, vDaylightEnd);
                case ICalComponent.VALARM:
                    return GetStrings(allLines, vAlarmBegin, vAlarmEnd);
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

        [GeneratedRegex("(\\r\\n )", RegexOptions.None)]
        internal static partial Regex ReplaceNewLinesRegex();
        [GeneratedRegex(@"\r\n?|\n", RegexOptions.None)]
        internal static partial Regex ReplaceAllNewLinesRegex();
        [GeneratedRegex("(?=[,;])", RegexOptions.IgnorePatternWhitespace)]
        internal static partial Regex EscapeSpecialCharRegex();
        [GeneratedRegex("(.+?)((;.+?)*):(.+)", RegexOptions.Singleline)]
        internal static partial Regex ContentLineRegex();
        [GeneratedRegex("(.+?)=(.+)", RegexOptions.None)]
        internal static partial Regex ContentLineNameRegex();
        [GeneratedRegex("([^,]+)(?=,|$)", RegexOptions.None)]
        internal static partial Regex ContentLineValuesRegex();
        [GeneratedRegex("([^;]+)(?=;|$)", RegexOptions.None)]
        internal static partial Regex ContentLineParametersRegex();

        [GeneratedRegex(@"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None)]
        public static partial Regex Base64Regex();
    }
}
