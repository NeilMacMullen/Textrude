using System;
using CommandLine;
using Engine.Application;
using SharedApplication;

namespace Textrude;

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

        new Parser(settings =>
            {
                settings.CaseInsensitiveEnumValues = true;
                settings.CaseSensitive = false;
                settings.HelpWriter = Console.Out;
            }).ParseArguments(args,
                typeof(RenderOptions),
                typeof(RenderFromFileOptions),
                typeof(CmdPipe.Options),
                typeof(CmdInfo.Options),
                typeof(CmdItemFilter.Options),
                typeof(CmdItemDo.Options),
                typeof(CmdItemMap.Options),
                typeof(CmdItemMapObject.Options),
                typeof(CmdModelDo.Options),
                typeof(CmdDo.Options)
            )
            .WithParsed<CmdInfo.Options>(o => CmdInfo.Run(o, rte).Wait())
            .WithParsed<RenderOptions>(o => CmdRender.Run(o, rte, sys))
            .WithParsed<RenderFromFileOptions>(o => CmdRenderFromFile.Run(o, rte, sys))
            .WithParsed<CmdPipe.Options>(o => CmdPipe.Run(o, rte, sys))
            .WithParsed<CmdItemFilter.Options>(o => CmdItemFilter.Run(o, rte, sys))
            .WithParsed<CmdItemDo.Options>(o => CmdItemDo.Run(o, rte, sys))
            .WithParsed<CmdItemMap.Options>(o => CmdItemMap.Run(o, rte, sys))
            .WithParsed<CmdItemMapObject.Options>(o => CmdItemMapObject.Run(o, rte, sys))
            .WithParsed<CmdModelDo.Options>(o => CmdModelDo.Run(o, rte, sys))
            .WithParsed<CmdDo.Options>(o => CmdDo.Run(o, rte, sys))
            ;
    }
}
