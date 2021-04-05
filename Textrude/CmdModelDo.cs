using CommandLine;
using Engine.Application;

namespace Textrude
{
    public class CmdModelDo
    {
        public static void Run(Options options, RunTimeEnvironment rte, Helpers sys)
        {
            var template = ConvenienceScriptMaker.ModelPipedToExpression(options.Expression);

            var renderOptions = options.CreateRenderOptions(template);
            var cmd = new CmdRender(renderOptions, rte, sys);
            cmd.Run();
        }

        [Verb("mDo", HelpText = "Run an arbitrary expression that the model has been piped into")]
        public class Options : ConvenienceOptions
        {
        }
    }
}
