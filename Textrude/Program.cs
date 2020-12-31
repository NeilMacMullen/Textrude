using CommandLine;

namespace Textrude
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var result = Parser.Default
                .ParseArguments(args,
                    typeof(CmdRender.Options),
                    typeof(CmdTest.Options)
                )
                .WithParsed<CmdTest.Options>(CmdTest.Run)
                .WithParsed<CmdRender.Options>(CmdRender.Run);
        }
    }
}