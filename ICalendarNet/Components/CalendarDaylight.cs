﻿using ICalendarNet.Base;
using ICalendarNet.Extensions;
using System;
using System.Collections.Generic;
using static ICalendarNet.Statics;

namespace ICalendarNet.Components
{
    public class CalendarDaylight : CalendarObject
    {
        public override ICalComponent ComponentType => ICalComponent.DAYLIGHT;

        /// <summary>
        ///   <see cref="ICalProperty.COMMENT" />
        /// </summary>
        public virtual string? Comment
        {
            get => string.Join(Environment.NewLine, Properties.GetContentlinesValue(ICalProperty.COMMENT));
            set => Properties.UpdateLineProperty(value!, ICalProperty.COMMENT);
        }

        /// <summary>
        ///   <see cref="ICalProperty.TZNAME" />
        /// </summary>
        public virtual string? TimezoneName
        {
            get => Properties.GetContentlineValue(ICalProperty.TZNAME);
            set => Properties.UpdateLineProperty(value!, ICalProperty.TZNAME);
        }

        /// <summary>
        ///   <see cref="ICalProperty.TZOFFSETFROM" />
        /// </summary>
        public virtual string? TimezoneOffsetFrom
        {
            get => Properties.GetContentlineValue(ICalProperty.TZOFFSETFROM);
            set => Properties.UpdateLineProperty(value!, ICalProperty.TZOFFSETFROM);
        }

        /// <summary>
        ///   <see cref="ICalProperty.TZOFFSETTO" />
        /// </summary>
        public virtual string? TimezoneOffsetTo
        {
            get => Properties.GetContentlineValue(ICalProperty.TZOFFSETTO);
            set => Properties.UpdateLineProperty(value!, ICalProperty.TZOFFSETTO);
        }

        /// <summary>
        ///   <see cref="ICalProperty.EXDATE" />
        /// </summary>
        public virtual IEnumerable<DateTimeOffset>? ExceptionDateTimes
        {
            get => Properties.GetContentlineDateTimes(ICalProperty.EXDATE);
            set => Properties.UpdateLineProperty(value!, ICalProperty.EXDATE);
        }
    }
}