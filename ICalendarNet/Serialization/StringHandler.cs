using ICalendarNet.Base;
using ICalendarNet.Components;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICalendarNet.Serialization
{
    public ref struct CalCompontentBlock(ReadOnlySpan<char> content, int componentCount, ICalComponent? calComponent, ReadOnlySpan<char> properties)
    {
        public ICalComponent? CalComponent { get; set; } = calComponent;
        public ReadOnlySpan<char> Content { get; set; } = content;
        public ReadOnlySpan<char> Properties { get; set; } = properties;
        public int ComponentCount { get; set; } = componentCount;

    }
    // This class implements a text reader that reads from a string.
    public class StringHandler : IDisposable
    {
        private string? reader;
        private readonly List<CalComponentIndex> indexes = [];
        private int currentWorkingBlock = 0;

        public int BlocksLeft => indexes.Count - currentWorkingBlock;

        public StringHandler(string s)
        {
            reader = s;
            for (int i = 0; i < s.Length;)
            {
                int indexFound = s.IndexOf(ICalSerializor.vBeginString, i, StringComparison.OrdinalIgnoreCase);

                if (indexes.Count > 0)
                {
                    CalComponentIndex previOuseWorkingItem = indexes.Last();
                    previOuseWorkingItem.EndContentIndex = indexFound == -1 ? (s.Length - 1) : (indexFound - 1);
                }

                if (indexFound == -1)
                    break;

                i = indexFound + 6;

                CalComponentIndex currentWorkingItem = new()
                {
                    StartIndex = indexFound,
                    CalComponent = GetComponent(i, s)
                };

                if (currentWorkingItem.CalComponent == null)
                    continue;

                currentWorkingItem.EndIndex = 
                    s.IndexOf($"END:{currentWorkingItem.CalComponent.Value}", i, StringComparison.OrdinalIgnoreCase) + ICalSerializor.GetEndLength(currentWorkingItem.CalComponent.Value);

                indexes.Add(currentWorkingItem);
            }
        }

        public CalCompontentBlock GetNextBlock()
        {
            if (indexes.Count < currentWorkingBlock)
                return new CalCompontentBlock();

            CalComponentIndex lastblock = indexes[currentWorkingBlock];
            currentWorkingBlock++;

            if (lastblock.CalComponent == null)
                return new CalCompontentBlock();

            return new CalCompontentBlock(
                reader.AsSpan().Slice(lastblock.StartIndex, lastblock.EndIndex - lastblock.StartIndex),
                indexes.Where(FilterSubComponents(lastblock.CalComponent!.Value)).Count(t => t.StartIndex > lastblock.StartIndex && t.EndIndex < lastblock.EndIndex),
                lastblock.CalComponent.Value,
                reader.AsSpan().Slice(lastblock.StartIndex, lastblock.EndContentIndex - lastblock.StartIndex));
        }

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
                };
            return t => false;
        }

        private static ICalComponent? GetComponent(int startIndex, ReadOnlySpan<char> source)
        {
            return TryGetComponent(startIndex, 7, source)
                ?? TryGetComponent(startIndex, 8, source)
                ?? TryGetComponent(startIndex, 9, source)
                ?? TryGetComponent(startIndex, 6, source);
        }

        private static ICalComponent? TryGetComponent(int startIndex, int length, ReadOnlySpan<char> source)
        {
            if (Enum.TryParse(source.Slice(startIndex, length), true, out ICalComponent foundComponent))
            {
                return foundComponent;
            }
            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                indexes.Clear();
                reader = null;
                currentWorkingBlock = 0;
            }
        }


        private class CalComponentIndex
        {
            public int StartIndex { get; set; } = -1;
            public int EndIndex { get; set; } = -1;
            public int EndContentIndex { get; set; } = -1;
            public ICalComponent? CalComponent { get; set; } = null;
        }
    }
}
