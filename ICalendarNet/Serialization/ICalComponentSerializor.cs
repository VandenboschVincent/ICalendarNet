using ICalendarNet.Base;
using ICalendarNet.Components;
using ICalendarNet.Extensions;
using System.Text;

namespace ICalendarNet.Serialization
{
    public partial class ICalSerializor
    {
        private T InternalDeserialize<T>(ReadOnlySpan<string> source, T parentObject) where T : ICalendarComponent, new()
        {
            parentObject.Properties.AddRange(InternalDeserializeContentLines(parentObject.ComponentType, source));
            switch (parentObject.ComponentType)
            {
                case ICalComponent.VCALENDAR:
                    parentObject.SubComponents.AddRange(InternalDeserializeComponents<CalendarEvent>(source));
                    parentObject.SubComponents.AddRange(InternalDeserializeComponents<CalendarTodo>(source));
                    parentObject.SubComponents.AddRange(InternalDeserializeComponents<CalendarJournal>(source));
                    parentObject.SubComponents.AddRange(InternalDeserializeComponents<CalendarFreeBusy>(source));
                    parentObject.SubComponents.AddRange(InternalDeserializeComponents<CalendarTimeZone>(source));
                    break;
                case ICalComponent.VEVENT:
                    parentObject.SubComponents.AddRange(InternalDeserializeComponents<CalendarAlarm>(source));
                    break;
                case ICalComponent.VTODO:
                    parentObject.SubComponents.AddRange(InternalDeserializeComponents<CalendarAlarm>(source));
                    break;
                case ICalComponent.VJOURNAL:
                    parentObject.SubComponents.AddRange(InternalDeserializeComponents<CalendarAlarm>(source));
                    break;
                case ICalComponent.VFREEBUSY:
                    break;
                case ICalComponent.VTIMEZONE:
                    parentObject.SubComponents.AddRange(InternalDeserializeComponents<CalendarAlarm>(source));
                    parentObject.SubComponents.AddRange(InternalDeserializeComponents<CalendarStandard>(source));
                    parentObject.SubComponents.AddRange(InternalDeserializeComponents<CalendarDaylight>(source));
                    break;
                case ICalComponent.STANDARD:
                    break;
                case ICalComponent.DAYLIGHT:
                    break;
                case ICalComponent.VALARM:
                    break;
                default: throw new ArgumentException(message: "invalid component", paramName: parentObject.ComponentType.ToString());
            }
            return parentObject;
        }
        private IEnumerable<T> InternalDeserializeComponents<T>(ReadOnlySpan<string> source) where T : ICalendarComponent, new()
        {
            List<T> components = [];
            foreach (var index in GetObjectSourceIndexes<T>(source, new T().ComponentType))
            {
                components.Add(InternalDeserialize(source.Slice(1 + index.Item1, index.Item2 - index.Item1), new T()));
            }
            return components;
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
