using ICalendarNet.Base;
using ICalendarNet.Converters;
using ICalendarNet.DataTypes.Recurrence;
using ICalendarNet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
namespace ICalendarNet.DataTypes
{

    /// <summary>
    /// An iCalendar representation of the <c>RRULE</c> property.
    /// https://tools.ietf.org/html/rfc5545#section-3.3.10
    /// </summary>
    public class CalendarRecurrenceRule : ContentLine
    {
        internal static readonly Dictionary<string, DayOfWeek> dayMap = new(StringComparer.OrdinalIgnoreCase)
        {
            {"MO", DayOfWeek.Monday}, {"TU", DayOfWeek.Tuesday}, {"WE", DayOfWeek.Wednesday},
            {"TH", DayOfWeek.Thursday}, {"FR", DayOfWeek.Friday}, {"SA", DayOfWeek.Saturday}, {"SU", DayOfWeek.Sunday}
        };

        internal static readonly Dictionary<string, FrequencyType> freqMap = new(StringComparer.OrdinalIgnoreCase)
        {
            {"SECONDLY", FrequencyType.Secondly}, {"MINUTELY", FrequencyType.Minutely}, {"HOURLY", FrequencyType.Hourly},
            {"DAILY", FrequencyType.Daily}, {"WEEKLY", FrequencyType.Weekly}, {"MONTHLY", FrequencyType.Monthly},
            {"YEARLY", FrequencyType.Yearly }
        }
;

        /// <summary>
        /// Specifies the frequency <i>FREQ</i> of the recurrence.
        /// The default value is <see cref="FrequencyType.Yearly"/>.
        /// </summary>
        public FrequencyType Frequency
        {
            get => freqMap.GetValueOrDefault(ValueParameters.GetValue("FREQ") ?? string.Empty);
            set => ValueParameters.SetOrAddValue("FREQ", freqMap.First(t => t.Value == value).Key);
        }

        /// <summary>
        /// Specifies the end date of the recurrence (optional).
        /// This property <b>must be null</b> if the <see cref="Count"/> property is set.
        /// </summary>
        public DateTimeOffset? Until
        {
            get => ICalTypeConverters.ConvertToDateTimeOffset(ValueParameters.GetValue("UNTIL"), GetTimeZone());
            set => ValueParameters.SetOrAddValue("UNTIL", ICalTypeConverters.ConvertFromDateTimeOffset(value!.Value, GetTimeZone()));
        }

        /// <summary>
        /// Specifies the number of occurrences of the recurrence (optional).
        /// This property <b>must be null</b> if the <see cref="Until"/> property is set.
        /// </summary>
        public int? Count
        {
            get { 
                var found = ValueParameters.GetValue("COUNT");
                return found is null ? null : int.Parse(found);
            }
            set => ValueParameters.SetOrAddValue("COUNT", value!.ToString()!);
        }


        /// <summary>
        /// The INTERVAL rule part contains a positive integer representing at
        /// which intervals the recurrence rule repeats. The default value is
        /// 1, meaning every second for a SECONDLY rule, every minute for a
        /// MINUTELY rule, every hour for an HOURLY rule, every day for a
        /// DAILY rule, every week for a WEEKLY rule, every month for a
        /// MONTHLY rule, and every year for a YEARLY rule. For example,
        /// within a DAILY rule, a value of 8 means every eight days.
        /// </summary>
        public int Interval
        {
            get
            {
                var found = ValueParameters.GetValue("INTERVAL");
                return found is null ? 1 : int.Parse(found);
            }
            set => ValueParameters.SetOrAddValue("INTERVAL", value!.ToString());
        }

        public List<int> BySecond
        {
            get => [.. ValueParameters.GetValues("BYSECOND")?.Select(int.Parse) ?? []];
            set => ValueParameters.SetOrAddValue("BYSECOND", value.Select(t => t.ToString()));
        }

        /// <summary> The ordinal minutes of the hour associated with this recurrence pattern. Valid values are 0-59. </summary>
        public List<int> ByMinute
        {
            get => [.. ValueParameters.GetValues("BYMINUTE")?.Select(int.Parse) ?? []];
            set => ValueParameters.SetOrAddValue("BYMINUTE", value.Select(t => t.ToString()));
        }

        public List<int> ByHour
        {
            get => [.. ValueParameters.GetValues("BYHOUR")?.Select(int.Parse) ?? []];
            set => ValueParameters.SetOrAddValue("BYHOUR", value.Select(t => t.ToString()));
        }

        public List<WeekDay> ByDay
        {
            get => [.. ValueParameters.GetValues("BYDAY")?.Select(t => new WeekDay(t)) ?? []];
            set => ValueParameters.SetOrAddValue("BYDAY", value.Select(t => t.ToString()));
        }

        /// <summary> The ordinal days of the month associated with this recurrence pattern. Valid values are 1-31. </summary>
        public List<int> ByMonthDay
        {
            get => [.. ValueParameters.GetValues("BYMONTHDAY")?.Select(int.Parse) ?? []];
            set => ValueParameters.SetOrAddValue("BYMONTHDAY", value.Select(t => t.ToString()));
        }

        /// <summary>
        /// The ordinal days of the year associated with this recurrence pattern. Something recurring on the first day of the year would be a list containing
        /// 1, and would also be New Year's Day.
        /// </summary>
        public List<int> ByYearDay
        {
            get => [.. ValueParameters.GetValues("BYYEARDAY")?.Select(int.Parse) ?? []];
            set => ValueParameters.SetOrAddValue("BYYEARDAY", value.Select(t => t.ToString()));
        }

        /// <summary>
        /// The ordinal week of the year. Valid values are -53 to +53. Negative values count backwards from the end of the specified year.
        /// A week is defined by ISO.8601.2004
        /// </summary>
        public List<int> ByWeekNo
        {
            get => [.. ValueParameters.GetValues("BYWEEKNO")?.Select(int.Parse) ?? []];
            set => ValueParameters.SetOrAddValue("BYWEEKNO", value.Select(t => t.ToString()));
        }

        /// <summary>
        /// List of months in the year associated with this rule. Valid values are 1 through 12.
        /// </summary>
        public List<int> ByMonth
        {
            get => [.. ValueParameters.GetValues("BYMONTH")?.Select(int.Parse) ?? []];
            set => ValueParameters.SetOrAddValue("BYMONTH", value.Select(t => t.ToString()));
        }

        /// <summary>
        /// Specify the n-th occurrence within the set of occurrences specified by the RRULE.
        /// It is typically used in conjunction with other rule parts like BYDAY, BYMONTHDAY, etc.
        /// </summary>
        public List<int> BySetPosition
        {
            get => [.. ValueParameters.GetValues("BYSETPOS")?.Select(int.Parse) ?? []];
            set => ValueParameters.SetOrAddValue("BYSETPOS", value.Select(t => t.ToString()));
        }

        public DayOfWeek FirstDayOfWeek
        {
            get => dayMap.GetValueOrDefault(ValueParameters.GetValue("WKST") ?? "MO");
            set => ValueParameters.SetOrAddValue("WKST", dayMap.First(t => t.Value == value).Key);
        }

        public override string Value 
        { 
            get => string.Join(';',ValueParameters.Select(t => $"{t.Key}={string.Join(",", t.Value)}"));
            set {
                if (!string.IsNullOrWhiteSpace(value))
                    ValueParameters = new ContentLineParameters(value.Split(";").Select(t => t.Split('=')).ToDictionary(t => t[0], t => t[1].Split(",").AsEnumerable()));
            }
        }

        /// <summary>
        /// store value as different obj since it is handled as a value but it is actually a set of parameters.
        /// </summary>
        private ContentLineParameters ValueParameters
        {
            get;
            set;
        }

        public CalendarRecurrenceRule(string value) : this(Statics.ICalProperties[(int)Statics.ICalProperty.RRULE], value, null)
        {
        }
        public CalendarRecurrenceRule(string name, string value, ContentLineParameters? parameter) : base(name, value, null)
        {
           ValueParameters ??= parameter ?? [];
        }
    }
}