using System.Globalization;

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
            if (string.IsNullOrEmpty(value)) return null;
            if ((TryParseToTimeSpan(value, "'PT'%h'H'%m'M'%s'S'") ??
                TryParseToTimeSpan(value, "'PT'%h'H'%m'M'") ??
                TryParseToTimeSpan(value, "'PT'%h'H'") ??
                TryParseToTimeSpan(value, "'PT'%h'H'%s'S'") ??
                TryParseToTimeSpan(value, "'PT'%m'M'%s'S'") ??
                TryParseToTimeSpan(value, "'PT'%m'M'"))
                is TimeSpan ts)
                return ts;
            return null;
        }
        public static DateTimeOffset? ConvertToDateTimeOffset(string? value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            if (value.Last() == 'Z' && (TryParseToDateTime(value, "yyyyMMddTHHmmssZ") ??
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
            string format = "'PT'";
            if (value.Hours > 0)
            {
                format += "%h'H'";
            }
            if (value.Minutes > 0 || value.Seconds > 0)
            {
                format += "%m'M'";
            }
            if (value.Seconds > 0)
            {
                format += "%s'S'";
            }
            return value.ToString(format);
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
