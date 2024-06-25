using ICalendarNet.Base;
using ICalendarNet.Components;
using ICalendarNet.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICalendarNet.Serialization
{
    public partial class CalSerializor
    {
        private ICalendarComponent InternalDesirializeComponents(StringHandler handler, CalCompontentBlock parentBlock)
        {
            return parentBlock.CalComponent!.Value switch
            {
                ICalComponent.VCALENDAR => InternalDesirializeComponentsBlock(handler, new Calendar(), parentBlock),
                ICalComponent.VEVENT => InternalDesirializeComponentsBlock(handler, new CalendarEvent(), parentBlock),
                ICalComponent.VTODO => InternalDesirializeComponentsBlock(handler, new CalendarTodo(), parentBlock),
                ICalComponent.VJOURNAL => InternalDesirializeComponentsBlock(handler, new CalendarJournal(), parentBlock),
                ICalComponent.VFREEBUSY => InternalDesirializeComponentsBlock(handler, new CalendarFreeBusy(), parentBlock),
                ICalComponent.VTIMEZONE => InternalDesirializeComponentsBlock(handler, new CalendarTimeZone(), parentBlock),
                ICalComponent.STANDARD => InternalDesirializeComponentsBlock(handler, new CalendarStandard(), parentBlock),
                ICalComponent.DAYLIGHT => InternalDesirializeComponentsBlock(handler, new CalendarDaylight(), parentBlock),
                ICalComponent.VALARM => InternalDesirializeComponentsBlock(handler, new CalendarAlarm(), parentBlock),
                _ => throw new ArgumentException(message: "invalid component", paramName: parentBlock.CalComponent!.Value.ToString()),
            };
        }

        private IEnumerable<T> InternalDeserializeComponents<T>(StringHandler handler) where T : ICalendarComponent, new()
        {
            while (handler.BlocksLeft > 0)
            {
                yield return InternalDesirializeComponentsBlock(handler, new T());
            }
        }

        private T InternalDesirializeComponentsBlock<T>(StringHandler handler, T parent) where T : ICalendarComponent, new()
        {
            CalCompontentBlock parentBlock = handler.GetNextBlock();
            return InternalDesirializeComponentsBlock(handler, parent, parentBlock);
        }

        private T InternalDesirializeComponentsBlock<T>(StringHandler handler, T parent, CalCompontentBlock parentBlock) where T : ICalendarComponent, new()
        {
            if (!parentBlock.CalComponent.HasValue)
                throw new ArgumentException($"Could not desirialize to {nameof(parent)}");
            parent.Properties.AddRange(InternalDeserializeContentLines(parentBlock.Properties));
            for (int i = 0; i < parentBlock.ComponentCount; i++)
            {
                CalCompontentBlock block = handler.GetNextBlock();
                if (!block.CalComponent.HasValue)
                    continue;
                parent.SubComponents.Add(InternalDesirializeComponents(handler, block));
                if (handler.BlocksLeft <= 0)
                    break;
            }
            return parent;
        }

        private StringBuilder SerializeComponent(ICalendarComponent component, StringBuilder builder)
        {
            builder.AppendLine(component.ComponentType.ToBegin());
            for (int i = 0; i < component.Properties.Count; i++)
            {
                SerializeProperty(component.Properties[i], builder);
                builder.AppendLine();
            }
            for (int i = 0; i < component.SubComponents.Count; i++)
            {
                SerializeComponent(component.SubComponents[i], builder);
            }
            builder.AppendLine(component.ComponentType.ToEnd());

            return builder;
        }

        private string SerializeComponent(ICalendarComponent parentObject)
        {
            return SerializeComponent(parentObject, new StringBuilder()).ToString();
        }
    }
}