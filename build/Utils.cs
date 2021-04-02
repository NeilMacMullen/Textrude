using System;
using System.Text.RegularExpressions;
using Spectre.Console;

namespace Build
{
    public static class Render
    {
        public const string ColGrey = "[grey54]";
        public const string ColGreen = "[green]";
        public const string ColYellow = "[yellow]";
        public const string EndCol = "[/]";

        private static string Sec(string col, string rest) => col + rest + EndCol;
        public static string Grey(this string str) => Sec(ColGrey, str);
        public static string Green(this string str) => Sec(ColGreen, str);
        public static string Yellow(this string str) => Sec(ColYellow, str);

        public static void Line(params string[] items)
        {
            var text = string.Join("", items);

            var line =
                new Markup(text + Environment.NewLine)
                    .Overflow(Overflow.Ellipsis);
            AnsiConsole.Render(line);
        }
    }

    public static class Utils
    {
        public static bool TryMatch(this Regex regex, string input, out Match firstMatch)
        {
            firstMatch = regex.Match(input);
            return firstMatch.Success;
        }

        public static void Run(this ProgressTask task, BuildContext context, Action<BuildContext> action)
        {
            task.StartTask();
            action(context);
            task.Increment(1);
            task.StopTask();
        }
    }
}
