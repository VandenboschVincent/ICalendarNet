﻿using ICalendarNet.Base;
using ICalendarNet.Extensions;
using System.Text;
using System.Text.RegularExpressions;

namespace ICalendarNet.Serialization
{
    public partial class ICalSerializor
    {
        private ContentLineParameters? GetParameters(CaptureCollection keys, CaptureCollection values)
        {
            if (keys.Count == 0)
                return null;
            ContentLineParameters keyValuePairs = new();
            for (int i = 0; i < keys.Count; i++)
            {
                keyValuePairs.SetOrAddValue(keys[i].Value, values[i].Value);
            }

            return keyValuePairs;
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
            builder.Append(string.Join(";", parameters.Select(t => $"{t.Key}={string.Join(",", t.Value)}")));
        }
    }
}
