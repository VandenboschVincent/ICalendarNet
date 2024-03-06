using ICalendarNet.Base;

namespace ICalendarNet.Extensions
{
    public static class ContentLineParametersExtensions
    {
        public static string? GetValue(this ContentLineParameters parameters, string key)
        {
            return parameters.GetValueOrDefault(key)?.FirstOrDefault();
        }

        public static IEnumerable<string>? GetValues(this ContentLineParameters parameters, string key)
        {
            return parameters.GetValueOrDefault(key);
        }

        public static void SetOrAddValue(this ContentLineParameters parameters, string key, string value)
        {
            parameters.SetOrAddValue(key, new List<string>() { value });
        }
        public static void SetOrAddValue(this ContentLineParameters parameters, string key, IEnumerable<string> value)
        {
            parameters[key] = value;
        }

        public static ContentLineParameters ToDictionary(this IEnumerable<KeyValuePair<string, IEnumerable<string>>> source) =>
           source.ToDictionary(null);

        public static ContentLineParameters ToDictionary(this IEnumerable<KeyValuePair<string, IEnumerable<string>>> source, IEqualityComparer<string>? comparer)
        {
            return new(source, comparer);
        }

    }
}
