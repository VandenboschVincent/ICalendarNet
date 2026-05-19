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

        public static DateTimeOffset? ConvertToDateTimeOffset(string? value, CalendarTimeZone? tzone = null)
        {
            if (string.IsNullOrEmpty(value)) return null;
            if (value[^1] == 'Z' && (TryParseToDateTime(value, "yyyyMMddTHHmmssZ") ??
                TryParseToDateTime(value, "yyyyMMddTHHmmZ") ??
                TryParseToDateTime(value, "yyyyMMddTHHZ") ??
                TryParseToDateTime(value, "yyyyMMddZ") ??
                TryParseToDateTime(value, "yyyyMMddHHmmssZ") ??
                TryParseToDateTime(value, "yyyyMMddHHmmZ") ??
                TryParseToDateTime(value, "yyyyMMddHHZ")) is DateTimeOffset udate)
                return udate;
            else if ((TryParseToDateTime(value, "yyyyMMddTHHmmss", tzone) ??
                TryParseToDateTime(value, "yyyyMMddTHHmm", tzone) ??
                TryParseToDateTime(value, "yyyyMMddTHH", tzone) ??
                TryParseToDateTime(value, "yyyyMMdd", tzone) ??
                TryParseToDateTime(value, "yyyyMMddHHmmss", tzone) ??
                TryParseToDateTime(value, "yyyyMMddHHmm", tzone) ??
                TryParseToDateTime(value, "yyyyMMddHH", tzone)) is DateTimeOffset date)
                return date;
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

        public static string ConvertFromDateTimeOffset(DateTimeOffset value, CalendarTimeZone? tzone = null)
        {
            string format = "yyyyMMdd";
            if (tzone == null || value.Offset == TimeSpan.Zero)
            {
                value = value.ToUniversalTime();
                if (value.Hour > 0 || value.Minute > 0 || value.Second > 0)
                {
                    format += "THHmmss";
                }
                return value.ToString(format + "Z");
            }
            int offset = tzone.GetOffsetInMinutes(value);
            value = value.ToOffset(TimeSpan.FromMinutes(offset));
            if (value.Hour > 0 || value.Minute > 0 || value.Second > 0)
            {
                format += "THHmmss";
            }
            return value.ToString(format);
        }

        public static string ConvertFromInt(int value)
        {
            return value.ToString();
        }

        private static DateTimeOffset? TryParseToDateTime(string value, string format, CalendarTimeZone? tzone = null)
        {
            DateTimeStyles timeStyles = format.EndsWith('Z') ? DateTimeStyles.AssumeUniversal : DateTimeStyles.AssumeLocal;
            if (DateTimeOffset.TryParseExact(value, format, CultureInfo.InvariantCulture, timeStyles, out DateTimeOffset UtcTime))
            {
                if (timeStyles == DateTimeStyles.AssumeLocal && tzone is not null)
                {
                    int offset = tzone.GetOffsetInMinutes(UtcTime);
                    return new DateTimeOffset(UtcTime.DateTime, TimeSpan.FromMinutes(offset));
                }
                return UtcTime;
            }
            return null;
        }
    }
}