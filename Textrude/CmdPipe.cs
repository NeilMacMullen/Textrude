using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using Engine.Application;
using SharedApplication;

namespace Textrude;

public class CmdPipe
{
    private readonly Options _options;
    private readonly RunTimeEnvironment _rte;
    private readonly Helpers _sys;

    public CmdPipe(Options options, RunTimeEnvironment rte, Helpers sys)
    {
        _options = options;
        _rte = rte;
        _sys = sys;
    }

    public void Run()
    {
        var renderOptions = new RenderOptions
        {
            Models = new[] { "line!model=-" },
            Template = _options.Template
        };
        var cmd = new CmdRender(renderOptions, _rte, _sys);
        cmd.Run();
    }

    public static void Run(Options options, RunTimeEnvironment rte, Helpers sys)
    {
        var cmd = new CmdPipe(options, rte, sys);
        cmd.Run();
    }

    [Verb("pipe", HelpText = "Performs a render by applying a template to stdin then writing to stdout")]
    public class Options
    {
        [Value(0, MetaName = "template", Required = true, HelpText = "path to template file")]
        public string Template { get; set; } = string.Empty;

        [Usage]
        public static IEnumerable<Example> Examples => new[]
        {
            new Example("apply the template 'clean.sbn' to stdin",
                new Options { Template = "clean.sbn" })
        };
    }
}
