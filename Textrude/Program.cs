using CommandLine;
using Engine.Application;
using SharedApplication;

namespace Textrude
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Note that the handlers needed to be supplied
            //in order of how picky they are
            var fs = new HybridFileSystem(
                new ConsoleFileSystem(),
                new WebFileSystem(),
                new FileSystem()
            );
            var rte = new RunTimeEnvironment(fs);
            var sys = new Helpers();
            var result = Parser.Default
                    .ParseArguments(args,
                        typeof(RenderOptions),
                        typeof(RenderFromFileOptions),
                        typeof(CmdPipe.Options),
                        typeof(CmdInfo.Options)
                    )
                    .WithParsed<CmdInfo.Options>(o => CmdInfo.Run(o, rte).Wait())
                    .WithParsed<RenderOptions>(o => CmdRender.Run(o, rte, sys))
                    .WithParsed<RenderFromFileOptions>(o => CmdRenderFromFile.Run(o, rte, sys))
                    .WithParsed<CmdPipe.Options>(o => CmdPipe.Run(o, rte, sys))
                ;
        }
    }
}
