using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Frosting;

namespace Build.Tasks
{
    [TaskName("Test")]
    [TaskDescription("Runs dotnet test")]
    [IsDependentOn(typeof(BuildTask))]
    public class TestTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.DotNetCoreTest(context.SolutionPath.FullPath, new DotNetCoreTestSettings()
            {
                NoRestore = true,
                NoBuild = true,
                Configuration = context.BuildConfiguration,
                Verbosity = DotNetCoreVerbosity.Minimal
            });
        }
    }
}
