using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ICalendarNet.Extensions
{
    public static class StringExtensions
    {
        public static bool IsBase64(this string value)
        {
            Span<byte> buffer = new Span<byte>(new byte[value.Length]);
            return Convert.TryFromBase64String(value, buffer, out int bytesParsed);
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
