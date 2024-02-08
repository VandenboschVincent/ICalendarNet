using ICalendarNet.Base;
using ICalendarNet.Extensions;
using System.Text.RegularExpressions;

namespace ICalendarNet.Serialization
{
    public partial class ICalSerializor
    {
        private Task DeserializePropertiesToICalObject<T>(string source, T parentObject) where T : ICalendarComponent, new()
        {
            return Task.Run(() => AddContentLines(parentObject.ComponentType, source, parentObject));
        }
        private void AddContentLines(ICalComponent calComponent, string source, ICalendarComponent parentObject)
        {
            string content = GetContentRegex(calComponent).Match(source).Groups[0].Value;
            if (content == string.Empty)
                content = source;
            string[] lines = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            int currentWorkingLine = 0;
            int linesToWork = 1;
            for (int x = 0; x < lines.Length; x++)
            {
                string line = lines[x];
                if (x == 0) continue;
                else if (line.IsNewProperty())
                {
                    ICalendarProperty contentLine = GetProperty(string.Join(Environment.NewLine, lines.Skip(currentWorkingLine).Take(linesToWork)));
                    currentWorkingLine = x;
                    linesToWork = 1;
                    if (string.IsNullOrEmpty(contentLine.Name) ||
                        (string.IsNullOrEmpty(contentLine.Value) && !contentLine.Parameters.Any()))
                        continue;
                    parentObject.contentLines.Enqueue(contentLine);
                }
                else linesToWork++;
            }
            ICalendarProperty finalcontentLine = GetProperty(string.Join(Environment.NewLine, lines.Skip(currentWorkingLine).Take(linesToWork)));
            if (!string.IsNullOrEmpty(finalcontentLine.Name) &&
                (!string.IsNullOrEmpty(finalcontentLine.Value) || finalcontentLine.Parameters.Any()))
                parentObject.contentLines.Enqueue(finalcontentLine);
        }
        private ICalendarProperty GetProperty(string source)
        {
            Match match = ContentLineRegex().Match(source);
            return CreateProperty(match.Groups[1].ToString().Trim(),
                match.Groups[4].ToString().Trim(),
                match.Groups[2].ToString().Trim()
                );
        }
        private ICalendarProperty CreateProperty(string key, string name, string param)
        {
            return ICalendarPropertyExtensions.GetPropertyType(key).GetContentLine(key, name, GetParameters(param));
        }
    }
}
