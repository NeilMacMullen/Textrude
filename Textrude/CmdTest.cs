using CommandLine;

namespace Textrude
{
    public class CmdTest
    {
        public static void Run(Options o)
        {
        }

        [Verb("test")]
        public class Options
        {
            [Option(Required = true)] public string Model { get; set; } = string.Empty;
        }
    }
}