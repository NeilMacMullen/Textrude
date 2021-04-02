using Cake.Frosting;

namespace Build.Tasks
{
    [TaskName("Choco")]
    [TaskDescription("Builds the chocolately package at the current version")]
    [IsDependentOn(typeof(PackageTask))]
    public class ChocoTask : ChocoNugetTask
    {
        public override void Run(BuildContext context)
        {
            TargetDir = context.ChocoDir;
            Client = "choco";
            base.Run(context);
            Render.Line("Use 'choco push  chocolatey\textrude...   -s https://push.chocolatey.org/'");
        }
    }
}
