using ICalendarNet.Base;
using ICalendarNet.DataTypes;
using ICalendarNet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICalendarNet.Serialization
{
    public partial class CalSerializor
    {
        private static IEnumerable<ICalendarProperty> InternalDeserializeContentLines(ReadOnlySpan<char> source)
        {
            List<ICalendarProperty> calendarProperties = [.. new List<ICalendarProperty>()];
            SpanLineEnumerator lineEnumerator = new(source);
            bool needvalue = false;
            int nextPropertySeparator;
            while (lineEnumerator.MoveNext())
            {
                ReadOnlySpan<char> preProcess = lineEnumerator.Current.TrimStart();
                if (preProcess.Length < 3)
                    continue;
                if (calendarProperties.Count == 0 && preProcess.StartsWith("BEGIN", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (calendarProperties.Count > 0 && preProcess.StartsWith("END", StringComparison.OrdinalIgnoreCase))
                    continue;
                nextPropertySeparator = preProcess.IndexOf(':');
                ReadOnlySpan<char> toProcess = nextPropertySeparator == 1 ? preProcess[1..] : preProcess;
                if (needvalue)
                {
                    if (nextPropertySeparator >= 0)
                    {
                        calendarProperties[^1].Value = toProcess[(nextPropertySeparator + 1)..].ToString();
                    }
                    else
                    {
                        calendarProperties[^1].Value = toProcess.ToString();
                    }
                    needvalue = false;
                    continue;
                }
                if (ICalendarPropertyExtensions.TryGetNewProperty(toProcess, out Statics.ICalProperty? property))
                {
                    if (nextPropertySeparator >= 0)
                    {
                        calendarProperties.Add(ToContentLine(
                            property,
                            toProcess[..nextPropertySeparator],
                            toProcess[(nextPropertySeparator + 1)..]));
                        needvalue = nextPropertySeparator == toProcess.Length - 1;
                    }
                    else
                    {
                        calendarProperties.Add(ToContentLine(
                            property,
                            toProcess,
                            []));
                        needvalue = true;
                    }
                }
                else if (calendarProperties.Count > 0)
                {
                    // Use StringBuilder to reduce intermediate string allocations when appending repeatedly.
                    ICalendarProperty last = calendarProperties[^1];
                    string prev = last.Value ?? string.Empty;
                    string addition = toProcess.ToString();
                    if (prev.Length == 0)
                    {
                        last.Value = addition;
                    }
                    else
                    {
                        var sb = new StringBuilder(prev.Length + Environment.NewLine.Length + addition.Length);
                        sb.Append(prev).Append(Environment.NewLine).Append(addition);
                        last.Value = sb.ToString();
                    }
                }
            }

            return calendarProperties;
        }

        public static ICalendarProperty ToContentLine(Statics.ICalProperty? property, ReadOnlySpan<char> key, ReadOnlySpan<char> value)
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
            if (key.Length == Statics.ICalProperties[(int)property].Length)
                return ICalendarPropertyExtensions.GetContentLine(property,
                    value,
                    null);
            return ICalendarPropertyExtensions.GetContentLine(property,
                value,
                key[Statics.ICalProperties[(int)property].Length..]
                    .ToString()
                    .Split(';', StringSplitOptions.RemoveEmptyEntries).Select(x =>
                    {
                        string[] splitted = x.Split('=');
                        return new KeyValuePair<string, IEnumerable<string>>(splitted[0], splitted.Length == 1 ? [] : splitted[^1].Split(','));
                    }).ToDictionary());
        }
    }
}