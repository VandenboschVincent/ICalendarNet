using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace ICalendarNet.Base
{
    public class ContentLineParameters : Dictionary<string, IEnumerable<string>>
    {
        public ContentLineParameters() : base() { }

        public ContentLineParameters(IEqualityComparer<string>? comparer) : base(comparer) { }

        public ContentLineParameters(IDictionary<string, IEnumerable<string>> dictionary) : base(dictionary) { }

        public ContentLineParameters(IDictionary<string, IEnumerable<string>> dictionary, IEqualityComparer<string>? comparer) : base(dictionary, comparer) { }

        public ContentLineParameters(IEnumerable<KeyValuePair<string, IEnumerable<string>>> collection) : base(collection) { }

        public ContentLineParameters(IEnumerable<KeyValuePair<string, IEnumerable<string>>> collection, IEqualityComparer<string>? comparer) : base(collection, comparer) { }
    }
}