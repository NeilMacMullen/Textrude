var target = Argument("target", "Build");
var configuration = Argument("configuration", "Debug");
var clean = HasArgument("clean");
var dotnetVerbosity = Argument("dotnet_verbosity", DotNetCoreVerbosity.Minimal);

var solution = File("Textrude.sln");

Setup(c => {
    Information("Running {0} with {1} clean: {2}", target, configuration, clean);
});

Task("Clean")
    .WithCriteria(clean)
    .Does(() =>
{
    DotNetCoreClean(solution, new DotNetCoreCleanSettings() {
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
    DotNetCoreRestore(solution, new DotNetCoreRestoreSettings() {
        ArgumentCustomization = args => args.Append($"-p:Configuration={configuration}"),
        Verbosity = dotnetVerbosity
    });
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetCoreBuild(solution, new DotNetCoreBuildSettings() {
        NoRestore = true,
        Configuration = configuration,
        Verbosity = dotnetVerbosity
    });
    
    DotNetCoreRun("Textrude", @"render --models ScriptLibrary\doc.yaml --template ScriptLibrary\doctemplate.sbn --output doc\lib.md");
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreTest(solution, new DotNetCoreTestSettings() {
        NoRestore = true,
        NoBuild = true,
        Configuration = configuration,
        Verbosity = dotnetVerbosity
    });
});

RunTarget(target);

