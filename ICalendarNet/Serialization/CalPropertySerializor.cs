using ICalendarNet.Extensions;
using ICalendarNet.Models.Base;
using ICalendarNet.Models.DataTypes;
using ICalendarNet.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Serialization
{
    internal static class CalPropertySerializor
    {
        private const int MinLineLength = 3;
        private const string BeginToken = "BEGIN";
        private const string EndToken = "END";

        internal static List<ICalendarProperty> InternalDeserializeContentLines(ReadOnlySpan<char> source)
        {
            var properties = new List<ICalendarProperty>();
            var lineEnumerator = new SpanLineEnumerator(source);
            bool needsValueOnNextLine = false;

            while (lineEnumerator.MoveNext())
            {
                ReadOnlySpan<char> line = lineEnumerator.Current.TrimStart();

                if (ShouldSkipLine(line, properties.Count))
                    continue;

                int separatorIndex = line.IndexOf(':');
                // Handle leading-colon edge case (e.g. ":VALUE")
                ReadOnlySpan<char> content = separatorIndex == 1 ? line[1..] : line;

                if (needsValueOnNextLine)
                {
                    AssignValueToLastProperty(properties, content, separatorIndex);
                    needsValueOnNextLine = false;
                    continue;
                }

                if (ICalendarPropertyExtensions.TryGetNewProperty(content, out Statics.ICalProperty? property))
                {
                    needsValueOnNextLine = TryAddNewProperty(properties, property, content, separatorIndex);
                }
                else if (properties.Count > 0)
                {
                    AppendContinuationLine(properties[^1], content);
                }
            }

            return properties;
        }

        private static bool ShouldSkipLine(ReadOnlySpan<char> line, int propertyCount)
        {
            if (line.Length < MinLineLength)
                return true;

            // Skip the opening BEGIN line, but only when it's the very first line.
            if (propertyCount == 0 && line.StartsWith(BeginToken, StringComparison.OrdinalIgnoreCase))
                return true;

            // Skip closing END lines once we've started collecting properties.
            if (propertyCount > 0 && line.StartsWith(EndToken, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private static void AssignValueToLastProperty(
            List<ICalendarProperty> properties,
            ReadOnlySpan<char> content,
            int separatorIndex)
        {
            ICalendarProperty last = properties[^1];
            last.Value = separatorIndex >= 0
                ? content[(separatorIndex + 1)..].ToString()
                : content.ToString();
        }

        private static bool TryAddNewProperty(
            List<ICalendarProperty> properties,
            Statics.ICalProperty? property,
            ReadOnlySpan<char> content,
            int separatorIndex)
        {
            if (separatorIndex < 0)
            {
                properties.Add(ToContentLine(property, content, []));
                return true; // value must come on the next line
            }

            ReadOnlySpan<char> key = content[..separatorIndex];
            ReadOnlySpan<char> value = content[(separatorIndex + 1)..];
            properties.Add(ToContentLine(property, key, value));

            // If the colon is the last character, the value continues on the next line.
            return separatorIndex == content.Length - 1;
        }

        private static void AppendContinuationLine(ICalendarProperty target, ReadOnlySpan<char> addition)
        {
            string additionStr = addition.ToString();
            target.Value = string.IsNullOrEmpty(target.Value)
                ? additionStr
                : target.Value + Environment.NewLine + additionStr;
        }

        internal static ICalendarProperty ToContentLine(
            Statics.ICalProperty? property,
            ReadOnlySpan<char> key,
            ReadOnlySpan<char> value)
        {
            return property is null
                ? new CalendarDefaultDataType(key.ToString(), value.ToString(), null)
                : ToInternalContentLine(property.Value, key, value);
        }

        private static ICalendarProperty ToInternalContentLine(
            Statics.ICalProperty property,
            ReadOnlySpan<char> key,
            ReadOnlySpan<char> value)
        {
            string propName = Statics.ICalProperties[(int)property];

            // No parameters attached -> simple content line.
            if (key.Length == propName.Length)
                return ICalendarPropertyExtensions.GetContentLine(property, value, null);

            ReadOnlySpan<char> paramsSpan = key[propName.Length..];
            var parameters = ParseParameters(paramsSpan);
            return ICalendarPropertyExtensions.GetContentLine(property, value, parameters);
        }

#if NET9_0_OR_GREATER
        private static ContentLineParameters ParseParameters(ReadOnlySpan<char> paramsSpan)
        {
            var dict = new ContentLineParameters();

            foreach (Range segRange in paramsSpan.Split(';'))
            {
                ReadOnlySpan<char> segment = paramsSpan[segRange];
                if (segment.IsEmpty)
                    continue;

                int equalsIndex = segment.IndexOf('=');
                if (equalsIndex < 0)
                {
                    dict.Add(new(segment.ToString(), []));
                    continue;
                }

                string name = segment[..equalsIndex].ToString();
                ReadOnlySpan<char> rest = segment[(equalsIndex + 1)..]; // preserves any further '=' chars

                var values = new List<string>();
                foreach (Range valueRange in rest.Split(','))
                    values.Add(rest[valueRange].ToString());

                dict.Add(new(name, values));
            }

            return dict;
        }
#else
        private static ContentLineParameters ParseParameters(ReadOnlySpan<char> paramsSpan)
        {
            return [.. paramsSpan
                .ToString()
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(ParseParameterSegment)];
        }

        private static KeyValuePair<string, IEnumerable<string>> ParseParameterSegment(string segment)
        {
            string[] parts = segment.Split('=');
            IEnumerable<string> values = parts.Length == 1
                ? []
                : parts[^1].Split(',');
            if (string.IsNullOrWhiteSpace(parts[0]))
            {

            }
            return new KeyValuePair<string, IEnumerable<string>>(parts[0], values);
        }
#endif
    }
}