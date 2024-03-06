using ICalendarNet.Base;
using ICalendarNet.DataTypes;
using ICalendarNet.Extensions;
using System.Globalization;

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
            List<ICalendarProperty> lines = [new CalendarDefaultDataType(Statics.ICalProperty.DURATION, value, null)];

            if (totalSeconds.HasValue)
            {
                lines.GetContentlineTimeSpan(Statics.ICalProperty.DURATION).Should().Be(TimeSpan.FromSeconds(totalSeconds.Value));
                lines.Clear();
                lines.UpdateLineProperty(TimeSpan.FromSeconds(totalSeconds.Value), Statics.ICalProperty.DURATION);
                lines.GetContentlineTimeSpan(Statics.ICalProperty.DURATION).Should().Be(TimeSpan.FromSeconds(totalSeconds.Value));
            }
            else
            {
                lines.GetContentlineTimeSpan(Statics.ICalProperty.DURATION).Should().BeNull();
            }
        }

        [TestCase("19950712T140715Z", "07/12/1995 14:07:15", true)]
        [TestCase("19950712T140715", "07/12/1995 14:07:15", false)]
        [TestCase("199507121407", "07/12/1995 14:07:00", false)]
        [TestCase("19950712", "07/12/1995", false)]
        [TestCase("PT", null, false)]
        [TestCase("T15M", null, false)]
        [TestCase("46qsd54f9q4df63a64dsfq", null, false)]
        public void Test_GetContentlineDateTime_ShouldReturnDateTime(string value, string? time, bool utc)
        {
            List<ICalendarProperty> lines = [new CalendarDefaultDataType(Statics.ICalProperty.DTEND, value, null)];
            if (time != null)
            {
                DateTimeStyles timeStyles = utc ? DateTimeStyles.AssumeUniversal : DateTimeStyles.AssumeLocal;
                if (!DateTime.TryParse(time, CultureInfo.InvariantCulture, timeStyles, out DateTime dt))
                    throw new InvalidDataException(time.ToString());
                DateTimeOffset offset = new(dt);

                lines.GetContentlineDateTime(Statics.ICalProperty.DTEND)!.Value.UtcDateTime.Should().Be(offset.UtcDateTime);
                lines.Clear();
                lines.UpdateLineProperty(offset, Statics.ICalProperty.DTEND);
                lines.GetContentlineDateTime(Statics.ICalProperty.DTEND)!.Value.UtcDateTime.Should().Be(offset.UtcDateTime);
            }
            else
            {
                lines.GetContentlineTimeSpan(Statics.ICalProperty.DTEND).Should().BeNull();
            }
        }
    }
}
