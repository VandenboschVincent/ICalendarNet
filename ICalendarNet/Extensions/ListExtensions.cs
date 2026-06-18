using System.Collections.Generic;

namespace ICalendarNet.Extensions
{
    internal static class ListExtensions
    {
        internal static T? IndexOrDefault<T>(this List<T> list, int index) where T : class
        {
            if (index < 0 || index >= list.Count)
                return default;
            return list[index];
        }
    }
}