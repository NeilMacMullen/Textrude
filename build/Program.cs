using System;
using System.IO;
using Build.Tasks;
using Cake.Core;
using Cake.Frosting;

namespace Build;

public static class Program
{
    public static int Main(string[] args)
    {
        var projectDir = new DirectoryInfo(".");
        Environment.CurrentDirectory = projectDir.Parent.FullName;

        return new CakeHost()
            .InstallTool(new Uri("nuget:?package=ReportGenerator&version=5.4.9"))
            .UseContext<BuildContext>()
            .UseSetup<BuildSetup>()
            .Run(args);
    }
}

public class BuildSetup : FrostingSetup<BuildContext>
{
   public override void Setup(BuildContext buildContext,ISetupContext setupContext)
    {
        buildContext.Describe();
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(PackageTask))]
public class DefaultTask : FrostingTask
{
}
