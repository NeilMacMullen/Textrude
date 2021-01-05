using CommandLine;
using Engine.Application;

namespace Textrude
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var filesystem = new FileSystemOperations();
            var sys = new Helpers();
            var result = Parser.Default
                .ParseArguments(args,
                    typeof(CmdRender.Options),
                    typeof(CmdInfo.Options)
                )
                .WithParsed<CmdInfo.Options>(o => CmdInfo.Run(o).Wait())
                .WithParsed<CmdRender.Options>(o => CmdRender.Run(o, filesystem, sys));
        }
    }
}