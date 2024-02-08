namespace ICalendarNet.Extensions
{
    public static class ICalComponentExtensions
    {
        public static string ToBegin(this ICalComponent component)
        {
            return $"BEGIN:{component}".ToUpper();
        }
        public static string ToEnd(this ICalComponent component)
        {
            return $"END:{component}".ToUpper();
        }
    }
}
