using System.Text.RegularExpressions;

namespace ICalendarNet.Serialization
{
    public partial class ICalSerializor
    {
        private static int? FindInString(ReadOnlySpan<string> source)
        {
            int search = source.IndexOfAny(new ReadOnlySpan<string>(searchBeginKeys));
            return search >= 0 ? search : null;
        }
        private static int? FindIndex(ReadOnlySpan<string> source, string target)
        {
            int search = source.IndexOf(target);
            return search >= 0 ? search : null;
        }
        private static Tuple<int, int> FindIndexes(ReadOnlySpan<string> source, string[] delims)
        {
            return new(
                FindIndex(source, delims[0]) ?? -1,
                FindIndex(source, delims[1]) ?? -1);
        }
        private static List<Tuple<int, int>> GetIndexes(ReadOnlySpan<string> source, params string[] targets)
        {
            List<Tuple<int, int>> filterIndexes = [];
            while (true)
            {
                int currentIndex = (filterIndexes.LastOrDefault()?.Item2 + 1) ?? 0;
                Tuple<int, int> indexesFound = FindIndexes(source[currentIndex..], targets);
                if (indexesFound.Item2 == -1)
                    break;
                filterIndexes.Add(new Tuple<int, int>(currentIndex + indexesFound.Item1, currentIndex + indexesFound.Item2));
                if (filterIndexes[^1].Item2 >= source.Length)
                    break;
            }
            return filterIndexes;
        }
        private static List<Tuple<int, int>> GetObjectSourceIndexes<T>(ReadOnlySpan<string> source, ICalComponent component)
        {
            switch (component)
            {
                case ICalComponent.VCALENDAR:
                    return GetIndexes(source, vCalendarBegin, vCalendarEnd);
                case ICalComponent.VEVENT:
                    return GetIndexes(source, vEventBegin, vEventEnd);
                case ICalComponent.VTODO:
                    return GetIndexes(source, vTodoBegin, vTodoEnd);
                case ICalComponent.VJOURNAL:
                    return GetIndexes(source, vJournalBegin, vJournalEnd);
                case ICalComponent.VFREEBUSY:
                    return GetIndexes(source, vFreeBusyBegin, vFreeBusyEnd);
                case ICalComponent.VTIMEZONE:
                    return GetIndexes(source, vTimezoneBegin, vTimezoneEnd);
                case ICalComponent.STANDARD:
                    return GetIndexes(source, vStandardBegin, vStandardEnd);
                case ICalComponent.DAYLIGHT:
                    return GetIndexes(source, vDaylightBegin, vDaylightEnd);
                case ICalComponent.VALARM:
                    return GetIndexes(source, vAlarmBegin, vAlarmEnd);
                default:
                    break;
            }
            throw new NotSupportedException(component.ToString());
        }
        private ReadOnlySpan<string> GetComponentContent(ReadOnlySpan<string> source, ICalComponent component)
        {
            switch (component)
            {
                case ICalComponent.VCALENDAR:
                    return source[..(FindInString(source) ?? FindIndex(source, vCalendarEnd) ?? source.Length)];
                case ICalComponent.VEVENT:
                    return source[..(FindInString(source) ?? FindIndex(source, vEventEnd) ?? source.Length)];
                case ICalComponent.VTODO:
                    return source[..(FindInString(source) ?? FindIndex(source, vTodoEnd) ?? source.Length)];
                case ICalComponent.VJOURNAL:
                    return source[..(FindInString(source) ?? FindIndex(source, vJournalEnd) ?? source.Length)];
                case ICalComponent.VFREEBUSY:
                    return source[..(FindInString(source) ?? FindIndex(source, vFreeBusyEnd) ?? source.Length)];
                case ICalComponent.VTIMEZONE:
                    return source[..(FindInString(source) ?? FindIndex(source, vTimezoneEnd) ?? source.Length)];
                case ICalComponent.STANDARD:
                    return source[..(FindInString(source) ?? FindIndex(source, vStandardEnd) ?? source.Length)];
                case ICalComponent.DAYLIGHT:
                    return source[..(FindInString(source) ?? FindIndex(source, vDaylightEnd) ?? source.Length)];
                case ICalComponent.VALARM:
                    return source[..(FindInString(source) ?? FindIndex(source, vAlarmEnd) ?? source.Length)];
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
        internal static readonly string[] searchEndKeys = [vCalendarEnd, vEventEnd, vTodoEnd, vJournalEnd, vFreeBusyEnd, vTimezoneEnd, vStandardEnd, vDaylightEnd, vAlarmEnd];
        internal static readonly List<string> allKeys = [.. searchBeginKeys, .. searchEndKeys];

        [GeneratedRegex(@"\r\n?|\n", RegexOptions.None)]
        internal static partial Regex ReplaceAllNewLinesRegex();
        [GeneratedRegex("(.+?)((;(.+?)=(.+?))*):(.+)", RegexOptions.Singleline)]
        internal static partial Regex ContentLineRegex();

    }
}
