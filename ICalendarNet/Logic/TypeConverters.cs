using ICalendarNet.Extensions;
using ICalendarNet.Models.Components;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ICalendarNet.Logic
{
    public static class TypeConverters
    {
        public static double? ConvertToDouble(string? value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            if (double.TryParse(value, out double result)) return result;
            return null;
        }
        public static TimeSpan? ConvertToTimeSpan(string? value)
        {
            if (string.IsNullOrEmpty(value) ||
                !value[..2].Contains('P') ||
                !value.Any(char.IsDigit)) return null;
            bool isNegative = string.Equals(value[..2], "-P", StringComparison.OrdinalIgnoreCase);
            var enumerator = value.GetEnumerator();
            StringBuilder currentNumber = new();
            TimeSpan result = TimeSpan.Zero;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (char.IsNumber(current))
                {
                    currentNumber.Append(current);
                }
                else if (int.TryParse(currentNumber.ToString(), out int numberFound))
                {
                    switch (current)
                    {
                        case 'D':
                            result += TimeSpan.FromDays(numberFound);
                            break;

                        case 'H':
                            result += TimeSpan.FromHours(numberFound);
                            break;

                        case 'M':
                            result += TimeSpan.FromMinutes(numberFound);
                            break;

                        case 'S':
                            result += TimeSpan.FromSeconds(numberFound);
                            break;

                        case 'W':
                            result += TimeSpan.FromDays(numberFound * 7);
                            break;

                        default:
                            break;
                    }
                    currentNumber.Clear();
                }
            }
            if (isNegative) result = result.Negate();
            return result;
        }

        public static DateTimeOffset? ConvertToDateTimeOffset(ReadOnlySpan<char> value, CalendarTimeZone? tzone = null)
        {
            if (value.IsEmpty) return null;

            // Trailing 'Z' => UTC
            bool isUtc = value[^1] == 'Z';
            if (isUtc) value = value[..^1];

            // Must have at least the date (8 digits)
            if (value.Length < 8) return null;

            if (!value.Slice(0, 4).TryReadInt(out int year)) return null;
            if (!value.Slice(4, 2).TryReadInt(out int month)) return null;
            if (!value.Slice(6, 2).TryReadInt(out int day)) return null;

            int idx = 8;
            int hour = 0, minute = 0, second = 0;

            if (idx < value.Length)
            {
                if (value[idx] == 'T') idx++;        // optional separator

                int remaining = value.Length - idx;
                // remaining must be exactly 2 (HH), 4 (HHmm), or 6 (HHmmss)
                if (remaining is not (2 or 4 or 6)) return null;

                if (remaining >= 2 && !value.Slice(idx, 2).TryReadInt(out hour)) return null;
                if (remaining >= 4 && !value.Slice(idx + 2, 2).TryReadInt(out minute)) return null;
                if (remaining >= 6 && !value.Slice(idx + 4, 2).TryReadInt(out second)) return null;
            }

            // Validate ranges (DateTime ctor throws otherwise)
            if (month is < 1 or > 12 || day < 1 || year > 9999 || year < 1 || day > DateTime.DaysInMonth(year, month)
                || hour > 23 || minute > 59 || second > 59)
                return null;

            if (isUtc)
                return new DateTimeOffset(year, month, day, hour, minute, second, TimeSpan.Zero);

            var local = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
            if (tzone is not null)
            {
                int offset = tzone.GetOffsetInMinutes(local);
                return new DateTimeOffset(local, TimeSpan.FromMinutes(offset));
            }
            // matches DateTimeStyles.AssumeLocal behavior
            return new DateTimeOffset(local, TimeZoneInfo.Local.GetUtcOffset(local));
        }

        public static int? ConvertToInt(string? value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            if (int.TryParse(value, out int result)) return result;
            return null;
        }

        public static string ConvertFromDouble(double value)
        {
            return value.ToString();
        }

        public static string ConvertFromTimeSpan(TimeSpan value)
        {
            var sb = new StringBuilder(16);

            if (value < TimeSpan.Zero)
            {
                sb.Append('-');
                value = value.Negate();
            }
            sb.Append('P');

            if (value.Days != 0)
            {
                if (value.Days % 7 == 0)
                    sb.Append(value.Days / 7).Append('W');
                else
                    sb.Append(value.Days).Append('D');
            }

#if NET7_0_OR_GREATER
            if (!double.IsInteger(value.TotalDays))
#else
            if (value.Ticks % TimeSpan.TicksPerDay != 0)
#endif
            {
                sb.Append('T');
                if (value.Hours != 0)
                    sb.Append(value.Hours).Append('H');
                if (value.Minutes != 0 || value.Seconds != 0)
                    sb.Append(value.Minutes).Append('M');
                if (value.Seconds != 0)
                    sb.Append(value.Seconds).Append('S');
            }

            return sb.ToString();
        }

        public static string ConvertFromDateTimeOffset(DateTimeOffset value, CalendarTimeZone? tzone = null)
        {
            bool isUtc = tzone is null;

            value = isUtc
                ? value.ToUniversalTime()
                : value.ToOffset(TimeSpan.FromMinutes(tzone.GetOffsetInMinutes(value)));

            bool hasTime = value.Hour > 0 || value.Minute > 0 || value.Second > 0;

            // Max length is "yyyyMMddTHHmmssZ" = 16 chars
            Span<char> buffer = stackalloc char[16];

            ReadOnlySpan<char> format = (isUtc, hasTime) switch
            {
                (true, true) => "yyyyMMddTHHmmssZ",
                (true, false) => "yyyyMMddZ",
                (false, true) => "yyyyMMddTHHmmss",
                (false, false) => "yyyyMMdd",
            };

            value.TryFormat(buffer, out int written, format, CultureInfo.InvariantCulture);
            return new string(buffer[..written]);
        }

        public static string ConvertFromInt(int value)
        {
            return value.ToString();
        }

        internal static string ConvertFromBase64(string value)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }

        internal static string ConvertToBase64(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

    }
}