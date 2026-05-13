using ICalendarNet.Base;
using ICalendarNet.DataTypes;
using ICalendarNet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Serialization
{
    internal static class CalPropertySerializor
    {
        internal static List<ICalendarProperty> InternalDeserializeContentLines(ReadOnlySpan<char> source)
        {
            List<ICalendarProperty> calendarProperties = new();
            SpanLineEnumerator lineEnumerator = new(source);
            bool needValue = false;
            while (lineEnumerator.MoveNext())
            {
                ReadOnlySpan<char> preProcess = lineEnumerator.Current.TrimStart();
                if (preProcess.Length < 3)
                    continue;

                int count = calendarProperties.Count;
                if (count == 0 && preProcess.StartsWith("BEGIN", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (count > 0 && preProcess.StartsWith("END", StringComparison.OrdinalIgnoreCase))
                    continue;
                int sepIdx = preProcess.IndexOf(':');
                ReadOnlySpan<char> toProcess = sepIdx == 1 ? preProcess[1..] : preProcess;
                if (needValue)
                {
                    ICalendarProperty last = calendarProperties[count - 1];
                    last.Value = sepIdx >= 0
                        ? toProcess[(sepIdx + 1)..].ToString()
                        : toProcess.ToString();
                    needValue = false;
                    continue;
                }
                if (ICalendarPropertyExtensions.TryGetNewProperty(toProcess, out Statics.ICalProperty? property))
                {
                    if (sepIdx >= 0)
                    {
                        calendarProperties.Add(ToContentLine(
                            property,
                            toProcess[..sepIdx],
                            toProcess[(sepIdx + 1)..]));
                        needValue = sepIdx == toProcess.Length - 1;
                    }
                    else
                    {
                        calendarProperties.Add(ToContentLine(
                            property,
                            toProcess,
                            []));
                        needValue = true;
                    }
                }
                else if (count > 0)
                {
                    // Use StringBuilder to reduce intermediate string allocations when appending repeatedly.
                    ICalendarProperty last = calendarProperties[count - 1];
                    string addition = toProcess.ToString();
                    string? prev = last.Value;
                    if (string.IsNullOrEmpty(prev))
                    {
                        last.Value = addition;
                    }
                    else
                    {
                        last.Value = prev + Environment.NewLine + addition;
                    }
                }
            }

            return calendarProperties;
        }

        internal static ICalendarProperty ToContentLine(Statics.ICalProperty? property, ReadOnlySpan<char> key, ReadOnlySpan<char> value)
        {
            if (property != null)
            {
                return ToInternalContentLine(property.Value,
                    key,
                    value);
            }
            else
            {
                return new CalendarDefaultDataType(key.ToString(),
                    value.ToString(),
                    null);
            }
        }

        private static ICalendarProperty ToInternalContentLine(Statics.ICalProperty property, ReadOnlySpan<char> key, ReadOnlySpan<char> value)
        {
            string propName = Statics.ICalProperties[(int)property]; // cached once
            if (key.Length == propName.Length)
                return ICalendarPropertyExtensions.GetContentLine(property,
                    value,
                    null);
#if NET9_0_OR_GREATER
            ReadOnlySpan<char> paramsSpan = key[propName.Length..];
            var dict = new ContentLineParameters();
            foreach (Range segRange in paramsSpan.Split(';'))
            {
                ReadOnlySpan<char> seg = paramsSpan[segRange];
                if (seg.IsEmpty) continue;

                int eq = seg.IndexOf('=');
                if (eq < 0)
                {
                    dict[seg.ToString()] = [];
                }
                else
                {
                    string name = seg[..eq].ToString();
                    ReadOnlySpan<char> rest = seg[(eq + 1)..]; // preserves any further '=' chars
                                                               // Materialize comma-split values
                    var values = new List<string>();
                    foreach (Range vRange in rest.Split(','))
                        values.Add(rest[vRange].ToString());
                    dict[name] = values;
                }
            }

            return ICalendarPropertyExtensions.GetContentLine(property, value, dict);
#else
            return ICalendarPropertyExtensions.GetContentLine(property,
                value,
                key[propName.Length..]
                    .ToString()
                    .Split(';', StringSplitOptions.RemoveEmptyEntries).Select(x =>
                    {
                        string[] splitted = x.Split('=');
                        return new KeyValuePair<string, IEnumerable<string>>(splitted[0], splitted.Length == 1 ? [] : splitted[^1].Split(','));
                    }).ToDictionary());
#endif
        }
    }
}