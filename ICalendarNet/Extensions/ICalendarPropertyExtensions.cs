using ICalendarNet.Base;
using ICalendarNet.Converters;
using ICalendarNet.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using static ICalendarNet.Statics;

namespace ICalendarNet.Extensions
{
    public static class ICalendarPropertyExtensions
    {
        public static string? GetContentlineValue(this List<ICalendarProperty> lines, ICalProperty key)
        {
            return lines.GetContentlineValue(ICalProperties[(int)key]);
        }

        public static string? GetContentlineValue(this List<ICalendarProperty> lines, string key)
        {
            return lines.Find(t => t.Name.Equals(key, StringComparison.OrdinalIgnoreCase))?.Value;
        }

        public static IEnumerable<string> GetContentlinesValue(this List<ICalendarProperty> lines, ICalProperty key)
        {
            return lines.GetContentlinesValue(ICalProperties[(int)key]);
        }

        public static IEnumerable<string> GetContentlinesValue(this List<ICalendarProperty> lines, params ICalProperty[] keys)
        {
            return lines.GetContentlinesValue(keys.Select(t => ICalProperties[(int)t]));
        }

        internal static IEnumerable<string> GetContentlinesValue(this List<ICalendarProperty> lines, string key)
        {
            return lines.Where(t => t.Name.Equals(key, StringComparison.OrdinalIgnoreCase)).Select(t => t.Value);
        }

        internal static IEnumerable<string> GetContentlinesValue(this List<ICalendarProperty> lines, IEnumerable<string> keys)
        {
            return lines.Where(t => keys.Contains(t.Name, StringComparer.OrdinalIgnoreCase)).Select(t => t.Value);
        }

        public static IEnumerable<string> GetContentlinesSeperatedValue(this List<ICalendarProperty> lines, ICalProperty key)
        {
            return lines.GetContentlinesSeperatedValue(ICalProperties[(int)key]);
        }

        public static IEnumerable<string> GetContentlinesSeperatedValue(this List<ICalendarProperty> lines, params ICalProperty[] keys)
        {
            return lines.GetContentlinesSeperatedValue(keys.Select(t => ICalProperties[(int)t]));
        }

        internal static IEnumerable<string> GetContentlinesSeperatedValue(this List<ICalendarProperty> lines, string key)
        {
            return lines.Where(t => t.Name.Equals(key, StringComparison.OrdinalIgnoreCase)).SelectMany(t => t.Value.Split(','));
        }

        internal static IEnumerable<string> GetContentlinesSeperatedValue(this List<ICalendarProperty> lines, IEnumerable<string> keys)
        {
            return lines.Where(t => keys.Contains(t.Name, StringComparer.OrdinalIgnoreCase)).SelectMany(t => t.Value.Split(", "));
        }

        public static IEnumerable<ICalendarProperty> GetContentlines(this List<ICalendarProperty> lines, ICalProperty key)
        {
            return lines.GetContentlines(ICalProperties[(int)key]);
        }

        internal static IEnumerable<ICalendarProperty> GetContentlines(this List<ICalendarProperty> lines, string key)
        {
            return lines.Where(t => t.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
        }

        public static int? GetContentlineInt(this List<ICalendarProperty> lines, ICalProperty key)
        {
            return TypeConverters.ConvertToInt(lines.GetContentlineValue(ICalProperties[(int)key]));
        }

        public static double? GetContentlineDouble(this List<ICalendarProperty> lines, ICalProperty key)
        {
            return TypeConverters.ConvertToDouble(lines.GetContentlineValue(ICalProperties[(int)key]));
        }

        public static DateTimeOffset? GetContentlineDateTime(this List<ICalendarProperty> lines, ICalProperty key)
        {
            return TypeConverters.ConvertToDateTimeOffset(lines.GetContentlineValue(ICalProperties[(int)key]));
        }

        public static TimeSpan? GetContentlineTimeSpan(this List<ICalendarProperty> lines, ICalProperty key)
        {
            return TypeConverters.ConvertToTimeSpan(lines.GetContentlineValue(ICalProperties[(int)key]));
        }

        public static IEnumerable<DateTimeOffset>? GetContentlineDateTimes(this List<ICalendarProperty> lines, ICalProperty key)
        {
            return lines.GetContentlines(ICalProperties[(int)key]).Select(t => TypeConverters.ConvertToDateTimeOffset(t.Value)!.Value);
        }

        public static void UpdateLineProperty(this List<ICalendarProperty> lines, string value, ICalProperty key, ContentLineParameters? parameters = null)
        {
            var foundLine = lines.Find(t => t.Name.Equals(ICalProperties[(int)key], StringComparison.OrdinalIgnoreCase));
            if (foundLine != null)
            {
                if (parameters != null)
                    foundLine.Parameters = parameters;
                foundLine.Value = value;
                return;
            }
            lines.Add(key.GetContentLine(value, parameters));
        }

        public static void UpdateLineProperty(this List<ICalendarProperty> lines, TimeSpan? value, ICalProperty key, ContentLineParameters? parameters = null)
        {
            if (!value.HasValue)
                return;
            lines.UpdateLineProperty(TypeConverters.ConvertFromTimeSpan(value.Value), key, parameters);
        }

        public static void UpdateLineProperty(this List<ICalendarProperty> lines, int? value, ICalProperty key, ContentLineParameters? parameters = null)
        {
            if (!value.HasValue)
                return;
            lines.UpdateLineProperty(TypeConverters.ConvertFromInt(value.Value), key, parameters);
        }

        public static void UpdateLineProperty(this List<ICalendarProperty> lines, double? value, ICalProperty key, ContentLineParameters? parameters = null)
        {
            if (!value.HasValue)
                return;
            lines.UpdateLineProperty(TypeConverters.ConvertFromDouble(value.Value), key, parameters);
        }

        public static void UpdateLineProperty(this List<ICalendarProperty> lines, DateTimeOffset? value, ICalProperty key, ContentLineParameters? parameters = null)
        {
            if (!value.HasValue)
                return;
            lines.UpdateLineProperty(TypeConverters.ConvertFromDateTimeOffset(value.Value), key, parameters);
        }

        public static void UpdateLineProperty(this List<ICalendarProperty> lines, IEnumerable<DateTimeOffset> value, ICalProperty key, ContentLineParameters? parameters = null)
        {
            lines.UpdateLineProperty(string.Join(",", value.Select(TypeConverters.ConvertFromDateTimeOffset)), key, parameters);
        }

        internal static void UpdateLineProperty(this List<ICalendarProperty> lines, IEnumerable<ICalendarProperty> value, string key)
        {
            lines.RemoveAll(t => t.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
            lines.AddRange(value);
        }

        public static void UpdateLineProperty(this List<ICalendarProperty> lines, IEnumerable<ICalendarProperty> value, ICalProperty key)
        {
            lines.UpdateLineProperty(value, ICalProperties[(int)key]);
        }

        public static void UpdateLinesProperty(this List<ICalendarProperty> lines, IEnumerable<string> value, ICalProperty key, ContentLineParameters? parameters = null)
        {
            lines.RemoveAll(t => t.Name.Equals(ICalProperties[(int)key], StringComparison.OrdinalIgnoreCase));
            lines.AddRange(value.Select(t => key.GetContentLine(t, parameters)));
        }

        public static void UpdateLinesSeperatedProperty(this List<ICalendarProperty> lines, IEnumerable<string> value, ICalProperty key, ContentLineParameters? parameters = null)
        {
            lines.RemoveAll(t => t.Name.Equals(ICalProperties[(int)key], StringComparison.OrdinalIgnoreCase));
            lines.Add(key.GetContentLine(string.Join(", ", value), parameters));
        }

        public static bool TryGetNewProperty(this ReadOnlySpan<char> line, out ICalProperty? property)
        {
            property = null;
            for (int i = 0; i < ICalProperties.Length; i++)
            {
                if (line.StartsWith(ICalProperties[i]))
                {
                    property = (ICalProperty)i;
                    return true;
                }
            }
            if (line.StartsWith("X-"))
                return true;
            return false;
        }

        internal static ICalProperty? GetPropertyType(ReadOnlySpan<char> key)
        {
            if (TryGetNewProperty(key, out ICalProperty? property))
                return property;
            return null;
        }

        internal static ICalendarProperty GetContentLine(this ICalProperty? property, ReadOnlySpan<char> key, ReadOnlySpan<char> value, ContentLineParameters? parameters)
        {
            if (property == null)
                return new CalendarDefaultDataType(key.ToString(), value.ToString(), parameters);
            return property.Value.GetContentLine(value, parameters);
        }

        internal static ICalendarProperty GetContentLine(this ICalProperty property, ReadOnlySpan<char> value, ContentLineParameters? parameters)
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
                case ICalProperty.CONTACT:
                case ICalProperty.RECURRENCE_ID:
                case ICalProperty.RELATED_TO:
                case ICalProperty.URL:
                case ICalProperty.UID:
                case ICalProperty.EXDATE:
                case ICalProperty.RDATE:
                case ICalProperty.RRULE:
                case ICalProperty.ACTION:
                case ICalProperty.REPEAT:
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
                    return new CalendarDefaultDataType(ICalProperties[(int)property], value.ToString(), parameters);

                case ICalProperty.ATTACH:
                    return new CalendarAttachment(ICalProperties[(int)property], value.ToString(), parameters);

                case ICalProperty.ATTENDEE:
                case ICalProperty.ORGANIZER:
                    return new CalendarCalAddress(ICalProperties[(int)property], value.ToString(), parameters);

                case ICalProperty.TRIGGER:
                    return new CalendarTrigger(ICalProperties[(int)property], value.ToString(), parameters);

                default:
                    throw new NotSupportedException(property.ToString());
            }
        }
    }
}