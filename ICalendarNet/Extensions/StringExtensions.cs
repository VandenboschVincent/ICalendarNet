using FastEnumUtility;
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

        public static bool TryReadInt(this ReadOnlySpan<char> s, out int result)
        {
            int acc = 0;
            for (int i = 0; i < s.Length; i++)
            {
                uint d = (uint)(s[i] - '0');
                if (d > 9) { result = 0; return false; }
                acc = acc * 10 + (int)d;
            }
            result = acc;
            return true;
        }

        public static string GetString<TEnum>(this TEnum value) where TEnum : struct, Enum
        {
            var enumMember = FastEnum.GetMember(value);
            return enumMember?.EnumMemberAttribute?.Value ?? enumMember?.Name ?? value.ToString();
        }

        public static bool IsNumeric(char x)
        {
            return x switch
            {
                '+' => true,
                '-' => true,
                '0' => true,
                '1' => true,
                '2' => true,
                '3' => true,
                '4' => true,
                '5' => true,
                '6' => true,
                '7' => true,
                '8' => true,
                '9' => true,
                _ => false,
            };
        }
    }
}