using ICalendarNet.Extensions;
using System.Collections.Generic;

namespace ICalendarNet.Models.Base
{
    public class ContentLineParameters : List<KeyValuePair<string, IEnumerable<string>>>
    {
        private const string EncodingString = "ENCODING";
        public string? Encoding
        {
            get => this.GetValue(EncodingString);
            set => this.SetOrAddValue(EncodingString, value);
        }

        public ContentLineParameters() : base()
        {
        }

        public ContentLineParameters(IEnumerable<KeyValuePair<string, IEnumerable<string>>> collection) : base(collection)
        {
        }

    }
}