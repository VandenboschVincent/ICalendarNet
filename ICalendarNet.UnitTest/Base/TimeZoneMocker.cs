using System.Reflection;

namespace ICalendarNet.UnitTest.Base
{
    internal class TimeZoneMocker : IDisposable
    {
        public TimeZoneMocker(TimeZoneInfo mockTimeZoneInfo)
        {
            var info = typeof(TimeZoneInfo).GetField("s_cachedData", BindingFlags.NonPublic | BindingFlags.Static);
            var cachedData = info?.GetValue(null);
            var field = cachedData?.GetType().GetField("_localTimeZone",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Instance);
            field?.SetValue(cachedData, mockTimeZoneInfo);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Cleanup
            TimeZoneInfo.ClearCachedData();
        }
    }
}
