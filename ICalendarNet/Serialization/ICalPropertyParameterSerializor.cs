using ICalendarNet.Base;
using System.Text;
using System.Text.RegularExpressions;

namespace ICalendarNet.Serialization
{
    public partial class ICalSerializor
    {
        private ContentLineParameters GetParameters(string source)
        {
            ContentLineParameters keyValuePairs = new();
            MatchCollection matches = ContentLineParametersRegex().Matches(source);
            foreach (Match match in matches)
            {
                ContentLineParameter contentLineParameter = GetParameter(match.Groups[1].ToString());
                keyValuePairs[contentLineParameter.Name] = contentLineParameter;
            }

            return keyValuePairs;
        }

        private ContentLineParameter GetParameter(string source)
        {
            Match match = ContentLineNameRegex().Match(source);
            MatchCollection matches = ContentLineValuesRegex().Matches(match.Groups[2].ToString());
            return new(match.Groups[1].ToString().Trim(), matches.Cast<Match>().Select(t => t.Groups[1].ToString().Trim()));
        }

        private string SerializeProperty(ICalendarProperty parentObject)
        {
            return SerializeProperty(parentObject, new StringBuilder()).ToString();
        }
        private StringBuilder SerializeProperty(ICalendarProperty component, StringBuilder builder)
        {
            if (component.Parameters.Any())
            {
                builder.Append(component.Name);
                builder.Append(string.Equals(component.Name, "RRULE", StringComparison.OrdinalIgnoreCase) ? ":" : ";");
                SerializeParameters(component.Parameters, builder);
                builder.Append(string.IsNullOrEmpty(component.Value) ? "" : ":");
                builder.Append(component.Value);
            }
            else
                builder.Append($"{component.Name}:{component.Value}");
            return builder;
        }
        private void SerializeParameters(ContentLineParameters parameters, StringBuilder builder)
        {
            builder.Append(string.Join(";", parameters.Select(t => SerializeParameter(t.Value))));
        }
        private string SerializeParameter(ContentLineParameter parameter)
        {
            return $"{parameter.Name}={string.Join(",", parameter.Values)}";
        }
    }
}
