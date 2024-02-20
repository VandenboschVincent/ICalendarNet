namespace ICalendarNet.Base
{
    public ref struct SpanEnumerated
    {
        public readonly Span<string> Span;
        public readonly int Index;
        public SpanEnumerated(Span<string> span, int index)
        {
            Span = span;
            Index = index;
        }
    }
}
