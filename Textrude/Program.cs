using CommandLine;
using Engine.Application;
using SharedApplication;

namespace Textrude
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var rte = new RunTimeEnvironment(new FileSystemOperations());
            var sys = new Helpers();
            var result = Parser.Default
                    .ParseArguments(args,
                        typeof(RenderOptions),
                        typeof(RenderFromFileOptions),
                        typeof(CmdInfo.Options)
                    )
                    .WithParsed<CmdInfo.Options>(o => CmdInfo.Run(o, rte).Wait())
                    .WithParsed<RenderOptions>(o => CmdRender.Run(o, rte, sys))
                    .WithParsed<RenderFromFileOptions>(o => CmdRenderFromFile.Run(o, rte, sys))
                ;
        }
    }
}