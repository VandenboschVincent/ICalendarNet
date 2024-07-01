using System.Globalization;
using System;
using System.Linq;
using System.Text;

namespace ICalendarNet.Converters
{
    internal static class TypeConverters
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
                !value![..2].Contains('P') ||
                !value!.Any(char.IsDigit)) return null;
            bool isNegative = string.Equals(value[..2], "-P", StringComparison.OrdinalIgnoreCase);
            var enumerator = value.GetEnumerator();
            StringBuilder currentNumber = new StringBuilder();
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

        public static DateTimeOffset? ConvertToDateTimeOffset(string? value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            if (value[^1] == 'Z' && (TryParseToDateTime(value, "yyyyMMddTHHmmssZ") ??
                TryParseToDateTime(value, "yyyyMMddTHHmmZ") ??
                TryParseToDateTime(value, "yyyyMMddTHHZ") ??
                TryParseToDateTime(value, "yyyyMMddZ") ??
                TryParseToDateTime(value, "yyyyMMddHHmmssZ") ??
                TryParseToDateTime(value, "yyyyMMddHHmmZ") ??
                TryParseToDateTime(value, "yyyyMMddHHZ")) is DateTimeOffset uoffset)
                return uoffset;
            else if ((TryParseToDateTime(value, "yyyyMMddTHHmmss") ??
                TryParseToDateTime(value, "yyyyMMddTHHmm") ??
                TryParseToDateTime(value, "yyyyMMddTHH") ??
                TryParseToDateTime(value, "yyyyMMdd") ??
                TryParseToDateTime(value, "yyyyMMddHHmmss") ??
                TryParseToDateTime(value, "yyyyMMddHHmm") ??
                TryParseToDateTime(value, "yyyyMMddHH")) is DateTimeOffset offset)
                return offset;
            return null;
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
            string format = "P";
            if (value < TimeSpan.Zero)
            {
                format = "-P";
                value = value.Negate();
            }
            if (value.Days != 0)
            {
                if (value.Days % 7 == 0)
                {
                    format += string.Format("{0}W", value.Days / 7);
                }
                else
                    format += string.Format("{0}D", value.Days);
            }
#if NET7_0_OR_GREATER
            if (!double.IsInteger(value.TotalDays))
#else
            if (Math.Abs(value.TotalDays % 1) >= double.Epsilon)
#endif
            {
                format += "T";
                if (value.Hours != 0)
                {
                    format += string.Format("{0}H", value.Hours);
                }
                if (value.Minutes != 0 || value.Seconds != 0)
                {
                    format += string.Format("{0}M", value.Minutes);
                }
                if (value.Seconds != 0)
                {
                    format += string.Format("{0}S", value.Seconds);
                }
            }
            return format;
        }

        public static string ConvertFromDateTimeOffset(DateTimeOffset value)
        {
            value = value.ToUniversalTime();
            string format = "yyyyMMdd";
            if (value.Hour > 0 || value.Minute > 0 || value.Second > 0)
            {
                format += "THHmm";
            }
            if (value.Second > 0)
            {
                format += "ss";
            }
            return value.ToString(format + "Z");
        }

        public static string ConvertFromInt(int value)
        {
            return value.ToString();
        }

        private static TimeSpan? TryParseToTimeSpan(string value, string format)
        {
            if (TimeSpan.TryParseExact(value, format, new CultureInfo("en-US"), out TimeSpan ts))
                return ts;
            return null;
        }

        private static DateTimeOffset? TryParseToDateTime(string value, string format)
        {
            DateTimeStyles timeStyles = format.EndsWith('Z') ? DateTimeStyles.AssumeUniversal : DateTimeStyles.AssumeLocal;
            if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, timeStyles, out DateTime UtcTime))
                return UtcTime;
            return null;
        }
    }
}