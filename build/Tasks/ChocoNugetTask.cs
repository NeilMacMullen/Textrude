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
    public abstract class ChocoNugetTask : FrostingTask<BuildContext>
    {
        protected const string NuspecName = "textrude.nuspec";

        private ConvertableFilePath _targetNuspec;
        private ConvertableFilePath _versionJson;

        protected bool CanSetOutDir;
        protected string Client;

        protected ConvertableDirectoryPath TargetDir;

        public override void Run(BuildContext context)
        {
            if (context.GitHubActions().IsRunningOnGitHubActions)
            {
                //don't build this on github
            }
            else
            {
                context.CleanDirectory(TargetDir);
                _versionJson = TargetDir + context.File("version.json");
                _targetNuspec = TargetDir + context.File(NuspecName);

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
                        var packTask = ctx.AddTask($"{Client} packing...", new ProgressTaskSettings
                        {
                            MaxValue = 1,
                            AutoStart = false
                        });

                        var installTask = ctx.AddTask($"{Client} unpacking...", new ProgressTaskSettings
                        {
                            MaxValue = 1,
                            AutoStart = false
                        });

                        getVersionTask.Run(context, GetVersion);
                        nuspecTask.Run(context, MakeNuspec);
                        packTask.Run(context, CreatePackage);
                        installTask.Run(context, InstallPackage);

                        $"{Client} artefacts are in {TargetDir}".Yellow();
                    });
            }
        }

        private void GetVersion(BuildContext context)
        {
            context.StartProcess("gitversion.exe", new ProcessSettings
            {
                Arguments = $"/output file /outputfile {_versionJson}"
            });
        }

        private void MakeNuspec(BuildContext context)
        {
            var nuspecTemplate = context.BuildDir + context.File(NuspecName);


            Render.Line("textrude creating nuspec file:".Green());
            context
                .DotNetCoreRun("Textrude",
                    $"render --models {_versionJson}" +
                    $" --template {nuspecTemplate}" +
                    $" --definitions CLIENT={Client}" +
                    $" --output {_targetNuspec} ");
        }


        protected virtual void CreatePackage(BuildContext context)
        {
            //remove the zip file since we don't want that !
            context.DeleteFile(context.ZipFile);
            context.StartProcess($"{Client}.exe", new ProcessSettings
            {
                Arguments = $" pack {_targetNuspec} -OutputDirectory {TargetDir}",
                RedirectStandardOutput = true,
            });
        }

        private void InstallPackage(BuildContext context)
        {
            var outDir = CanSetOutDir ? $" -outputDirectory {TargetDir}" : string.Empty;
            context.StartProcess($"{Client}.exe", new ProcessSettings
            {
                Arguments =
                    $" install  {outDir}  -source {TargetDir} textrude ",
                RedirectStandardOutput = true,
            });
        }
    }
}
