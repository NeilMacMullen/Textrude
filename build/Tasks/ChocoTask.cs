using Cake.Common.IO;
using Cake.Frosting;

namespace Build.Tasks;

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
        Render.Line($"Use 'choco push {TargetDir}...   -s https://push.chocolatey.org/'".Yellow());
    }

    protected override void CreatePackage(BuildContext context)
    {
        //Chocolately needs us to add some additional content

        context.CopyFileToDirectory(context.File("LICENSE"), context.PublishDir);
        context.CopyFileToDirectory(context.BuildDir + context.File("VERIFICATION.md"), context.PublishDir);

        base.CreatePackage(context);
    }
}
