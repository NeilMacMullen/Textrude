using CommandLine;
using Engine.Application;

namespace Textrude
{
    public class CmdItemDo
    {
        public static void Run(Options options, RunTimeEnvironment rte, Helpers sys)
        {
            var expression = $@"{options.Expression}
ret i";
            var template =
                ConvenienceScriptMaker.ModelPipedToArrayProcessing(expression, "each");

            var renderOptions = options.CreateRenderOptions(template);
            var cmd = new CmdRender(renderOptions, rte, sys);
            cmd.Run();
        }

        [Verb("iDo", HelpText = "Runs operation for each item in the input")]
        public class Options : ConvenienceOptions
        {
        }
    }
}
