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
                if (needvalue)
                {
                    if (nextPropertySeparator >= 0)
                    {
                        calendarProperties.Last().Value = lineEnumerator.Current.Slice(nextPropertySeparator + 1, lineEnumerator.Current.Length - nextPropertySeparator - 1).ToString();
                    }
                    else
                    {
                        calendarProperties.Last().Value = lineEnumerator.Current.ToString();
                    }
                    needvalue = false;
                    continue;
                }
                if (nextPropertySeparator >= 0)
                {
                    calendarProperties.Add(new CalendarDefaultDataType(
                        lineEnumerator.Current.Slice(0, nextPropertySeparator).ToString(),
                        lineEnumerator.Current.Slice(nextPropertySeparator + 1, lineEnumerator.Current.Length - nextPropertySeparator - 1).ToString(), null));
                }
                else
                {
                    calendarProperties.Add(new CalendarDefaultDataType(
                        lineEnumerator.Current.ToString(),
                        string.Empty, null));
                    needvalue = true;
                }
            }

            return calendarProperties;
        }
        private ICalendarProperty GetProperty(string source)
        {
            Match match = ContentLineRegex().Match(source);
            return CreateProperty(match.Groups[1].ToString().Trim(),
                match.Groups[6].ToString().Trim(),
                match.Groups[4].Captures,
                match.Groups[5].Captures
                );
        }

        private ICalendarProperty CreateProperty(string key, string value, CaptureCollection paramkey, CaptureCollection paramvalue)
        {
            return ICalendarPropertyExtensions.GetPropertyType(key).GetContentLine(key, value, GetParameters(paramkey, paramvalue));
        }
    }
}
