﻿using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Common.Tools.ReportGenerator;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;

namespace Build.Tasks
{
    [TaskName("Test")]
    [TaskDescription("Runs dotnet test")]
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
