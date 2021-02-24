using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Common.Tools.DotNetCore.Pack;
using Cake.Common.Tools.DotNetCore.Publish;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Build.Tasks
{
    [TaskName("Package")]
    [IsDependentOn(typeof(CleanTask))]
    [IsDependentOn(typeof(BrandTask))]
    public sealed class PackageTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.CleanDirectory(context.PublishDir);

            ZipRelease(context);
            NuGet(context);
        }

        public void ZipRelease(BuildContext context)
        {
            var stageDir = context.PublishDir + context.Directory("_ziprelease");

            // Publish Textrude (Win64, Linux64)
            context.DotNetCorePublish("src/Textrude/Textrude.csproj",
                (context.DefaultPublishConfiguration with
                {
                    Runtime = "win-x64",
                    PublishDirectory = stageDir
                })
                .ToDotNetCorePublishSettings()
            );
            context.DotNetCorePublish("src/Textrude/Textrude.csproj",
                (context.DefaultPublishConfiguration with
                {
                    Runtime = "linux-x64",
                    PublishDirectory = stageDir + context.Directory("linux")
                })
                .ToDotNetCorePublishSettings()
            );

            // Publish TextrudeInteractive (Win64)
            context.DotNetCorePublish("src/TextrudeInteractive/TextrudeInteractive.csproj",
                (context.DefaultPublishConfiguration with
                {
                    Runtime = "win-x64",
                    Framework = "net5.0-windows",
                    PublishDirectory = stageDir
                })
                .ToDotNetCorePublishSettings()
            );

            // Move linux executable up
            context.MoveFile(
                stageDir + context.Directory("linux") + context.File("Textrude"),
                stageDir + context.File("Textrude_linux")
            );

            // Remove unneeded files
            context.DeleteFiles(GlobPattern.FromString(stageDir + context.File("*.pdb")));
            DeleteDirRecursive(stageDir + context.Directory("linux"));
            DeleteDirRecursive(stageDir + context.Directory("x86"));
            DeleteDirRecursive(stageDir + context.Directory("arm64"));

            // Copy examples to publish
            context.CopyDirectory("examples", stageDir + context.Directory("examples"));

            context.Zip(
                stageDir,
                context.PublishDir + context.File("Textrude.zip")
            );
            context.DeleteDirectory(stageDir, new DeleteDirectorySettings()
            {
                Recursive = true
            });

            void DeleteDirRecursive(params DirectoryPath[] pathsToCombine)
            {
                var dir = pathsToCombine.Aggregate((l, r) => l.Combine(r));
                context.DeleteDirectory(dir, new DeleteDirectorySettings()
                {
                    Recursive = true
                });
            }
        }

        public void NuGet(BuildContext context)
        {
            context.DotNetCorePack("src/Textrude", new DotNetCorePackSettings()
            {
                Configuration = "Release",
                IncludeSymbols = true,
                MSBuildSettings = new DotNetCoreMSBuildSettings()
                    .WithProperty("PackageVersion", context.Version.NuGetVersion)
                    .WithProperty("Authors", "Textrude contributers")
                    .WithProperty("Description", @"Textrude is a cross-platform general-purpose code-generation tool.
It can easily import data from CSV, YAML, JSON or plain-text files and apply Scriban templates to quickly scaffold output files.")
                    .WithProperty("Copyright", "Copyright (c) 2021 Textrude contributors")
                    .WithProperty("PackageLicenseExpression", "MIT")
                    .WithProperty("PackageProjectUrl", "https://github.com/NeilMacMullen/Textrude")
                    .WithProperty("RepositoryUrl", "https://github.com/NeilMacMullen/Textrude.git")
                    .WithProperty("RepositoryType", "git")
                    .WithProperty("RepositoryBranch", context.Version.BranchName)
                    .WithProperty("RepositoryCommit", context.Version.Sha)
                    .WithProperty("PackageOutputPath", context.PublishDir)
            });
        }
    }
}
