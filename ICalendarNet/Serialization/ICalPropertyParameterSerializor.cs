﻿using ICalendarNet.Base;
using ICalendarNet.Extensions;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ICalendarNet.Serialization
{
    public partial class CalSerializor
    {
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
            builder.Append(string.Join(";", parameters.Select(t => $"{t.Key}{(t.Value.Any() ? "=" : "")}{string.Join(",", t.Value)}")));
        }
    }
}