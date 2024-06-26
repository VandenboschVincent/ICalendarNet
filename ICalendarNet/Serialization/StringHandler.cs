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
    public ref struct CalCompontentBlock
    {
        public CalCompontentBlock(ReadOnlySpan<char> content, int componentCount, ICalComponent? calComponent, ReadOnlySpan<char> properties)
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
        private ReadOnlySpan<char> reader;
        private readonly List<CalComponentIndex> indexes;
        private int currentWorkingBlock;

        public readonly int BlocksLeft => indexes.Count - currentWorkingBlock;

        /// <summary>
        /// Initializes and parses a string in ical format
        /// </summary>
        /// <param name="s"></param>
        public StringHandler(ReadOnlySpan<char> s)
        {
            reader = s;
            indexes = new List<CalComponentIndex>();
            currentWorkingBlock = 0;
            int i = 0;
            while (i < s.Length)
            {
                //Find the next BEGIN statement
                int indexFound = s.FindIndexOf(CalSerializor.vBeginString, i, StringComparison.OrdinalIgnoreCase);

                if (indexes.Count > 0)
                {
                    //Sets the previous End index (only of the parameters) to just after this BEGIN
                    CalComponentIndex previOuseWorkingItem = indexes[^1];
                    previOuseWorkingItem.EndContentIndex = indexFound == -1 ? (s.Length - 1) : (indexFound - 1);
                }

                if (indexFound == -1)
                    break;

                //Move the index to after BEGIN:
                i = indexFound + 6;

                CalComponentIndex currentWorkingItem = new CalComponentIndex()
                {
                    StartIndex = indexFound,
                    CalComponent = GetComponent(i, s)
                };

                if (currentWorkingItem.CalComponent == null)
                    continue;

                //Sets the End index (including subcomponents) to just after this BEGIN
                currentWorkingItem.EndIndex =
                    s.FindIndexOf($"END:{currentWorkingItem.CalComponent.Value}", i, StringComparison.OrdinalIgnoreCase) + CalSerializor.GetEndLength(currentWorkingItem.CalComponent.Value);

                indexes.Add(currentWorkingItem);
            }
        }

        /// <summary>
        /// Get the next component of the ical string
        /// </summary>
        /// <returns></returns>
        public CalCompontentBlock GetNextBlock()
        {
            if (indexes.Count < currentWorkingBlock)
                return new CalCompontentBlock();

            CalComponentIndex nextBlock = indexes[currentWorkingBlock];
            currentWorkingBlock++;

            if (nextBlock.CalComponent == null)
                return new CalCompontentBlock();

            //Reads the next block
            return new CalCompontentBlock(
                //Content (including subcomponents)
                reader[nextBlock.StartIndex..nextBlock.EndIndex],
                //Gets the count of al subcomponents
                indexes.Where(FilterSubComponents(nextBlock.CalComponent!.Value)).Count(t => t.StartIndex > nextBlock.StartIndex && t.EndIndex < nextBlock.EndIndex),
                //Type of the component
                nextBlock.CalComponent.Value,
                //The Content (not including subcomponents)
                reader[nextBlock.StartIndex..nextBlock.EndContentIndex]);
        }

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

        /// <summary>
        /// Tries to find out what type the next block is
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private static ICalComponent? GetComponent(int startIndex, ReadOnlySpan<char> source)
        {
            return TryGetComponent(startIndex, 7, source)
                ?? TryGetComponent(startIndex, 8, source)
                ?? TryGetComponent(startIndex, 9, source)
                ?? TryGetComponent(startIndex, 6, source);
        }

        /// <summary>
        /// Tries to find out what type by trying to parse an enum
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private static ICalComponent? TryGetComponent(int startIndex, int length, ReadOnlySpan<char> source)
        {

            if (Enum.TryParse(
#if NET6_0_OR_GREATER
                source.Slice(startIndex, length)
#else
                source.Slice(startIndex, length).ToString()
#endif
                , true, out ICalComponent foundComponent))
                return foundComponent;
            return null;
        }

        private sealed class CalComponentIndex
        {
            public int StartIndex { get; set; } = -1;
            public int EndIndex { get; set; } = -1;
            public int EndContentIndex { get; set; } = -1;
            public ICalComponent? CalComponent { get; set; } = null;
        }
    }
}