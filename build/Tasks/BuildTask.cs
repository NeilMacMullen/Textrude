using Cake.Common;
using Cake.Common.Build;
using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using Spectre.Console;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Build.Tasks
{
    [TaskName("Build")]
    [TaskDescription("Build all the projects for the current configuration and outputs current doc. for ScriptLibrary")]
    [IsDependentOn(typeof(RestoreTask))]
    public sealed class BuildTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            if (context.GitHubActions().IsRunningOnGitHubActions)
            {
                BuildSolution(context, color: false);
                GenerateDocumentation(context);
            }
            else
            {
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
                        var buildTask = ctx.AddTask("Build", new ProgressTaskSettings()
                        {
                            MaxValue = context.ProjectsToBuild.Count(),
                            AutoStart = false
                        });
                        var buildDocTask = ctx.AddTask("Build Doc.", new ProgressTaskSettings()
                        {
                            MaxValue = 1,
                            AutoStart = false
                        });

                        buildTask.StartTask();
                        BuildSolution(context,
                            color: true,
                            new Progress<int>(p => buildTask.Increment(p)));
                        buildTask.StopTask();

                        buildDocTask.StartTask();
                        GenerateDocumentation(context);
                        buildDocTask.Increment(1);
                        buildDocTask.StopTask();
                    });
            }
        }

        private void BuildSolution(BuildContext context, bool color, IProgress<int> progress = default(IProgress<int>))
        {
            var projFinished = new Regex("(?<project>.+) -> (?<output>.+)");
            var warning = new Regex(
                @"(?<file>.+)\((?<row>\d+),(?<col>\d+)\): warning (?<code>[A-Z0-9]+): (?<text>.+) \[(?<project>.+)\]");

            var exit = context.StartProcess("dotnet", new ProcessSettings()
            {
                Arguments = new ProcessArgumentBuilder()
                    .Append("build")
                    .Append("--no-restore")
                    .AppendSwitch("-c", context.BuildConfiguration)
                    .AppendSwitch("-v", DotNetCoreVerbosity.Minimal.ToString())
                    .Append("-consoleLoggerParameters:NoSummary;NoItemAndPropertyList")
                    .Append("-binaryLogger:LogFile=build.binlog"),
                RedirectStandardOutput = true,
                RedirectedStandardOutputHandler = o =>
                {
                    if (o == null) return null;
                    if (!color)
                    {
                        Console.WriteLine(o);
                        return null;
                    }

                    if (o.Length < 2000)
                    {
                        if (projFinished.TryMatch(o, out var pfm))
                        {
                            progress?.Report(1);
                            RenderLine(
                                "[grey54]dotnet build:[/] [green]{0}[/] -> [grey54]{1}[/]",
                                pfm.Groups["project"].Value,
                                pfm.Groups["output"].Value
                            );
                            return null;
                        }
                        else if (warning.TryMatch(o, out var warn))
                        {
                            var offendingFile = context.File(warn.Groups["file"].Value);
                            RenderLine(
                                "[grey54]dotnet build:[/] [yellow]{0}({1},{2}): warning {3}[/]",
                                offendingFile.Path.GetFilename(),
                                warn.Groups["row"].Value,
                                warn.Groups["col"].Value,
                                warn.Groups["text"].Value.EscapeMarkup()
                            );
                            return null;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(o))
                    {
                        RenderLine(
                            "[grey54]dotnet build:[/] {0}",
                            o.EscapeMarkup()
                        );
                    }
                    return null;
                },
            });
            if (exit != 0)
                throw new Exception("Build failed!");
        }
        private void RenderLine(string format, params object[] args)
        {
            var line =
                new Markup(string.Format(format, args) + Environment.NewLine)
                    .Overflow(Overflow.Ellipsis);
            AnsiConsole.Render(line);
        }

        private string FileStub(IFile path) =>
            path.Path.GetFilenameWithoutExtension().ToString();

        private void GenerateDocumentation(BuildContext context)
        {
            var libFolder = "ScriptLibrary";
            var extractionScript = $"{libFolder}/extractDoc.sbn";
            var markDownScript = $"{libFolder}/doctemplate.sbn";
            var tempDocModel = $"{libFolder}/doc.yaml";
            var markDownOutput = "doc/lib.md";
            var libFiles =
                context
                    .FileSystem
                    .GetDirectory("./ScriptLibrary/lib")
                    .GetFiles("*.sbn", SearchScope.Current);

            var modelNames = string.Join(" ", libFiles.Select(FileStub));

            var modelPaths =
                string
                    .Join(" ",
                        libFiles
                            .Select(file => $"__{FileStub(file)}=\"{file.Path}\""));

            RenderLine("[green]textrude extracting docs:[/] {0}", modelNames);
            context
                .DotNetCoreRun("Textrude",
                    $"render --models {modelPaths}" +
                    $" --template {extractionScript}" +
                    $" --definitions \"MODELLIST={modelNames}\"" +
                    $" --output {tempDocModel} ");

            RenderLine("[green]textrude generating lib.md:[/]");
            context
                .DotNetCoreRun("Textrude",
                    $"render --models {tempDocModel}" +
                    $" --template {markDownScript}" +
                    $" --output {markDownOutput}");
        }
    }
}
