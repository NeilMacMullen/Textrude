using CommandLine;
using Engine.Application;

namespace Textrude;

public class CmdItemFilter
{
    public static void Run(Options options, RunTimeEnvironment rte, Helpers sys)
    {
        var expression = $"ret ({options.Expression})";
        var template =
            ConvenienceScriptMaker.ModelPipedToArrayProcessing(expression, "filter");

        var renderOptions = options.CreateRenderOptions(template);
        var cmd = new CmdRender(renderOptions, rte, sys);
        cmd.Run();
    }

    [Verb("iFilter", HelpText = "Performs a filter operation on structured data")]
    public class Options : ConvenienceOptions
    {
    }
}
