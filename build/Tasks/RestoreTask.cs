using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Restore;
using Cake.Core;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("Restore")]
[TaskDescription("Runs dotnet restore")]
[IsDependentOn(typeof(CleanTask))]
public sealed class RestoreTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetRestore(context.SolutionFile, new DotNetRestoreSettings
        {
            ArgumentCustomization = args => args.Append($"-p:Configuration={context.BuildConfiguration}"),
            Verbosity = DotNetVerbosity.Minimal
        });
    }
}
