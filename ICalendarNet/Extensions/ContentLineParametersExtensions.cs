using ICalendarNet.Models.Base;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Extensions
{
    public static class ContentLineParametersExtensions
    {
        public static string? GetValue(this ContentLineParameters parameters, string key) =>
            parameters.GetValues(key)?.FirstOrDefault();

        public static IEnumerable<string>? GetValues(this ContentLineParameters parameters, string key) =>
            parameters.FirstOrDefault(t => t.Key.Equals(key, System.StringComparison.OrdinalIgnoreCase)).Value;

        public static void SetOrAddValue(this ContentLineParameters parameters, string key, string? value)
        {
            parameters.SetOrAddValue(key, value is null ? null : [value]);
        }

        public static void SetOrAddValue(this ContentLineParameters parameters, string key, IEnumerable<string>? value)
        {

            parameters.RemoveAll(t => t.Key.Equals(key, System.StringComparison.OrdinalIgnoreCase));
            if (value != null)
                parameters.Add(new KeyValuePair<string, IEnumerable<string>>(key, value));
        }
    }
}