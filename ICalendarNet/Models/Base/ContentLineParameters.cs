using System.Collections.Generic;
using System.Linq;

namespace ICalendarNet.Models.Base
{
    public class ContentLineParameters : Dictionary<string, IEnumerable<string>>
    {
        private const string EncodingString = "ENCODING";
        public string? Encoding
        {
            get => this.GetValueOrDefault(EncodingString)?.FirstOrDefault();
            set
            {
                if (value == null)
                {
                    Remove(EncodingString);
                }
                else
                {
                    this[EncodingString] = [value];
                }
            }
        }

        public ContentLineParameters() : base()
        {
        }

        public ContentLineParameters(IEqualityComparer<string>? comparer) : base(comparer)
        {
        }

        public ContentLineParameters(IDictionary<string, IEnumerable<string>> dictionary) : base(dictionary)
        {
        }

        public ContentLineParameters(IDictionary<string, IEnumerable<string>> dictionary, IEqualityComparer<string>? comparer) : base(dictionary, comparer)
        {
        }

        public ContentLineParameters(IEnumerable<KeyValuePair<string, IEnumerable<string>>> collection) : base(collection)
        {
        }

        public ContentLineParameters(IEnumerable<KeyValuePair<string, IEnumerable<string>>> collection, IEqualityComparer<string>? comparer) : base(collection, comparer)
        {
        }
    }
}