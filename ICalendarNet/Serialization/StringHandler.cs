using ICalendarNet.Components;
using ICalendarNet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Serialization
{
    /// <summary>
    /// Block that contains content and properties
    /// </summary>
    /// <param name="content"></param>
    /// <param name="componentCount"></param>
    /// <param name="calComponent"></param>
    /// <param name="properties"></param>
    public ref struct CalComponentBlock
    {
        public CalComponentBlock(ReadOnlySpan<char> content, int componentCount, ICalComponent? calComponent, ReadOnlySpan<char> properties)
        {
            Content = content;
            ComponentCount = componentCount;
            CalComponent = calComponent;
            Properties = properties;
        }

        public ICalComponent? CalComponent { get; set; }
        public ReadOnlySpan<char> Content { get; set; }
        public ReadOnlySpan<char> Properties { get; set; }
        public int ComponentCount { get; set; }
    }

    /// <summary>
    /// This class implements a text reader that reads from a string.
    /// Will output that string in blocks that start with BEGIN: and END:
    /// </summary>
    public ref struct StringHandler
    {
        private const int BeginPrefixLength = 6; // "BEGIN:"
        private static readonly Dictionary<ICalComponent, string> EndTokens = new()
        {
            [ICalComponent.VCALENDAR] = "END:VCALENDAR",
            [ICalComponent.VEVENT] = "END:VEVENT",
            [ICalComponent.VTODO] = "END:VTODO",
            [ICalComponent.VJOURNAL] = "END:VJOURNAL",
            [ICalComponent.VFREEBUSY] = "END:VFREEBUSY",
            [ICalComponent.VTIMEZONE] = "END:VTIMEZONE",
            [ICalComponent.STANDARD] = "END:STANDARD",
            [ICalComponent.DAYLIGHT] = "END:DAYLIGHT",
            [ICalComponent.VALARM] = "END:VALARM",
        };
        private readonly ReadOnlySpan<char> reader;
        private readonly List<CalComponentIndex> indexes;
        private int currentWorkingBlock;
        public List<CalendarTimeZone> TimeZones { get; set; } = [];
        public readonly int BlocksLeft => indexes.Count - currentWorkingBlock;

        /// <summary>
        /// Initializes and parses a string in ical format
        /// </summary>
        /// <param name="s"></param>
        public StringHandler(ReadOnlySpan<char> s)
        {
            reader = s;
            indexes = [];
            currentWorkingBlock = 0;
            int i = 0;
            while (i < s.Length)
            {
                //Find the next BEGIN statement
                int indexFound = s.FindIndexOf(CalFilters.vBeginString, i, StringComparison.OrdinalIgnoreCase);

                if (indexes.Count > 0)
                {
                    //Sets the previous End index (only of the parameters) to just after this BEGIN
                    CalComponentIndex previOuseWorkingItem = indexes[^1];
                    previOuseWorkingItem.EndContentIndex = indexFound == -1 ? (s.Length - 1) : (indexFound - 1);
                    indexes[^1] = previOuseWorkingItem;
                }

                if (indexFound == -1)
                    break;

                //Move the index to after BEGIN:
                i = indexFound + BeginPrefixLength;

                CalComponentIndex currentWorkingItem = new()
                {
                    StartIndex = indexFound,
                    CalComponent = GetComponent(i, s)
                };

                if (currentWorkingItem.CalComponent == null)
                    continue;

                //Sets the End index (including subcomponents) to just after this BEGIN
                currentWorkingItem.EndIndex =
                    s.FindIndexOf(EndTokens[currentWorkingItem.CalComponent.Value], i, StringComparison.OrdinalIgnoreCase) + CalFilters.GetEndLength(currentWorkingItem.CalComponent.Value);

                indexes.Add(currentWorkingItem);
            }
        }

        /// <summary>
        /// Get the next component of the ical string
        /// </summary>
        /// <returns></returns>
        public CalComponentBlock GetNextBlock()
        {
            if (indexes.Count <= currentWorkingBlock)
                return new CalComponentBlock();

            CalComponentIndex nextBlock = indexes[currentWorkingBlock];
            currentWorkingBlock++;

            if (nextBlock.CalComponent == null)
                return new CalComponentBlock();

            //Reads the next block
            return new CalComponentBlock(
                //Content (including subcomponents)
                reader[nextBlock.StartIndex..nextBlock.EndIndex],
                //Gets the count of al subcomponents
#if NET6_0_OR_GREATER
                CountSubComponents(nextBlock),
#else
                indexes.Where(FilterSubComponents(nextBlock.CalComponent!.Value)).Count(t => t.StartIndex > nextBlock.StartIndex && t.EndIndex < nextBlock.EndIndex),
#endif
                //Type of the component
                nextBlock.CalComponent.Value,
                //The Content (not including subcomponents)
                reader[nextBlock.StartIndex..nextBlock.EndContentIndex]);
        }

#if NET6_0_OR_GREATER
        private static bool IsValidChild(ICalComponent parent, ICalComponent child) => parent switch
        {
            ICalComponent.VCALENDAR => child is ICalComponent.VEVENT or ICalComponent.VTODO
                                              or ICalComponent.VJOURNAL or ICalComponent.VFREEBUSY
                                              or ICalComponent.VTIMEZONE,
            ICalComponent.VEVENT => child == ICalComponent.VALARM,
            ICalComponent.VTODO => child == ICalComponent.VALARM,
            ICalComponent.VJOURNAL => child == ICalComponent.VALARM,
            ICalComponent.VTIMEZONE => child is ICalComponent.VALARM or ICalComponent.STANDARD or ICalComponent.DAYLIGHT,
            ICalComponent.VFREEBUSY or ICalComponent.STANDARD or ICalComponent.DAYLIGHT or ICalComponent.VALARM => false,
            _ => throw new ArgumentException("invalid component", nameof(parent))
        };

        private int CountSubComponents(in CalComponentIndex parent)
        {
            int count = 0;
            var parentType = parent.CalComponent!.Value;
            // Span over the backing array avoids List<T> indexer/bounds checks
            foreach (var idx in System.Runtime.InteropServices.CollectionsMarshal.AsSpan(indexes))
            {
                if (idx.StartIndex > parent.StartIndex
                    && idx.EndIndex < parent.EndIndex
                    && idx.CalComponent is { } c
                    && IsValidChild(parentType, c))
                {
                    count++;
                }
            }
            return count;
        }
 #else

        /// <summary>
        /// Get all types of component that can be found in the parent component
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static Func<CalComponentIndex, bool> FilterSubComponents(ICalComponent component)
        {
            switch (component)
            {
                case ICalComponent.VCALENDAR:
                    return t => t.CalComponent == ICalComponent.VEVENT ||
                        t.CalComponent == ICalComponent.VTODO ||
                        t.CalComponent == ICalComponent.VJOURNAL ||
                        t.CalComponent == ICalComponent.VFREEBUSY ||
                        t.CalComponent == ICalComponent.VTIMEZONE;

                case ICalComponent.VEVENT:
                    return t => t.CalComponent == ICalComponent.VALARM;

                case ICalComponent.VTODO:
                    return t => t.CalComponent == ICalComponent.VALARM;

                case ICalComponent.VJOURNAL:
                    return t => t.CalComponent == ICalComponent.VALARM;

                case ICalComponent.VFREEBUSY:
                    break;

                case ICalComponent.VTIMEZONE:
                    return t => t.CalComponent == ICalComponent.VALARM ||
                        t.CalComponent == ICalComponent.STANDARD ||
                        t.CalComponent == ICalComponent.DAYLIGHT;

                case ICalComponent.STANDARD:
                    break;

                case ICalComponent.DAYLIGHT:
                    break;

                case ICalComponent.VALARM:
                    break;

                default: throw new ArgumentException(message: "invalid component", paramName: component.ToString());
            }
            return t => false;
        }
#endif
        /// <summary>
        /// Tries to find out what type the next block is
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private static ICalComponent? GetComponent(int startIndex, ReadOnlySpan<char> source)
        {
            // Find end of the BEGIN:XXX line
            var rest = source.Slice(startIndex);
            int eol = rest.IndexOfAny('\r', '\n');
            if (eol < 0) eol = rest.Length;
            var name = rest.Slice(0, eol);

            // Case-insensitive switch on span (no allocation, no reflection)
            if (name.Equals("VEVENT", StringComparison.OrdinalIgnoreCase)) return ICalComponent.VEVENT;
            if (name.Equals("STANDARD", StringComparison.OrdinalIgnoreCase)) return ICalComponent.STANDARD;
            if (name.Equals("DAYLIGHT", StringComparison.OrdinalIgnoreCase)) return ICalComponent.DAYLIGHT;
            if (name.Equals("VTIMEZONE", StringComparison.OrdinalIgnoreCase)) return ICalComponent.VTIMEZONE;
            if (name.Equals("VCALENDAR", StringComparison.OrdinalIgnoreCase)) return ICalComponent.VCALENDAR;
            if (name.Equals("VTODO", StringComparison.OrdinalIgnoreCase)) return ICalComponent.VTODO;
            if (name.Equals("VALARM", StringComparison.OrdinalIgnoreCase)) return ICalComponent.VALARM;
            if (name.Equals("VJOURNAL", StringComparison.OrdinalIgnoreCase)) return ICalComponent.VJOURNAL;
            if (name.Equals("VFREEBUSY", StringComparison.OrdinalIgnoreCase)) return ICalComponent.VFREEBUSY;
            return null;
        }

        private struct CalComponentIndex
        {
            public CalComponentIndex()
            {

            }

            public int StartIndex { get; set; } = -1;
            public int EndIndex { get; set; } = -1;
            public int EndContentIndex { get; set; } = -1;
            public ICalComponent? CalComponent { get; set; } = null;
        }
    }
}