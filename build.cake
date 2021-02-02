#addin nuget:?package=Spectre.Console&version=0.37.0

using System.Text.RegularExpressions;
using Spectre.Console;

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Debug");
var clean = HasArgument("clean");
var dotnetVerbosity = Argument("dotnet_verbosity", DotNetCoreVerbosity.Minimal);

var solutionPath = File("Textrude.sln");
var solution = ParseSolution(solutionPath);
var toBuildProjects = solution.Projects
    .Where(p => p.Type != solutionFolderType);

const string solutionFolderType = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";

Setup(c => {
    Information("Running {0} with {1} clean: {2}", target, configuration, clean);
});

Task("Clean")
    .WithCriteria(clean)
    .Does(() =>
{
    DotNetCoreClean(solutionPath, new DotNetCoreCleanSettings() {
        Configuration = configuration,
        Verbosity = dotnetVerbosity
    });

    if (GitHubActions.IsRunningOnGitHubActions) {
        int nugetLocalsClearResult = StartProcess("dotnet", "nuget locals all --clear");
        if (nugetLocalsClearResult != 0)
            Error("Could not clear local NuGet cache!");
    }
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore(solutionPath, new DotNetCoreRestoreSettings() {
        ArgumentCustomization = args => args.Append($"-p:Configuration={configuration}"),
        Verbosity = dotnetVerbosity
    });
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
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
        .Start(pctx => {
            var buildTask = pctx.AddTask("Build", new ProgressTaskSettings() {
                MaxValue = toBuildProjects.Count(),
                AutoStart = false
            });
            var buildDocTask = pctx.AddTask("Build Doc.", new ProgressTaskSettings() {
                MaxValue = 1,
                AutoStart = false
            });

            var projFinished = new Regex("(?<project>.+) -> (?<output>.+)");

            buildTask.StartTask();
            var exit = StartProcess("dotnet", new ProcessSettings() {
                Arguments = new ProcessArgumentBuilder()
                    .Append("build")
                    .Append("--no-restore")
                    .Append($"-v {dotnetVerbosity}"),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectedStandardOutputHandler = o => {
                    if (o == null) return null;

                    if (projFinished.TryMatch(o, out var pfm)) {
                        buildTask.Increment(1);
                        AnsiConsole.MarkupLine("[grey]dotnet build:[/] [green]{0}[/] -> [grey]{1}[/]", pfm.Groups["project"].Value, pfm.Groups["output"].Value);
                    } else if (!string.IsNullOrWhiteSpace(o))  {
                        AnsiConsole.MarkupLine("[grey]dotnet build:[/] {0}", o.EscapeMarkup());
                    }
                    return null;
                },
                RedirectedStandardErrorHandler = o => {
                    if (!string.IsNullOrWhiteSpace(o))
                        AnsiConsole.MarkupLine("[red]{0}[/]", o.EscapeMarkup());
                    return null;
                },
            });
            buildTask.StopTask();
            if (exit != 0) 
                throw new Exception("Build failed!");

            buildDocTask.StartTask();
            DotNetCoreRun("Textrude", @"render --models ScriptLibrary/doc.yaml --template ScriptLibrary/doctemplate.sbn --output doc/lib.md");
            buildDocTask.Increment(1);
            buildDocTask.StopTask();
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreTest(solutionPath, new DotNetCoreTestSettings() {
        NoRestore = true,
        NoBuild = true,
        Configuration = configuration,
        Verbosity = dotnetVerbosity
    });
});

RunTarget(target);

public static bool TryMatch(this Regex regex, string input, out Match firstMatch) {
    firstMatch = regex.Match(input);
    return firstMatch.Success;
}