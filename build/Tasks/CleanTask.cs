using Cake.Common;
using Cake.Common.Build;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Clean;
using Cake.Frosting;

namespace Build.Tasks
{
    [TaskName("Clean")]
    [TaskDescription("Runs dotnet clean and cleans the NuGet cache (when in GitHub Actions)")]
    public sealed class CleanTask : FrostingTask<BuildContext>
    {
        public override bool ShouldRun(BuildContext context)
            => context.DoClean;

        public override void Run(BuildContext context)
        {
            context.DotNetCoreClean(context.SolutionPath.FullPath, new DotNetCoreCleanSettings()
            {
                Configuration = context.BuildConfiguration,
                Verbosity = DotNetCoreVerbosity.Minimal
            });

            if (context.GitHubActions().IsRunningOnGitHubActions)
            {
                // NuGet cache has to cleaned so that windows-latest picks up the NuGet packages
                // see https://github.com/dotnet/core/issues/5881
                int nugetLocalsClearResult = context.StartProcess("dotnet", "nuget locals all --clear");
                if (nugetLocalsClearResult != 0)
                    context.Error("Could not clear local NuGet cache!");
            }
        }
    }
}
