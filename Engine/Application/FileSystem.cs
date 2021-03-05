using System;
using System.IO;

namespace Engine.Application
{
    /// <summary>
    ///     Provides access to the actual file-system
    /// </summary>
    /// <remarks>
    ///     By routing through an interface we can test application logic a bit more easily
    /// </remarks>
    public class FileSystem : IFileSystemOperations
    {
        public bool Exists(string path) => File.Exists(path);

        public string ReadAllText(string path) => File.ReadAllText(path);

        public DateTime GetLastWriteTimeUtc(string path) => File.GetLastWriteTimeUtc(path);

        public void WriteAllText(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        public bool CanHandle(string path) => true;
        public ModelFormat DefaultFormat(string path) => ModelDeserializerFactory.FormatFromExtension(path);
    }
}
