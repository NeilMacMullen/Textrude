using CommandLine;
using Engine.Application;

namespace Textrude
{
    public class CmdDo
    {
        public static void Run(Options options, RunTimeEnvironment rte, Helpers sys)
        {
            var template = ConvenienceScriptMaker.BareExpression(options.Expression);

            var renderOptions = options.CreateRenderOptions(template);
            var cmd = new CmdRender(renderOptions, rte, sys);
            cmd.Run();
        }

        [Verb("Do", HelpText = "Run an arbitrary expression.  Model is accessible as 'm' ")]
        public class Options : ConvenienceOptions
        {
        }
    }
}
