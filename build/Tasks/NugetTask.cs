using Cake.Frosting;

namespace Build.Tasks
{
    [TaskName("Nuget")]
    [TaskDescription("Builds the nuget package at the current version")]
    [IsDependentOn(typeof(PackageTask))]
    public class NugetTask : ChocoNugetTask
    {
        public override void Run(BuildContext context)
        {
            CanSetOutDir = true;
            TargetDir = context.NugetDir;
            Client = "nuget";
            base.Run(context);
            Render.Line("Use 'nuget push  nuget\textrude...'");
        }
    }
}
