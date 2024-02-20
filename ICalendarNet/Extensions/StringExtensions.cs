using System.Buffers;

namespace ICalendarNet.Extensions
{
    public static class StringExtensions
    {
        private const string base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        private static readonly SearchValues<char> base64SearchValues = SearchValues.Create(base64Chars);
        public static bool IsBase64(this string value)
        {
            return value.AsSpan().TrimEnd('=').IndexOfAnyExcept(base64SearchValues) == -1;
        }
    }
}
