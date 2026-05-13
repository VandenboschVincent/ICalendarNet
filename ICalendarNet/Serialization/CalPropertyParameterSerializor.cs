using ICalendarNet.Base;
using System.Linq;
using System.Text;

namespace ICalendarNet.Serialization
{
    internal static class CalPropertyParameterSerializor
    {
        internal static string SerializeProperty(ICalendarProperty parentObject)
        {
            return SerializeProperty(parentObject, new StringBuilder()).ToString();
        }

        internal static StringBuilder SerializeProperty(ICalendarProperty component, StringBuilder builder)
        {
            if (component.Parameters.Any())
            {
                builder.Append(component.Name);
                builder.Append(';');
                SerializeParameters(component.Parameters, builder);
                builder.Append(string.IsNullOrEmpty(component.Value) ? "" : ":");
                builder.Append(component.Value);
            }
            else
                builder.Append($"{component.Name}:{component.Value}");
            return builder;
        }

        private static void SerializeParameters(ContentLineParameters parameters, StringBuilder builder)
        {
            builder.Append(string.Join(";", parameters.Select(t => $"{t.Key}{(t.Value.Any() ? "=" : "")}{string.Join(",", t.Value)}")));
        }
    }
}