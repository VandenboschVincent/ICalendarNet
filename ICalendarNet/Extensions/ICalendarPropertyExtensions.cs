using FastEnumUtility;
using ICalendarNet.Logic;
using ICalendarNet.Models.Base;
using ICalendarNet.Models.Components;
using ICalendarNet.Models.DataTypes;
using ICalendarNet.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using static ICalendarNet.Models.Enum.Statics;

namespace ICalendarNet.Extensions
{
    public static class ICalendarPropertyExtensions
    {
        // ---------- Internal helpers ----------

        private static ICalendarProperty? FindByName(this List<ICalendarProperty> lines, string key)
            => lines.Find(t => t.Name.Equals(key, StringComparison.OrdinalIgnoreCase));

        public static CalendarTimeZone? GetTimeZone(this ICalendarProperty? line)
        {
            if (line?.Metadata is null) return null;
            if (line.Parameters.GetValue(nameof(ICalProperty.TZID)) is not string tzid) return null;
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
            var line = lines.FindByName(key.GetString());
            if (line is null) return null;
            return line.GetLineValue();
        }

        public static TEnum GetContentlineValue<TEnum>(this List<ICalendarProperty> lines, ICalProperty key, string defaultEnum)
            where TEnum : struct, Enum
            => FastEnum.Parse<TEnum>(lines.GetContentlineValue(key) ?? defaultEnum, ignoreCase: true);

        public static int? GetContentlineInt(this List<ICalendarProperty> lines, ICalProperty key)
            => TypeConverters.ConvertToInt(lines.GetContentlineValue(key));

        public static double? GetContentlineDouble(this List<ICalendarProperty> lines, ICalProperty key)
            => TypeConverters.ConvertToDouble(lines.GetContentlineValue(key));

        public static TimeSpan? GetContentlineTimeSpan(this List<ICalendarProperty> lines, ICalProperty key)
            => TypeConverters.ConvertToTimeSpan(lines.GetContentlineValue(key));

        public static DateTimeOffset? GetContentlineDateTime(this List<ICalendarProperty> lines, ICalProperty key, MetadataContainer? metadata = null)
            => ParseDateTime(lines.FindByName(key.GetString()), metadata);

        public static IEnumerable<DateTimeOffset> GetContentlineDateTimes(this List<ICalendarProperty> lines, ICalProperty key, MetadataContainer? metadata = null)
            => lines.GetContentlines(key)
                    .Select(l => ParseDateTime(l, metadata))
                    .Where(d => d.HasValue)
                    .Select(d => d!.Value);

        private static DateTimeOffset? ParseDateTime(ICalendarProperty? line, MetadataContainer? metadata)
        {
            if (line is null) return null;
            if (metadata is not null &&
                line.Parameters.GetValue(nameof(ICalProperty.TZID)) is string tzid)
            {
                return TypeConverters.ConvertToDateTimeOffset(line.GetLineValue(), metadata.GetTimeZone(tzid));
            }
            return TypeConverters.ConvertToDateTimeOffset(line.GetLineValue());
        }

        // ---------- Read: multiple lines / values ----------

        public static IEnumerable<ICalendarProperty> GetContentlines(this List<ICalendarProperty> lines, ICalProperty key)
        {
            var name = key.GetString();
            return lines.Where(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<string> GetContentlinesValue(this List<ICalendarProperty> lines, params ICalProperty[] keys)
        {
            var names = keys.Select(k => k.GetString()).ToHashSet(StringComparer.OrdinalIgnoreCase);
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
            => lines.UpdateLineProperty(value.GetString(), key, parameters);

        public static void UpdateLineProperty(this List<ICalendarProperty> lines, string? value, ICalProperty key, ContentLineParameters? parameters = null)
        {
            if (value is null) { lines.RemoveLineProperty(key); return; }

            var foundLine = lines.FindByName(key.GetString());
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
            var tz = GetTimeZone(lines.FindByName(key.GetString()));
            lines.UpdateLineProperty(TypeConverters.ConvertFromDateTimeOffset(value.Value, tz), key, parameters);
        }

        public static void UpdateLineProperty(this List<ICalendarProperty> lines, IEnumerable<DateTimeOffset>? value, ICalProperty key, ContentLineParameters? parameters = null)
        {
            if (value is null) { lines.RemoveLineProperty(key); return; }
            var tz = GetTimeZone(lines.FindByName(key.GetString()));
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
            => lines.RemoveLineProperty(key.GetString());

        public static void RemoveLineProperty(this List<ICalendarProperty> lines, string key)
            => lines.RemoveAll(t => t.Name.Equals(key, StringComparison.OrdinalIgnoreCase));

        // ---------- Parsing ----------
        public static bool TryGetNewProperty(this ReadOnlySpan<char> line, out ICalProperty? property)
        {
            //Ignore numeric values
            if (StringExtensions.IsNumeric(line[0]))
            {
                property = default;
                return false;
            }

            // Property name ends at the first ':' or ';' (whichever comes first)
            int end = line.IndexOfAny(':', ';');
            ReadOnlySpan<char> name = end >= 0 ? line[..end] : line;
            Span<char> nameNormalized = stackalloc char[name.Length];
            name.Replace(nameNormalized, '-', '_');
            if (FastEnum.TryParse(nameNormalized, false, out ICalProperty prop))
            {
                property = prop;
                return true;
            }

            property = null;
            // Experimental properties: "X-..." prefix on the *name* portion
            return name.StartsWith("X-", StringComparison.OrdinalIgnoreCase);
        }

        public static bool HasProperty(this List<ICalendarProperty> lines, ICalProperty propertyName)
        {
            return lines.HasProperty(propertyName.GetString());
        }
        public static bool HasProperty(this List<ICalendarProperty> lines, string propertyName)
        {
            return lines.Any(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
        }

        internal static ICalendarProperty GetContentLine(this ICalProperty property, ReadOnlySpan<char> value, ContentLineParameters? parameters)
        {
            return property switch
            {
                ICalProperty.CALSCALE or ICalProperty.METHOD or ICalProperty.PRODID or ICalProperty.VERSION or ICalProperty.X_OWNER or ICalProperty.CATEGORIES or ICalProperty.CLASS or ICalProperty.COMMENT or ICalProperty.DESCRIPTION or ICalProperty.GEO or ICalProperty.LOCATION or ICalProperty.PERCENT_COMPLETE or ICalProperty.PRIORITY or ICalProperty.RESOURCES or ICalProperty.STATUS or ICalProperty.SUMMARY or ICalProperty.COMPLETED or ICalProperty.DTEND or ICalProperty.DUE or ICalProperty.DTSTART or ICalProperty.DURATION or ICalProperty.TRANSP or ICalProperty.TZID or ICalProperty.TZNAME or ICalProperty.TZOFFSETFROM or ICalProperty.TZOFFSETTO or ICalProperty.TZURL or ICalProperty.CONTACT or ICalProperty.RECURRENCE_ID or ICalProperty.RELATED_TO or ICalProperty.URL or ICalProperty.UID or ICalProperty.EXDATE or ICalProperty.ACTION or ICalProperty.REPEAT or ICalProperty.CREATED or ICalProperty.DTSTAMP or ICalProperty.LAST_MODIFIED or ICalProperty.SEQUENCE or ICalProperty.REQUEST_STATUS or ICalProperty.BUSYTYPE or ICalProperty.NAME or ICalProperty.REFRESH_INTERVAL or ICalProperty.SOURCE or ICalProperty.COLOR or ICalProperty.IMAGE or ICalProperty.CONFERENCE or ICalProperty.CALENDAR_ADDRESS or ICalProperty.LOCATION_TYPE or ICalProperty.PARTICIPANT_TYPE or ICalProperty.RESOURCE_TYPE or ICalProperty.STRUCTURED_DATA or ICalProperty.STYLED_DESCRIPTION or ICalProperty.ACKNOWLEDGED or ICalProperty.PROXIMITY or ICalProperty.CONCEPT or ICalProperty.LINK or ICalProperty.REFID or ICalProperty.SYNCTOKEN or ICalProperty.ETAG or ICalProperty.EXRULE or ICalProperty.CATEGORY => new CalendarDefaultDataType(property.GetString(), value.ToString(), parameters),
                ICalProperty.ATTACH => new CalendarAttachment(property.GetString(), value.ToString(), parameters),
                ICalProperty.ATTENDEE or ICalProperty.ORGANIZER => new CalendarCalAddress(property.GetString(), value.ToString(), parameters),
                ICalProperty.TRIGGER => new CalendarTrigger(property.GetString(), value.ToString(), parameters),
                ICalProperty.FREEBUSY or ICalProperty.RDATE => new CalendarPeriods(property.GetString(), value.ToString(), parameters),
                ICalProperty.RRULE => new CalendarRecurrenceRule(property.GetString(), value.ToString(), parameters),
                _ => throw new NotSupportedException(property.GetString()),
            };
        }
    }
}