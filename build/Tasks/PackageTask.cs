using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
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
    [IsDependentOn(typeof(BrandTask))]
    public sealed class PackageTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.CleanDirectory(context.PublishDir);

            // Publish Textrude (Win64, Linux64)
            context.DotNetCorePublish(@"Textrude\Textrude.csproj", new DotNetCorePublishSettings()
            {
                ArgumentCustomization = args => args.Append(@"/p:PublishProfile=Textrude\Properties\PublishProfiles\WinX64.pubxml")
            });
            context.DotNetCorePublish(@"Textrude\Textrude.csproj", new DotNetCorePublishSettings()
            {
                ArgumentCustomization = args => args.Append(@"/p:PublishProfile=Textrude\Properties\PublishProfiles\LinuxX64.pubxml")
            });

            // Publish TextrudeInteractive (Win64)
            context.DotNetCorePublish(@"TextrudeInteractive\TextrudeInteractive.csproj", new DotNetCorePublishSettings()
            {
                ArgumentCustomization = args => args.Append(@"/p:PublishProfile=TextrudeInteractive\Properties\PublishProfiles\WinX64.pubxml")
            });

            // Move linux executable up
            context.MoveFile(
                context.PublishDir + context.Directory("linux") + context.File("Textrude"),
                context.PublishDir + context.File("Textrude_linux")
            );

            // Remove unneeded files
            context.DeleteFiles(GlobPattern.FromString(context.PublishDir + context.File("*.pdb")));
            DeleteDirRecursive(context.PublishDir + context.Directory("linux"));
            DeleteDirRecursive(context.PublishDir + context.Directory("x86"));
            DeleteDirRecursive(context.PublishDir + context.Directory("arm64"));

            // Copy examples to publish
            context.CopyDirectory("examples", context.PublishDir + context.Directory("examples"));

            context.Zip(
                context.PublishDir,
                context.PublishDir + context.File("Textrude.zip")
            );

            void DeleteDirRecursive(params DirectoryPath[] pathsToCombine)
            {
                var dir = pathsToCombine.Aggregate((l, r) => l.Combine(r));
                context.DeleteDirectory(dir, new DeleteDirectorySettings()
                {
                    Recursive = true
                });
            }
        }
    }
}
