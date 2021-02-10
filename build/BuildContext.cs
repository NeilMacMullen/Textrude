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
            SolutionPath = "Textrude.sln";
            Solution = context.ParseSolution(SolutionPath);
        }

        public string BuildConfiguration { get; }
        public bool DoClean { get; }
        public FilePath SolutionPath { get; }
        public SolutionParserResult Solution { get; }

        public IEnumerable<SolutionProject> ProjectsToBuild
            => Solution.Projects
            .Where(p => p.Name != "Build")
            .Where(p => p.Type != solutionFolderType);

        public void Describe()
        {
            var repoDir = new DirectoryPath(System.Environment.CurrentDirectory);

            var build = new Tree(":rocket: Build");
            build
                .AddNode(":pencil: Settings")
                .AddNode(new Table()
                    .AddColumns("Clean", "Configuration")
                    .AddRow(DoClean.ToString(), BuildConfiguration)
                );

            var projects = new Table()
                .AddColumns("Name", "Path");
            foreach (var project in ProjectsToBuild.OrderBy(p => p.Name))
            {
                projects.AddRow(project.Name, repoDir.GetRelativePath(project.Path).FullPath);
            }

            build.AddNode(":pick: Projects")
                .AddNode(projects);
            AnsiConsole.Render(build);
        }
    }
}
