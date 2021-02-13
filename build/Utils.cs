using Spectre.Console;
using System;
using System.Text.RegularExpressions;

namespace Build
{
    public static class Utils
    {
        public static Markup BuildMarkup(string format, params object[] args)
        {
            return new Markup(string.Format(format, args));
        }

        public static bool TryMatch(this Regex regex, string input, out Match firstMatch)
        {
            firstMatch = regex.Match(input);
            return firstMatch.Success;
        }
    }
}
