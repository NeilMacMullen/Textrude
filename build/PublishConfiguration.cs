using Cake.Common.IO.Paths;
using Cake.Common.Tools.DotNetCore.Publish;

public record PublishConfiguration(
    string Configuration,
    string Runtime,
    string Framework,
    bool SelfContained,
    bool PublishSingleFile,
    bool PublishReadyToRun,
    bool PublishTrimmed,
    ConvertableDirectoryPath PublishDirectory
) {
    public DotNetCorePublishSettings ToDotNetCorePublishSettings() {
        return new DotNetCorePublishSettings() {
            Configuration = Configuration,
            Runtime = Runtime,
            Framework = Framework,
            SelfContained = SelfContained,
            PublishSingleFile = PublishSingleFile,
            PublishTrimmed = PublishTrimmed,
            OutputDirectory = PublishDirectory
        };
    }
}
