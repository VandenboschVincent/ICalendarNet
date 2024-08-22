using ICalendarNet.Base;
using ICalendarNet.DataTypes;
using ICalendarNet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Serialization
{
    public partial class CalSerializor
    {
        private IEnumerable<ICalendarProperty> InternalDeserializeContentLines(ReadOnlySpan<char> source)
        {
            List<ICalendarProperty> calendarProperties = new List<ICalendarProperty>();
            SpanLineEnumerator lineEnumerator = new SpanLineEnumerator(source);
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
                        calendarProperties[^1].Value = toProcess.Slice(nextPropertySeparator + 1, toProcess.Length - nextPropertySeparator - 1).ToString();
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
                            toProcess.Slice(nextPropertySeparator + 1, toProcess.Length - nextPropertySeparator - 1)));
                    }
                    else
                    {
                        calendarProperties.Add(ToContentLine(
                            property,
                            toProcess,
                            ReadOnlySpan<char>.Empty));
                        needvalue = true;
                    }
                }
                else if (calendarProperties.Count > 0)
                {
                    calendarProperties[^1].Value += 
                        (calendarProperties[^1].Value.Length > 0 ? Environment.NewLine : "") + 
                        toProcess.ToString();
                }
                else
                {
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
                        return new KeyValuePair<string, IEnumerable<string>>(splitted[0], splitted[^1].Split(','));
                    }).ToDictionary());
        }
    }
}