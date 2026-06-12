using ICalendarNet.Extensions;
using ICalendarNet.Logic;
using ICalendarNet.Models.Enum;
using System;
using System.Collections.Generic;
using static ICalendarNet.Models.Enum.Statics;

namespace ICalendarNet.Models.Base
{
    public abstract class CalendarObject : ICalendarComponent
    {
        public abstract ICalComponent ComponentType { get; }
        public List<ICalendarProperty> Properties { get; } = [];
        public List<ICalendarComponent> SubComponents { get; } = [];
        public MetadataContainer Metadata { get; } = new MetadataContainer();

        /// <summary>
        ///   <see cref="ICalProperty.DTSTART" />
        /// </summary>
        public virtual string? GetCustomPropertyValue(string propertyName)
        {
            return Properties.Find(t => t.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))?.GetLineValue();
        }

        public virtual void SetCustomPropertyValue(string propertyName, string? value, ContentLineParameters? parameters = null)
        {
            Properties.UpdateLineProperty(value, propertyName, parameters);
        }
    }
}