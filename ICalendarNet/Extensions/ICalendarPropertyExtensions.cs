using ICalendarNet.Base;
using ICalendarNet.DataTypes;
using System.Globalization;
using static ICalendarNet.Statics;

namespace ICalendarNet.Extensions
{
    public static class ICalendarPropertyExtensions
    {
        private static DateTimeOffset? TryParseToDateTime(string value, string format)
        {
            DateTimeStyles timeStyles = format.EndsWith('Z') ? DateTimeStyles.AssumeUniversal : DateTimeStyles.AssumeLocal;
            if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, timeStyles, out DateTime UtcTime))
                return UtcTime;
            return null;
        }

        public static string? GetContentlineProperty(this List<ICalendarProperty> lines, string key)
        {
            return lines.Find(t => t.Name.Equals(key, StringComparison.OrdinalIgnoreCase))?.Value;
        }
        public static IEnumerable<ICalendarProperty> GetContentlines(this List<ICalendarProperty> lines, string key)
        {
            return lines.Where(t => t.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
        }
        public static DateTimeOffset? GetContentlineDateTime(this List<ICalendarProperty> lines, string key)
        {
            string? value = lines.GetContentlineProperty(key);
            if (string.IsNullOrWhiteSpace(value))
                return null;
            if ((TryParseToDateTime(value, "yyyyMMddTHHmmssZ") ??
                TryParseToDateTime(value, "yyyyMMddTHHmmss") ??
                TryParseToDateTime(value, "yyyyMMddHHmm") ??
                TryParseToDateTime(value, "yyyyMMdd")) is DateTimeOffset offset)
                return offset;
            return null;
        }
        public static IEnumerable<string> GetContentlinesProperty(this List<ICalendarProperty> lines, string key)
        {
            return lines.Where(t => t.Name.Equals(key, StringComparison.OrdinalIgnoreCase)).SelectMany(t => t.Value.Split(','));
        }

        public static void UpdateLineProperty(this List<ICalendarProperty> lines, string value, string key)
        {
            key = key.ToUpper();
            var foundLines = lines.Where(t => t.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (foundLines.Any())
            {
                foundLines.First().Value = value;
                return;
            }
            lines.Add(GetPropertyType(key).GetContentLine(key, value, null));
        }
        public static void UpdateLineProperty(this List<ICalendarProperty> lines, DateTimeOffset? value, string key)
        {
            if (!value.HasValue)
                return;
            key = key.ToUpper();
            string sValue = value.Value.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
            var foundLines = lines.Where(t => t.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (foundLines.Any())
            {
                foundLines.First().Value = sValue;
                return;
            }
            lines.Add(GetPropertyType(key).GetContentLine(key, sValue, null));
        }
        public static void UpdateLinesProperty(this List<ICalendarProperty> lines, IList<string> value, string key)
        {
            key = key.ToUpper();
            lines.RemoveAll(t => t.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
            lines.AddRange(value.Select(t => GetPropertyType(key).GetContentLine(key, t, null)));
        }
        public static bool IsNewProperty(this string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return false;
            string _line = line.Trim();
            if (_line.StartsWith("X-") || Array.Exists(Statics.ICalProperties, _line.StartsWith))
                return true;
            return false;
        }

        public static ICalProperty? GetPropertyType(string key)
        {
            if (Enum.TryParse<ICalProperty>(key.Replace("-", "_"), out var property))
                return property;
            return null;
        }
        public static ICalendarProperty GetContentLine(this ICalProperty? property, string key, string value, ContentLineParameters? parameters)
        {
            switch (property)
            {
                case ICalProperty.CALSCALE:
                case ICalProperty.METHOD:
                case ICalProperty.PRODID:
                case ICalProperty.VERSION:
                case ICalProperty.X_CALEND:
                case ICalProperty.X_CALSTART:
                case ICalProperty.X_CLIPEND:
                case ICalProperty.X_CLIPSTART:
                case ICalProperty.X_MICROSOFT_CALSCALE:
                case ICalProperty.X_MS_OLK_FORCEINSPECTOROPEN:
                case ICalProperty.X_MS_WKHRDAYS:
                case ICalProperty.X_MS_WKHREND:
                case ICalProperty.X_MS_WKHRSTART:
                case ICalProperty.X_OWNER:
                case ICalProperty.X_PRIMARY_CALENDAR:
                case ICalProperty.X_PUBLISHED_TTL:
                case ICalProperty.X_WR_CALDESC:
                case ICalProperty.X_WR_CALNAME:
                case ICalProperty.X_WR_RELCALID:
                case ICalProperty.CATEGORIES:
                case ICalProperty.CLASS:
                case ICalProperty.COMMENT:
                case ICalProperty.DESCRIPTION:
                case ICalProperty.GEO:
                case ICalProperty.LOCATION:
                case ICalProperty.PERCENT_COMPLETE:
                case ICalProperty.PRIORITY:
                case ICalProperty.RESOURCES:
                case ICalProperty.STATUS:
                case ICalProperty.SUMMARY:
                case ICalProperty.COMPLETED:
                case ICalProperty.DTEND:
                case ICalProperty.DUE:
                case ICalProperty.DTSTART:
                case ICalProperty.DURATION:
                case ICalProperty.FREEBUSY:
                case ICalProperty.TRANSP:
                case ICalProperty.TZID:
                case ICalProperty.TZNAME:
                case ICalProperty.TZOFFSETFROM:
                case ICalProperty.TZOFFSETTO:
                case ICalProperty.TZURL:
                case ICalProperty.ATTENDEE:
                case ICalProperty.CONTACT:
                case ICalProperty.ORGANIZER:
                case ICalProperty.RECURRENCE_ID:
                case ICalProperty.RELATED_TO:
                case ICalProperty.URL:
                case ICalProperty.UID:
                case ICalProperty.EXDATE:
                case ICalProperty.RDATE:
                case ICalProperty.RRULE:
                case ICalProperty.ACTION:
                case ICalProperty.REPEAT:
                case ICalProperty.TRIGGER:
                case ICalProperty.CREATED:
                case ICalProperty.DTSTAMP:
                case ICalProperty.LAST_MODIFIED:
                case ICalProperty.SEQUENCE:
                case ICalProperty.REQUEST_STATUS:
                case ICalProperty.X_ALT_DESC:
                case ICalProperty.X_MICROSOFT_CDO_ALLDAYEVENT:
                case ICalProperty.X_MICROSOFT_CDO_APPT_SEQUENCE:
                case ICalProperty.X_MICROSOFT_CDO_ATTENDEE_CRITICAL_CHANGE:
                case ICalProperty.X_MICROSOFT_CDO_BUSYSTATUS:
                case ICalProperty.X_MICROSOFT_CDO_IMPORTANCE:
                case ICalProperty.X_MICROSOFT_CDO_INSTTYPE:
                case ICalProperty.X_MICROSOFT_CDO_INTENDEDSTATUS:
                case ICalProperty.X_MICROSOFT_CDO_OWNERAPPTID:
                case ICalProperty.X_MICROSOFT_CDO_OWNER_CRITICAL_CHANGE:
                case ICalProperty.X_MICROSOFT_CDO_REPLYTIME:
                case ICalProperty.X_MICROSOFT_DISALLOW_COUNTER:
                case ICalProperty.X_MICROSOFT_EXDATE:
                case ICalProperty.X_MICROSOFT_ISDRAFT:
                case ICalProperty.X_MICROSOFT_MSNCALENDAR_ALLDAYEVENT:
                case ICalProperty.X_MICROSOFT_MSNCALENDAR_BUSYSTATUS:
                case ICalProperty.X_MICROSOFT_MSNCALENDAR_IMPORTANCE:
                case ICalProperty.X_MICROSOFT_MSNCALENDAR_INTENDEDSTATUS:
                case ICalProperty.X_MICROSOFT_RRULE:
                case ICalProperty.X_MS_OLK_ALLOWEXTERNCHECK:
                case ICalProperty.X_MS_OLK_APPTLASTSEQUENCE:
                case ICalProperty.X_MS_OLK_APPTSEQTIME:
                case ICalProperty.X_MS_OLK_AUTOFILLLOCATION:
                case ICalProperty.X_MS_OLK_AUTOSTARTCHECK:
                case ICalProperty.X_MS_OLK_COLLABORATEDOC:
                case ICalProperty.X_MS_OLK_CONFCHECK:
                case ICalProperty.X_MS_OLK_CONFTYPE:
                case ICalProperty.X_MS_OLK_DIRECTORY:
                case ICalProperty.X_MS_OLK_MWSURL:
                case ICalProperty.X_MS_OLK_NETSHOWURL:
                case ICalProperty.X_MS_OLK_ONLINEPASSWORD:
                case ICalProperty.X_MS_OLK_ORGALIAS:
                case ICalProperty.X_MS_OLK_SENDER:
                case ICalProperty.BUSYTYPE:
                case ICalProperty.NAME:
                case ICalProperty.REFRESH_INTERVAL:
                case ICalProperty.SOURCE:
                case ICalProperty.COLOR:
                case ICalProperty.IMAGE:
                case ICalProperty.CONFERENCE:
                case ICalProperty.CALENDAR_ADDRESS:
                case ICalProperty.LOCATION_TYPE:
                case ICalProperty.PARTICIPANT_TYPE:
                case ICalProperty.RESOURCE_TYPE:
                case ICalProperty.STRUCTURED_DATA:
                case ICalProperty.STYLED_DESCRIPTION:
                case ICalProperty.ACKNOWLEDGED:
                case ICalProperty.PROXIMITY:
                case ICalProperty.CONCEPT:
                case ICalProperty.LINK:
                case ICalProperty.REFID:
                case ICalProperty.SYNCTOKEN:
                case ICalProperty.ETAG:
                case ICalProperty.CATEGORY:
                default:
                    return new CalendarDefaultDataType(key, value, parameters);

                case ICalProperty.ATTACH: return new CalendarAttachment(key, value, parameters);
            }
        }
    }
}
