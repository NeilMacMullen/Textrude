using CommandLine;
using Engine.Model;
using SharedApplication;

namespace Textrude;

public class ConvenienceOptions
{
    [Option('i', HelpText = "path to data - use '-' or omit for stdin")]
    public string Input { get; set; } = "-";

    [Option('f', HelpText =
        "data format.  One of Json|Csv|Yaml.  If omitted and Data is a file, will try to guess from file extension or content")]
    public ModelFormat Format { get; set; } = ModelFormat.Unknown;

    [Option('e', Required = true, HelpText = "Scriban expression")]
    public string Expression { get; set; } = string.Empty;

    [Option('o', HelpText = "Output file - use '-' or omit for stdout")]
    public string Output { get; set; } = "-";


    public RenderOptions CreateRenderOptions(string filterTemplate)
    {
        return new RenderOptions
        {
            Models = new[] { $"{Format}!m={Input}" },
            Template = filterTemplate,
            Output = new[] { $"{Format}!output={Output}" },
        };
    }
}
