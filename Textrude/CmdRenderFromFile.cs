using System.IO;
using System.Text.Json;
using Engine.Application;
using SharedApplication;
using YamlDotNet.Serialization;

namespace Textrude;

public class CmdRenderFromFile
{
    public static void Run(RenderFromFileOptions o, RunTimeEnvironment rte, Helpers sys)
    {
        var ext = (Path.GetExtension(o.Arguments)).ToLowerInvariant();
        var indirectOptions = new RenderOptions();
        switch (ext)
        {
            case ".yaml":
                indirectOptions =
                    sys.GetOrQuit(
                        () => new Deserializer().Deserialize<RenderOptions>(File.ReadAllText(o.Arguments)),
                        "Unable to load/parse argument file");
                break;
            case ".json":
                indirectOptions =
                    sys.GetOrQuit(() => JsonSerializer.Deserialize<RenderOptions>(File.ReadAllText(o.Arguments)),
                        "Unable to load/parse argument file");
                break;
            default:
                sys.ExitHandler($"unrecognised extension '{ext}' for arguments file");
                return;
        }

        CmdRender.Run(indirectOptions!, rte, sys);
    }
}
