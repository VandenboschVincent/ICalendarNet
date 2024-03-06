using ICalendarNet.Base;
using ICalendarNet.DataTypes;
using ICalendarNet.Extensions;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ICalendarNet.Serialization
{
    public partial class ICalSerializor
    {
        private IEnumerable<ICalendarProperty> InternalDeserializeContentLines(ICalComponent calComponent, ReadOnlySpan<char> source)
        {
            List<ICalendarProperty> calendarProperties = [];
            var lineEnumerator = source.EnumerateLines();
            bool needvalue = false;
            int nextPropertySeparator;
            while (lineEnumerator.MoveNext())
            {
                if (lineEnumerator.Current.Length < 3)
                    continue;
                if (calendarProperties.Count == 0 && lineEnumerator.Current.StartsWith("BEGIN", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (calendarProperties.Count > 0 && lineEnumerator.Current.StartsWith("END", StringComparison.OrdinalIgnoreCase))
                    continue;
                nextPropertySeparator = lineEnumerator.Current.IndexOf(':');
                ReadOnlySpan<char> toProcess = nextPropertySeparator == 1 ? lineEnumerator.Current.Slice(1, lineEnumerator.Current.Length - 1) : lineEnumerator.Current;
                if (needvalue)
                {
                    if (nextPropertySeparator >= 0)
                    {
                        calendarProperties.Last().Value = toProcess.Slice(nextPropertySeparator + 1, toProcess.Length - nextPropertySeparator - 1).ToString();
                    }
                    else
                    {
                        calendarProperties.Last().Value = toProcess.ToString();
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
                            toProcess.Slice(0, nextPropertySeparator),
                            toProcess.Slice(nextPropertySeparator + 1, toProcess.Length - nextPropertySeparator - 1)));
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
                    calendarProperties[^1].Value += toProcess.ToString();
                }
                else
                {

                }
            }

            return calendarProperties;
        }

        public static ContentLine ToContentLine(Statics.ICalProperty? property, ReadOnlySpan<char> key, ReadOnlySpan<char> value)
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
        public static ContentLine ToInternalContentLine(Statics.ICalProperty property, ReadOnlySpan<char> key, ReadOnlySpan<char> value)
        {
            if(key.Length == Statics.ICalProperties[(int)property].Length)
                return new CalendarDefaultDataType(property,
                    value.ToString(),
                    null);
            return new CalendarDefaultDataType(property,
                value.ToString(),
                key.Slice(Statics.ICalProperties[(int)property].Length, key.Length - Statics.ICalProperties[(int)property].Length).ToString().Split(';', StringSplitOptions.RemoveEmptyEntries).Select(x =>
                {
                    var splitted = x.Split('=');
                    return new KeyValuePair<string, IEnumerable<string>>(splitted.First(), splitted.Skip(1));
                }).ToDictionary());
        }

        private ICalendarProperty? GetProperty(string source)
        {
            Match match = ContentLineRegex().Match(source);
            if (!match.Success)
                return null;
            return CreateProperty(match.Groups[1].ValueSpan,
                match.Groups[6].ValueSpan,
                match.Groups[4].Captures,
                match.Groups[5].Captures
                );
        }

        private ICalendarProperty CreateProperty(ReadOnlySpan<char> key, ReadOnlySpan<char> value, CaptureCollection paramkey, CaptureCollection paramvalue)
        {
            return ICalendarPropertyExtensions.GetPropertyType(key.ToString()).GetContentLine(key, value, GetParameters(paramkey, paramvalue));
        }
    }
}
