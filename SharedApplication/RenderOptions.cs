using System.Collections.Generic;
using System.Linq;
using CommandLine;

namespace SharedApplication;

/// <summary>
///     The options for the render command
/// </summary>
/// <remarks>
///     This class needs to be visible to TextrudeInteractive in order it to perform the ExportInvocation
/// </remarks>
[Verb("render")]
public class RenderOptions
{
    [Option(HelpText = "list of model files")]
    public IEnumerable<string> Models { get; set; } = Enumerable.Empty<string>();

    [Option(Required = true, HelpText = "template file")]
    public string Template { get; set; } = string.Empty;

    [Option(HelpText = "list of output files.  If omitted, output will be written to console")]
    public IEnumerable<string> Output { get; set; } = Enumerable.Empty<string>();

    [Option(HelpText = "list of definitions")]
    public IEnumerable<string> Definitions { get; set; } = Enumerable.Empty<string>();

    [Option(HelpText = "list of additional include paths")]
    public IEnumerable<string> Include { get; set; } = Enumerable.Empty<string>();

    [Option(HelpText = "prevent environment variables being passed to the template")]
    public bool HideEnvironment { get; set; }


    [Option(HelpText = "Only write output files if models/template have been modified since last render pass")]
    public bool Lazy { get; set; }

    [Option(HelpText = "Writes the set of dynamic output files")]
    public bool DynamicOutput { get; set; }

    [Option(HelpText = "Provides commentary on what textrude is doing behind the scenes")]
    public bool Verbose { get; set; }
}
