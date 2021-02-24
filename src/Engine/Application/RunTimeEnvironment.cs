using System.Diagnostics;
using System.IO;

namespace Engine.Application
{
    public class RunTimeEnvironment
    {
        public readonly IFileSystemOperations FileSystem;

        public RunTimeEnvironment(IFileSystemOperations fileSystem) => FileSystem = fileSystem;

        /// <summary>
        ///     Returns the folder the application is running from.
        /// </summary>
        /// <remarks>
        ///     This is useful when setting up include paths
        /// </remarks>
        public string ApplicationFolder() => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        /// <summary>
        ///     Path to application exe
        /// </summary>
        /// <remarks>
        ///     This is the approved formulation for compatibility with single-exe apps.
        ///     See https://github.com/dotnet/runtime/issues/3704
        /// </remarks>
        public string ApplicationPath() => Process.GetCurrentProcess().MainModule.FileName;
    }
}