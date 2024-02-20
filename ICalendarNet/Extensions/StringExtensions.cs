using System.Buffers;

namespace ICalendarNet.Extensions
{
    public static class StringExtensions
    {
        private const string base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        private static readonly SearchValues<char> base64SearchValues = SearchValues.Create(base64Chars);
        public static bool IsBase64(this string value)
        {
            return value.AsSpan().TrimEnd('=').IndexOfAnyExcept(base64SearchValues) == -1;
        }

        public static IEnumerable<int> FindAllIndexes(this ReadOnlySpan<char> span, char toFind, int maxIndex)
        {
            for (int i = 0; i < maxIndex; i++)
            {
                if (span[i] == toFind)
                    yield return i;
            }
        }
        public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetParametersOfString(this ReadOnlySpan<char> value,
            List<int> indexes,
            int maxIndex,
            List<int> paramSeperators,
            List<int> multipleParamSeperator)
        {
            for (int i = 0; i < indexes.Count - 1; i++)
            {
                if (i >= indexes.Count)
                {

                }
                else
                {
                    paramSeperators.Where(t => t > indexes[i] && t < indexes[i + 1]);
                    yield return new KeyValuePair<string, IEnumerable<string>>(
                        value.Slice(indexes[i] + 1, i >= indexes.Count ? maxIndex : indexes[i + 1]).ToString(),
                    );
                }
            }
        }

        private static KeyValuePair<string, IEnumerable<string>> GetParameterOfString(string key,
            List<string> values)
        {
            return new KeyValuePair<string, IEnumerable<string>>(key, values);
        }
    }
}
