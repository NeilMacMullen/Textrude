using Build.Tasks;
using System;
using Cake.Frosting;
using System.IO;
using Cake.Core;
using Cake.Common.Tools.GitVersion;

namespace Build
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var projectDir = new DirectoryInfo(".");
            Environment.CurrentDirectory = projectDir.Parent.FullName;

            return new CakeHost()
                .InstallTool(new Uri("nuget:?package=ReportGenerator&version=4.8.5"))
                .InstallTool(new Uri("nuget:?package=GitVersion.CommandLine&version=5.6.6"))
                .UseContext<BuildContext>()
                .UseSetup<BuildSetup>()
                .Run(args);
        }
    }

    public class BuildSetup : FrostingSetup<BuildContext>
    {
        public override void Setup(BuildContext context)
        {
            context.Version = context.GitVersion();
            
            context.Describe();
        }
    }

    [TaskName("Default")]
    [IsDependentOn(typeof(BuildTask))]
    public class DefaultTask : FrostingTask { }
}
