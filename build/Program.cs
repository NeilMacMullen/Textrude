using Build.Tasks;
using System;
using Cake.Frosting;
using System.IO;
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
                .UseModule<Cake.DotNetTool.Module.DotNetToolModule>()
                .InstallTool(new Uri("dotnet:?package=dotnet-reportgenerator-globaltool&version=4.8.6"))
                .InstallTool(new Uri("dotnet:?package=GitVersion.Tool&version=5.6.6"))
                .UseContext<BuildContext>()
                .UseSetup<BuildSetup>()
                .Run(args);
        }
    }

    public class BuildSetup : FrostingSetup<BuildContext>
    {
        public override void Setup(BuildContext context)
        {
            context.Version = context.GitVersion(new GitVersionSettings() {
                Verbosity = GitVersionVerbosity.Debug
            });
            
            context.Describe();
        }
    }

    [TaskName("Default")]
    [IsDependentOn(typeof(BuildTask))]
    public class DefaultTask : FrostingTask { }
}
