using Cake.Common;
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

                    var projFinished = new Regex("(?<project>.+) -> (?<output>.+)");
                    var warning = new Regex(@"(?<file>.+)\((?<row>\d+),(?<col>\d+)\): warning (?<code>[A-Z0-9]+): (?<text>.+)");

                    buildTask.StartTask();
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
                        RedirectStandardError = true,
                        RedirectedStandardOutputHandler = o =>
                        {
                            if (o == null) return null;

                            if (o.Length < 2000)
                            {
                                if (projFinished.TryMatch(o, out var pfm))
                                {
                                    if (AnsiConsole.Capabilities.SupportsInteraction)
                                    {
                                        AnsiConsole.MarkupLine(
                                            "[grey54]dotnet build:[/] [green]{0}[/] -> [grey54]{1}[/]",
                                            pfm.Groups["project"].Value,
                                            pfm.Groups["output"].Value.Truncate(AnsiConsole.Width
                                                - pfm.Groups["project"].Value.Length
                                                - 20
                                            )
                                        );
                                    }
                                    else
                                    {
                                        AnsiConsole.MarkupLine(
                                            "[grey54]dotnet build:[/] [green]{0}[/] -> [grey54]{1}[/]",
                                            pfm.Groups["project"].Value,
                                            pfm.Groups["output"].Value
                                        );
                                    }
                                    buildTask.Increment(1);
                                    return null;
                                }
                                else if (warning.TryMatch(o, out var warn))
                                {
                                    var offendingFile = context.File(warn.Groups["file"].Value);
                                    if (AnsiConsole.Capabilities.SupportsInteraction)
                                    {
                                        AnsiConsole.MarkupLine(
                                            "[grey54]dotnet build:[/] [yellow]{0}({1},{2}): warning {3}[/]",
                                            offendingFile.Path.GetFilename(),
                                            warn.Groups["row"].Value,
                                            warn.Groups["col"].Value,
                                            warn.Groups["text"].Value.EscapeMarkup().Truncate(AnsiConsole.Width
                                                - offendingFile.Path.GetFilename().FullPath.Length
                                                - warn.Groups["row"].Value.Length
                                                - warn.Groups["col"].Value.Length
                                                - 35
                                            )
                                        );
                                    }
                                    else
                                    {
                                        AnsiConsole.MarkupLine(
                                            "dotnet build: {0}({1},{2}): warning {3}: {4}",
                                            warn.Groups["file"].Value,
                                            warn.Groups["row"].Value,
                                            warn.Groups["col"].Value,
                                            warn.Groups["code"].Value,
                                            warn.Groups["text"].Value.EscapeMarkup()
                                        );
                                    }
                                    return null;
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(o))
                            {
                                if (AnsiConsole.Capabilities.SupportsInteraction)
                                {
                                    AnsiConsole.MarkupLine("[grey54]dotnet build:[/] {0}",
                                        o.EscapeMarkup().Truncate(AnsiConsole.Width - 25)
                                    );
                                }
                                else
                                {
                                    AnsiConsole.MarkupLine("[grey54]dotnet build:[/] {0}",
                                        o.EscapeMarkup()
                                    );
                                }
                            }
                            return null;
                        },
                        RedirectedStandardErrorHandler = o =>
                        {
                            if (!string.IsNullOrWhiteSpace(o))
                                AnsiConsole.MarkupLine("[red]{0}[/]", o.EscapeMarkup());
                            return null;
                        },
                    });
                    buildTask.StopTask();
                    if (exit != 0)
                        throw new Exception("Build failed!");

                    buildDocTask.StartTask();
                    context.DotNetCoreRun("Textrude", @"render --models ScriptLibrary/doc.yaml --template ScriptLibrary/doctemplate.sbn --output doc/lib.md");
                    buildDocTask.Increment(1);
                    buildDocTask.StopTask();
                });
        }
    }
}
