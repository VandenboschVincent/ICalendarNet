using ICalendarNet.Base;
using ICalendarNet.Extensions;

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
                    ICalendarProperty contentLine = GetProperty(string.Join(Environment.NewLine, lines.Slice(currentWorkingLine, linesToWork).ToArray()));
                    currentWorkingLine = x;
                    linesToWork = 1;
                    if (string.IsNullOrEmpty(contentLine.Name) ||
                        (string.IsNullOrEmpty(contentLine.Value) && !contentLine.Parameters.Any()))
                        continue;
                    calendarProperties.Add(contentLine);
                }
                else linesToWork++;
            }
            ICalendarProperty finalcontentLine = GetProperty(string.Join(Environment.NewLine, lines.Slice(currentWorkingLine, linesToWork).ToArray()));
            if (!string.IsNullOrEmpty(finalcontentLine.Name) &&
                (!string.IsNullOrEmpty(finalcontentLine.Value) || finalcontentLine.Parameters.Any()))
                calendarProperties.Add(finalcontentLine);
            return calendarProperties;
        }
        private ICalendarProperty GetProperty(string source)
        {
            ReadOnlySpan<char> chars = source.AsSpan();
            int keyIndex = chars.IndexOf(':');
            List<int> paramIndexes = chars.FindAllIndexes(';', keyIndex).ToList();
            if (paramIndexes.Count > 0)
            {
                List<int> paramSepertorIndexes = chars.FindAllIndexes('=', keyIndex).ToList();
                List<int> MultipleParamSepertors = chars.FindAllIndexes(',', keyIndex).ToList();

                return CreateProperty(source[..(paramIndexes.Any() ? paramIndexes[0] : keyIndex)],
                    source.Substring(keyIndex + 1, source.Length - keyIndex),
                    source.GetParametersOfString(paramIndexes, keyIndex, paramSepertorIndexes, MultipleParamSepertors).ToDictionary()
                    );
            }
            else
            {

                return CreateProperty(source[..(paramIndexes.Any() ? paramIndexes[0] : keyIndex)],
                    source.Substring(keyIndex + 1, source.Length - keyIndex),
                    null
                    );
            }
        }

        private ICalendarProperty CreateProperty(string key, string value, Dictionary<string, IEnumerable<string>>? param)
        {
            return ICalendarPropertyExtensions.GetPropertyType(key).GetContentLine(key, value, GetParameters(param));
        }
    }
}
