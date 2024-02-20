using ICalendarNet.Base;

namespace ICalendarNet.Extensions
{
    public static class ContentLineParametersExtensions
    {
        public static string? GetValue(this ContentLineParameters parameters, string key)
        {
            return parameters.GetValueOrDefault(key);
        }

        public static void SetOrAddValue(this ContentLineParameters parameters, string key, string value)
        {
            if (parameters.ContainsKey(key))
                parameters.Remove(key);
            parameters.Add(key, value);
        }
    }
}
