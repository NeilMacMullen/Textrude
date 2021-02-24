using Cake.Common.Build;
using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Common.Tools.ReportGenerator;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;

namespace Build.Tasks
{
    [TaskName("Test")]
    [TaskDescription("Runs all unit tests and builds coverage reports in HTML and lcov")]
    [IsDependentOn(typeof(BuildTask))]
    public class TestTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.CleanDirectory("TestResults");
            context.CleanDirectory("TestReports");
            context.CleanDirectories(GlobPattern.FromString(
                context.Directory("**") + context.Directory("TestResults")
            ));

            context.DotNetCoreTest(context.SolutionFile, new DotNetCoreTestSettings()
            {
                NoRestore = true,
                NoBuild = true,
                Configuration = context.BuildConfiguration,
                Verbosity = DotNetCoreVerbosity.Minimal,
                Collectors = new[]
                {
                    "XPlat Code Coverage"
                },
                Settings = context.BuildDir + context.File("coverlet.runsettings")
            });

            var allOpenCoverReports = GlobPattern.FromString(
                context.RepoDir
                + context.Directory("**")
                + context.Directory("TestResults")
                + context.Directory("*")
                + context.File("coverage.opencover.xml")
            );

            if (context.BuildSystem.IsLocalBuild)
            {
                context.Log.Information("Generating HTML report under {0}", "TestReports");
                // Build HTML report
                context.ReportGenerator(
                    allOpenCoverReports,
                    context.Directory("TestReports"),
                    new ReportGeneratorSettings()
                    {
                        ReportTypes = new[]
                        {
                            ReportGeneratorReportType.Html,
                        }
                    }
                );
            }

            if (context.BuildSystem.GitHubActions.IsRunningOnGitHubActions) {
                // Build lcov report for Coveralls
                context.ReportGenerator(
                    allOpenCoverReports,
                    context.Directory("TestResults"),
                    new ReportGeneratorSettings()
                    {
                        ArgumentCustomization = args => args.Append("-reporttypes:lcov")
                    }
                );
            }
        }
    }
}
