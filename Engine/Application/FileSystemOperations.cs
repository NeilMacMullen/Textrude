using System;
using System.IO;
using System.Reflection;

namespace Engine.Application
{
    /// <summary>
    ///     Provides access to the actual file-system
    /// </summary>
    /// <remarks>
    ///     By routing through an interface we can test application logic a bit more easily
    /// </remarks>
    public class FileSystemOperations : IFileSystemOperations
    {
        public bool Exists(string path) => File.Exists(path);

        public string ReadAllText(string path) => File.ReadAllText(path);

        public DateTime GetLastWriteTimeUtc(string path) => File.GetLastWriteTimeUtc(path);

        public void WriteAllText(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        /// <summary>
        ///     Returns the folder the application is running from.
        /// </summary>
        /// <remarks>
        ///     This is useful when setting up include paths
        /// </remarks>
        public string ApplicationFolder() => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}