using ICalendarNet.Extensions;
using ICalendarNet.Models.Base;
using ICalendarNet.Models.Enum;
using System;
using static ICalendarNet.Models.Enum.Statics;

namespace ICalendarNet.Models.Components
{
    public class CalendarStandard : CalendarRecurrableObject
    {
        public override ICalComponent ComponentType => ICalComponent.STANDARD;

        /// <summary>
        ///   <see cref="ICalProperty.TZOFFSETFROM" />
        /// </summary>
        public virtual string? TimezoneOffsetFrom
        {
            get => Properties.GetContentlineValue(ICalProperty.TZOFFSETFROM);
            set => Properties.UpdateLineProperty(value, ICalProperty.TZOFFSETFROM);
        }

        /// <summary>
        ///   <see cref="ICalProperty.TZOFFSETTO" />
        /// </summary>
        public virtual string? TimezoneOffsetTo
        {
            get => Properties.GetContentlineValue(ICalProperty.TZOFFSETTO);
            set => Properties.UpdateLineProperty(value, ICalProperty.TZOFFSETTO);
        }

        /// <summary>
        ///   <see cref="ICalProperty.COMMENT" />
        /// </summary>
        public virtual string? Comment
        {
            get => string.Join(Environment.NewLine, Properties.GetContentlinesValue(ICalProperty.COMMENT));
            set => Properties.UpdateLineProperty(value, ICalProperty.COMMENT);
        }

        /// <summary>
        ///   <see cref="ICalProperty.TZNAME" />
        /// </summary>
        public virtual string? TimezoneName
        {
            get => Properties.GetContentlineValue(ICalProperty.TZNAME);
            set => Properties.UpdateLineProperty(value, ICalProperty.TZNAME);
        }

        protected override CalendarRecurrableObject Clone(DateTimeOffset occurence)
        {
            return CloneComponent(this, occurence);
        }
    }
}