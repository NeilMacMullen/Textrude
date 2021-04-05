using CommandLine;
using Engine.Application;

namespace Textrude
{
    public class CmdItemMap
    {
        public static void Run(Options options, RunTimeEnvironment rte, Helpers sys)
        {
            var expression =
                $@"newObj= {{
            {options.Expression}
            }}
ret newObj";

            var template = ConvenienceScriptMaker.ModelPipedToArrayProcessing(expression, "each");

            var renderOptions = options.CreateRenderOptions(template);
            var cmd = new CmdRender(renderOptions, rte, sys);
            cmd.Run();
        }

        [Verb("iMap", HelpText = "Maps each item in the input (accessible as 'i') to a new object")]
        public class Options : ConvenienceOptions
        {
        }
    }
}
