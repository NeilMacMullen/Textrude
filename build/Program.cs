using Build.Tasks;
using System;
using Cake.Frosting;
using System.IO;
using Cake.Common.Tools.GitVersion;
using Cake.Common.Build;

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
            context.BuildSystem = context.BuildSystem();
            context.Version = context.GitVersion(
                DetermineGitVersionSettings(context)
            );
            
            context.Describe();
        }

        private GitVersionSettings DetermineGitVersionSettings(BuildContext context) {
            if (context.BuildSystem.GitHubActions.IsRunningOnGitHubActions) {
                var workflow = context.BuildSystem.GitHubActions.Environment.Workflow;
                return new GitVersionSettings() {
                    Url = $"{workflow.ServerUrl}/{workflow.Repository}",
                    Branch = workflow.Ref,
                    Commit = workflow.Sha
                };
            } else {
                return new GitVersionSettings(); // run locally - no config needed
            }
        }
    }

    [TaskName("Default")]
    [IsDependentOn(typeof(BuildTask))]
    public class DefaultTask : FrostingTask { }
}
