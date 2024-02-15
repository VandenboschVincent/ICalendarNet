using ICalendarNet.Base;
using ICalendarNet.DataTypes;
using ICalendarNet.Extensions;

namespace ICalendarNet.UnitTest.DataTypesTests
{
    public class ICalendarPropertyExtensionsTests
    {
        [TestCase("PT1H0M0S", 3600)]
        [TestCase("PT01H0M0S", 3600)]
        [TestCase("PT01H0M20S", 3620)]
        [TestCase("PT15M", 900)]
        [TestCase("PT15M1S", 901)]
        [TestCase("PT", null)]
        [TestCase("T15M", null)]
        [TestCase("46qsd54f9q4df63a64dsfq", null)]
        public void Test_GetContentlineTimeSpan_ShouldReturnTimespan(string value, double? totalSeconds)
        {
            List<ICalendarProperty> lines = [new CalendarDefaultDataType("key", value, null)];

            if (totalSeconds.HasValue)
            {
                lines.GetContentlineTimeSpan("key").Should().Be(TimeSpan.FromSeconds(totalSeconds.Value));
                lines.Clear();
                lines.UpdateLineProperty(TimeSpan.FromSeconds(totalSeconds.Value), "key");
                lines.GetContentlineTimeSpan("key").Should().Be(TimeSpan.FromSeconds(totalSeconds.Value));
            }
            else
            {
                lines.GetContentlineTimeSpan("key").Should().BeNull();
            }
        }

        [TestCase("19950712T140715Z", 629411620350000000)]
        [TestCase("19950712T140715", 629411548350000000)]
        [TestCase("199507121407", 629411548200000000)]
        [TestCase("19950712", 629411040000000000)]
        [TestCase("PT", null)]
        [TestCase("T15M", null)]
        [TestCase("46qsd54f9q4df63a64dsfq", null)]
        public void Test_GetContentlineDateTime_ShouldReturnTimespan(string value, long? ticks)
        {
            List<ICalendarProperty> lines = [new CalendarDefaultDataType("key", value, null)];
            if (ticks.HasValue)
            {
                lines.GetContentlineDateTime("key").Should().Be(new DateTimeOffset(ticks.Value, TimeZoneInfo.Local.BaseUtcOffset));
                lines.Clear();
                lines.UpdateLineProperty(new DateTimeOffset(ticks.Value, TimeZoneInfo.Local.BaseUtcOffset), "key");
                lines.GetContentlineDateTime("key").Should().Be(new DateTimeOffset(ticks.Value, TimeZoneInfo.Local.BaseUtcOffset));
            }
            else
            {
                lines.GetContentlineTimeSpan("key").Should().BeNull();
            }
        }
    }
}
