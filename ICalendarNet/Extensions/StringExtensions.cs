using System;

namespace ICalendarNet.Extensions
{
    public static class StringExtensions
    {
        public static bool IsBase64(this string value)
        {
            Span<byte> buffer = new Span<byte>(new byte[value.Length]);
            return Convert.TryFromBase64String(value, buffer, out int bytesParsed);
        }
    }
}
