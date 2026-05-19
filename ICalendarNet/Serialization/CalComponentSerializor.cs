using ICalendarNet.Extensions;
using ICalendarNet.Models.Base;
using ICalendarNet.Models.Components;
using ICalendarNet.Models.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICalendarNet.Serialization
{
    internal static class CalComponentSerializor
    {
        private static CalendarTimeZone InternalDeserializeTimeZone(ref StringHandler handler, CalComponentBlock parentBlock)
        {
            var block = InternalDeserializeComponentsBlock(ref handler, new CalendarTimeZone(), parentBlock);
            handler.TimeZones.Add(block);
            return block;
        }

        private static ICalendarComponent InternalDeserializeComponents(ref StringHandler handler, CalComponentBlock parentBlock)
        {
            return parentBlock.CalComponent!.Value switch
            {
                ICalComponent.VCALENDAR => InternalDeserializeComponentsBlock(ref handler, new Calendar(), parentBlock),
                ICalComponent.VEVENT => InternalDeserializeComponentsBlock(ref handler, new CalendarEvent(), parentBlock),
                ICalComponent.VTODO => InternalDeserializeComponentsBlock(ref handler, new CalendarTodo(), parentBlock),
                ICalComponent.VJOURNAL => InternalDeserializeComponentsBlock(ref handler, new CalendarJournal(), parentBlock),
                ICalComponent.VFREEBUSY => InternalDeserializeComponentsBlock(ref handler, new CalendarFreeBusy(), parentBlock),
                ICalComponent.VTIMEZONE => InternalDeserializeTimeZone(ref handler, parentBlock),
                ICalComponent.STANDARD => InternalDeserializeComponentsBlock(ref handler, new CalendarStandard(), parentBlock),
                ICalComponent.DAYLIGHT => InternalDeserializeComponentsBlock(ref handler, new CalendarDaylight(), parentBlock),
                ICalComponent.VALARM => InternalDeserializeComponentsBlock(ref handler, new CalendarAlarm(), parentBlock),
                _ => throw new ArgumentException(message: "invalid component", paramName: parentBlock.CalComponent!.Value.ToString()),
            };
        }

        internal static List<T> InternalDeserializeComponents<T>(ref StringHandler handler) where T : ICalendarComponent, new()
        {
            var result = new List<T>();
            while (handler.BlocksLeft > 0)
            {
                result.Add(InternalDeserializeComponentsBlock(ref handler, new T()));
            }
            if (handler.TimeZones.Count > 0)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    SetMetaData(ref handler, result[i]);
                }
            }
            return result;
        }

        private static void SetMetaData(ref StringHandler handler, ICalendarComponent component)
        {
            if (component.ComponentType == ICalComponent.VTIMEZONE)
                return;
            for (int x = 0; x < component.Properties.Count; x++)
            {
                component.Properties[x].Metadata.SetTimeZones(handler.TimeZones);
            }
            component.Metadata.SetTimeZones(handler.TimeZones);
            for (int i = 0; i < component.SubComponents.Count; i++)
            {
                SetMetaData(ref handler, component.SubComponents[i]);
            }
        }

        private static T InternalDeserializeComponentsBlock<T>(ref StringHandler handler, T parent) where T : ICalendarComponent, new()
        {
            CalComponentBlock parentBlock = handler.GetNextBlock();
            return InternalDeserializeComponentsBlock(ref handler, parent, parentBlock);
        }

        private static T InternalDeserializeComponentsBlock<T>(ref StringHandler handler, T parent, CalComponentBlock parentBlock) where T : ICalendarComponent, new()
        {
            if (!parentBlock.CalComponent.HasValue)
                throw new ArgumentException($"Could not deserialize to {nameof(parent)}");
            parent.Properties.AddRange(CalPropertySerializor.InternalDeserializeContentLines(parentBlock.Properties));
            for (int i = 0; i < parentBlock.ComponentCount; i++)
            {
                CalComponentBlock block = handler.GetNextBlock();
                if (!block.CalComponent.HasValue)
                    continue;
                parent.SubComponents.Add(InternalDeserializeComponents(ref handler, block));
                if (handler.BlocksLeft <= 0)
                    break;
            }
            return parent;
        }

        internal static StringBuilder SerializeComponent(ICalendarComponent component, StringBuilder builder)
        {
            builder.AppendLine(component.ComponentType.ToBegin());
            for (int i = 0; i < component.Properties.Count; i++)
            {
                CalPropertyParameterSerializor.SerializeProperty(component.Properties[i], builder);
                builder.AppendLine();
            }
            for (int i = 0; i < component.SubComponents.Count; i++)
            {
                SerializeComponent(component.SubComponents[i], builder);
            }
            builder.AppendLine(component.ComponentType.ToEnd());

            return builder;
        }

        internal static string SerializeComponent(ICalendarComponent parentObject)
        {
            return SerializeComponent(parentObject, new StringBuilder()).ToString();
        }
    }
}