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
        private ICalendarComponent InternalDeserializeComponents(ref StringHandler handler, CalCompontentBlock parentBlock)
        {
            return parentBlock.CalComponent!.Value switch
            {
                ICalComponent.VCALENDAR => InternalDeserializeComponentsBlock(ref handler, new Calendar(), parentBlock),
                ICalComponent.VEVENT => InternalDeserializeComponentsBlock(ref handler, new CalendarEvent(), parentBlock),
                ICalComponent.VTODO => InternalDeserializeComponentsBlock(ref handler, new CalendarTodo(), parentBlock),
                ICalComponent.VJOURNAL => InternalDeserializeComponentsBlock(ref handler, new CalendarJournal(), parentBlock),
                ICalComponent.VFREEBUSY => InternalDeserializeComponentsBlock(ref handler, new CalendarFreeBusy(), parentBlock),
                ICalComponent.VTIMEZONE => InternalDeserializeComponentsBlock(ref handler, new CalendarTimeZone(), parentBlock),
                ICalComponent.STANDARD => InternalDeserializeComponentsBlock(ref handler, new CalendarStandard(), parentBlock),
                ICalComponent.DAYLIGHT => InternalDeserializeComponentsBlock(ref handler, new CalendarDaylight(), parentBlock),
                ICalComponent.VALARM => InternalDeserializeComponentsBlock(ref handler, new CalendarAlarm(), parentBlock),
                _ => throw new ArgumentException(message: "invalid component", paramName: parentBlock.CalComponent!.Value.ToString()),
            };
        }

        private List<T> InternalDeserializeComponents<T>(ref StringHandler handler) where T : ICalendarComponent, new()
        {
            var result = new List<T>();
            while (handler.BlocksLeft > 0)
            {
                result.Add(InternalDeserializeComponentsBlock(ref handler, new T()));
            }
            return result;
        }

        private T InternalDeserializeComponentsBlock<T>(ref StringHandler handler, T parent) where T : ICalendarComponent, new()
        {
            CalCompontentBlock parentBlock = handler.GetNextBlock();
            return InternalDeserializeComponentsBlock(ref handler, parent, parentBlock);
        }

        private T InternalDeserializeComponentsBlock<T>(ref StringHandler handler, T parent, CalCompontentBlock parentBlock) where T : ICalendarComponent, new()
        {
            if (!parentBlock.CalComponent.HasValue)
                throw new ArgumentException($"Could not deserialize to {nameof(parent)}");
            parent.Properties.AddRange(InternalDeserializeContentLines(parentBlock.Properties));
            for (int i = 0; i < parentBlock.ComponentCount; i++)
            {
                CalCompontentBlock block = handler.GetNextBlock();
                if (!block.CalComponent.HasValue)
                    continue;
                parent.SubComponents.Add(InternalDeserializeComponents(ref handler, block));
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