using System;

namespace ICalendarNet.Extensions
{
    public static class StringExtensions
    {

        public static bool IsBase64(this string value)
        {
            Span<byte> buffer = [.. new byte[value.Length]];
            return Convert.TryFromBase64String(value, buffer, out _);
        }

        public static int FindIndexOf(this ReadOnlySpan<char> span, ReadOnlySpan<char> search, int index, StringComparison stringComparison)
        {
            int found = span[index..].IndexOf(search, stringComparison);
            if (found != -1)
                found += index;
            return found;
        }
    }
}