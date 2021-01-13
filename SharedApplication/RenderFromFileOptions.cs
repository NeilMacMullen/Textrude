using CommandLine;

namespace SharedApplication
{
    [Verb("renderFromFile", HelpText = "performs a render but takes the argument list from a yaml or json file")]
    public class RenderFromFileOptions
    {
        [Option(Required = true, HelpText = "file containing arguments (.yaml or .json)")]
        public string Arguments { get; set; }
    }
}