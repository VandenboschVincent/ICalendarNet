using ICalendarNet.Logic;
using ICalendarNet.Models.Base;
using ICalendarNet.Models.Components;
using ICalendarNet.Models.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using static ICalendarNet.Models.Enum.Statics;

namespace ICalendarNet.Extensions
{
    public static class ICalendarPropertyExtensions
    {
        // ---------- Internal helpers ----------
        private static string ToName(this ICalProperty key) => ICalProperties[(int)key];

        private static ICalendarProperty? FindByName(this List<ICalendarProperty> lines, string key)
            => lines.Find(t => t.Name.Equals(key, StringComparison.OrdinalIgnoreCase));

        private static CalendarTimeZone? GetTimeZone(ICalendarProperty? line)
        {
            if (line?.Metadata is null) return null;
            if (line.Parameters.GetValue(ICalProperty.TZID.ToName()) is not string tzid) return null;
            return line.Metadata.GetTimeZone(tzid);
        }

        private static string GetLineValue(this ICalendarProperty line)
            => line.Parameters.Encoding?.Equals("BASE64", StringComparison.OrdinalIgnoreCase) == true
                ? TypeConverters.ConvertFromBase64(line.Value)
                : line.Value;

        private static void SetLineValue(this ICalendarProperty line, string value)
        {
            if (line.Parameters.Encoding?.Equals("BASE64", StringComparison.OrdinalIgnoreCase) == true)
                line.Value = TypeConverters.ConvertToBase64(value);
            else
                line.Value = value;
        }

        // ---------- Read: single value ----------

        public static string? GetContentlineValue(this List<ICalendarProperty> lines, ICalProperty key)
        {
            var line = lines.FindByName(key.ToName());
            if (line is null) return null;
            return line.GetLineValue();
        }

        public static TEnum GetContentlineValue<TEnum>(this List<ICalendarProperty> lines, ICalProperty key, string defaultEnum)
            where TEnum : struct, Enum
            => Enum.Parse<TEnum>(lines.GetContentlineValue(key) ?? defaultEnum, ignoreCase: true);

        public static int? GetContentlineInt(this List<ICalendarProperty> lines, ICalProperty key)
            => TypeConverters.ConvertToInt(lines.GetContentlineValue(key));

        public static double? GetContentlineDouble(this List<ICalendarProperty> lines, ICalProperty key)
            => TypeConverters.ConvertToDouble(lines.GetContentlineValue(key));

        public static TimeSpan? GetContentlineTimeSpan(this List<ICalendarProperty> lines, ICalProperty key)
            => TypeConverters.ConvertToTimeSpan(lines.GetContentlineValue(key));

        public static DateTimeOffset? GetContentlineDateTime(this List<ICalendarProperty> lines, ICalProperty key, MetadataContainer? metadata = null)
            => ParseDateTime(lines.FindByName(key.ToName()), metadata);

        public static IEnumerable<DateTimeOffset> GetContentlineDateTimes(this List<ICalendarProperty> lines, ICalProperty key, MetadataContainer? metadata = null)
            => lines.GetContentlines(key)
                    .Select(l => ParseDateTime(l, metadata))
                    .Where(d => d.HasValue)
                    .Select(d => d!.Value);

        private static DateTimeOffset? ParseDateTime(ICalendarProperty? line, MetadataContainer? metadata)
        {
            if (line is null) return null;
            if (metadata is not null &&
                line.Parameters.GetValue(ICalProperty.TZID.ToName()) is string tzid)
            {
                return TypeConverters.ConvertToDateTimeOffset(line.GetLineValue(), metadata.GetTimeZone(tzid));
            }
            return TypeConverters.ConvertToDateTimeOffset(line.GetLineValue());
        }

        // ---------- Read: multiple lines / values ----------

        public static IEnumerable<ICalendarProperty> GetContentlines(this List<ICalendarProperty> lines, ICalProperty key)
            => lines.Where(t => t.Name.Equals(key.ToName(), StringComparison.OrdinalIgnoreCase));

        public static IEnumerable<string> GetContentlinesValue(this List<ICalendarProperty> lines, params ICalProperty[] keys)
        {
            var names = keys.Select(k => k.ToName()).ToHashSet(StringComparer.OrdinalIgnoreCase);
            return lines.Where(t => names.Contains(t.Name, StringComparer.OrdinalIgnoreCase)).Select(t => t.GetLineValue());
        }
#if NET5_0_OR_GREATER
        public static IEnumerable<string> GetContentlinesSeperatedValue(this List<ICalendarProperty> lines, params ICalProperty[] keys)
            => lines.GetContentlinesValue(keys).SelectMany(v => v.Split(',', StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries));
#else
        public static IEnumerable<string> GetContentlinesSeperatedValue(this List<ICalendarProperty> lines, params ICalProperty[] keys)
            => lines.GetContentlinesValue(keys).SelectMany(v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()));
#endif
        // ---------- Write ----------

        public static void UpdateLineProperty<TEnum>(this List<ICalendarProperty> lines, TEnum value, ICalProperty key, ContentLineParameters? parameters = null)
            where TEnum : struct, Enum
            => lines.UpdateLineProperty(value.ToString(), key, parameters);

        public static void UpdateLineProperty(this List<ICalendarProperty> lines, string? value, ICalProperty key, ContentLineParameters? parameters = null)
        {
            if (value is null) { lines.RemoveLineProperty(key); return; }

            var foundLine = lines.FindByName(key.ToName());
            if (foundLine is not null)
            {
                if (parameters is not null) foundLine.Parameters = parameters;
                foundLine.SetLineValue(value);
            }
            else
            {
                if (parameters?.Encoding?.Equals("BASE64", StringComparison.OrdinalIgnoreCase) == true)
                    lines.Add(key.GetContentLine(TypeConverters.ConvertToBase64(value), parameters));
                else
                    lines.Add(key.GetContentLine(value, parameters));
            }
        }

        public static void UpdateLineProperty(this List<ICalendarProperty> lines, int? value, ICalProperty key, ContentLineParameters? parameters = null)
            => lines.UpdateOrRemove(value, key, parameters, TypeConverters.ConvertFromInt);

        public static void UpdateLineProperty(this List<ICalendarProperty> lines, double? value, ICalProperty key, ContentLineParameters? parameters = null)
            => lines.UpdateOrRemove(value, key, parameters, TypeConverters.ConvertFromDouble);

        public static void UpdateLineProperty(this List<ICalendarProperty> lines, TimeSpan? value, ICalProperty key, ContentLineParameters? parameters = null)
            => lines.UpdateOrRemove(value, key, parameters, TypeConverters.ConvertFromTimeSpan);

        public static void UpdateLineProperty(this List<ICalendarProperty> lines, DateTimeOffset? value, ICalProperty key, ContentLineParameters? parameters = null)
        {
            if (value is null) { lines.RemoveLineProperty(key); return; }
            var tz = GetTimeZone(lines.FindByName(key.ToName()));
            lines.UpdateLineProperty(TypeConverters.ConvertFromDateTimeOffset(value.Value, tz), key, parameters);
        }

        public static void UpdateLineProperty(this List<ICalendarProperty> lines, IEnumerable<DateTimeOffset>? value, ICalProperty key, ContentLineParameters? parameters = null)
        {
            if (value is null) { lines.RemoveLineProperty(key); return; }
            var tz = GetTimeZone(lines.FindByName(key.ToName()));
            var joined = string.Join(",", value.Select(d => TypeConverters.ConvertFromDateTimeOffset(d, tz)));
            lines.UpdateLineProperty(joined, key, parameters);
        }

        public static void UpdateLineProperty(this List<ICalendarProperty> lines, IEnumerable<ICalendarProperty>? value, ICalProperty key)
        {
            lines.RemoveLineProperty(key);
            if (value is not null) lines.AddRange(value);
        }

        public static void UpdateLinesProperty(this List<ICalendarProperty> lines, IEnumerable<string>? value, ICalProperty key, ContentLineParameters? parameters = null)
        {
            lines.RemoveLineProperty(key);
            foreach (var newValue in value ?? [])
            {
                lines.UpdateLineProperty(newValue, key, parameters);
            }
        }

        public static void UpdateLinesSeperatedProperty(this List<ICalendarProperty> lines, IEnumerable<string>? value, ICalProperty key, ContentLineParameters? parameters = null)
        {
            lines.RemoveLineProperty(key);
            if (value is not null)
                lines.UpdateLineProperty(string.Join(", ", value), key, parameters); 
        }

        // Generic helper that eliminates the repeated "null -> remove, else convert+update" pattern
        private static void UpdateOrRemove<T>(this List<ICalendarProperty> lines, T? value, ICalProperty key,
            ContentLineParameters? parameters, Func<T, string> convert) where T : struct
        {
            if (value is null) lines.RemoveLineProperty(key);
            else lines.UpdateLineProperty(convert(value.Value), key, parameters);
        }

        // ---------- Remove ----------

        public static void RemoveLineProperty(this List<ICalendarProperty> lines, ICalProperty key)
            => lines.RemoveLineProperty(key.ToName());

        public static void RemoveLineProperty(this List<ICalendarProperty> lines, string key)
            => lines.RemoveAll(t => t.Name.Equals(key, StringComparison.OrdinalIgnoreCase));

        // ---------- Parsing ----------
        public static bool TryGetNewProperty(this ReadOnlySpan<char> line, out ICalProperty? property)
        {
            for (int i = 0; i < ICalProperties.Length; i++)
            {
                if (line.StartsWith(ICalProperties[i], StringComparison.OrdinalIgnoreCase))
                {
                    property = (ICalProperty)i;
                    return true;
                }
            }
            property = null;
            return line.StartsWith("X-");
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
                case ICalProperty.X_APPLE_STRUCTURED_LOCATION:
                case ICalProperty.EXRULE:
                case ICalProperty.CATEGORY:
                    return new CalendarDefaultDataType(ICalProperties[(int)property], value.ToString(), parameters);

                case ICalProperty.ATTACH:
                    return new CalendarAttachment(ICalProperties[(int)property], value.ToString(), parameters);

                case ICalProperty.ATTENDEE:
                case ICalProperty.ORGANIZER:
                    return new CalendarCalAddress(ICalProperties[(int)property], value.ToString(), parameters);

                case ICalProperty.TRIGGER:
                    return new CalendarTrigger(ICalProperties[(int)property], value.ToString(), parameters);

                case ICalProperty.FREEBUSY:
                case ICalProperty.RDATE:
                    return new CalendarPeriods(ICalProperties[(int)property], value.ToString(), parameters);

                case ICalProperty.RRULE:
                    return new CalendarRecurrenceRule(ICalProperties[(int)property], value.ToString(), parameters);

                default:
                    throw new NotSupportedException(property.ToString());
            }
        }
    }
}