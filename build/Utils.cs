using System;
using System.Text.RegularExpressions;

namespace Build
{
    public static class Utils
    {
        public static bool TryMatch(this Regex regex, string input, out Match firstMatch)
        {
            firstMatch = regex.Match(input);
            return firstMatch.Success;
        }

        public static string Truncate(this string value, int maxChars, string elipsis = "...")
        {
            if (maxChars <= elipsis.Length)
                throw new ArgumentException("maxChars must be longer than elipsis!", nameof(maxChars));
            return value.Length <= maxChars - elipsis.Length
                ? value
                : value.Substring(0, maxChars - elipsis.Length) + elipsis;
        }
    }
}
