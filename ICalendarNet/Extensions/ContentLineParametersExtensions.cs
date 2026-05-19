using ICalendarNet.Models.Base;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Extensions
{
    public static class ContentLineParametersExtensions
    {
        public static string? GetValue(this ContentLineParameters parameters, string key) =>
            parameters.GetValueOrDefault(key)?.FirstOrDefault();

        public static IEnumerable<string>? GetValues(this ContentLineParameters parameters, string key) =>
            parameters.GetValueOrDefault(key);

        public static void SetOrAddValue(this ContentLineParameters parameters, string key, string? value)
        {
            if (value is null)
            {
                parameters.Remove(key);
                return;
            }
            parameters.SetOrAddValue(key, [value]);
        }

        public static void SetOrAddValue(this ContentLineParameters parameters, string key, IEnumerable<string>? value)
        {
            if (value is null)
            {
                parameters.Remove(key);
                return;
            }
            parameters[key] = value;
        }

        public static ContentLineParameters ToDictionary(this IEnumerable<KeyValuePair<string, IEnumerable<string>>> source) =>
           source.ToDictionary(null);

        public static ContentLineParameters ToDictionary(this IEnumerable<KeyValuePair<string, IEnumerable<string>>> source, IEqualityComparer<string>? comparer) =>
#if NET8_0_OR_GREATER
            new(source.DistinctBy(t => t.Key), comparer);

#else
            new ContentLineParameters(source.GroupBy(t => t.Key).Select(g => g.First()), comparer);
#endif
    }
}