var target = Argument("target", "Build");
var configuration = Argument("configuration", "Debug");
var solution = File("Textrude.sln");

Task("Restore").Does(() => {
    DotNetCoreRestore(solution);
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetCoreBuild(solution, new DotNetCoreBuildSettings() {
        NoRestore = true
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreTest(solution, new DotNetCoreTestSettings() {
        NoRestore = true,
        NoBuild = true,
    });
});

RunTarget(target);