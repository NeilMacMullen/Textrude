using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.Solution;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using Cake.Common.IO;
using Cake.Common.IO.Paths;

namespace Build
{
    public class BuildContext : FrostingContext
    {
        private const string solutionFolderType = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";

        public BuildContext(ICakeContext context)
            : base(context)
        {
            BuildConfiguration = context.Argument<string>("configuration", "Debug");
            DoClean = context.HasArgument("clean");
            RepoDir = context.Directory(System.Environment.CurrentDirectory);
            PublishDir = RepoDir + context.Directory("publish");
            SolutionFile = RepoDir + context.File("Textrude.sln");
            Solution = context.ParseSolution(SolutionFile);
        }

        public string BuildConfiguration { get; }
        public bool DoClean { get; }
        public ConvertableDirectoryPath RepoDir { get; }
        public ConvertableDirectoryPath PublishDir { get; }
        public ConvertableFilePath SolutionFile { get; }
        public SolutionParserResult Solution { get; }

        public IEnumerable<SolutionProject> ProjectsToBuild
            => Solution.Projects
            .Where(p => p.Name != "Build")
            .Where(p => p.Type != solutionFolderType);

        public void Describe()
        {
            var build = new Tree(":rocket: Build");
            build
                .AddNode(":notebook: Settings")
                .AddNode(new Table()
                    .AddColumns("Clean", "Configuration")
                    .AddRow(DoClean.ToString(), BuildConfiguration)
                );

            var projects = new Table()
                .AddColumns("Name", "Path");
            foreach (var project in ProjectsToBuild.OrderBy(p => p.Name))
            {
                projects.AddRow(project.Name, RepoDir.Path.GetRelativePath(project.Path).FullPath);
            }

            build.AddNode(":hammer: Projects")
                .AddNode(projects);
            AnsiConsole.Render(build);
        }
    }
}
