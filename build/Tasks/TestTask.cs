using Cake.Common;
using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Common.Tools.ReportGenerator;
using Cake.Core;
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

            // Build HTML report
            context.ReportGenerator(
                allOpenCoverReports,
                context.Directory("TestReports"),
                new ReportGeneratorSettings()
                {
                    ToolPath = context.IsRunningOnLinux()
                        ? context.RepoDir + context.File("tools/ReportGenerator.4.8.5/tools/net5.0/ReportGenerator.dll")
                        : null,
                    ReportTypes = new[]
                    {
                        ReportGeneratorReportType.Html,
                    }
                }
            );

            // Build lcov report for Coveralls
            context.ReportGenerator(
                allOpenCoverReports,
                context.Directory("TestResults"),
                new ReportGeneratorSettings()
                {
                    ToolPath = context.IsRunningOnLinux()
                        ? context.RepoDir + context.File("tools/ReportGenerator.4.8.5/tools/net5.0/ReportGenerator.dll")
                        : null,
                    ArgumentCustomization = args => args.Append("-reporttypes:lcov")
                }
            );
        }
    }
}
