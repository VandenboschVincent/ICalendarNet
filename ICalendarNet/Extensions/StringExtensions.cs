using System.Buffers;

namespace ICalendarNet.Extensions
{
    public static class StringExtensions
    {
        private const string base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        private static SearchValues<char>? _searchValues;
        private static SearchValues<char> Base64SearchValues
        {
            get 
            {
                if (_searchValues != null)
                    return _searchValues;
                _searchValues = SearchValues.Create(base64Chars);
                return _searchValues;
            } 
        }
        public static bool IsBase64(this string value)
        {
            var trimmed = value.AsSpan().TrimEnd('=');
            var index = trimmed.IndexOfAnyExcept(Base64SearchValues);
            return index == -1;
        }
    }
}
