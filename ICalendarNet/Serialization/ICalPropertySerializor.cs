using ICalendarNet.Base;
using ICalendarNet.Extensions;
using System.Text.RegularExpressions;

namespace ICalendarNet.Serialization
{
    public partial class ICalSerializor
    {
        private IEnumerable<ICalendarProperty> InternalDeserializeContentLines(ICalComponent calComponent, ReadOnlySpan<string> source)
        {
            List<ICalendarProperty> calendarProperties = [];
            ReadOnlySpan<string> lines = GetComponentContent(source, calComponent);
            if (lines.Length <= 0)
                lines = source;
            int currentWorkingLine = 0;
            int linesToWork = 1;
            for (int x = 0; x < lines.Length; x++)
            {
                string line = lines[x];
                if (x == 0) continue;
                else if (line.IsNewProperty())
                {
                    ICalendarProperty? contentLine = GetProperty(string.Join(Environment.NewLine, lines.Slice(currentWorkingLine, linesToWork).ToArray()));
                    currentWorkingLine = x;
                    linesToWork = 1;
                    if (contentLine == null ||
                        string.IsNullOrEmpty(contentLine.Name) ||
                        (string.IsNullOrEmpty(contentLine.Value) && !contentLine.Parameters.Any()))
                        continue;
                    calendarProperties.Add(contentLine);
                }
                else linesToWork++;
            }
            ICalendarProperty? finalcontentLine = GetProperty(string.Join(Environment.NewLine, lines.Slice(currentWorkingLine, linesToWork).ToArray()));
            if (finalcontentLine != null &&
                !string.IsNullOrEmpty(finalcontentLine.Name) &&
                (!string.IsNullOrEmpty(finalcontentLine.Value) || finalcontentLine.Parameters.Any()))
                calendarProperties.Add(finalcontentLine);
            return calendarProperties;
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
