using Build.Tasks;
using System;
using Cake.Frosting;
using System.IO;
using Cake.Core;

namespace Build
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            Spectre.Console.AnsiConsole.Capabilities.SupportsInteraction = true;
            var projectDir = new DirectoryInfo(".");
            Environment.CurrentDirectory = projectDir.Parent.FullName;

            return new CakeHost()
                .UseContext<BuildContext>()
                .UseSetup<BuildSetup>()
                .Run(args);
        }
    }

    public class BuildSetup : FrostingSetup<BuildContext>
    {
        public override void Setup(BuildContext context)
        {
            context.Describe();
        }
    }

    [TaskName("Default")]
    [IsDependentOn(typeof(BuildTask))]
    public class DefaultTask : FrostingTask { }
}
