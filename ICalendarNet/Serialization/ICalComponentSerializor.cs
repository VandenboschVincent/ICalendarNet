using ICalendarNet.Base;
using ICalendarNet.Components;
using ICalendarNet.Extensions;
using iCalNET;
using System.Text;
using System.Text.RegularExpressions;

namespace ICalendarNet.Serialization
{
    public partial class ICalSerializor
    {
        private async Task<T> DeserializeComponentsToICalObject<T>(string source, T parentObject) where T : ICalendarComponent, new()
        {
            await Task.WhenAll(DeserializeComponents(source, parentObject));
            parentObject.Properties.AddRange(parentObject.contentLines);
            parentObject.SubComponents.AddRange(parentObject.components.OrderBy(t => t.ComponentType));
            parentObject.contentLines.Clear();
            parentObject.components.Clear();
            return parentObject;
        }
        private IEnumerable<Task> DeserializeComponents<T>(string source, T parentObject) where T : ICalendarComponent, new()
            => parentObject.ComponentType switch
            {
                ICalComponent.VCALENDAR =>
                    [DeserializePropertiesToICalObject(source, parentObject),
                        .. DeserializeToICalComponents<CalendarEvent>(ICalComponent.VEVENT, source, parentObject),
                        .. DeserializeToICalComponents<CalendarTodo>(ICalComponent.VTODO, source, parentObject),
                        .. DeserializeToICalComponents<CalendarJournal>(ICalComponent.VJOURNAL, source, parentObject),
                        .. DeserializeToICalComponents<CalendarFreeBusy>(ICalComponent.VFREEBUSY, source, parentObject),
                        .. DeserializeToICalComponents<CalendarTimeZone>(ICalComponent.VTIMEZONE, source, parentObject)],
                ICalComponent.VEVENT =>
                    [DeserializePropertiesToICalObject(source, parentObject),
                        .. DeserializeToICalComponents<CalendarAlarm>(ICalComponent.VALARM, source, parentObject)],
                ICalComponent.VTODO =>
                    [DeserializePropertiesToICalObject(source, parentObject),
                        .. DeserializeToICalComponents<CalendarAlarm>(ICalComponent.VALARM, source, parentObject)],
                ICalComponent.VJOURNAL =>
                    [DeserializePropertiesToICalObject(source, parentObject),
                        .. DeserializeToICalComponents<CalendarAlarm>(ICalComponent.VALARM, source, parentObject)],
                ICalComponent.VFREEBUSY =>
                    [DeserializePropertiesToICalObject(source, parentObject)],
                ICalComponent.VTIMEZONE =>
                    [DeserializePropertiesToICalObject(source, parentObject),
                        .. DeserializeToICalComponents<CalendarAlarm>(ICalComponent.VALARM, source, parentObject),
                        .. DeserializeToICalComponents<CalendarStandard>(ICalComponent.STANDARD, source, parentObject),
                        .. DeserializeToICalComponents<CalendarDaylight>(ICalComponent.DAYLIGHT, source, parentObject)],
                ICalComponent.STANDARD =>
                    [DeserializePropertiesToICalObject(source, parentObject)],
                ICalComponent.DAYLIGHT =>
                    [DeserializePropertiesToICalObject(source, parentObject)],
                ICalComponent.VALARM =>
                    [DeserializePropertiesToICalObject(source, parentObject)],
                _ => throw new ArgumentException(message: "invalid component", paramName: parentObject.ComponentType.ToString()),
            };
        private IEnumerable<Task> DeserializeToICalComponents<T>(ICalComponent calComponent, string source, ICalendarComponent parentObject) where T : ICalendarComponent, new()
        {
            return GetObjectRegex(calComponent).Matches(source).Cast<Match>().Select(t => GetComponent<T>(t.Groups[1].ToString(), parentObject));
        }
        private async Task<T> GetComponent<T>(string source, ICalendarComponent parentObject) where T : ICalendarComponent, new()
        {
            T comp = await DeserializeICalComponent<T>(source);
            parentObject.components.Add(comp);
            return comp;
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
