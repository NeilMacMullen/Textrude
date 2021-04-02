using System.Collections.Generic;
using System.Linq;
using Cake.Common;
using Cake.Common.IO;
using Cake.Common.IO.Paths;
using Cake.Common.Solution;
using Cake.Core;
using Cake.Frosting;
using Spectre.Console;

namespace Build
{
    public class BuildContext : FrostingContext
    {
        private const string SolutionFolderType = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";

        public BuildContext(ICakeContext context)
            : base(context)
        {
            BuildConfiguration = context.Argument("configuration", "Debug");
            DoClean = context.HasArgument("clean");
            RepoDir = context.Directory(System.Environment.CurrentDirectory);
            BuildDir = RepoDir + context.Directory("build");
            PublishDir = RepoDir + context.Directory("publish");
            SolutionFile = RepoDir + context.File("Textrude.sln");
            Solution = context.ParseSolution(SolutionFile);
            ScriptLibrary = context.Directory("ScriptLibrary");
        }

        public string BuildConfiguration { get; }
        public bool DoClean { get; }
        public ConvertableDirectoryPath ScriptLibrary { get; }
        public ConvertableDirectoryPath RepoDir { get; }
        public ConvertableDirectoryPath BuildDir { get; }
        public ConvertableDirectoryPath PublishDir { get; }
        public ConvertableFilePath SolutionFile { get; }
        public SolutionParserResult Solution { get; }

        public IEnumerable<SolutionProject> ProjectsToBuild
            => Solution.Projects
                .Where(p => p.Name != "Build")
                .Where(p => p.Type != SolutionFolderType);

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
