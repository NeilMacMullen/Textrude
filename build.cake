#addin nuget:?package=Wcwidth&version=0.2.0
#addin nuget:?package=Spectre.Console&version=0.37.0

using System.Text.RegularExpressions;
using Spectre.Console;

var target = Argument("target", "Build");

Setup<BuildData>(ctx => {
    var build = new BuildData(
        ctx,
        target,
        configuration: Argument("configuration", "Debug"),
        doClean: HasArgument("clean"),
        dotnetVerbosity: Argument("dotnet_verbosity", DotNetCoreVerbosity.Minimal),
        solutionPath: File("Textrude.sln")
    );
    build.Describe();
    return build;
});

Task("Clean")
    .Description("Runs dotnet clean and cleans the NuGet cache (when in GitHub Actions)")
    .WithCriteria<BuildData>((ctx, build) => build.DoClean)
    .Does<BuildData>(build =>
{
    DotNetCoreClean(build.SolutionPath.FullPath, new DotNetCoreCleanSettings() {
        Configuration = build.Configuration,
        Verbosity = build.DotnetVerbosity
    });

    if (GitHubActions.IsRunningOnGitHubActions) {
        // NuGet cache has to cleaned so that windows-latest picks up the NuGet packages
        // see https://github.com/dotnet/core/issues/5881
        int nugetLocalsClearResult = StartProcess("dotnet", "nuget locals all --clear");
        if (nugetLocalsClearResult != 0)
            Error("Could not clear local NuGet cache!");
    }
});

Task("Restore")
    .Description("Runs dotnet restore")
    .IsDependentOn("Clean")
    .Does<BuildData>(build =>
{
    DotNetCoreRestore(build.SolutionPath.FullPath, new DotNetCoreRestoreSettings() {
        ArgumentCustomization = args => args.Append($"-p:Configuration={build.Configuration}"),
        Verbosity = build.DotnetVerbosity
    });
});

Task("Build")
    .Description("Build all the projects for the current configuration and outputs current doc. for ScriptLibrary")
    .IsDependentOn("Restore")
    .Does<BuildData>(build =>
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
        .Start(ctx => {
            var buildTask = ctx.AddTask("Build", new ProgressTaskSettings() {
                MaxValue = build.ProjectsToBuild.Count(),
                AutoStart = false
            });
            var buildDocTask = ctx.AddTask("Build Doc.", new ProgressTaskSettings() {
                MaxValue = 1,
                AutoStart = false
            });

            var projFinished = new Regex("(?<project>.+) -> (?<output>.+)");

            buildTask.StartTask();
            var exit = StartProcess("dotnet", new ProcessSettings() {
                Arguments = new ProcessArgumentBuilder()
                    .Append("build")
                    .Append("--no-restore")
                    .Append($"-c {build.Configuration}")
                    .Append($"-v {build.DotnetVerbosity}"),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectedStandardOutputHandler = o => {
                    if (o == null) return null;

                    if (projFinished.TryMatch(o, out var pfm)) {
                        AnsiConsole.MarkupLine("[grey54]dotnet build:[/] [green]{0}[/] -> [grey54]{1}[/]", pfm.Groups["project"].Value, pfm.Groups["output"].Value);
                        buildTask.Increment(1);
                    } else if (!string.IsNullOrWhiteSpace(o))  {
                        AnsiConsole.MarkupLine("[grey54]dotnet build:[/] {0}", o.EscapeMarkup());
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
    .Description("Runs dotnet test")
    .IsDependentOn("Build")
    .Does<BuildData>(build =>
{
    DotNetCoreTest(build.SolutionPath.FullPath, new DotNetCoreTestSettings() {
        NoRestore = true,
        NoBuild = true,
        Configuration = build.Configuration,
        Verbosity = build.DotnetVerbosity
    });
});

RunTarget(target);

public class BuildData
{
    private const string solutionFolderType = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";

    private readonly ICakeContext ctx;

    public BuildData(
        ICakeContext ctx,
        string target,
        string configuration,
        bool doClean,
        DotNetCoreVerbosity dotnetVerbosity,
        FilePath solutionPath)
    {
        this.ctx = ctx;
        Target = target;
        Configuration = configuration;
        DoClean = doClean;
        DotnetVerbosity = dotnetVerbosity;
        SolutionPath = solutionPath;
        Solution = ctx.ParseSolution(solutionPath);
    }

    public string Target { get; }
    public string Configuration { get; }
    public bool DoClean { get; }
    public DotNetCoreVerbosity DotnetVerbosity { get; }
    public FilePath SolutionPath { get; }
    public SolutionParserResult Solution { get; }

    public IEnumerable<SolutionProject> ProjectsToBuild 
        => Solution.Projects
        .Where(p => p.Type != solutionFolderType);

    public void Describe() {
        ctx.Information("Running {0} with {1} clean: {2}", Target, Configuration, DoClean);
    }
}

public static bool TryMatch(this Regex regex, string input, out Match firstMatch) {
    firstMatch = regex.Match(input);
    return firstMatch.Success;
}
