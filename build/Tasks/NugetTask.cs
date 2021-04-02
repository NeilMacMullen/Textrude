using Cake.Common;
using Cake.Common.Build;
using Cake.Common.IO;
using Cake.Common.IO.Paths;
using Cake.Common.Tools.DotNetCore;
using Cake.Core.IO;
using Cake.Frosting;
using Spectre.Console;

namespace Build.Tasks
{
    [TaskName("Nuget")]
    [TaskDescription("Builds the nuget package at the current version")]
    [IsDependentOn(typeof(PackageTask))]
    public sealed class NugetTask : FrostingTask<BuildContext>
    {
        private const string NuspecName = "textrude.nuspec";
        private ConvertableFilePath TargetNuspec;
        private ConvertableFilePath VersionJson;

        public override void Run(BuildContext context)
        {
            if (context.GitHubActions().IsRunningOnGitHubActions)
            {
                //don't build this on github
            }
            else
            {
                context.CleanDirectory(context.NugetDir);
                VersionJson = context.NugetDir + context.File("version.json");
                TargetNuspec = context.NugetDir + context.File(NuspecName);

                AnsiConsole.Progress()
                    .AutoClear(false)
                    .AutoRefresh(true)
                    .Columns(new ProgressColumn[]
                    {
                        new TaskDescriptionColumn(),
                        new ProgressBarColumn(),
                        new PercentageColumn(),
                        new RemainingTimeColumn(),
                        new SpinnerColumn(Spinner.Known.CircleQuarters),
                    })
                    .Start(ctx =>
                    {
                        var getVersionTask = ctx.AddTask("Gitversion ", new ProgressTaskSettings
                        {
                            MaxValue = 1,
                            AutoStart = false
                        });
                        var nuspecTask = ctx.AddTask("Create nuspec", new ProgressTaskSettings
                        {
                            MaxValue = 1,
                            AutoStart = false
                        });
                        var packTask = ctx.AddTask("Pack", new ProgressTaskSettings
                        {
                            MaxValue = 1,
                            AutoStart = false
                        });

                        var installTask = ctx.AddTask("Test unpack", new ProgressTaskSettings
                        {
                            MaxValue = 1,
                            AutoStart = false
                        });

                        getVersionTask.Run(context, GetVersion);
                        nuspecTask.Run(context, MakeNuspec);
                        packTask.Run(context, CreatePackage);
                        installTask.Run(context, InstallPackage);

                        $"Nuget artefacts are in {context.NugetDir}".Yellow();
                    });
            }
        }

        private void GetVersion(BuildContext context)
        {
            context.StartProcess("gitversion.exe", new ProcessSettings
            {
                Arguments = $"/output file /outputfile {VersionJson}"
            });
        }

        private void MakeNuspec(BuildContext context)
        {
            var nuspecTemplate = context.BuildDir + context.File(NuspecName);


            Render.Line("textrude creating nuspec file:".Green());
            context
                .DotNetCoreRun("Textrude",
                    $"render --models {VersionJson}" +
                    $" --template {nuspecTemplate}" +
                    $" --output {TargetNuspec} ");
        }

        private void CreatePackage(BuildContext context)
        {
            //remove the zip file since we don't want that !
            context.DeleteFile(context.ZipFile);
            context.StartProcess("nuget.exe", new ProcessSettings
            {
                Arguments = $" pack {TargetNuspec} -OutputDirectory {context.NugetDir}",
                RedirectStandardOutput = true,
            });
        }

        private void InstallPackage(BuildContext context)
        {
            context.StartProcess("nuget.exe", new ProcessSettings
            {
                Arguments =
                    $" install -outputDirectory {context.NugetDir} -source {context.NugetDir} textrude ",
                RedirectStandardOutput = true,
            });
        }
    }
}
