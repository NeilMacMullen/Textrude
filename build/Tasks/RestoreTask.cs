using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Restore;
using Cake.Core;
using Cake.Frosting;

namespace Build.Tasks
{
    [TaskName("Restore")]
    [TaskDescription("Runs dotnet restore")]
    [IsDependentOn(typeof(CleanTask))]
    public sealed class RestoreTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.DotNetCoreRestore(context.SolutionFile, new DotNetCoreRestoreSettings()
            {
                ArgumentCustomization = args => args.Append($"-p:Configuration={context.BuildConfiguration}"),
                Verbosity = DotNetCoreVerbosity.Minimal
            });
        }
    }
}
